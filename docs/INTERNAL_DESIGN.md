# TestConnection 内部設計

この文書は、[`EXTERNAL_DESIGN.md`](EXTERNAL_DESIGN.md) の外部 contract を実現するために維持する内部構成、component responsibility、lifecycle、concurrency、resource ownership、error handling を定義します。

利用者・接続相手・OS・設定 file・result output から観測できる事項は外部設計を正本とし、この文書へ重複記載しません。

## 1. 設計方針

### INT-BOUNDARY-001: 要件・外部仕様を実装都合で変更しない

内部構造は `REQUIREMENTS.md` と `EXTERNAL_DESIGN.md` を満たすために決めます。実装しやすいという理由だけで上位仕様を暗黙に変更しません。

### INT-SIMPLE-001: 変更理由が同じものだけを共通化する

単に code が似ているだけでは共通化しません。contract、lifecycle、resource ownership が同じ理由で変わるものだけを共通化します。将来拡張だけの service layer、DI framework、registry 等は置きません。

## 2. Component responsibility

### INT-UI-001: MainForm

`MainForm` は WinForms presentation と application orchestration の入口を担当します。

- tester definition の追加・削除
- `TestSession` への開始・停止指示
- result 表示、file 出力、統計表示、効果音
- NIC の一時設定・復元の UI 操作
- background thread からの通知を UI thread へ marshal する

CSV parsing、validation、protocol I/O を UI event handler へ直接埋め込みません。

### INT-CFG-001: Configuration

`TesterDefinition` は role、local / remote endpoint、protocol、port の内部 model とします。

- `TesterDefinitionFile` は `EXT-CFG-002` の CSV parse / serialize と行番号付き format error を担当する。
- `TesterDefinitionValidator` は `EXT-CFG-001` の validation を担当する。
- 同じ設定値を tester 側で別 field として重複管理しない。

### INT-SESSION-001: TestSession

`TestSession` は一回の試験実行における tester 生成と client / server lifecycle を調停します。

- definition を validation し、`TesterFactory` で具象 tester を生成する。
- callback を接続する。
- `EXT-RUN-001` に従い server readiness を確定してから client loop を開始する。
- stop 時は client loop を終了してから server 群を停止する。
- 二重 start / stop を防止する。

### INT-RUNNER-001: ClientTestRunner

`ClientTestRunner` は一つの background worker で client tester を登録順に実行します。

- finite repeat は指定回数で終了する。
- repeat count 0 は stop まで反復する。
- interval wait は stop event で解除できる。
- stop は現在実行中 tester の `Cancel()` を呼び、worker 終了まで待つ。

### INT-SERVER-001: TesterServer

`TesterServer` は server thread、起動同期、停止順序を共通管理します。

- `Start()` は listener / socket の初期化結果が確定するまで caller を待たせる。
- protocol-specific blocking I/O は具象 server が担当する。
- stop 時は listener / socket close で blocking I/O を解除し、thread 終了を待つ。
- stop に伴う close を通信 failure として数えない。

### INT-TESTER-001: Tester model

`TesterBase` は definition、timeout、success / failure count、共通 result formatting と通知を所有します。

`TesterClient` は一回の試行を `RunOnce()` として実行し、`Cancel()` で現在の blocking I/O を解除します。stop による cancellation は通常 failure と区別します。

`TesterFactory` は validation 済み definition から TCP / UDP / DNS / Ping の具象 tester を単純な分岐で生成します。

### INT-RESOLVE-001: RemoteEndpointResolver

TCP / UDP / Ping client の hostname resolution と IPv4 selection rule を `RemoteEndpointResolver` に集約します。

- `EXT-RESOLVE-001` を一箇所で実現する。
- resolution は各試行の直前に行う。
- DNS tester はこの resolver を使用しない。

### INT-NIC-001: NicConfigurationService

`NicConfigurationService` は Windows NIC の現在状態取得、設定、復元を担当します。

- `EXT-NIC-*` を実現する snapshot を NIC ごとに保持する。
- TestConnection が変更しない DNS 等の設定へ ownership を広げない。
- UI は WMI 操作の詳細を持たない。

## 3. Protocol implementation boundary

| 外部仕様 | 主な component | 内部責務 |
| --- | --- | --- |
| `EXT-TCP-001` | `TcpTesterClient` | local bind、remote resolve、connect、timeout / cancel、cleanup |
| `EXT-TCP-002` | `TcpTesterServer` | listen / accept、accepted connection cleanup |
| `EXT-UDP-001` | `UdpTesterClient` | local bind、remote resolve、datagram send、cleanup |
| `EXT-UDP-002` | `UdpTesterServer` | datagram receive、stop による socket unblock |
| `EXT-DNS-001` | `DnsTesterClient` | fixed query send、response wait、timeout / cancel、cleanup |
| `EXT-PING-001` | `PingTesterClient`, `IcmpEchoPacket` | raw ICMP send / receive と request / reply correlation |

protocol-specific class は各 contract に必要な通信だけを担当し、application-level protocol へ責務を広げません。

## 4. 起動・停止 sequence

開始:

1. definition を validation して tester を生成する。
2. server を登録順に `Start()` し、ready / failure を確定する。
3. client runner を開始する。

停止:

1. client runner へ stop を要求する。
2. current client の `Cancel()` で blocking I/O を解除する。
3. client worker 終了を待つ。
4. server を停止し blocking I/O を解除する。
5. server thread 終了を待つ。

`Thread.Abort()` は使用しません。

## 5. Resource lifecycle

TCP / UDP / DNS / Ping client は normal / error / timeout / cancel の各経路を cleanup へ収束させます。resource owner を曖昧にしません。

stop のために close された socket 等が発生させる exception を success / failure として扱いません。

## 6. Result model

`TesterBase` は `EXT-RESULT-001` の共通 prefix を組み立てます。具象 tester は protocol-specific message だけを生成し、共通 formatting を重複実装しません。

## 7. 上位仕様との traceability

| Internal design | 主な上位仕様 |
| --- | --- |
| `INT-UI-001` | `REQ-RESULT-001`, `REQ-NIC-*`, `EXT-RESULT-001`, `EXT-NIC-*` |
| `INT-CFG-001` | `REQ-CONFIG-001`, `EXT-CFG-*` |
| `INT-SESSION-001`, `INT-RUNNER-001`, `INT-SERVER-001` | `REQ-REPEAT-001`, `REQ-SERVER-001`, `EXT-RUN-001` |
| `INT-TESTER-001` | `REQ-PROTOCOL-001`, `EXT-TCP-*`, `EXT-UDP-*`, `EXT-DNS-001`, `EXT-PING-001` |
| `INT-RESOLVE-001` | `REQ-DESTINATION-001`, `EXT-RESOLVE-001` |
| `INT-NIC-001` | `REQ-NIC-*`, `EXT-NIC-*` |

内部設計を変更するときは、先に `REQUIREMENTS.md` と `EXTERNAL_DESIGN.md` が変更不要かを確認します。必要なら上位文書から順に改訂し、内部設計を確定してから source code を変更します。
