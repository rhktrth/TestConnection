# TestConnection 向け AI・開発ルール

このリポジトリは、Windows 上で TCP / UDP / DNS / Ping の疎通確認を行う小型ツールを、個人で把握・保守できる規模に保ちます。

正本は役割ごとに分けます。

- 利用者向けの使い方: [`README.md`](README.md)
- 現在の内部構成と責務: [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md)
- ビルド・テスト・配布: [`docs/OPERATIONS.md`](docs/OPERATIONS.md)
- 現在有効な設計判断と理由: [`docs/adr/`](docs/adr/)
- release履歴とrelease note: GitHub Releases

## 原則

- 現在必要な疎通確認機能を単純に保ち、将来拡張だけを目的とする abstraction、framework、compatibility layer、設定項目を追加しない。
- 挙動変更を伴わない整理では、利用者から見える通信仕様、設定ファイル、ログの意味を変えない。
- 過去の構成、移行経緯、廃止済み仕様をコードや文書へ残さない。履歴は Git history / Issue / PR / GitHub Releases に任せる。
- runtime dependency は .NET Framework 標準ライブラリだけを基本とする。
- Visual Studio 固有の発行設定を配布の正本にしない。ClickOnce は使用しない。
- インストーラを前提にせず、展開して実行できる ZIP を配布単位とする。
- 配布ZIPの構成と生成処理は `src/TestConnection/TestConnection.csproj` の `Package` target を正本とし、同じ処理を行う補助scriptを追加しない。
- 実名、個人メールアドレス、ローカルユーザー名、個人環境固有のパスなど、作者個人を特定し得る情報をコード・設定・配布物へ含めない。ライセンス上必要な著作権表示と公開 GitHub リポジトリ識別子は除く。
- project GUID、assembly GUID、署名・発行識別子などの application-specific identifier は、不要に旧配布物や別ソフトとの相関を作る値を再利用しない。Windows / Microsoft が仕様として定義する標準識別子はこの対象外とする。

## C# / WinForms

- target framework は `net481`（.NET Framework 4.8.1）に固定する。理由と変更条件は [`ADR-0001`](docs/adr/0001-winforms-net481-zip.md) を正本とする。
- target framework を変更する実装を先に行わない。変更が必要な場合は、まず ADR-0001 の Decision / Rationale を現在の判断へ更新する。
- Windows 11 で追加 runtime なしに動くことを優先し、「新しい .NET だから」という理由だけで modern .NET へ移行しない。
- project file は SDK-style とし、target framework の古さを理由に旧形式 csproj へ戻さない。
- UI は WinForms を維持する。別 UI framework への移行は、それ自体を目的に行わない。
- C# はスペース4文字、XML / YAML はスペース2文字、UTF-8 / LF とし、共有書式は `.editorconfig` を正本とする。
- コメントは日本語で、コードから分からない理由・制約だけを書く。過去実装の説明は書かない。
- `MainForm` と通信処理の分離は、実際の変更理由がある範囲で段階的に行う。一括した全面再設計はしない。

## 文書

- README は利用者向け情報だけを書く。
- ARCHITECTURE は現在の内部構成・責務・通信上の意味だけを書く。
- OPERATIONS はビルド・CI・配布方法だけを書く。
- repository内にCHANGELOGを維持しない。release履歴とrelease noteはGitHub Releasesを正本とする。
- 同じ仕様を複数文書へ重複記載しない。正本への参照で済ませる。
- ADR は現在有効な判断だけを置く。判断が変わったら既存 ADR を編集・統合・削除し、superseded ADR を保存しない。

## テスト

- coverage 率ではなく、通信判定、停止処理、設定読み書き、配布物欠落など利用時に重大な不具合を優先する。
- ネットワーク integration test を追加する場合は原則 loopback を使用し、通常 CI から外部ホストへ接続しない。
- 不具合修正では、現実的に自動化できる場合は再現テストを追加する。
- private 実装の細かな呼出順だけを固定するテストを増やさない。

## GitHub 上の変更

- 挙動変更・構造変更は Issue と branch / PR を基本とする。
- Issue、PR、コミット説明は日本語で書く。識別子・製品名・技術用語は不自然に日本語化しない。
- unrelated refactoring を同じ PR に混ぜない。ただし最小構成へ整理するため不可分な移動・削除・設定変更はまとめてよい。
- GitHub Actionsはbuild / test / releaseのorchestrationに留め、csprojやtest codeが正本として検証している内容をworkflow scriptで二重実装しない。
- GitHub Release は `master` の履歴上の commit に付けた `vX.Y.Z.W` 形式の tag を push して作成する。release trigger 用ファイルや Visual Studio Publish を追加しない。
- release note は GitHub の自動生成機能を使用し、repository内に同じ履歴を再記載しない。PR title は自動生成release noteにそのまま現れても意味が通る内容にする。
- version tag の `X.Y.Z.W` は `AssemblyVersion` と完全一致させる。配布ZIP名も同じ4桁versionを使用する。公開済みの version tag は通常、削除・付け替え・force update しない。
- ユーザーの明示指示なしに PR を merge しない。

## Definition of Done

1. `net481` の Release build が成功する。
2. `TestConnection.csproj` の `Package` target で version 付き ZIP が生成される。
3. ZIP に `TestConnection.exe`、`TestConnection.exe.config`、必要な `res/`、README、LICENSE が含まれる。
4. 変更後に不要になった file / setting / document を残さない。
5. README / ARCHITECTURE / OPERATIONS / 現行 ADR と実装が矛盾・重複しない。

Pull Request CI の正本は `.github/workflows/test.yml` とする。
