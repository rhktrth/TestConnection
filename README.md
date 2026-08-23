# TestConnection

TestConnection は、Windows 上で TCP / UDP / DNS / Ping の疎通確認を行うためのツールです。特に、ルータやファイアウォールによる通信制限があるネットワークで、複数の疎通試験をまとめて実行する用途を想定しています。

## 主な機能

- TCP / UDP のサーバとクライアントとして動作します。
- TCP クライアントは接続を試行し、TCP セッションが確立した時点で成功と判定して直ちに切断します。
- UDP クライアントは小さなデータグラムを送信します。UDP は送信だけでは相手への到達を保証できないため、TestConnection の UDP サーバ、通信経路上のキャプチャ、またはファイアウォールログ等と組み合わせて確認します。
- DNS クライアントは指定した DNS サーバへ問い合わせを送信し、応答を受信できた場合に成功と判定します。
- Ping クライアントは ICMP Echo Request を送信し、同じ宛先から対応する Echo Reply を受信した場合に成功と判定します。
- TCP / UDP / Ping クライアントの接続先には IPv4 address または hostname / FQDN を指定できます。
- 複数の TCP / UDP サーバを同時に起動できます。
- TCP / UDP / DNS / Ping の接続試行を連続実行でき、冗長構成機器の切替試験などに利用できます。
- 試験設定を CSV で保存・読み込みできます。
- 実行結果を画面またはファイルへ出力できます。

## 動作環境

- Windows 11 22H2 以降
- Microsoft .NET Framework 4.8.1
- TCP/IP を使用できるネットワークインターフェース

Windows 11 22H2 以降には .NET Framework 4.8.1 が OS の一部として含まれているため、TestConnection の実行だけを目的とした .NET runtime の追加インストールは不要です。

表示はライトモード固定です。Windows のダークモード設定には追随しません。

TCP / UDP / DNS の疎通確認は通常権限で利用できます。Ping は raw ICMP socket を使用するため、Windows の実行環境によっては管理者権限が必要です。NIC の IP アドレスやデフォルトゲートウェイを TestConnection から変更する機能も管理者権限を必要とします。

## インストール

GitHub Releases から ZIP を取得して任意のフォルダへ展開し、`TestConnection.exe` を実行してください。インストーラはありません。

`TestConnection.exe.config` と `res` ディレクトリは実行ファイルと同じディレクトリ構成のまま配置してください。

## アンインストール

展開したディレクトリを削除してください。TestConnection 自体はインストール時にレジストリへ登録しません。

## TCP の判定について

TCP は接続確立のみを確認し、アプリケーションプロトコルのデータは送信しません。このため既存の TCP サーバを接続先に指定できますが、接続を受け付けること自体が相手側へ影響する可能性はあります。利用するシステムの仕様を確認して実行してください。

TCP セッションを確立したまま保持しないため、既存セッションを維持するステートフルフェイルオーバ試験には適しません。

## UDP の判定について

UDP クライアント側の送信成功は、ローカル OS がデータグラムの送信を受け付けたことを示すもので、相手への到達確認ではありません。相手側の TestConnection UDP サーバで受信を確認するか、別の観測手段を併用してください。

## Ping の判定について

Ping は raw ICMP socket で Echo Request を送信し、送信先 IP アドレス、ICMP type、identifier、sequence number が対応する Echo Reply を受信した場合だけ成功とします。送信後に無関係な ICMP packet を受信しても成功にはせず、timeout まで対応する reply を待ちます。

## Remote endpoint の名前解決

TCP / UDP / Ping クライアントの remote endpoint には、IPv4 address のほか hostname / FQDN を指定できます。hostname は試験項目の登録時ではなく、各試行の直前に名前解決します。このため DNS の応答が切り替わった場合は次の試行から反映されます。

一つの hostname から複数の IPv4 address が得られた場合は、IPv4 address を数値として昇順に並べた先頭を使用します。IPv4 address を解決できない場合は、その試行を failure として記録して次の試行へ進みます。TCP / UDP の result には実際に使用した IPv4 address、Ping の result には `hostname(IPv4 address)` の形式で解決結果を出力します。

local IP address は従来どおり具体的な IP address を指定します。DNS クライアントの remote 欄は問い合わせ先 DNS server を表すため、hostname ではなく IP address を指定します。

## 設定ファイル

設定は CSV 形式で保存できます。1 行が 1 つのサーバまたはクライアント定義です。付属の `res/default.csv` と `res/sample-tcp100.csv` を例として利用できます。

TCP / UDP / Ping の remote 欄へ hostname / FQDN を指定した場合、CSV には解決後の IP address へ置換せず、利用者が指定した文字列をそのまま保存します。

行頭が `#` の行はコメントとして読み飛ばします。保存時はコメント行を出力せず、試験定義だけを保存します。

## 既知の注意事項

- Windows Defender Firewall、EDR、ウイルス対策製品などが通信を遮断する場合があります。
- Windows が既に Listen しているポートでは TestConnection のサーバを起動できません。`netstat -ano` や `Get-NetTCPConnection` 等で確認してください。
- サーバで特定のローカル IP アドレスを指定する場合、そのアドレスが対象 PC に設定されている必要があります。
- NIC 設定変更機能を使用する場合は、試験前に現在のネットワーク設定を別途確認しておくことを推奨します。

## ライセンス

MIT License。詳細は [`LICENSE.txt`](LICENSE.txt) を参照してください。
