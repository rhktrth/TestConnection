# Operations

## Runtime baseline

利用者向けの基準環境は Windows 11 22H2 以降です。target framework は `net481`（.NET Framework 4.8.1）に固定します。

Windows 11 22H2 以降には .NET Framework 4.8.1 が OS の一部として含まれるため、通常の配布物には .NET runtime を同梱せず、利用者にも runtime の追加インストールを要求しません。target framework を変更する判断は [`ADR-0001`](adr/0001-winforms-net481-zip.md) を正本とします。

## Build requirements

- Windows
- Visual Studio 2022 または Visual Studio 2022 Build Tools
- .NET Framework 4.8.1 Developer Pack
- MSBuild / NuGet restore が利用できること

Developer Pack は開発・ビルド時に必要な targeting pack であり、Windows 11 上で TestConnection を実行する利用者が別途導入するものではありません。

## Build

repository root で実行します。

```powershell
msbuild src\TestConnection\TestConnection.csproj /restore /m /p:Configuration=Release
```

applicationのbuild対象は単一のSDK-style projectなので、repositoryではsolution fileを維持しません。regression testは下記のtest projectを別途buildします。Visual Studioで開発する場合も `src/TestConnection/TestConnection.csproj` を直接開けば、WinForms Designerを含む通常のproject開発ができます。

SDK-style project は restore で build assets を生成してから build します。

## Regression tests

外部test frameworkへの依存を増やさず、`tests/TestConnection.Tests` の小さな `net481` console executableで重要なpure logic / lifecycle回帰を検証します。

```powershell
msbuild tests\TestConnection.Tests\TestConnection.Tests.csproj /restore /m /p:Configuration=Release
.\tests\TestConnection.Tests\bin\Release\net481\TestConnection.Tests.exe
```

regression testでは次を検証します。

- ICMP Echo Request生成とEcho Replyのtype / identifier / sequence number照合
- malformed / 無関係なICMP packetをsuccessにしないこと
- client loopの有限反復
- client `Stop()` が実行中clientを`Cancel()`し、worker終了まで待つこと
- server `Start()` がlisten準備完了まで待つこと
- result logの共通prefixが空endpointへ不要な区切り文字を出さないこと
- 既存5列CSVのparse / serialize互換性と不正formatの拒否

通常CIでは外部hostへの疎通、実NIC設定変更、管理者権限が必要なraw ICMP送受信は行いません。network I/Oそのものではなく、外部環境に依存せず再現できる判定・停止・設定互換性を優先します。

## Package

repository root で実行します。

```powershell
msbuild src\TestConnection\TestConnection.csproj /restore /m /t:Package /p:Configuration=Release
```

`Package` target は Release build を含めて実行し、`dist/TestConnection-<version>.zip` を生成します。version の正本は `src/TestConnection/TestConnection.csproj` の `Version` propertyで、AssemblyVersion、FileVersion、ZIP名もこの値から生成します。

ZIP には次を含めます。

- `TestConnection.exe`
- `TestConnection.exe.config`
- `res/`
- `README.md`
- `LICENSE.txt`

配布物の構成とZIP生成は `src/TestConnection/TestConnection.csproj` の `Package` target を正本とし、専用のpackage scriptは持ちません。target framework、version形式、AssemblyVersion、application manifest versionなど、package生成に必要な整合性検証もこのproject内で行います。

## CI

`.github/workflows/test.yml` は pull request と `master` への push で次だけを実行します。

1. `Package` targetでRelease配布ZIPを生成する。
2. regression test projectをbuildする。
3. regression test executableを実行する。

Package targetが正本として持つ検証をworkflow側で再実装したり、生成したZIPを再展開して同じ構成を二重検証したりしません。PRごとの配布artifact保存も行わず、実際の配布物はrelease workflowで生成します。

## Version update

release version は4桁の `X.Y.Z.W` とします。versionを上げるときは次の順で更新します。

1. `src/TestConnection/TestConnection.csproj` の `<Version>X.Y.Z.W</Version>` を更新する。これがversion metadataの正本です。
2. `src/TestConnection/Properties/app.manifest` の `assemblyIdentity version` を同じ値へ更新する。
3. 必要に応じてREADME等の現行文書を更新する。
4. `Package` または通常CIを実行する。

手書きの AssemblyVersion / AssemblyFileVersion は持ちません。

## Release

release履歴とrelease noteの正本は GitHub Releases とし、repository内に別のCHANGELOGを維持しません。

GitHub Release は `vX.Y.Z.W` 形式の version tag を push したときだけ `.github/workflows/release.yml` が作成します。`RELEASE_VERSION` のような release trigger 用ファイルは使用しません。

version更新と必要な現行文書の更新を通常の pull request と CI に通して `master` へ mergeし、そのrelease対象commitにtagを付けてpushします。

```powershell
git switch master
git pull --ff-only
git tag v0.3.1.0
git push origin v0.3.1.0
```

release workflowは、tag名が `v` + project `Version` と完全一致することを確認して `Package` targetを実行します。Packageが成功すると、既存のversion tagを使って GitHub Release `TestConnection X.Y.Z.W` を作成し、ZIPを添付します。

release note は `gh release create --generate-notes` により、前回release以降のmerged pull request等からGitHubが自動生成します。このため、releaseへ含める変更のPR titleは利用者から見て内容が分かるものにします。

release 後の version tag は release identity として扱い、通常は削除・付け替え・force update しません。過去の変更履歴を確認する場合は GitHub Releases、pull request、issue、Git history を参照します。

Visual Studio の Publish / ClickOnce は使用しません。
