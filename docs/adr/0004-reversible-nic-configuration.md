# ADR-0004: NIC 設定変更は可逆操作として扱う

## Decision

TestConnection が Windows NIC の設定を変更する場合は、**変更前状態を保持した一時的で可逆な操作**として扱います。TestConnection が ownership を持つのは、試験のために実際に変更する設定範囲だけとします。

現在の外部 contract は [`../SPEC.md`](../SPEC.md) の `EXT-NIC-001`, `EXT-NIC-002`、内部設計は [`../ARCHITECTURE.md`](../ARCHITECTURE.md) の `INT-NIC-001` を正本とします。

この ADR は snapshot 項目、復元手順、UI の具体的な挙動を重複定義せず、NIC state change を reversible boundary として扱う判断理由を保持します。

## Rationale

TestConnection は試験条件として local IPv4 address 等を Windows NIC に一時設定できます。これは通常の application state より高リスクな OS state change であり、設定失敗や終了時の取り残しが端末の通信断につながります。

一方、試験で変更しない DNS 等の設定まで管理対象へ広げると、変更範囲と rollback risk が増えます。そのため、変更する state の直前 snapshot を owner が保持し、触った範囲だけを復元する方式を採ります。

## Consequences

- 通常の手動復元・window close 経路では TestConnection が変更した NIC state を元へ戻せる設計を維持する。
- process kill、application crash、OS 強制終了等では cleanup 自体を実行できないため、完全な transaction 性は保証しない。
- NIC 設定機能は Windows、管理者権限、WMI の behavior に依存する。
- 実 NIC を変更する automated test は通常 CI では行わず、外部 contract の該当 acceptance は manual verification とする。
- 将来 DNS、route 等を変更対象へ追加する場合は、実装を先に広げず、`SPEC.md` で ownership / rollback contract を定義し、`ARCHITECTURE.md` で snapshot / restore responsibility を更新してから実装する。
