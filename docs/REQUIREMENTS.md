# TestConnection 要件定義

この文書は TestConnection が満たすべき**現在の要件**を定義します。実現方法、クラス構成、通信 API、テスト方式はここでは決めません。

要件を具体的な外部仕様へ落とす文書は [`EXTERNAL_DESIGN.md`](EXTERNAL_DESIGN.md)、内部方式は [`INTERNAL_DESIGN.md`](INTERNAL_DESIGN.md) を正本とします。

## 1. 目的

### REQ-SCOPE-001: ネットワーク疎通の観測

Windows 端末から、router、firewall、network redundancy、経路切替等がある環境に対して、指定した送信元・宛先・protocol の疎通状態を繰り返し確認できること。

TestConnection は application health check、load test、packet analyzer、route / firewall rule の自動診断を主目的にしない。

## 2. 機能要件

### REQ-PROTOCOL-001: 対象 protocol

TCP、UDP、DNS、ICMP Echo を用いた疎通確認を実行できること。

protocol ごとに「疎通できた」と判断する観測点が異なるため、外部設計で success / failure の意味を明確にすること。

### REQ-SOURCE-001: 送信元条件

試験時に local IP address を指定できること。指定しない場合は OS の通常の address selection を利用できること。

### REQ-DESTINATION-001: 宛先指定

TCP / UDP / Ping では IPv4 address または hostname / FQDN を宛先として指定できること。DNS では問い合わせ先 DNS server を明示できること。

### REQ-REPEAT-001: 複数試験と反復

複数の疎通試験を登録し、一定の順序と間隔で繰り返し実行できること。利用者が停止を要求した場合は、実行中の通信を安全に中断して終了できること。

### REQ-SERVER-001: 受信側観測

TCP / UDP については server として待受でき、client 側だけでは確認できない受信側の成立を観測できること。

### REQ-CONFIG-001: 試験設定の保存

試験定義を file へ保存し、後から読み込んで再利用できること。既存の TestConnection 設定 file との互換性を維持すること。

### REQ-RESULT-001: 結果の観測

各試行の success / failure と接続条件を、利用者が時系列で確認できること。画面表示と file 出力を利用できること。

### REQ-NIC-001: NIC 設定の一時変更

試験条件として必要な場合、Windows NIC の IP address、subnet mask、default gateway を一時的に変更できること。

### REQ-NIC-002: NIC 設定の復元

TestConnection が変更した NIC 設定は、通常の操作・終了経路で変更前状態へ復元できること。変更していない NIC 設定まで管理対象へ広げないこと。

## 3. 非機能・制約要件

### REQ-RUNTIME-001: 実行環境

Windows 11 22H2 以降の標準環境で、利用者が TestConnection のためだけに追加 runtime を導入せず実行できること。

### REQ-DISTRIBUTION-001: 配布

installer を必須とせず、ZIP を展開して実行できること。配布物は個人で把握・保守できる単純な構成に保つこと。

### REQ-MAINTAIN-001: 保守性

将来拡張だけを目的とした abstraction、framework、compatibility layer、設定項目を増やさず、要件・外部設計・内部設計・実装・テストの対応を人間が追跡できること。

### REQ-TESTABILITY-001: 決定的な検証

通常の automated test は外部 host や実 NIC の偶然の状態へ依存せず、可能な範囲で loopback や pure logic を用いて決定的に検証できること。実 NIC 等、自動化が適切でない要件は manual verification として明示すること。

## 4. 要件から設計への対応

要件は「何を満たすか」を定義し、具体的な observable contract は `EXTERNAL_DESIGN.md` で定義します。

| 要件 | 主な外部仕様 |
| --- | --- |
| `REQ-SCOPE-001` | `EXT-SCOPE-001` |
| `REQ-PROTOCOL-001` | `EXT-TCP-*`, `EXT-UDP-*`, `EXT-DNS-001`, `EXT-PING-001` |
| `REQ-SOURCE-001` | `EXT-ENDPOINT-001` |
| `REQ-DESTINATION-001` | `EXT-CFG-001`, `EXT-RESOLVE-001` |
| `REQ-REPEAT-001`, `REQ-SERVER-001` | `EXT-RUN-001` |
| `REQ-CONFIG-001` | `EXT-CFG-001`, `EXT-CFG-002` |
| `REQ-RESULT-001` | `EXT-RESULT-001` |
| `REQ-NIC-001`, `REQ-NIC-002` | `EXT-NIC-001`, `EXT-NIC-002` |
| `REQ-RUNTIME-001`, `REQ-DISTRIBUTION-001` | `EXT-RUNTIME-001` |
| `REQ-MAINTAIN-001`, `REQ-TESTABILITY-001` | 文書体系、内部設計、acceptance / test 方針全体 |

要件を変更するときは、先にこの文書を変更して変更後の要求を確定し、その後に外部設計へ進みます。外部設計、内部設計、source code、test の現在状態から要件を逆算して後付けしません。
