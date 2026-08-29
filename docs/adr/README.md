# Architecture Decision Records

`docs/adr/` は TestConnection の現在有効な重要な設計判断について、「なぜその判断を選ぶか」と「どの前提が変われば見直すか」を置く場所です。

ADR は変更履歴の台帳にせず、判断が変われば現在の設計を最も簡潔に表すよう編集・統合・削除します。過去経緯は Git history / Issue / Pull Request を参照します。

正本の役割分担は次です。

- 外部から観測できる現在の contract: [`../SPEC.md`](../SPEC.md)
- 現在の内部構成・責務: [`../ARCHITECTURE.md`](../ARCHITECTURE.md)
- build / test / package / release: [`../OPERATIONS.md`](../OPERATIONS.md)
- 判断理由と見直し条件: この `adr/`

ADR に SPEC / ARCHITECTURE の contract を詳細にコピーしません。Decision には判断の境界だけを書き、現在の具体的な contract は stable ID / section を参照します。

## Current ADRs

- [ADR-0001: Windows 11 標準の .NET Framework 4.8.1 と WinForms を使用する](0001-winforms-net481-zip.md)
- [ADR-0002: 疎通判定は protocol の最小成立点に限定する](0002-minimal-protocol-success.md)
- [ADR-0003: client 試験は順次反復し協調的に停止する](0003-sequential-client-loop.md)
- [ADR-0004: NIC 設定変更は可逆操作として扱う](0004-reversible-nic-configuration.md)
