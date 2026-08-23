# Architecture

## 目的と設計境界

TestConnection は、Windows 端末から見たネットワーク疎通を、送信元・宛先・protocol を明示して繰り返し観測するための WinForms アプリケーションです。主な対象は router、firewall、network redundancy、経路切替等によって通信可否が変化する環境です。

中心に置くのは application の正常性ではなく、「指定した endpoint 間で、その protocol の通信がどこまで成立したか」という観測です。application protocol 固有の認証・業務処理、throughput、長時間 session 維持は基本的な判定対象にしません。

TestConnection は次を主目的にしません。

- application health check
- load / performance test
- packet capture / protocol analyzer
- route discovery や firewall rule の自動診断
- 既存 TCP session を保持した stateful failover test

## Runtime baseline

利用者が Windows 11 の標準環境で追加 runtime なしに実行できることを優先し、target framework は `net481`（.NET Framework 4.8.1）に固定します。project format は SDK-style、UI framework は WinForms です。

この判断と変更条件は [`ADR-0001`](adr/0001-winforms-net481-zip.md) を正本とします。

## 設計原則

### Protocol の最小成立点を観測する

各 tester は疎通確認に必要な最小限の通信だけを行い、上位層の処理を成功条件へ広げません。protocol ごとに success の意味は異なり、特に UDP の送信成功を遠端到達と同一視しません。

詳細は [`ADR-0002`](adr/0002-minimal-protocol-success.md) を正本とします。

### 送信元を試験条件として扱う

client tester は local IP address が指定されていれば、その address に bind して通信します。未指定の場合は OS に送信元 address の選択を委ねます。

server tester は local IP address が指定されていればその address で待受し、未指定の場合は任意の local address で待受します。

### 観測の時系列を優先する

client 試験は一つの background worker で登録順に一項目ずつ実行し、指定間隔を挟んで反復します。server 群は client loop とは独立した background thread で継続待受します。

各 server の `Start()` は listener / socket の初期化完了または失敗まで待ってから return し、client loop が開始される時点で server の起動結果が確定していることを保証します。

詳細は [`ADR-0003`](adr/0003-sequential-client-loop.md) を正本とします。

### OS 状態の変更は可逆にする

NIC 設定変更は通常の application state 変更より高リスクな境界です。TestConnection が NIC を変更する場合は変更直前の状態を snapshot として保持し、手動復元または終了時に元へ戻すことを前提とします。

詳細は [`ADR-0004`](adr/0004-reversible-nic-configuration.md) を正本とします。

## 実行モデル

### MainForm

`MainForm` は WinForms の presentation と application の入口を担当します。

- tester 定義の追加・削除
- `TestSession` への tester 生成、開始・停止の指示
- result 表示、file 出力、統計表示、効果音
- NIC の一時設定と復元の UI 操作
- background tester からの result 通知を WinForms UI thread へ marshal すること

CSV の解釈と definition validation は `Configuration` 配下、tester の生成と client / server の実行順序は `TestSession` / `TesterFactory`、protocol 固有の通信処理は各 tester が担当します。UI 固有処理と疎通判定を同一ロジックへ混ぜません。

### Configuration

`TesterDefinition` は一つの tester の設定値を表し、role、local / remote address、protocol、port の正本です。各 tester はこの definition を保持し、同じ設定値を別 field として重複管理しません。

`TesterDefinitionFile` は既存5列CSVの読み書き、列値の解釈、行番号付き format error の生成を担当します。`TesterDefinitionValidator` は role / protocol の組合せ、local / remote address、port を検証します。CSV 専用の中間 service や将来形式のための abstraction は置きません。

### TestSession / ClientTestRunner

`TestSession` は一回の試験実行における tester 生成と client / server lifecycle を調停します。

- definition を検証して `TesterFactory` で具象 tester を生成し、result / success / failure callback を接続する
- 開始時は server 群を登録順に開始してから client loop を開始する
- 停止時は client loop を停止してから server 群を登録順に停止する
- 二重 `Start()` / `Stop()` で同じ実行を重複して開始・停止しない
- 実行対象の success / failure count をまとめて clear する

server 群の単純な foreach だけを包む別 runner は置きません。server thread lifecycle は各 `TesterServer` が管理します。

`ClientTestRunner` は構築時に client tester 一覧と item / list interval を受け取り、一つの background worker で登録順に `Try()` を呼びます。repeat count が 0 の場合は停止要求まで反復します。停止時は `ManualResetEvent` で interval 待機を解除し、実行中 tester の `Cancel()` を呼んで worker 終了まで待ちます。

### Tester model

`TesterBase` は `TesterDefinition`、timeout、success / failure count、result の共通整形と通知を持ちます。WinForms DataBinding との互換性のため、local / remote address、role、protocol、port は definition を参照する read-only property として公開します。

`TesterClient` は一回の試行を `Try()` として実行し、停止時に現在の blocking I/O を解除するための `Cancel()` を持ちます。

`TesterServer` は server thread、起動同期、停止順序を共通管理します。具象 TCP / UDP server は `Listen()` に protocol 固有の待受処理、`StopListening()` に blocking I/O を解除する最小限の socket / listener close だけを実装します。

`TesterFactory` は validation 済みの `TesterDefinition` から TCP / UDP / DNS / Ping の具象 tester を選択し、timeout を設定します。protocol 数が少ないため単純な `switch` を維持し、DI framework や strategy registry は置きません。

## 起動・停止順序

試験開始時は `TestSession` が server 群を先に起動し、その後 client loop を開始します。同一 TestConnection 内の client / server を組み合わせる場合も、受信側を先に準備します。

停止時は client loop を止めてから server 群を停止します。client loop は停止 event と実行中 tester の `Cancel()` による協調停止で、`Thread.Abort()` は使用しません。server は listener / socket を閉じて blocking accept / receive を解除し、background thread の終了を待ちます。

## Protocol ごとの success semantics

| Protocol / role | success とする現在の観測 | success が意味しないこと |
| --- | --- | --- |
| TCP client | 指定 endpoint への TCP connection が確立した | application protocol、認証、業務処理の成功 |
| TCP server | TCP connection を accept した | 接続元 application の正常性 |
| UDP client | local OS が datagram 送信を受け付けた | datagram の遠端到達、応答受信 |
| UDP server | datagram を受信した | application payload の妥当性 |
| DNS client | 固定 DNS query を送信し UDP response を受信した | transaction ID、RCODE、answer 内容の妥当性 |
| Ping client | 指定先へ送信した ICMP Echo Request に対応する Echo Reply を受信した | application port、上位 application の正常性 |

TCP client / server は connection 成立後に application data を交換せず、直ちに connection を閉じます。

UDP client は小さな datagram を送信します。遠端到達を確認する場合は UDP server、packet capture、firewall log 等の別観測点を組み合わせます。

DNS client は `A.ROOT-SERVERS.NET` の A record に相当する固定 query を使用します。response payload の内容解析は success 判定に含めません。

Ping は raw ICMP socket を使用し、送信先 address、ICMP type=Echo Reply、identifier、sequence number が送信した Echo Request と対応する packet だけを success とします。

## Remote endpoint resolution

TCP / UDP / Ping client の remote endpoint は、IPv4 literal または hostname / FQDN という設定値として保持します。hostname は tester 作成時に固定IPへ変換せず、各 `Try()` の冒頭で `Dns.GetHostAddresses` により解決します。

名前解決結果に複数の IPv4 address が含まれる場合、address bytes を数値昇順で比較した最小の IPv4 address を選びます。IPv6 address は remote endpoint の候補にしません。DNS tester の remote 欄は問い合わせ先 DNS server なので、IP literal のままとします。

## Resource lifecycle

TCP / UDP / DNS / Ping client は normal / error / timeout / cancel の経路で socket / client / wait handle 等を共通の cleanup 経路へ収束させます。`Cancel()` による resource close は通常の通信 failure と区別し、cancel時は success / failure count と result を追加しません。

TCP / UDP server は停止要求時に listener / socket を閉じて blocking accept / receive を解除します。socket close による通常の中断は停止経路として扱い、通信 failure として数えません。

## Result model

各 tester は success count と failure count を保持し、protocol ごとの判定点で更新します。result message は `TesterBase` で timestamp、role、確定済み local endpoint、protocol を付加して callback へ渡します。

local endpoint が確定している通常の形式は `yyyy/MM/dd HH:mm:ss Role address:port/Protocol message` です。endpoint がまだ確定していない場合は不要な `:/` を出力しません。
