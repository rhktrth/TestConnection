# TestConnection

TestConnection は、Windows 上で TCP / UDP / DNS / Ping の疎通確認を行うためのツールです。特に、ルーターやファイアウォールによる通信制限、冗長構成、経路切替などがある環境で、複数の疎通試験をまとめて繰り返す用途を想定しています。

## 主な機能

- TCP / UDP のサーバーとクライアントとして動作します。
- DNS サーバーへの問い合わせ、ICMP Echo による Ping を実行できます。
- TCP / UDP / Ping クライアントの接続先には、IPv4 アドレスまたはホスト名 / FQDN を指定できます。
- 複数の TCP / UDP サーバーを同時に起動できます。
- TCP / UDP / DNS / Ping のクライアント試験を登録順に連続実行し、繰り返せます。
- 試験設定を CSV で保存・読込みできます。
- 実行結果を画面またはファイルへ出力できます。
- 試験用に NIC の IP アドレス、サブネットマスク、デフォルトゲートウェイを一時変更し、元の状態へ復元できます。

## 動作環境

- Windows 11 22H2 以降
- Microsoft .NET Framework 4.8.1
- TCP/IP を使用できるネットワークインターフェース

Windows 11 22H2 以降には .NET Framework 4.8.1 が OS の一部として含まれているため、TestConnection の実行だけを目的とした .NET 実行環境の追加インストールは不要です。

表示はライトモード固定です。Windows のダークモード設定には追随しません。

TCP / UDP / DNS の疎通確認は通常権限で利用できます。Ping は raw ICMP socket を使用するため、Windows の実行環境によっては管理者権限が必要です。NIC 設定変更も管理者権限を必要とします。

## インストール

GitHub Releases から ZIP を取得して任意のフォルダーへ展開し、`TestConnection.exe` を実行してください。インストーラーはありません。

`TestConnection.exe.config` と `res` ディレクトリは、実行ファイルと同じ構成のまま配置してください。

## アンインストール

展開したディレクトリを削除してください。TestConnection 自体はインストール時にレジストリへ登録しません。

## 疎通判定の考え方

TestConnection はアプリケーション全体の正常性を確認するものではなく、各プロトコルの最小成立点を確認します。

- TCP は接続の確立または受付を確認し、アプリケーションデータは交換しません。
- UDP クライアントの送信成功は、ローカル OS がデータグラムを受け付けたことを示すだけで、接続先への到達を保証しません。
- DNS は指定した DNS サーバーから UDP 応答を受信できたかを確認します。
- Ping は送信した ICMP Echo Request に対応する Echo Reply だけを成功とします。

このため、TCP の成功は接続先アプリケーションの認証や業務処理まで正常であることを意味しません。また、UDP の遠端到達を確認する場合は、TestConnection の UDP サーバー、パケットキャプチャ、ファイアウォールログなどの別の観測手段を併用してください。

正確な成功・失敗条件、名前解決、実行順序、結果出力、NIC 復元の仕様は [`docs/EXTERNAL_DESIGN.md`](docs/EXTERNAL_DESIGN.md) を参照してください。

## 接続先の名前解決

TCP / UDP / Ping クライアントの接続先にホスト名 / FQDN を指定した場合は、各試行の直前に名前解決します。このため、DNS の応答が切り替わった場合は次の試行から反映されます。

複数アドレスからの選択規則や結果表示を含む詳細は、[`docs/EXTERNAL_DESIGN.md`](docs/EXTERNAL_DESIGN.md) の `EXT-RESOLVE-001` を参照してください。

## 設定ファイル

設定は CSV 形式で保存できます。1 行が 1 つのサーバーまたはクライアントの試験定義です。付属の `res/default.csv` と `res/sample-tcp100.csv` を例として利用できます。

行頭が `#` の行は読込み時にコメントとして扱います。CSV の仕様と入力検証は、[`docs/EXTERNAL_DESIGN.md`](docs/EXTERNAL_DESIGN.md) の `EXT-CFG-*` を参照してください。

## NIC 設定変更

NIC の IP アドレス、サブネットマスク、デフォルトゲートウェイを試験用に一時変更できます。TestConnection は変更前の状態を保持し、手動復元または通常のウィンドウ終了時に元の状態へ戻す設計です。

プロセス強制終了、アプリケーション異常終了、OS 強制終了などでは復元処理自体を実行できないため、試験前に現在のネットワーク設定を別途確認しておくことを推奨します。詳細は [`docs/EXTERNAL_DESIGN.md`](docs/EXTERNAL_DESIGN.md) の `EXT-NIC-*` を参照してください。

## 既知の注意事項

- Windows Defender Firewall、EDR、ウイルス対策製品などが通信を遮断する場合があります。
- Windows が既に待受しているポートでは TestConnection のサーバーを起動できません。`netstat -ano` や `Get-NetTCPConnection` などで確認してください。
- サーバーで特定のローカル IP アドレスを指定する場合、そのアドレスが対象 PC に設定されている必要があります。
- TCP 接続を確立したまま保持しないため、既存セッションを維持するステートフルフェイルオーバー試験には適しません。

## 開発者向け文書

- 仕様駆動の工程・文書体系: [`docs/README.md`](docs/README.md)
- 要件定義: [`docs/REQUIREMENTS.md`](docs/REQUIREMENTS.md)
- 外部設計: [`docs/EXTERNAL_DESIGN.md`](docs/EXTERNAL_DESIGN.md)
- 内部設計: [`docs/INTERNAL_DESIGN.md`](docs/INTERNAL_DESIGN.md)
- ビルド・テスト・配布・リリース: [`docs/OPERATIONS.md`](docs/OPERATIONS.md)
- 設計判断: [`docs/adr/`](docs/adr/)

## ライセンス

MIT License。詳細は [`LICENSE.txt`](LICENSE.txt) を参照してください。
