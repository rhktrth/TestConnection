# TestConnection 外部仕様

この文書は、TestConnection の利用者、接続相手、Windows、設定ファイル、result output から観測できる**現在の外部仕様**を定義します。

内部構成は [`ARCHITECTURE.md`](ARCHITECTURE.md)、build / test / release は [`OPERATIONS.md`](OPERATIONS.md)、文書体系と仕様駆動の運用規則は [`README.md`](README.md) を正本とします。

仕様 ID は追跡が必要な契約だけに付与します。すべての文章を ID 化しません。

## 1. 目的と範囲

### EXT-SCOPE-001: 疎通確認の対象

TestConnection は、Windows 端末から TCP / UDP / DNS / ICMP Echo の通信を発生させ、指定した endpoint 間で各 protocol の最小成立点まで通信できたかを観測するツールです。

主な用途は、router、firewall、network redundancy、経路切替等によって通信可否が変化する環境での疎通確認です。

次は基本的な判定対象にしません。

- application protocol 固有の handshake、認証、業務処理
- throughput / load / performance
- packet capture / protocol analysis
- route discovery や firewall rule の自動診断
- TCP session を長時間保持した stateful failover

### EXT-RUNTIME-001: 実行環境と配布形態

- 利用者向け基準環境は Windows 11 22H2 以降とする。
- application は Microsoft .NET Framework 4.8.1 上で動作する。
- 配布単位は展開して実行できる ZIP とし、installer / ClickOnce を前提にしない。
- UI は WinForms とし、表示は light mode 固定とする。
- TCP / UDP / DNS は通常権限で利用できる。raw ICMP socket と NIC 設定変更は Windows の実行環境によって管理者権限を必要とする。

runtime を選ぶ理由と変更条件は [`ADR-0001`](adr/0001-winforms-net481-zip.md) を参照します。

## 2. Tester definition と設定ファイル

### EXT-CFG-001: 有効な tester definition

一つの tester definition は role、local IP address、remote endpoint、protocol、port を持ちます。

- `Server` role は TCP / UDP だけを受け付ける。
- `Client` role は TCP / UDP / DNS / Ping を受け付ける。
- local IP address が空でなければ IP address として解釈可能でなければならない。
- TCP / UDP / Ping client の remote endpoint は IPv4 literal または hostname / FQDN を受け付ける。IPv6 literal は remote endpoint として受け付けない。
- DNS client の remote endpoint は問い合わせ先 DNS server の IP address とする。hostname は受け付けない。
- Ping 以外の port は 0 から 65535 の範囲とする。

### EXT-CFG-002: CSV の保存・読込み

設定ファイルは 1 行を 1 tester definition とする CSV で、論理的な列順は次です。

```text
Role,LocalIpAddress,RemoteIpAddress,Protocol,Port
```

- 行頭が `#` の行は comment として読込み時に無視する。
- 保存時は tester definition だけを出力し、comment 行は再出力しない。
- TCP / UDP / Ping の remote endpoint に hostname / FQDN を指定した場合、保存時に解決済み IP address へ置換せず、利用者が指定した文字列を保持する。
- 読込み時に無効な行があれば、その行番号を含む format error として扱う。

## 3. Endpoint の扱い

### EXT-ENDPOINT-001: local endpoint

- client tester は local IP address が指定されていれば、その address に bind して通信する。
- client tester の local IP address が空の場合、送信元 address の選択は OS に委ねる。
- TCP / UDP server は local IP address が指定されていればその address で待受し、空の場合は利用可能な local address で待受する。

### EXT-RESOLVE-001: remote endpoint の名前解決

TCP / UDP / Ping client の hostname / FQDN は、tester 登録時ではなく**各試行の直前**に解決します。

- 一つの hostname から複数の IPv4 address が得られた場合、IPv4 address を数値として昇順に並べた先頭を使用する。
- IPv4 address を一つも得られない場合、その試行を failure として記録し、後続の試行へ進む。
- TCP / UDP の result には実際に使用した remote IPv4 address を出力する。
- Ping の result には `hostname(IPv4 address)` の形式で解決結果を出力する。
- DNS client は問い合わせ先そのものが IP address であるため、この hostname resolution の対象外とする。

この規則により、DNS 応答が切り替わった場合は次の試行から新しい解決結果を反映できます。

## 4. Protocol ごとの success contract

各 protocol の success は「対象 application が正常である」ことを意味せず、その protocol で TestConnection が観測する成立点だけを意味します。判断理由は [`ADR-0002`](adr/0002-minimal-protocol-success.md) を参照します。

### EXT-TCP-001: TCP client

TCP client は指定 endpoint への TCP connection が確立した時点で success とします。

- application data は送信しない。
- connection 成立後は session を保持せず切断する。
- success は接続先 application の handshake、認証、業務処理の成功を保証しない。

### EXT-TCP-002: TCP server

TCP server は TCP connection を accept した時点で success とします。

- application data は交換しない。
- accept した connection は疎通確認後に保持しない。

### EXT-UDP-001: UDP client

UDP client は小さな datagram の送信を local OS が受け付けた時点で success とします。

この success は remote endpoint への到達を保証しません。遠端到達を確認する場合は TestConnection の UDP server、packet capture、firewall log 等の別観測点を併用します。

### EXT-UDP-002: UDP server

UDP server は datagram を受信した時点で success とします。payload の application-level validity は判定しません。

### EXT-DNS-001: DNS client

DNS client は指定された DNS server へ固定の A record query を UDP で送信し、その送信先から UDP response を受信した時点で success とします。

- response の transaction ID、RCODE、answer 内容は success 判定に含めない。
- timeout、socket error 等で response を受信できなければ failure とする。
-停止要求によって receive が中断された場合は success / failure のどちらにも加算せず、result も追加しない。

### EXT-PING-001: Ping client

Ping client は ICMP Echo Request を送信し、次をすべて満たす Echo Reply を受信した場合だけ success とします。

- reply の送信元が試行で使用した remote IPv4 address と一致する。
- ICMP type が Echo Reply である。
- identifier が送信した request と一致する。
- sequence number が送信した request と一致する。

無関係な ICMP packet や malformed packet を受信しても success とせず、timeout まで対応する reply を待ちます。

## 5. 試行の実行順序と停止

### EXT-RUN-001: client / server の実行順序

- 一回の試験開始では server 群を先に開始し、各 server の待受準備が完了または失敗したことを確定してから client loop を開始する。
- client tester は登録順に一項目ずつ実行する。
- repeat count が有限値の場合は指定回数で終了し、0 の場合は停止要求まで反復する。
- 停止時は実行中 client の blocking I/O を cancel し、client loop の終了を待ってから server 群を停止する。
- 二重 start / stop によって同じ試験を重複実行・重複停止しない。

この順序を採る理由は [`ADR-0003`](adr/0003-sequential-client-loop.md) を参照します。

## 6. Result output

### EXT-RESULT-001: result の共通形式

通常の result は timestamp、role、確定済み local endpoint、protocol、message を含みます。

local endpoint が確定している場合の基本形式は次です。

```text
yyyy/MM/dd HH:mm:ss Role address:port/Protocol message
```

local endpoint がまだ確定していない error 等では、空 endpoint に対して不要な `:/` を出力しません。

protocol ごとの message には、必要に応じて実際に接続・送信・応答した remote endpoint を含めます。hostname を使用した場合の resolved address の表示は `EXT-RESOLVE-001` に従います。

## 7. NIC 設定変更

### EXT-NIC-001: 変更対象と snapshot

TestConnection から NIC 設定を変更する場合は、一時的で可逆な操作として扱います。

- 最初の変更直前に NIC ごとの元状態を snapshot として保持する。
- snapshot が残っている間に同一 NIC を複数回変更しても、復元基準は最初に取得した状態とする。
- 変更対象は IP address、subnet mask、default gateway とし、DNS 等の未変更項目へ処理範囲を広げない。
- 複数 NIC を変更した場合は NIC ごとに独立して元状態を保持する。

### EXT-NIC-002: 復元

- 手動復元と window close は同じ復元処理を使用する。
- NIC 設定途中で失敗した場合は、保存済み snapshot からの復元を試みる。
- window close 時に復元できない場合は close を中止し、利用者へ通知する。
- 疎通試験実行中は NIC 設定変更操作を無効化する。
- process kill、application crash、OS 強制終了等で復元処理自体を実行できない場合まで完全な rollback は保証しない。

詳細な設計理由は [`ADR-0004`](adr/0004-reversible-nic-configuration.md) を参照します。

## 8. 受入条件と traceability

`AC-*` は外部 contract の回帰を検出するための acceptance です。private method の実装方式を固定しません。

| 外部仕様 | 受入条件 | 現在の検証 | 主な production component |
| --- | --- | --- | --- |
| `EXT-TCP-001`, `EXT-TCP-002` | `AC-TCP-001`: loopback 上で TCP client が接続成功し、TCP server が accept を記録する | `ConnectivityTests.TestTcpLoopback` | `TcpTesterClient`, `TcpTesterServer` |
| `EXT-UDP-001`, `EXT-UDP-002` | `AC-UDP-001`: loopback 上で UDP client の送信が成功し、UDP server が datagram を受信する | `ConnectivityTests.TestUdpLoopback` | `UdpTesterClient`, `UdpTesterServer` |
| `EXT-DNS-001` | `AC-DNS-001`: loopback DNS endpoint が response を返すと success になり、停止による receive 中断は success / failure を増やさない | `ConnectivityTests.TestDnsLoopback`, `ConnectivityTests.TestDnsCancellation` | `DnsTesterClient` |
| `EXT-PING-001` | `AC-PING-001`: Echo Request を生成でき、対応する Echo Reply だけを受理する。`AC-PING-002`: malformed / unrelated packet を拒否する | `IcmpEchoPacketTests` | `PingTesterClient`, `IcmpEchoPacket` |
| `EXT-RESOLVE-001` | `AC-RESOLVE-001`: IPv4 literal / hostname の入力境界と、複数候補から数値最小 IPv4 を選ぶ規則を維持する | `ConnectivityTests.TestRemoteEndpointResolver` | `RemoteEndpointResolver` |
| `EXT-RUN-001` | `AC-RUN-001`: client を登録順・有限回数で実行する。`AC-RUN-002`: stop が current client を cancel して worker 終了を待つ。`AC-RUN-003`: server readiness と session start / stop 順序を維持する | `RunnerTests.TestFiniteClientLoop`, `TestClientLoopStop`, `TestSessionLifecycle`, `TestServerStartWaitsForReady` | `ClientTestRunner`, `TestSession`, `TesterServer` |
| `EXT-CFG-001`, `EXT-CFG-002` | `AC-CFG-001`: 既存 5 列 CSV を parse / serialize できる。`AC-CFG-002`: 全 definition を validation し、不正 format を拒否する | `ConfigurationTests` | `TesterDefinitionFile`, `TesterDefinitionValidator` |
| `EXT-RESULT-001` | `AC-RESULT-001`: endpoint 未確定時に不要な separator を出さず、確定時は共通 prefix を出す | `ConnectivityTests.TestResultLogFormat` | `TesterBase` |
| `EXT-NIC-001`, `EXT-NIC-002` | `AC-NIC-001`: Windows 上で変更前 snapshot へ手動復元でき、close 時にも同じ復元経路が使われる | 通常 CI では実 NIC を変更しないため manual verification | `NicConfigurationService`, `MainForm` |

### Acceptance を追加するとき

- 新しい外部 contract を追加・変更する場合は、回帰リスクがあるなら `EXT-*` と同時に `AC-*` を追加・更新する。
- network integration test は原則 loopback とし、通常 CI から外部 host へ接続しない。
- OS privilege や実 NIC へ依存して deterministic に自動化できないものは、manual verification を明示する。
- test 名や class 構成が変わっても、同じ `AC-*` をどこで検証しているか追える状態を維持する。
