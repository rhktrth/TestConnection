# TestConnection ドキュメント構成

このディレクトリの文書は、TestConnection を仕様駆動で変更するための現在有効な正本です。

仕様 ID や受入条件は追跡のために使いますが、文書自体は人間が上から読んで理解しやすい構成を優先します。ID のために同じ内容を複数文書へ複製しません。

## 1. 正本と責務

| 文書 | 正本として扱う内容 | 書かない内容 |
| --- | --- | --- |
| [`../README.md`](../README.md) | 利用者向け概要、導入、基本的な使い方、重要な注意 | 詳細な契約、内部構造、開発手順 |
| [`SPEC.md`](SPEC.md) | 利用者、接続相手、OS、設定ファイル、出力から観測できる現在の外部仕様、`EXT-*`、`AC-*`、外部仕様の追跡 | C# のクラス構成や非公開の実装方式 |
| [`ARCHITECTURE.md`](ARCHITECTURE.md) | コンポーネント責務、処理順、並行処理、resource ownership、内部不変条件、主要な `INT-*` | 利用者向け説明、外部仕様の詳細な再掲 |
| [`OPERATIONS.md`](OPERATIONS.md) | build、test、package、CI、release の実際の手順 | 製品仕様、設計判断の理由 |
| [`adr/`](adr/) | 現在有効な重要な設計判断の理由と見直し条件 | SPEC / ARCHITECTURE の契約のコピー |
| [`../AGENTS.md`](../AGENTS.md) | AI・開発者が上記の正本を維持する作業規則と Definition of Done | 製品仕様そのもの |
| [`.github/workflows/test.yml`](../.github/workflows/test.yml) | Pull Request CI で実際に実行する command | build 手順の説明書 |
| [`.github/workflows/release.yml`](../.github/workflows/release.yml) | GitHub Release asset を生成する実際の automation | release 手順の説明書 |

TestConnection では、他リポジトリで用いている `EXTERNAL_DESIGN.md` / `INTERNAL_DESIGN.md` を追加しません。既存の文書体系を活かし、**`SPEC.md` が外部設計、`ARCHITECTURE.md` が内部設計の役割を持つ**ものとします。同じ責務の文書を名前違いで増やさないことを優先します。

source code と test は仕様を実現・検証する成果物です。正本文書と矛盾した場合に、自動的に source code 側を正本として扱いません。要求と正本を確認し、どちらを直すべきかを先に決めます。

## 2. 仕様駆動の変更順序

機能、外部挙動、内部構造を変更するときは、production code を先に変更して文書を追認させません。着手時に変更を次のいずれかへ分類します。

- **A: 外部仕様変更** — protocol の success / failure、設定形式、入力、出力、NIC 操作、利用者から見える挙動が変わる。
- **B: 内部設計変更** — 外部仕様は維持したまま、責務、処理順、並行処理、resource ownership、error handling 等が変わる。
- **C: 運用変更** — build、test、package、CI、release の方法が変わる。
- **D: 実装のみ** — bug fix や整理で、既存の仕様・設計・運用が変更後もそのまま真である。

原則の作業順序は次です。

1. `docs/README.md` と関係する正本文書、ADR、test、production code の順に現状を確認する。
2. A の場合は `SPEC.md` を先に変更し、維持すべき契約を `EXT-*` として確定する。
3. 回帰リスクがある外部契約には、implementation detail に依存しない `AC-*` を追加・更新する。
4. B、または A に伴って内部構造が変わる場合は `ARCHITECTURE.md` を先に更新し、必要な設計単位だけ `INT-*` で識別する。
5. 複数の合理的な選択肢があり、将来も判断理由を残す価値がある場合だけ ADR を追加・変更・統合する。
6. 変更後の仕様・設計を検証する test を変更・追加する。
7. その仕様・設計・test を満たすよう production code、project file、workflow 等を変更する。
8. README / OPERATIONS 等へ影響する場合は、それぞれの責務の範囲だけ更新する。
9. 最後に関連する正本、test、implementation を横断し、古い挙動、用語、設定、重複記述が残っていないことを確認する。

内部設計を検討した結果、最初に考えた外部仕様が不適切と分かった場合は、implementation に合わせて黙って挙動を変えません。`SPEC.md` へ戻って外部仕様を明示的に改訂してから先へ進みます。

D の変更では文書を機械的に触りません。ただし Pull Request には、なぜ仕様・設計変更が不要なのかを記載し、関連する正本が変更後も真であることを確認します。

## 3. Stable ID と TBD

ID は章立ての代わりではなく、変更・review・test を追跡する価値がある契約だけに付与します。

- 外部仕様: `EXT-<AREA>-NNN`
- 受入条件: `AC-<AREA>-NNN`
- 内部設計: `INT-<AREA>-NNN`
- 未確定事項: `TBD-<AREA>-NNN`

同じ意味の契約は同じ ID を維持します。文章を整理するためだけに ID を振り直しません。契約を分割・廃止する必要がある場合は Pull Request で理由を説明します。

`AC-*` は private method や mock の呼出順ではなく、利用者、socket、file、出力等から観測できる Given / When / Then として定義します。自動化できない環境依存の受入条件は、無理に fake test を作らず manual verification と明示します。

未確定事項が implementation を止める場合は `TBD-*` として正本へ明記します。現在の source code、一般的な best practice、他製品の慣例、AI の推測だけで TBD を解消しません。

## 4. Traceability

外部挙動を変更する場合は、原則として次を双方向に追える状態を維持します。

```text
EXT -> AC -> test -> production component
              \
               -> manual verification（自動化できない場合）
```

内部設計が重要な場合は `INT-*` を間に置きます。

```text
EXT / AC -> INT -> test / production component
```

`SPEC.md` 末尾の対応表を automated acceptance の索引とします。別の巨大な traceability 文書は作りません。`ARCHITECTURE.md` は外部仕様の全文をコピーせず、必要な `EXT-*` / `AC-*` を参照して内部責務を説明します。

## 5. 重複を増やさない規則

- README は利用者が最初に必要とする概要と注意だけを書き、詳細な success 条件、名前解決規則、設定 contract は `SPEC.md` へ寄せる。
- SPEC は外部から観測できる契約を書く。C# class、thread API、private helper 等を仕様へ固定しない。
- ARCHITECTURE は内部設計を書く。外部契約を説明する必要がある場合は `EXT-*` を参照し、同じ表や条件を再掲しない。
- OPERATIONS は実際に実行する command と release 操作を書く。製品仕様を再説明しない。
- ADR は「なぜその判断か」「どの前提が変われば見直すか」に限定し、現在仕様の一覧を持たない。
- AGENTS は作業規則を定義し、製品の個別仕様をコピーしない。
- test は同じ契約を複数階層で重複して固定しない。上位 test で十分に検証できる場合は private detail の test を増やさない。
- 過去の構成、廃止済み仕様、移行経緯は現行文書へ蓄積せず、Git history / Issue / Pull Request / GitHub Releases に任せる。

## 6. 矛盾の扱い

正本同士、または正本と implementation に矛盾を見つけた場合は、次の順で解消します。

1. その挙動に対する現在の要求を確認する。
2. 責務表に従い、どの文書がその事項の正本かを特定する。
3. 正本を先に一意な内容へ直す。
4. acceptance、内部設計、test、implementation を正本へ合わせる。
5. 同じ内容を別文書へ再度コピーして整合を取ろうとしない。

仕様駆動開発は文書量を増やすことではなく、**変更前に契約を確定し、正本を一つにし、その契約を test と implementation へ追跡できる状態を保つこと**を目的とします。
