# TestConnection 向け AI・開発ルール

このファイルは TestConnection を変更するときの作業規則と完了条件を定めます。製品仕様そのものは再定義しません。

TestConnection は、Windows 上で TCP / UDP / DNS / Ping の疎通確認を行う小型ツールを、個人で全体を把握・保守できる規模に保ちます。

## 正本

- 文書体系、仕様 ID、acceptance、traceability の運用: [`docs/README.md`](docs/README.md)
- 外部から観測できる現在仕様: [`docs/SPEC.md`](docs/SPEC.md)
- 利用者向け概要・使い方: [`README.md`](README.md)
- 内部構成、責務、処理方式: [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md)
- build / test / package / release: [`docs/OPERATIONS.md`](docs/OPERATIONS.md)
- 現在有効な重要な設計判断と理由: [`docs/adr/`](docs/adr/)
- Pull Request CI: [`.github/workflows/test.yml`](.github/workflows/test.yml)
- Release automation: [`.github/workflows/release.yml`](.github/workflows/release.yml)
- release 履歴と release note: GitHub Releases

source code と test は仕様を実現・検証する成果物です。正本文書と矛盾した場合、自動的に code 側を正本として扱いません。

## 仕様駆動開発

機能、外部挙動、内部構造を変更するときは、production code を先に変更して仕様を後付けしません。

### 作業開始時の分類

変更を始める前に、少なくとも次のどれに該当するかを判断します。

- **A: 外部仕様変更** — protocol の success / failure、設定形式、入力、出力、NIC 操作等、利用者・接続相手・OS・file から観測できる挙動が変わる。
- **B: 内部設計変更** — 外部仕様は維持したまま、責務、処理順、並行処理、resource ownership、error handling 等が変わる。
- **C: 運用変更** — build、test、package、CI、release の方法が変わる。
- **D: 実装のみ** — bug fix、単純整理等で、既存の仕様・設計・運用が変更後もそのまま真である。

### 変更順序

原則として次の順序で変更します。

1. `docs/README.md` と関係する正本文書、ADR、関連 test、production code の順に現状を確認する。
2. A の場合は `docs/SPEC.md` を先に変更し、変更後の外部 contract を `EXT-*` で確定する。
3. 回帰リスクがある外部 contract には implementation detail に依存しない `AC-*` を追加・更新する。
4. B、または A に伴って内部構造が変わる場合は `docs/ARCHITECTURE.md` を先に更新し、必要な設計単位だけ `INT-*` で識別する。
5. 複数の合理的な選択肢があり、将来も判断理由を残す価値がある場合だけ ADR を追加・変更・統合する。
6. `EXT -> AC -> INT（必要な場合）-> test` の対応を確認し、変更後の仕様・設計を検証する test を変更・追加する。
7. 最後に production code、project file、workflow 等を変更する。
8. README / OPERATIONS 等へ影響がある場合は、それぞれの責務の範囲だけ更新する。
9. 仕様、設計、test、implementation を横断し、古い説明、用語、設定、期待値、不必要な重複が残っていないことを確認する。

内部設計を検討した結果、当初の外部仕様が不適切だと分かった場合は、implementation に合わせて黙って挙動を変えず、`docs/SPEC.md` へ戻って外部仕様を明示的に改訂してから先へ進みます。

D の変更では文書を機械的に変更しません。ただし Pull Request には「仕様・設計変更なし」と、その判断理由を明記し、関係する正本が変更後も真であることを確認します。

### Stable ID と TBD

- 外部仕様: `EXT-<AREA>-NNN`
- acceptance: `AC-<AREA>-NNN`
- 内部設計: `INT-<AREA>-NNN`
- 未確定事項: `TBD-<AREA>-NNN`

ID は全行へ機械的に付けません。変更、review、test で独立して参照する価値がある contract にだけ付与します。同じ意味の contract は同じ ID を維持します。

`AC-*` は public / private method の呼出順ではなく、利用者、socket、file、result output 等から観測できる条件として定義します。

未確定事項が implementation を止める場合は `TBD-*` として正本へ明記し、現在の source code、一般的 best practice、他製品の慣例、AI の推測だけで確定しません。

## 原則

- 現在必要な疎通確認機能を単純に保ち、将来拡張だけを目的とする abstraction、framework、compatibility layer、設定項目を追加しない。
- 共通化は変更理由が同じものに限定する。見た目が似ているだけではまとめない。
- 挙動変更を伴わない整理では、`docs/SPEC.md` の外部 contract を変えない。
- 過去の構成、移行経緯、廃止済み仕様を code / docs / comment に残さない。履歴は Git history / Issue / Pull Request / GitHub Releases に任せる。
- runtime / UI baseline は [`ADR-0001`](docs/adr/0001-winforms-net481-zip.md) を正本とし、実装を先に変更しない。
- runtime dependency は .NET Framework 標準ライブラリだけを基本とする。
- IDE 固有設定を build / package の正本にしない。build / package の実処理は project file、CI orchestration は workflow を正本とする。
- installer / ClickOnce を前提にせず、展開して実行できる ZIP を配布単位とする。
- 実名、個人メールアドレス、local user name、個人環境固有 path 等、作者個人を特定し得る情報を code、設定、配布物へ含めない。ライセンス上必要な表示と公開 GitHub repository identifier は除く。

## C# / WinForms

- target framework、UI framework、配布形態を変更する場合は、まず関連する SPEC / ADR / ARCHITECTURE を更新する。
- project file は SDK-style を維持する。
- C# はスペース4文字、XML / YAML はスペース2文字、UTF-8 / LF とし、共有書式は `.editorconfig` を正本とする。
- comment は日本語で、code から分からない理由・制約だけを書く。過去実装の説明は書かない。
- `MainForm` と通信処理の分離は、実際の変更理由がある範囲で段階的に行う。一括した全面再設計はしない。

## 文書

- 文書の責務境界は `docs/README.md` に従う。
- README は利用者向け概要、導入、基本的な使い方、重要な注意だけを書く。
- SPEC は外部から観測可能で維持すべき現在の contract、`EXT-*`、`AC-*`、acceptance の追跡を書く。
- ARCHITECTURE は内部構成、responsibility、lifecycle、concurrency、resource ownership、error handling、内部不変条件を書く。
- OPERATIONS は build / test / package / CI / release の実際の手順だけを書く。
- ADR は current specification をコピーせず、重要な判断の理由と見直し条件を書く。
- 同じ仕様を複数文書へ同じ粒度で重複記載しない。別の文書から必要な場合は正本の ID / section を参照する。
- repository 内に CHANGELOG を維持しない。release 履歴と release note は GitHub Releases を正本とする。
- 判断が変わった ADR は現在の設計を最も簡潔に表すよう編集・統合・削除し、superseded ADR を履歴目的で保存しない。

## テスト

- coverage 率より、外部 contract、通信判定、停止処理、設定読み書き、resource cleanup、配布物欠落等の重大な回帰を優先する。
- acceptance test は private implementation ではなく socket、file、result output 等の観測結果を検証する。
- network integration test は原則 loopback を使用し、通常 CI から外部 host へ接続しない。
- OS privilege や実 NIC に依存して deterministic に自動化できない項目は、無理に fake test を作らず manual verification として `docs/SPEC.md` に明示する。
- 不具合修正では、現実的に自動化できる場合は再現 test を追加する。
- private implementation の細かな呼出順だけを固定する test を増やさない。
- 上位 test で十分に検証できる処理を別階層で重複して test しない。
- 仕様変更でないのに、現在の implementation へ合わせるためだけに期待値を変更しない。

## GitHub 上の変更

- 挙動・構造変更は branch / Pull Request を基本とし、必要に応じて Issue で要求を記録する。
- Issue、Pull Request、commit 説明は日本語で書く。識別子・製品名・技術用語は不自然に日本語化しない。
- unrelated refactoring を同じ Pull Request に混ぜない。ただし仕様、test、implementation、文書を一致させるため不可分な整理はまとめてよい。
- Pull Request では [`.github/pull_request_template.md`](.github/pull_request_template.md) の変更分類、仕様先行、traceability、整合性確認を埋める。
- GitHub Actions は build / test / release の orchestration に留め、project file や test code が正本として検証する処理を workflow script で二重実装しない。
- default branch は `main` とする。
- ユーザーの明示指示なしに Pull Request を merge しない。

## Definition of Done

1. 外部挙動を変更した場合、production code より先に `docs/SPEC.md` と必要な `EXT-*` / `AC-*` を確定した。
2. 内部責務・lifecycle・resource ownership 等を変更した場合、必要な `INT-*` と ARCHITECTURE / ADR を更新した。
3. `EXT -> AC -> INT（必要な場合）-> test -> production component` を追跡できる。
4. `net481` の Release package build と regression test が Pull Request CI で成功する。
5. `Package` target で ZIP が生成され、配布物の構成が project file の定義と一致する。
6. README / SPEC / ARCHITECTURE / OPERATIONS / ADR / test / implementation に矛盾や不要な重複がない。
7. 削除・名称変更・意味変更した概念を repository 全体で確認し、古い説明や期待値を残していない。
8. 変更後に不要になった file、setting、document、test、compatibility logic を残していない。
9. Pull Request の差分が依頼・Issue の目的に閉じている。

実際の CI command は `.github/workflows/test.yml` を正本とします。
