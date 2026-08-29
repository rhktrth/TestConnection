# Architecture

## 目的と設計境界

この文書は、[`SPEC.md`](SPEC.md) の外部 contract を実現するために TestConnection が維持する内部構成、component responsibility、lifecycle、concurrency、resource ownership、error handling を定義します。

利用者、接続相手、OS、設定 file、result output から観測できる contract は `SPEC.md` を正本とします。この文書では外部仕様の詳細を再掲せず、必要な `EXT-*` / `AC-*` を参照して内部設計を説明します。

TestConnection は Windows 端末から見た network connectivity を観測する WinForms application です。application protocol 固有の正常性、throughput、packet capture、route diagnosis 等を内部責務へ広げません。外部 scope は `EXT-SCOPE-001` を参照してください。

## Runtime baseline

runtime / UI baseline は [`ADR-0001`](adr/0001-winforms-net481-zip.md) を正本とします。target framework は `net481`、project format は SDK-style、UI framework は WinForms です。

## 設計原則

### 外部 contract を内部抽象化より優先する

protocol ごとの success contract は `EXT-TCP-*`, `EXT-UDP-*`, `EXT-DNS-001`, `EXT-PING-001` を正本とします。共通化によって各 protocol の判定点や error boundary が見えにくくなる場合は、見た目の重複を残しても protocol ごとの実装を分けます。

判定を protocol の最小成立点に限定する理由は [`ADR-0002`](adr/0002-minimal-protocol-success.md) を参照します。

### 変更理由が同じものだけを共通化する

単に code が似ていることではなく、同じ contract / lifecycle / resource ownership によって一緒に変わる責務だけを共通化します。将来拡張のためだけの service layer、DI framework、strategy registry、generic state engine 等は置きません。

### source address を試験条件として扱う

`EXT-ENDPOINT-001` に従い、local address の指定有無を tester definition の contract として扱います。具象 tester が独自に source address policy を持たず、共通の endpoint 生成処理を使用します。

### OS 状態変更は通常の tester lifecycle と分離する

NIC 設定変更は socket test より高リスクな OS state change です。`EXT-NIC-*` を実現する専用 service と snapshot を持ち、通常の tester model に混ぜません。判断理由は [`ADR-0004`](adr/0004-reversible-nic-configuration.md) を参照します。

## Component design

### INT-UI-001: MainForm

`MainForm` は WinForms presentation と application orchestration の入口を担当します。

- tester definition の追加・削除
- `TestSession` への tester 生成、開始・停止の指示
- result 表示、file 出力、統計表示、効果音
- NIC の一時設定・復元の UI 操作
- background tester からの result 通知を WinForms UI thread へ marshal すること

CSV parsing / validation、tester lifecycle、protocol I/O を UI event handler の中へ直接埋め込みません。

### INT-CFG-001: Configuration

`TesterDefinition` は一つの tester の設定値を表し、role、local / remote endpoint、protocol、port の内部 model とします。各 tester はこの definition を保持し、同じ値を別 field として重複管理しません。

`TesterDefinitionFile` は `EXT-CFG-002` の CSV parse / serialize と行番号付き format error の生成を担当します。`TesterDefinitionValidator` は `EXT-CFG-001` の role / protocol / endpoint / port validation を担当します。

CSV 専用の中間 service や、存在しない将来 format のための abstraction は置きません。

### INT-SESSION-001: TestSession

`TestSession` は一回の試験実行における tester の生成と client / server lifecycle を調停します。

- definition を validation して `TesterFactory` で具象 tester を生成する。
- result / success / failure callback を接続する。
- `EXT-RUN-001` に従い、開始時は server 群の ready / failure を確定してから client loop を開始する。
- 停止時は client loop を停止してから server 群を停止する。
- 二重 `Start()` / `Stop()` で同じ実行を重複して開始・停止しない。
- 実行対象の success / failure count をまとめて clear する。

server 群の単純な `foreach` だけを包む別 runner は置きません。server thread lifecycle は `TesterServer` が所有します。

### INT-RUNNER-001: ClientTestRunner

`ClientTestRunner` は client tester 一覧と item / list interval を受け取り、一つの background worker で登録順に `RunOnce()` を呼びます。

- finite repeat は指定回数で終了する。
- repeat count 0 は停止要求まで反復する。
- interval 待機は停止 event で中断できる。
- stop は実行中 tester の `Cancel()` を呼び、worker 終了まで待つ。

これにより `AC-RUN-001` / `AC-RUN-002` を満たします。順次実行を選ぶ理由は [`ADR-0003`](adr/0003-sequential-client-loop.md) を参照します。

### INT-SERVER-001: TesterServer

`TesterServer` は server thread、起動同期、停止順序を共通管理します。

- `Start()` は listener / socket の初期化が成功または失敗するまで caller を待たせる。
- `Listen()` は具象 tester が protocol 固有の blocking I/O を実装する。
- `Stop()` は停止要求を設定し、`StopListening()` で blocking I/O を解除し、server thread の終了を待つ。
- stop に伴う listener / socket close は通常の通信 failure として数えない。

この lifecycle が `AC-RUN-003` を支えます。

### INT-TESTER-001: Tester model

`TesterBase` は `TesterDefinition`、timeout、success / failure count、共通 result formatting と通知を所有します。

`TesterClient` は一回の試行を `RunOnce()` として実行し、`Cancel()` で現在の blocking I/O を解除します。cancel は通常の failure と区別し、停止のための中断で count / result を増やしません。

`TesterFactory` は validation 済み definition から TCP / UDP / DNS / Ping の具象 tester を選びます。protocol 数が少ないため単純な分岐を維持し、registry / DI framework は導入しません。

### INT-RESOLVE-001: RemoteEndpointResolver

TCP / UDP / Ping client の remote endpoint policy は `RemoteEndpointResolver` に集約し、具象 tester ごとに hostname selection rule を重複実装しません。

- input boundary と address selection は `EXT-RESOLVE-001` を実現する。
- resolution は各試行の直前に行い、その試行で使用する endpoint を tester へ返す。
- DNS tester は `EXT-CFG-001` により remote DNS server が IP address に限定されるため、この resolver を使用しない。

### INT-NIC-001: NicConfigurationService

`NicConfigurationService` は Windows NIC の現在状態取得、設定、復元を担当します。UI は WMI 操作の詳細を持たず、この service を通じて `EXT-NIC-*` を実現します。

snapshot は NIC ごとの元状態を表し、DHCP / static、IP address、subnet mask、default gateway、gateway metric 等、復元に必要な値を保持します。TestConnection が変更しない設定項目まで ownership を広げません。

## Protocol implementation boundaries

外部の success 条件そのものは `SPEC.md` を正本とし、ここでは component boundary だけを示します。

| 外部 contract | component | 内部責務 |
| --- | --- | --- |
| `EXT-TCP-001` | `TcpTesterClient` | local bind、remote resolve、connect、timeout / cancel、resource cleanup |
| `EXT-TCP-002` | `TcpTesterServer` | listen / accept、accepted connection の cleanup |
| `EXT-UDP-001` | `UdpTesterClient` | local bind、remote resolve、datagram send、cleanup |
| `EXT-UDP-002` | `UdpTesterServer` | datagram receive、stop による socket unblock |
| `EXT-DNS-001` | `DnsTesterClient` | fixed query send、UDP response wait、timeout / cancel、cleanup |
| `EXT-PING-001` | `PingTesterClient`, `IcmpEchoPacket` | raw ICMP send / receive と request / reply correlation |

protocol-specific class は、各 contract に必要な通信だけを担当します。application-level protocol や将来用の generalized connectivity engine へ責務を広げません。

## 起動・停止 sequence

`INT-SESSION-001`, `INT-RUNNER-001`, `INT-SERVER-001` の組合せで `EXT-RUN-001` を実現します。

開始:

1. definition を validation して tester を生成する。
2. server を登録順に `Start()` し、それぞれ ready / failure を確定する。
3. server 初期化結果が確定した後に client runner を開始する。

停止:

1. client runner へ stop を要求する。
2. 実行中 client の `Cancel()` で blocking I/O を解除する。
3. client worker の終了を待つ。
4. server を登録順に停止し、listener / socket close で blocking I/O を解除する。
5. server thread の終了を待つ。

`Thread.Abort()` は使用しません。

## Resource lifecycle

TCP / UDP / DNS / Ping client は normal / error / timeout / cancel の各経路を共通の cleanup へ収束させます。socket / client / wait handle 等の ownership を曖昧にしません。

`Cancel()` による close は通常の通信 failure と区別します。停止要求のために閉じた resource が原因の exception を success / failure count へ反映しません。

TCP / UDP server は listener / socket を server thread の resource として扱い、stop 時に owner が閉じて blocking accept / receive を解除します。

## Result model

`TesterBase` は `EXT-RESULT-001` の共通 prefix を組み立てます。具象 tester は protocol-specific message だけを渡し、timestamp / role / local endpoint / protocol の formatting を重複実装しません。

remote hostname の解決結果を result に含める責務は `EXT-RESOLVE-001` に従って各 client tester が持ちます。

## 設計変更時の扱い

- 外部から観測できる挙動を変える必要が出た場合は、この文書だけを先に変えず `SPEC.md` へ戻る。
- component responsibility、lifecycle、resource ownership、error propagation を変える場合は、production code より先に該当 `INT-*` を更新する。
- 重要な判断に複数案がある場合だけ ADR を更新する。
- class 名、private helper、局所的な buffer size 等、contract ではない偶然の implementation detail を設計書へ固定しない。
