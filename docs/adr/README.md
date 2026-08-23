# Architecture Decision Records

`docs/adr/` は TestConnection の現在有効な設計判断と理由を置く場所です。変更履歴の台帳にはせず、判断が変われば現在の設計を最も簡潔に表すよう編集・統合・削除します。過去経緯は Git history / Issue / PR を参照します。

`docs/ARCHITECTURE.md` が現在の内部設計仕様・責務・通信 semantics の正本、ADR は「なぜその形にするか」を扱います。

## Current ADRs

- [ADR-0001: Windows 11 標準の .NET Framework 4.8.1 と WinForms を使用する](0001-winforms-net481-zip.md)
- [ADR-0002: 疎通判定は protocol の最小成立点に限定する](0002-minimal-protocol-success.md)
- [ADR-0003: client 試験は順次反復し協調的に停止する](0003-sequential-client-loop.md)
- [ADR-0004: NIC 設定変更は可逆操作として扱う](0004-reversible-nic-configuration.md)
