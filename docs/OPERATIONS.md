# TestConnection 運用・開発手順

この文書は、TestConnection のビルド、回帰テスト、配布物作成、CI、GitHub Release の実際の手順を定義します。製品の要件は [`REQUIREMENTS.md`](REQUIREMENTS.md)、外部仕様は [`EXTERNAL_DESIGN.md`](EXTERNAL_DESIGN.md)、内部設計は [`INTERNAL_DESIGN.md`](INTERNAL_DESIGN.md) を参照してください。

## 実行環境の基準

利用者向けの基準環境は Windows 11 22H2 以降、対象フレームワークは `net481`（.NET Framework 4.8.1）です。変更条件は [`ADR-0001`](adr/0001-winforms-net481-zip.md) を正本とします。

## ビルドに必要なもの

- Windows
- Visual Studio 2022 または Visual Studio 2022 Build Tools
- .NET Framework 4.8.1 Developer Pack
- MSBuild と NuGet restore が利用できること

Developer Pack は開発・ビルド時に必要な targeting pack です。Windows 11 上で配布 ZIP を実行する利用者へ追加インストールを要求するものではありません。

## ビルド

リポジトリのルートで実行します。

```powershell
msbuild src\TestConnection\TestConnection.csproj /restore /m /p:Configuration=Release
```

アプリケーションは単一の SDK-style プロジェクトなので、solution ファイルは維持しません。Visual Studio では `src/TestConnection/TestConnection.csproj` を直接開きます。

## 回帰テスト

外部テストフレームワークへの依存を増やさず、`tests/TestConnection.Tests` の小さな `net481` コンソールプログラムで重要な仕様・ライフサイクルの回帰を検証します。

```powershell
msbuild tests\TestConnection.Tests\TestConnection.Tests.csproj /restore /m /p:Configuration=Release
.\tests\TestConnection.Tests\bin\Release\net481\TestConnection.Tests.exe
```

現在の回帰テストでは主に次を検証します。

- TCP / UDP のループバック疎通
- DNS のループバック応答と停止処理
- ICMP Echo Request / Reply の対応確認と不正パケットの拒否
- クライアント試験の登録順、反復、停止
- `TestSession` とサーバー待受準備のライフサイクル
- 接続先の IPv4 アドレス選択規則
- 結果出力の共通書式
- 5 列 CSV の読込み・保存・入力検証
- WinForms のアプリケーション設定

要件・外部仕様とテストの対応は、[`EXTERNAL_DESIGN.md`](EXTERNAL_DESIGN.md) の受入条件表を正本とします。

通常の CI では、外部ホストへの疎通、実 NIC の設定変更、管理者権限が必要な raw ICMP の実送受信を行いません。ネットワーク統合テストはループバックを優先し、実 NIC に依存する `AC-NIC-001` は手動確認とします。

## 配布 ZIP の作成

リポジトリのルートで実行します。

```powershell
msbuild src\TestConnection\TestConnection.csproj /restore /m /t:Package /p:Configuration=Release
```

`Package` target は Release ビルドを含めて実行し、`dist/TestConnection-<Version>.zip` を生成します。ZIP 名に使う `Version` は MSBuild の `$(Version)` プロパティです。

ZIP の構成は `src/TestConnection/TestConnection.csproj` の `Package` target を正本とし、現在は次を含みます。

- `TestConnection.exe`
- `TestConnection.exe.config`
- `res/`
- `README.md`
- `LICENSE.txt`

同じ配布物作成処理を、別スクリプトや GitHub Actions の中へ重複実装しません。

## PR の CI

`.github/workflows/test.yml` は `main` への PR と `main` への push を対象に、コードやビルドに関係する変更で次を実行します。

1. `Package` target で Release 配布 ZIP を作成する。
2. 回帰テスト用プロジェクトをビルドする。
3. 回帰テストを実行する。

GitHub Actions は各処理を組み合わせて実行する役割に留め、`Package` target やテストコードが持つ検証を YAML や PowerShell で二重実装しません。

## リリースバージョン

正式リリースのバージョンは GitHub のタグ名を正本とし、`vX.Y.Z.W` 形式を使用します。

リリース用ワークフローはタグ名から先頭の `v` を除いた `X.Y.Z.W` をリリースバージョンとして使用し、対象コミットを取得した作業領域で次を行います。

- `app.manifest` の `assemblyIdentity version` をリリースバージョンへ一時的に書き換える。
- MSBuild へ `Version` / `AssemblyVersion` / `FileVersion` / `InformationalVersion` を指定する。
- `TestConnection-X.Y.Z.W.zip` を生成する。

そのため、リリースのたびに専用のバージョン管理ファイルやリポジトリ内の CHANGELOG を更新しません。コミット済み `app.manifest` のバージョン値は、リリース識別子の正本ではありません。

## GitHub Release

正式配布物の生成は `.github/workflows/release.yml` を正本とします。

通常のリリース手順は次のとおりです。

1. リリース対象の変更を PR と CI に通して `main` へマージする。
2. リリース対象コミットに `vX.Y.Z.W` タグを付ける。
3. そのタグの GitHub Release を公開する。
4. `release: published` イベントでリリース用ワークフローが起動する。
5. ワークフローがタグの内容を取得し、そのバージョンで配布 ZIP を作成する。
6. ZIP の SHA-256 チェックサムファイルを生成する。
7. ZIP と `.sha256` を同じ GitHub Release へ追加する。

既存の Release に配布物を作り直す必要がある場合は、`workflow_dispatch` の `tag` 入力へ既存の `vX.Y.Z.W` を指定できます。ワークフローは既存の配布物を `--clobber` で置き換えます。通常のリリースでは公開イベントを使用します。

リリース用ワークフロー自体は GitHub Release を新規作成しません。**既に公開された Release へ配布物を生成して追加する処理**です。

リリース履歴とリリースノートの正本は GitHub Releases とし、リポジトリ内に CHANGELOG やリリースノートの複製を維持しません。

公開済みタグはリリース識別子として扱い、通常は削除、付け替え、強制更新を行いません。

## リリース失敗時の確認

Release に配布物が付かない場合は、次を順に確認します。

1. タグが `vX.Y.Z.W` 形式か。
2. GitHub Release が公開済みか。
3. リリース用ワークフローが対象タグの内容を取得できているか。
4. `Package` target が成功しているか。
5. `gh release upload` が対象 Release を解決できているか。

ワークフロー内のバージョン書換えはリリースビルド用の作業領域だけを変更し、`main` へコミットや push は行いません。
