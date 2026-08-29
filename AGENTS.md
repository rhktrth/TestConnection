# TestConnection 向け AI・開発ルール

このファイルは TestConnection を変更するときの作業規則と完了条件を定めます。製品仕様そのものは再定義しません。

## 正本

- 仕様駆動の工程・文書体系: [`docs/README.md`](docs/README.md)
- 要件: [`docs/REQUIREMENTS.md`](docs/REQUIREMENTS.md)
- 外部設計: [`docs/EXTERNAL_DESIGN.md`](docs/EXTERNAL_DESIGN.md)
- 内部設計: [`docs/INTERNAL_DESIGN.md`](docs/INTERNAL_DESIGN.md)
- 利用者向け概要・使い方: [`README.md`](README.md)
- build / test / package / release: [`docs/OPERATIONS.md`](docs/OPERATIONS.md)
- 重要な設計判断: [`docs/adr/`](docs/adr/)
- Pull Request CI: [`.github/workflows/test.yml`](.github/workflows/test.yml)
- Release automation: [`.github/workflows/release.yml`](.github/workflows/release.yml)

source code と test は、上位の要件・設計を実現・検証する成果物です。矛盾した場合に code や test を自動的な正本として扱いません。

## 仕様駆動開発の必須順序

機能、挙動、内部構造を変更するときは、**要件 → 外部設計 → 内部設計 → 実装 → テスト** の順序を必ず守ります。

1. **要件確認・要件定義**
   - 依頼が既存 `REQ-*` の範囲内か、要件変更かを判断する。
   - 要件変更なら、最初に `docs/REQUIREMENTS.md` を変更して要求を確定する。
2. **外部設計**
   - 確定した要件を observable contract へ落とし、`docs/EXTERNAL_DESIGN.md` の `EXT-*` と必要な `AC-*` を確定する。
3. **内部設計**
   - 外部 contract を実現する responsibility、processing、state、concurrency、resource ownership、error handling を `docs/INTERNAL_DESIGN.md` と `INT-*` で確定する。
4. **実装**
   - 確定した内部設計を production code / project file / configuration / workflow へ実装する。
   - 実装途中で上位設計が不適切と分かった場合は、その場で code に合わせず、該当する上位段階へ戻って先に修正する。
5. **テスト**
   - 実装後に、その実装が `REQ-*` / `EXT-*` / `INT-*` / `AC-*` を満たすことを test / manual verification で検証する。
   - regression risk がある contract に不足する test があれば追加・更新する。
6. **横断確認**
   - 要件、外部設計、内部設計、実装、テスト、README、OPERATIONS、ADR の矛盾・古い記述・不要な重複を確認する。

この順序は commit 分割を要求しません。同一 Pull Request / commit 内でも構いませんが、判断・編集の順序は上流から下流へ進めます。

## 変更分類

作業開始時に次を分類します。

- **A: 要件変更** — 目的、機能要件、非機能要件、制約が変わる。
- **B: 外部設計変更** — 要件は同じだが、外部から観測できる contract が変わる。
- **C: 内部設計変更** — 外部 contract は同じだが、内部責務・処理方式・lifecycle 等が変わる。
- **D: 実装のみ** — 上位仕様を変えない bug fix / refactoring。
- **E: 運用のみ** — build / test / package / CI / release 手順だけが変わる。

A/B/C は下流への影響を必ず確認します。D でも production code を編集する前に、既存 REQUIREMENTS / EXTERNAL_DESIGN / INTERNAL_DESIGN が変更後も真であることを確認します。

## Stable ID と TBD

- 要件: `REQ-<AREA>-NNN`
- 外部仕様: `EXT-<AREA>-NNN`
- acceptance: `AC-<AREA>-NNN`
- 内部設計: `INT-<AREA>-NNN`
- 未確定事項: `TBD-<AREA>-NNN`

ID は全行へ付けません。変更、review、traceability に価値がある contract / design unit にだけ付与します。

未確定事項を source code、現在の test、一般的 best practice、他製品の慣例、AI の推測だけで確定しません。

## Traceability

原則として次を追跡可能にします。

```text
REQ -> EXT / AC -> INT -> implementation -> test
```

- requirement から、対応する外部 contract、内部設計、implementation、verification を探せること。
- test / implementation から、どの上位 requirement / design を実現・検証しているか戻れること。
- traceability のためだけに同じ仕様本文を別文書へコピーしないこと。

## 原則

- 現在必要な疎通確認機能を単純に保ち、将来拡張だけを目的とする abstraction、framework、compatibility layer、設定項目を追加しない。
- 共通化は変更理由が同じものに限定する。見た目が似ているだけではまとめない。
- 過去の構成、移行経緯、廃止済み仕様を code / docs / comment に残さない。履歴は Git history / Issue / Pull Request / GitHub Releases に任せる。
- runtime dependency は .NET Framework 標準ライブラリだけを基本とする。
- IDE 固有設定を build / package の正本にしない。
- installer / ClickOnce を前提にせず、展開して実行できる ZIP を配布単位とする。
- 実名、個人メールアドレス、local user name、個人環境固有 path 等を code、設定、配布物へ含めない。

## C# / WinForms

- target framework / UI framework / distribution contract の変更は、production code より先に REQUIREMENTS / EXTERNAL_DESIGN / ADR を更新する。
- project file は SDK-style を維持する。
- C# はスペース4文字、XML / YAML はスペース2文字、UTF-8 / LF とする。共有書式は `.editorconfig` を正本とする。
- comment は code から分からない理由・制約だけを書く。過去実装の説明は書かない。
- `MainForm` と通信処理の分離は、実際の変更理由がある範囲で段階的に行う。

## 文書

- REQUIREMENTS は「何を必要とするか」を定義し、implementation detail を書かない。
- EXTERNAL_DESIGN は observable contract を定義し、C# class や private API を書かない。
- INTERNAL_DESIGN は responsibility、processing、state、concurrency、resource ownership、error handling を定義し、外部仕様を詳細にコピーしない。
- README は利用者向け説明に限定する。
- OPERATIONS は build / test / package / CI / release の実際の手順に限定する。
- ADR は重要な判断の理由と見直し条件に限定し、current contract を複製しない。
- 同じ内容を複数文書へ同じ粒度で記載しない。
- repository 内に CHANGELOG を維持しない。release 履歴と release note は GitHub Releases を正本とする。

## テスト

- テストは工程上、要件・外部設計・内部設計・実装の後に行う検証段階とする。
- coverage 率より、外部 contract、通信判定、停止処理、設定互換性、resource cleanup、配布物等の重大な回帰を優先する。
- acceptance test は private implementation ではなく socket、file、result output 等の観測結果を検証する。
- network integration test は原則 loopback を使用し、通常 CI から外部 host へ接続しない。
- 実 NIC 等 deterministic に自動化しにくい項目は manual verification として明示する。
- private implementation の細かな呼出順だけを固定する test を増やさない。
- 現在の implementation へ合わせるためだけに上位 contract や test expectation を変更しない。

## GitHub 上の変更

- 挙動・構造変更は branch / Pull Request を基本とする。
- Issue、Pull Request、commit 説明は日本語で書く。識別子・製品名・技術用語は不自然に日本語化しない。
- unrelated refactoring を同じ Pull Request に混ぜない。
- Pull Request では [`.github/pull_request_template.md`](.github/pull_request_template.md) により、要件→外部設計→内部設計→実装→テストの順序と traceability を確認する。
- GitHub Actions は orchestration に留め、project file / test code の検証処理を workflow で二重実装しない。
- default branch は `main` とする。
- ユーザーの明示指示なしに Pull Request を merge しない。

## Definition of Done

1. 変更が既存要件内か要件変更かを最初に判断した。
2. 要件変更なら `REQUIREMENTS.md` を最初に更新した。
3. 外部 contract を変更する場合、`EXTERNAL_DESIGN.md` を内部設計・実装より先に更新した。
4. 内部方式を変更する場合、`INTERNAL_DESIGN.md` を implementation より先に更新した。
5. production code は確定した内部設計に従っている。
6. implementation 後の test / manual verification で上位 requirement / design を満たすことを確認した。
7. `REQ -> EXT / AC -> INT -> implementation -> test` を追跡できる。
8. Pull Request CI の Release package build と regression test が成功する。
9. README / REQUIREMENTS / EXTERNAL_DESIGN / INTERNAL_DESIGN / OPERATIONS / ADR / implementation / test に矛盾や不要な重複がない。
10. 変更後に不要になった file、setting、document、test、compatibility logic を残していない。

実際の CI command は `.github/workflows/test.yml` を正本とします。
