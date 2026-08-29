# ADR-0001: Windows 11 標準の .NET Framework 4.8.1 と WinForms を使用する

## 決定

TestConnection の対象フレームワークは `net481`（.NET Framework 4.8.1）に固定し、UI には WinForms を使用します。

利用者向けの基準環境は Windows 11 22H2 以降とし、OS に含まれる .NET Framework 4.8.1 だけで実行できることを優先します。通常の配布物へ .NET 実行環境を同梱せず、利用者にも追加インストールを要求しません。

配布はインストーラーを使用せず ZIP とします。

## 理由

TestConnection は、疎通確認が必要な Windows PC へ展開してすぐ実行できることを重視する小型ツールです。アプリケーション本体より大きな実行環境の同梱や、利用前の追加セットアップは、この用途では利便性を下げます。

Microsoft の .NET Framework システム要件では、Windows 11 22H2 以降に .NET Framework 4.8.1 が OS の一部として含まれています。現在サポートされている Windows 11 では、TestConnection のためだけに .NET Framework 4.8.1 を追加導入する必要がありません。

- Microsoft Learn: [.NET Framework system requirements](https://learn.microsoft.com/dotnet/framework/get-started/system-requirements)
- Microsoft Learn: [Install .NET Framework on Windows](https://learn.microsoft.com/dotnet/framework/install/on-windows-and-server)

.NET 10 などの新しい .NET は Windows 11 の標準実行環境ではありません。framework-dependent 配布では追加の実行環境が必要になり、self-contained 配布では実行環境をアプリケーション側へ同梱する必要があります。このため、「新しいフレームワークだから」という理由だけでは移行しません。

SDK-style のプロジェクト形式は対象フレームワークとは独立した開発上の選択です。プロジェクト形式は SDK-style を採用しつつ、実行時は Windows 11 標準の .NET Framework 4.8.1 を維持します。

## 制約

- `src/TestConnection/TestConnection.csproj` の `TargetFramework` は `net481` とする。
- 対象フレームワークを変更する場合は、実装より先に要件・外部設計とこの ADR を更新する。
- 新しい .NET への移行は、Windows の標準搭載状況が変わるか、`net481` では満たせない具体的な要件が生じた場合に検討する。
- self-contained での実行環境同梱は通常配布の前提にしない。
- 実行時依存を増やす NuGet パッケージは、標準ライブラリでは実現できない明確な理由がある場合に限定する。

## 影響

- Windows 11 22H2 以降では、TestConnection のための .NET 実行環境の追加セットアップを不要にできる。
- .NET Framework 4.8.1 の API と利用可能な C# コンパイラで実現可能な範囲を基本とする。
- リポジトリでは SDK-style プロジェクト、CI、MSBuild の `Package` target など現在の開発方式を利用できる。
- Visual Studio Publish、ClickOnce、インストーラー、実行環境同梱は通常の配布経路として使用しない。
