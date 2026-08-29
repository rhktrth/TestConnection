# TestConnection 外部設計

この文書は、[`REQUIREMENTS.md`](REQUIREMENTS.md) の要件を、利用者、接続相手、Windows、設定ファイル、result output から観測できる**外部仕様**へ具体化します。

内部方式は [`INTERNAL_DESIGN.md`](INTERNAL_DESIGN.md)、build / test / release は [`OPERATIONS.md`](OPERATIONS.md) を正本とします。

## 1. 目的と範囲

### EXT-SCOPE-001: 疎通確認の対象

`REQ-SCOPE-001` を受け、TestConnection は Windows 端末から TCP / UDP / DNS / ICMP Echo の通信を発生させ、指定した endpoint 間で各 protocol の最小成立点まで通信できたかを観測します。

application protocol 固有の handshake、認証、業務処理、throughput、packet analysis、route diagnosis、長時間 TCP session 維持は基本的な判定対象にしません。

### EXT-RUNTIME-001: 実行環境と配布形態

`REQ-RUNTIME-001`, `REQ-DISTRIBUTION-001` を受け、次を外部 contract とします。

- Windows 11 22H2 以降で動作する。
- Microsoft .NET Framework 4.8.1 を使用する。
- 配布単位は展開して実行できる ZIP とし、installer / ClickOnce を前提にしない。
- UI は WinForms、light mode 固定とする。
- TCP / UDP / DNS は通常権限で利用できる。raw ICMP socket と NIC 設定変更は環境によって管理者権限を必要とする。

runtime を選ぶ理由と変更条件は [`ADR-0001`](adr/0001-winforms-net481-zip.md) を参照します。

## 2. Tester definition と設定ファイル

### EXT-CFG-001: 有効な tester definition

`REQ-PROTOCOL-001`, `REQ-SOURCE-001`, `REQ-DESTINATION-001` を受け、一つの tester definition は role、local IP address、remote endpoint、protocol、port を持ちます。

- `Server` role は TCP / UDP だけを受け付ける。
- `Client` role は TCP / UDP / DNS / Ping を受け付ける。
- local IP address が空でなければ IP address として解釈可能でなければならない。
- TCP / UDP / Ping client の remote endpoint は IPv4 literal または hostname / FQDN を受け付ける。IPv6 literal は受け付けない。
- DNS client の remote endpoint は問い合わせ先 DNS server の IP address とし、hostname は受け付けない。
- Ping 以外の port は 0 から 65535 の範囲とする。

### EXT-CFG-002: CSV の保存・読込み

`REQ-CONFIG-001` を受け、設定ファイルは 1 行を 1 tester definition とする CSV とします。

```text
Role,LocalIpAddress,RemoteIpAddress,Protocol,Port
```

- 行頭が `#` の行は読込み時に comment として無視する。
- 保存時は tester definition だけを出力し、comment 行は再出力しない。
- hostname / FQDN は解決済み IP address へ置換せず、利用者が指定した文字列を保存する。
- 無効な行は行番号を含む format error として扱う。

## 3. Endpoint

### EXT-ENDPOINT-001: local endpoint

`REQ-SOURCE-001` を受け、client / server は local IP address が指定されていればその address を使用し、未指定の場合は OS の通常の address selection / listen behavior を利用します。

### EXT-RESOLVE-001: remote endpoint の名前解決

`REQ-DESTINATION-001` を受け、TCP / UDP / Ping client の hostname / FQDN は各試行の直前に解決します。

- 複数の IPv4 address が得られた場合、数値昇順の先頭を使用する。
- IPv4 address が得られない場合、その試行を failure とする。
- TCP / UDP result には実際に使用した IPv4 address を出力する。
- Ping result には `hostname(IPv4 address)` の形式で解決結果を出力する。
- DNS client は hostname resolution の対象外とする。

## 4. Protocol ごとの success contract

`REQ-PROTOCOL-001` を具体化し、success は application 全体の正常性ではなく各 protocol の最小成立点を意味します。判断理由は [`ADR-0002`](adr/0002-minimal-protocol-success.md) を参照します。

### EXT-TCP-001: TCP client

指定 endpoint への TCP connection が確立した時点で success とします。application data は送信せず、connection は成立確認後に保持しません。

### EXT-TCP-002: TCP server

TCP connection を accept した時点で success とします。application data は交換しません。

### EXT-UDP-001: UDP client

小さな datagram の送信を local OS が受け付けた時点で success とします。この success は remote endpoint への到達を保証しません。

### EXT-UDP-002: UDP server

Datagram を受信した時点で success とします。payload の application-level validity は判定しません。

### EXT-DNS-001: DNS client

指定 DNS server へ固定の A record query を UDP で送信し、その送信先から UDP response を受信した時点で success とします。

- transaction ID、RCODE、answer 内容は success 判定に含めない。
- timeout / socket error で response を受信できなければ failure とする。
- stop による receive 中断は success / failure のどちらにも加算せず result も追加しない。

### EXT-PING-001: Ping client

ICMP Echo Request を送信し、remote IPv4 address、Echo Reply type、identifier、sequence number が対応する Echo Reply を受信した場合だけ success とします。無関係・malformed packet は success としません。

## 5. 実行順序と停止

### EXT-RUN-001: client / server の実行順序

`REQ-REPEAT-001`, `REQ-SERVER-001` を受け、次を contract とします。

- server 群の待受準備を確定してから client loop を開始する。
- client tester は登録順に一項目ずつ実行する。
- repeat count が 0 の場合は停止要求まで反復する。
- stop 時は実行中 client の blocking I/O を中断し、client loop 終了後に server 群を停止する。
- 二重 start / stop で同じ試験を重複実行・重複停止しない。

判断理由は [`ADR-0003`](adr/0003-sequential-client-loop.md) を参照します。

## 6. Result output

### EXT-RESULT-001: result の共通形式

`REQ-RESULT-001` を受け、result は timestamp、role、確定済み local endpoint、protocol、message を含みます。

```text
yyyy/MM/dd HH:mm:ss Role address:port/Protocol message
```

local endpoint 未確定時は不要な `:/` を出力しません。

## 7. NIC 設定変更

### EXT-NIC-001: 変更対象と snapshot

`REQ-NIC-001` を受け、NIC 設定変更は一時的で可逆な操作として扱います。

- 最初の変更直前に NIC ごとの元状態を保持する。
- 同一 NIC を複数回変更しても復元基準は最初の状態とする。
- 変更対象は IP address、subnet mask、default gateway に限定する。

### EXT-NIC-002: 復元

`REQ-NIC-002` を受け、手動復元と通常の window close で元状態へ戻せること、設定途中の failure では復元を試みること、試験実行中は NIC 設定変更操作を無効化することを contract とします。

process kill、application crash、OS 強制終了等で復元処理自体が実行できない場合まで完全な rollback は保証しません。

判断理由は [`ADR-0004`](adr/0004-reversible-nic-configuration.md) を参照します。

## 8. Acceptance と traceability

| 要件 / 外部仕様 | Acceptance | 現在の検証 |
| --- | --- | --- |
| `REQ-PROTOCOL-001` / `EXT-TCP-*` | `AC-TCP-001`: loopback 上で client connect と server accept が成立する | `ConnectivityTests.TestTcpLoopback` |
| `REQ-PROTOCOL-001` / `EXT-UDP-*` | `AC-UDP-001`: loopback 上で datagram send / receive が成立する | `ConnectivityTests.TestUdpLoopback` |
| `REQ-PROTOCOL-001` / `EXT-DNS-001` | `AC-DNS-001`: loopback response は success、stop 中断は count / result を増やさない | `ConnectivityTests.TestDnsLoopback`, `TestDnsCancellation` |
| `REQ-PROTOCOL-001` / `EXT-PING-001` | `AC-PING-001`: 対応する Echo Reply だけを受理する。`AC-PING-002`: malformed / unrelated packet を拒否する | `IcmpEchoPacketTests` |
| `REQ-DESTINATION-001` / `EXT-RESOLVE-001` | `AC-RESOLVE-001`: input boundary と IPv4 selection rule を維持する | `ConnectivityTests.TestRemoteEndpointResolver` |
| `REQ-REPEAT-001` / `EXT-RUN-001` | `AC-RUN-001..003`: 順序、反復、stop、server readiness を維持する | `RunnerTests` |
| `REQ-CONFIG-001` / `EXT-CFG-*` | `AC-CFG-001..002`: 既存 CSV 互換と validation を維持する | `ConfigurationTests` |
| `REQ-RESULT-001` / `EXT-RESULT-001` | `AC-RESULT-001`: result prefix contract を維持する | `ConnectivityTests.TestResultLogFormat` |
| `REQ-NIC-*` / `EXT-NIC-*` | `AC-NIC-001`: snapshot から通常経路で復元できる | 実 NIC に依存するため manual verification |

外部仕様を変更する場合は、必ず先に `REQUIREMENTS.md` の要件変更要否を判断し、要件を変更する場合は要件を確定してからこの文書を更新します。
