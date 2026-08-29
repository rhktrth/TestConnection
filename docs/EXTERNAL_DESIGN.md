# TestConnection 外部設計

この文書は、[`REQUIREMENTS.md`](REQUIREMENTS.md) で定めた要件を、利用者、接続相手、Windows、設定ファイル、結果出力から観測できる**外部仕様**へ具体化します。

内部でどのように実現するかは [`INTERNAL_DESIGN.md`](INTERNAL_DESIGN.md)、ビルド・テスト・配布・リリースの手順は [`OPERATIONS.md`](OPERATIONS.md) を正本とします。

## 1. 目的と範囲

### EXT-SCOPE-001: 疎通確認の対象

`REQ-SCOPE-001` に基づき、TestConnection は Windows 端末から TCP / UDP / DNS / ICMP Echo の通信を発生させ、指定した送信元と宛先の間で、各プロトコルの最小成立点まで通信できたかを確認します。

アプリケーション固有のハンドシェイク、認証、業務処理、性能測定、パケット解析、経路診断、長時間の TCP セッション維持は基本的な判定対象にしません。

### EXT-RUNTIME-001: 実行環境と配布形態

`REQ-RUNTIME-001`, `REQ-DISTRIBUTION-001` に基づき、次を外部仕様とします。

- Windows 11 22H2 以降で動作する。
- Microsoft .NET Framework 4.8.1 を使用する。
- 配布単位は展開して実行できる ZIP とし、インストーラーや ClickOnce を前提にしない。
- UI は WinForms とし、表示はライトモード固定とする。
- TCP / UDP / DNS は通常権限で利用できる。raw ICMP socket と NIC 設定変更は、環境によって管理者権限を必要とする。

実行環境を選ぶ理由と変更条件は [`ADR-0001`](adr/0001-winforms-net481-zip.md) を参照します。

## 2. 試験定義と設定ファイル

### EXT-CFG-001: 有効な試験定義

`REQ-PROTOCOL-001`, `REQ-SOURCE-001`, `REQ-DESTINATION-001` に基づき、一つの試験定義は役割、ローカル IP アドレス、接続先、プロトコル、ポート番号を持ちます。

- `Server` は TCP / UDP だけを受け付ける。
- `Client` は TCP / UDP / DNS / Ping を受け付ける。
- ローカル IP アドレスが空でない場合は、IP アドレスとして解釈できなければならない。
- TCP / UDP / Ping クライアントの接続先は IPv4 アドレスまたはホスト名 / FQDN を受け付ける。IPv6 リテラルは受け付けない。
- DNS クライアントの接続先は問い合わせ先 DNS サーバーの IP アドレスとし、ホスト名は受け付けない。
- Ping 以外のポート番号は 0 から 65535 の範囲とする。

### EXT-CFG-002: CSV の保存・読込み

`REQ-CONFIG-001` に基づき、設定ファイルは 1 行を 1 つの試験定義とする CSV とします。

```text
Role,LocalIpAddress,RemoteIpAddress,Protocol,Port
```

- 行頭が `#` の行は読込み時にコメントとして無視する。
- 保存時は試験定義だけを出力し、コメント行は再出力しない。
- ホスト名 / FQDN は解決済み IP アドレスへ置換せず、利用者が指定した文字列を保存する。
- 無効な行は行番号を含む形式エラーとして扱う。

## 3. 接続先と送信元

### EXT-ENDPOINT-001: ローカル側のアドレス

`REQ-SOURCE-001` に基づき、クライアントとサーバーはローカル IP アドレスが指定されていればそのアドレスを使用し、未指定の場合は OS の通常のアドレス選択や待受動作を利用します。

### EXT-RESOLVE-001: 接続先の名前解決

`REQ-DESTINATION-001` に基づき、TCP / UDP / Ping クライアントのホスト名 / FQDN は各試行の直前に名前解決します。

- 複数の IPv4 アドレスが得られた場合は、数値として昇順に並べた先頭を使用する。
- IPv4 アドレスが一つも得られない場合は、その試行を失敗とする。
- TCP / UDP の結果には、実際に使用した IPv4 アドレスを出力する。
- Ping の結果には `hostname(IPv4 address)` の形式で解決結果を出力する。
- DNS クライアントは名前解決の対象外とする。

## 4. プロトコルごとの成功条件

`REQ-PROTOCOL-001` を具体化し、成功はアプリケーション全体の正常性ではなく、各プロトコルで TestConnection が確認する最小成立点を意味します。判断理由は [`ADR-0002`](adr/0002-minimal-protocol-success.md) を参照します。

### EXT-TCP-001: TCP クライアント

指定した接続先への TCP 接続が確立した時点で成功とします。アプリケーションデータは送信せず、接続は成立確認後に保持しません。

### EXT-TCP-002: TCP サーバー

TCP 接続を受け付けた時点で成功とします。アプリケーションデータは交換しません。

### EXT-UDP-001: UDP クライアント

小さなデータグラムの送信をローカル OS が受け付けた時点で成功とします。この成功は接続先への到達を保証しません。

### EXT-UDP-002: UDP サーバー

データグラムを受信した時点で成功とします。受信データのアプリケーション上の妥当性は判定しません。

### EXT-DNS-001: DNS クライアント

指定した DNS サーバーへ固定の A レコード問い合わせを UDP で送信し、その送信先から UDP 応答を受信した時点で成功とします。

- transaction ID、RCODE、answer の内容は成功判定に含めない。
- タイムアウトやソケットエラーで応答を受信できなければ失敗とする。
- 停止要求によって受信待ちが中断された場合は、成功・失敗のどちらにも加算せず、結果も追加しない。

### EXT-PING-001: Ping クライアント

ICMP Echo Request を送信し、送信先 IPv4 アドレス、Echo Reply の type、identifier、sequence number が対応する Echo Reply を受信した場合だけ成功とします。無関係なパケットや不正なパケットは成功としません。

## 5. 実行順序と停止

### EXT-RUN-001: クライアントとサーバーの実行順序

`REQ-REPEAT-001`, `REQ-SERVER-001` に基づき、次を外部仕様とします。

- サーバー群の待受準備が完了または失敗したことを確定してから、クライアントの反復実行を開始する。
- クライアント試験は登録順に一項目ずつ実行する。
- repeat count が 0 の場合は、停止要求があるまで反復する。
- 停止時は実行中クライアントの待ち状態を中断し、クライアント側の処理終了後にサーバー群を停止する。
- 二重の開始・停止によって同じ試験を重複実行・重複停止しない。

判断理由は [`ADR-0003`](adr/0003-sequential-client-loop.md) を参照します。

## 6. 結果出力

### EXT-RESULT-001: 結果の共通形式

`REQ-RESULT-001` に基づき、結果には時刻、役割、確定済みのローカル接続先、プロトコル、メッセージを含めます。

```text
yyyy/MM/dd HH:mm:ss Role address:port/Protocol message
```

ローカル接続先が未確定の場合は、不要な `:/` を出力しません。

## 7. NIC 設定変更

### EXT-NIC-001: 変更対象と変更前状態の保持

`REQ-NIC-001` に基づき、NIC 設定変更は一時的で元に戻せる操作として扱います。

- 最初の変更直前に NIC ごとの元状態を保持する。
- 同一 NIC を複数回変更しても、復元基準は最初の状態とする。
- 変更対象は IP アドレス、サブネットマスク、デフォルトゲートウェイに限定する。

### EXT-NIC-002: 復元

`REQ-NIC-002` に基づき、手動復元と通常のウィンドウ終了時に元状態へ戻せること、設定途中で失敗した場合は復元を試みること、試験実行中は NIC 設定変更操作を無効化することを外部仕様とします。

プロセス強制終了、アプリケーション異常終了、OS 強制終了など、復元処理自体を実行できない場合まで完全な復元は保証しません。

判断理由は [`ADR-0004`](adr/0004-reversible-nic-configuration.md) を参照します。

## 8. 受入条件と追跡関係

| 要件 / 外部仕様 | 受入条件 | 現在の検証 |
| --- | --- | --- |
| `REQ-PROTOCOL-001` / `EXT-TCP-*` | `AC-TCP-001`: ループバック上でクライアント接続とサーバー受付が成立する | `ConnectivityTests.TestTcpLoopback` |
| `REQ-PROTOCOL-001` / `EXT-UDP-*` | `AC-UDP-001`: ループバック上でデータグラム送受信が成立する | `ConnectivityTests.TestUdpLoopback` |
| `REQ-PROTOCOL-001` / `EXT-DNS-001` | `AC-DNS-001`: ループバックからの応答は成功となり、停止による中断では成功・失敗件数や結果を増やさない | `ConnectivityTests.TestDnsLoopback`, `TestDnsCancellation` |
| `REQ-PROTOCOL-001` / `EXT-PING-001` | `AC-PING-001`: 対応する Echo Reply だけを受理する。`AC-PING-002`: 不正・無関係なパケットを拒否する | `IcmpEchoPacketTests` |
| `REQ-DESTINATION-001` / `EXT-RESOLVE-001` | `AC-RESOLVE-001`: 入力条件と IPv4 選択規則を維持する | `ConnectivityTests.TestRemoteEndpointResolver` |
| `REQ-REPEAT-001` / `EXT-RUN-001` | `AC-RUN-001..003`: 順序、反復、停止、サーバー準備完了待ちを維持する | `RunnerTests` |
| `REQ-CONFIG-001` / `EXT-CFG-*` | `AC-CFG-001..002`: 既存 CSV との互換性と入力検証を維持する | `ConfigurationTests` |
| `REQ-RESULT-001` / `EXT-RESULT-001` | `AC-RESULT-001`: 結果の共通接頭部の仕様を維持する | `ConnectivityTests.TestResultLogFormat` |
| `REQ-NIC-*` / `EXT-NIC-*` | `AC-NIC-001`: 保持した変更前状態へ通常経路で復元できる | 実 NIC に依存するため手動確認 |

外部仕様を変更する場合は、必ず先に `REQUIREMENTS.md` の要件変更要否を判断します。要件を変更する場合は、要件を確定してからこの文書を更新します。
