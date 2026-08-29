# Operations

この文書は TestConnection の build、regression test、package、CI、GitHub Release の実際の手順を定義します。製品の外部仕様は [`SPEC.md`](SPEC.md)、内部設計は [`ARCHITECTURE.md`](ARCHITECTURE.md) を参照してください。

## Runtime baseline

利用者向け基準環境は Windows 11 22H2 以降、target framework は `net481`（.NET Framework 4.8.1）です。変更条件は [`ADR-0001`](adr/0001-winforms-net481-zip.md) を正本とします。

## Build requirements

- Windows
- Visual Studio 2022 または Visual Studio 2022 Build Tools
- .NET Framework 4.8.1 Developer Pack
- MSBuild / NuGet restore が利用できること

Developer Pack は開発・build 時の targeting pack です。Windows 11 上で配布 ZIP を実行する利用者へ追加 install を要求するものではありません。

## Build

repository root で実行します。

```powershell
msbuild src\TestConnection\TestConnection.csproj /restore /m /p:Configuration=Release
```

application は単一の SDK-style project なので solution file を維持しません。Visual Studio では `src/TestConnection/TestConnection.csproj` を直接開きます。

## Regression tests

外部 test framework への dependency を増やさず、`tests/TestConnection.Tests` の小さな `net481` console executable で重要な contract / lifecycle の回帰を検証します。

```powershell
msbuild tests\TestConnection.Tests\TestConnection.Tests.csproj /restore /m /p:Configuration=Release
.\tests\TestConnection.Tests\bin\Release\net481\TestConnection.Tests.exe
```

現在の regression test は、主に次を検証します。

- TCP / UDP の loopback connectivity
- DNS の loopback response と cancel
- ICMP Echo Request / Reply correlation と malformed packet rejection
- client runner の登録順・反復・stop
- `TestSession` と server readiness / lifecycle
- remote endpoint の IPv4 selection rule
- result prefix formatting
- 5 列 CSV の parse / serialize / validation
- WinForms application configuration

外部 contract と test の対応は [`SPEC.md`](SPEC.md) の acceptance / traceability table を正本とします。

通常 CI では外部 host への疎通、実 NIC 設定変更、管理者権限が必要な raw ICMP の実送受信を行いません。network integration test は loopback を優先し、実 NIC に依存する `AC-NIC-001` は manual verification とします。

## Package

repository root で実行します。

```powershell
msbuild src\TestConnection\TestConnection.csproj /restore /m /t:Package /p:Configuration=Release
```

`Package` target は Release build を含めて実行し、`dist/TestConnection-<Version>.zip` を生成します。package file 名に使う `Version` は MSBuild property `$(Version)` です。

ZIP の構成は `src/TestConnection/TestConnection.csproj` の `Package` target を正本とし、現在は次を含みます。

- `TestConnection.exe`
- `TestConnection.exe.config`
- `res/`
- `README.md`
- `LICENSE.txt`

package の生成処理を別 script や workflow 内へ二重実装しません。

## Pull Request CI

`.github/workflows/test.yml` は `main` への Pull Request と `main` への push を対象に、code / build に関係する変更で次を実行します。

1. `Package` target で Release package を build する。
2. regression test project を build する。
3. regression test executable を実行する。

workflow は orchestration に留め、`Package` target や test code が検証している内容を YAML / PowerShell で再実装しません。

## Release version

正式 release の version は GitHub tag 名を正本とし、`vX.Y.Z.W` 形式を使用します。

release workflow は tag から先頭 `v` を除いた `X.Y.Z.W` を release build の version として使用し、checkout した working tree に対して次を行います。

- `app.manifest` の `assemblyIdentity version` を release version へ一時的に書き換える。
- MSBuild へ `Version` / `AssemblyVersion` / `FileVersion` / `InformationalVersion` を release version として渡す。
- `TestConnection-X.Y.Z.W.zip` を生成する。

そのため、release のたびに version trigger file や repository 内 CHANGELOG を更新しません。committed `app.manifest` の version は release identity の正本ではありません。

## GitHub Release

正式配布物の生成は `.github/workflows/release.yml` を正本とします。

通常の release 手順は次です。

1. release 対象の変更を Pull Request と CI に通して `main` へ merge する。
2. release 対象 commit に `vX.Y.Z.W` tag を用意する。
3. その tag の GitHub Release を publish する。
4. `release: published` event で release workflow が起動する。
5. workflow が tag を checkout し、tag version で package を build する。
6. ZIP の SHA-256 checksum file を生成する。
7. ZIP と `.sha256` を同じ GitHub Release へ upload する。

既存 release asset を再生成する必要がある場合は、`workflow_dispatch` の `tag` input に既存の `vX.Y.Z.W` を指定できます。workflow は既存 asset を `--clobber` で置き換えます。通常の release では publish event を使用します。

release workflow 自体は GitHub Release を作成しません。**既に publish された Release に asset を生成・upload する automation** です。

release 履歴と release note の正本は GitHub Releases とし、repository 内に CHANGELOG や release note の複製を維持しません。

公開済み tag は release identity として扱い、通常は削除・付け替え・force update しません。

## Release failure の確認

release asset が付かない場合は、次を順に確認します。

1. tag が `vX.Y.Z.W` 形式か。
2. GitHub Release が publish 済みか。
3. release workflow が対象 tag を checkout できているか。
4. `Package` target が成功しているか。
5. `gh release upload` が対象 Release を解決できているか。

workflow 内の version injection は release build 用の working tree だけを変更し、`main` へ commit / push しません。
