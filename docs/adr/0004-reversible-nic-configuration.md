# ADR-0004: NIC 設定変更は可逆操作として扱う

## Decision

NIC 設定変更は「変更前状態を保持した一時操作」として扱います。

- NIC を最初に変更する直前に、DHCP/static、IP address、subnet mask、default gateway、gateway metric を snapshot として取得する。
- 同じ snapshot が残っている間に同一 NIC を複数回変更しても、復元基準は最初に取得した状態とする。
- 複数 NIC を変更した場合は NIC ごとに独立した snapshot を保持する。
- TestConnection が変更する範囲は IP address、subnet mask、default gateway に限定し、DNS 等の未変更項目には触れない。
- DHCP NIC は `EnableDHCP`、static NIC は保存した address / subnet / gateway を用いて復元する。
- WMI method の戻り値を検査し、設定・復元失敗を success として扱わない。
- 設定途中で失敗した場合は直ちに snapshot からの復元を試みる。
- 手動復元と window close は同じ復元処理を使用する。window close 時に復元できなければ close を中止して利用者へ通知する。
- 疎通試験実行中は NIC 設定変更 UI を無効化する。

## Rationale

TestConnection は試験項目で指定した複数の local IPv4 address を Windows NIC に一時設定し、送信元 address を含む通信試験を行えるようにします。これは OS のネットワーク状態を直接変更する高リスク操作であり、設定失敗や終了時の取り残しが端末の通信断につながります。

一方、試験で変更する必要があるのは IP address、subnet mask、default gateway です。DNS 等まで管理対象へ広げると変更範囲と復元リスクが増えるため、触った状態だけを復元対象にします。

## Consequences

- 通常の手動復元・終了経路では TestConnection が行った NIC 変更を元へ戻せる。
- process kill、application crash、OS 強制終了等では復元処理自体を実行できないため、完全な transaction 性は保証できない。
- NIC 設定機能は管理者権限と Windows WMI の仕様に依存する。
- 実 NIC を変更する automated test は通常 CI では行わない。
- 将来 DNS、route 等を変更対象へ追加する場合は、同じ snapshot / rollback 原則の下で復元可能性を確認し、この ADR の設計範囲を更新する。
