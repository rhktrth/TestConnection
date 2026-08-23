# ADR-0001: Windows 11 標準の .NET Framework 4.8.1 と WinForms を使用する

## Decision

TestConnection の対象 framework は `net481`（.NET Framework 4.8.1）に固定する。UI は WinForms を使用する。

利用者向けの基準環境は Windows 11 22H2 以降とし、OS に含まれる .NET Framework 4.8.1 だけで実行できることを優先する。通常の配布物へ .NET runtime を同梱せず、利用者に .NET runtime の追加インストールも要求しない。

配布はインストーラを使用せず ZIP とする。

## Rationale

TestConnection は、疎通確認が必要な Windows PC へ展開してすぐ実行できることを重視する小型ツールである。アプリケーション本体より大きい runtime の同梱や、利用前の runtime セットアップは、この用途では利便性を下げる。

Microsoft の .NET Framework システム要件では、Windows 11 22H2 以降に .NET Framework 4.8.1 が OS の一部としてプレインストールされている。現在サポートされている Windows 11 では .NET Framework 4.8.1 を追加導入せず利用できる。

- Microsoft Learn: [.NET Framework system requirements](https://learn.microsoft.com/dotnet/framework/get-started/system-requirements)
- Microsoft Learn: [Install .NET Framework on Windows](https://learn.microsoft.com/dotnet/framework/install/on-windows-and-server)

.NET 10 などの modern .NET は Windows 11 の標準ランタイムではない。framework-dependent 配布では runtime の追加インストールが必要になり、self-contained 配布では runtime をアプリケーション側へ同梱する必要がある。このため「新しい framework であること」だけを理由に移行しない。

SDK-style project は target framework とは独立した開発時の構成であるため、プロジェクト形式は SDK-style を採用してよい。これにより repository と build 定義を単純化しつつ、実行時は Windows 11 標準の .NET Framework 4.8.1 を維持できる。

## Constraints

- `src/TestConnection/TestConnection.csproj` の `TargetFramework` は `net481` とする。
- target framework を変更する場合は、実装より先にこの ADR の Decision / Rationale を更新する。
- modern .NET への移行は、Windows の標準搭載状況が変わるか、net481 では満たせない具体的要件が生じた場合にのみ検討する。
- self-contained runtime 同梱は通常配布の前提にしない。
- runtime dependency を増やす NuGet package は、標準ライブラリでは実現できない明確な理由がある場合に限定する。

## Consequences

- Windows 11 22H2 以降では、TestConnection のための .NET runtime セットアップを不要にできる。
- .NET Framework 4.8.1 の API / C# コンパイラで実現可能な範囲を基本とする。
- repository は SDK-style project、CI、MSBuild package target など現在の開発方式を採用できる。
- Visual Studio Publish / ClickOnce / installer / runtime bundle は配布経路として使用しない。
