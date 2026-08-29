# TestConnection

TestConnection は、Windows 上で TCP / UDP / DNS / Ping の疎通確認を行うためのツールです。特に、router や firewall による通信制限、network redundancy、経路切替等がある環境で、複数の疎通試験をまとめて繰り返す用途を想定しています。

## 主な機能

- TCP / UDP の server と client として動作します。
- DNS server への問い合わせ、ICMP Echo による Ping を実行できます。
- TCP / UDP / Ping client の接続先には IPv4 address または hostname / FQDN を指定できます。
- 複数の TCP / UDP server を同時に起動できます。
- TCP / UDP / DNS / Ping の client 試験を登録順に連続実行し、繰り返せます。
- 試験設定を CSV で保存・読込みできます。
- 実行結果を画面または file へ出力できます。
- 試験用に NIC の IP address / subnet mask / default gateway を一時変更し、元状態へ復元できます。

## 動作環境

- Windows 11 22H2 以降
- Microsoft .NET Framework 4.8.1
- TCP/IP を使用できる network interface

Windows 11 22H2 以降には .NET Framework 4.8.1 が OS の一部として含まれているため、TestConnection の実行だけを目的とした .NET runtime の追加 install は不要です。

表示は light mode 固定です。Windows の dark mode 設定には追随しません。

TCP / UDP / DNS の疎通確認は通常権限で利用できます。Ping は raw ICMP socket を使用するため、Windows の実行環境によっては管理者権限が必要です。NIC 設定変更も管理者権限を必要とします。

## インストール

GitHub Releases から ZIP を取得して任意の folder へ展開し、`TestConnection.exe` を実行してください。installer はありません。

`TestConnection.exe.config` と `res` directory は実行 file と同じ directory 構成のまま配置してください。

## アンインストール

展開した directory を削除してください。TestConnection 自体は install 時に registry へ登録しません。

## 疎通判定の考え方

TestConnection は application health check ではなく、各 protocol の最小成立点を観測します。

- TCP は connection の確立 / accept を確認し、application data は交換しません。
- UDP client の送信成功は、local OS が datagram を受け付けたことを示すだけで、remote endpoint への到達を保証しません。
- DNS は指定 DNS server から UDP response を受信できたかを確認します。
- Ping は送信した ICMP Echo Request に対応する Echo Reply だけを success とします。

このため、TCP success は接続先 application の認証・業務処理まで正常であることを意味しません。また UDP の遠端到達確認には、TestConnection の UDP server、packet capture、firewall log 等の別観測点を組み合わせてください。

正確な success / failure 条件、名前解決、実行順序、result、NIC 復元の contract は [`docs/SPEC.md`](docs/SPEC.md) を参照してください。

## Remote endpoint の名前解決

TCP / UDP / Ping client の remote endpoint に hostname / FQDN を指定した場合は、各試行の直前に名前解決します。このため DNS の応答が切り替わった場合は次の試行から反映されます。

複数 address からの選択規則や result 表示を含む詳細は [`docs/SPEC.md`](docs/SPEC.md) の `EXT-RESOLVE-001` を参照してください。

## 設定ファイル

設定は CSV 形式で保存できます。1 行が 1 つの server / client definition です。付属の `res/default.csv` と `res/sample-tcp100.csv` を例として利用できます。

行頭が `#` の行は読込み時に comment として扱います。CSV の contract と validation は [`docs/SPEC.md`](docs/SPEC.md) の `EXT-CFG-*` を参照してください。

## NIC 設定変更

NIC の IP address / subnet mask / default gateway を試験用に一時変更できます。TestConnection は変更前状態を保持し、手動復元または通常の window close で元状態へ戻す設計です。

process kill、application crash、OS 強制終了等では復元処理自体を実行できないため、試験前に現在の network 設定を別途確認しておくことを推奨します。詳細は [`docs/SPEC.md`](docs/SPEC.md) の `EXT-NIC-*` を参照してください。

## 既知の注意事項

- Windows Defender Firewall、EDR、antivirus 等が通信を遮断する場合があります。
- Windows が既に Listen している port では TestConnection の server を起動できません。`netstat -ano` や `Get-NetTCPConnection` 等で確認してください。
- server で特定の local IP address を指定する場合、その address が対象 PC に設定されている必要があります。
- TCP connection を確立したまま保持しないため、既存 session を維持する stateful failover test には適しません。

## 開発者向け文書

- 文書体系・仕様駆動ルール: [`docs/README.md`](docs/README.md)
- 外部仕様: [`docs/SPEC.md`](docs/SPEC.md)
- 内部設計: [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md)
- build / test / package / release: [`docs/OPERATIONS.md`](docs/OPERATIONS.md)
- 設計判断: [`docs/adr/`](docs/adr/)

## ライセンス

MIT License。詳細は [`LICENSE.txt`](LICENSE.txt) を参照してください。
