## 変更概要

<!-- 何を、なぜ変更するかを簡潔に記載する。 -->

## 変更分類

- [ ] A: 外部仕様を変更する
- [ ] B: 内部設計を変更する
- [ ] C: build / test / package / CI / release 等の運用を変更する
- [ ] D: 実装のみで、仕様・設計・運用は変更しない

A / B / C は複数選択できる。D は A / B / C のいずれにも該当しない場合だけ選択する。

D の場合、仕様・設計・運用変更なしと判断した理由:

<!-- 関連する正本文書が変更後も真である理由を書く。 -->

## 仕様・設計先行

- [ ] 着手時に `docs/README.md` と関連する正本文書、ADR、test、production code を確認した
- [ ] A の場合、`docs/SPEC.md` と必要な `EXT-*` を production code より先に更新した
- [ ] 回帰リスクがある外部 contract には必要な `AC-*` を追加・更新した
- [ ] B の場合、`docs/ARCHITECTURE.md` と必要な `INT-*` を production code より先に更新した
- [ ] 複数の合理的な選択肢がある重要判断は必要に応じて ADR を更新した
- [ ] 変更後の仕様・設計を検証する test を更新してから最終 implementation を整合させた

更新した正本文書・仕様 ID:

<!-- 例: SPEC.md EXT-DNS-001 / AC-DNS-001, ARCHITECTURE.md INT-TESTER-001 -->

## Traceability

外部仕様 (`EXT-*`):

受入条件 (`AC-*`):

内部設計 (`INT-*`, 必要な場合):

test / manual verification:

production component:

<!-- 外部仕様変更がない場合は「なし（既存 contract を維持）」等と記載する。 -->

## 整合性・重複確認

- [ ] README / SPEC / ARCHITECTURE / OPERATIONS / ADR / test / implementation の影響範囲を再確認した
- [ ] 削除・名称変更・意味変更した概念を repository 全体で確認し、古い説明や期待値を残していない
- [ ] 同じ contract を複数文書へ同じ粒度でコピーせず、正本への参照へ寄せた
- [ ] 不要になった仕様、設計、test、code、compatibility logic、setting、document を残していない
- [ ] 将来拡張だけを目的とする abstraction、framework、setting を追加していない

## 検証

<!-- 実行した build / test / package / manual verification と結果を記載する。docs-only の場合はその旨を記載する。 -->
