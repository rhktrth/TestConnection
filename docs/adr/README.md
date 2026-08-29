# アーキテクチャ決定記録（ADR）

`docs/adr/` は、TestConnection の現在有効な重要な設計判断について、「なぜその判断を選んだか」と「どの前提が変われば見直すか」を記録する場所です。

ADR は変更履歴の台帳にはしません。判断が変わった場合は、現在の設計を最も簡潔に表すよう編集・統合・削除します。過去の経緯は Git history、Issue、PR を参照します。

正本の役割分担は次のとおりです。

- 要件: [`../REQUIREMENTS.md`](../REQUIREMENTS.md)
- 外部仕様: [`../EXTERNAL_DESIGN.md`](../EXTERNAL_DESIGN.md)
- 内部設計: [`../INTERNAL_DESIGN.md`](../INTERNAL_DESIGN.md)
- ビルド・テスト・配布・リリース: [`../OPERATIONS.md`](../OPERATIONS.md)
- 判断理由と見直し条件: この `adr/`

ADR に要件・外部設計・内部設計の内容を詳細にコピーしません。具体的な現在仕様は各正本文書の仕様 ID や該当箇所を参照し、ADR では判断理由に集中します。

## 現在の ADR

- [ADR-0001: Windows 11 標準の .NET Framework 4.8.1 と WinForms を使用する](0001-winforms-net481-zip.md)
- [ADR-0002: 疎通判定はプロトコルの最小成立点に限定する](0002-minimal-protocol-success.md)
- [ADR-0003: クライアント試験は順次反復し協調的に停止する](0003-sequential-client-loop.md)
- [ADR-0004: NIC 設定変更は元に戻せる操作として扱う](0004-reversible-nic-configuration.md)
