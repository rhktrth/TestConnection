# TestConnection ドキュメント構成

このディレクトリの文書は、TestConnection を仕様駆動で変更するための現在有効な正本です。

TestConnection の仕様駆動開発は、一般的なソフトウェア設計の流れに沿って、**要件 → 外部設計 → 内部設計 → 実装 → テスト** の順序を必ず守ることを中心原則とします。

## 1. 正本と責務

| 段階 | 正本 | 定義する内容 |
| --- | --- | --- |
| 要件 | [`REQUIREMENTS.md`](REQUIREMENTS.md) | 何を満たす必要があるか、目的、機能要件、非機能要件、制約 |
| 外部設計 | [`EXTERNAL_DESIGN.md`](EXTERNAL_DESIGN.md) | 利用者、接続相手、OS、file、出力から観測できる contract、`EXT-*`、`AC-*` |
| 内部設計 | [`INTERNAL_DESIGN.md`](INTERNAL_DESIGN.md) | component responsibility、処理順、lifecycle、concurrency、resource ownership、`INT-*` |
| 実装 | `src/`、project file、必要な設定 | 内部設計を具体化する production implementation |
| テスト | `tests/` | 実装が要件・外部設計・内部設計を満たすことの検証 |
| 運用 | [`OPERATIONS.md`](OPERATIONS.md) | build、test、package、CI、release の実際の手順 |
| 設計判断 | [`adr/`](adr/) | 重要な設計判断の理由と見直し条件 |
| 利用者向け | [`../README.md`](../README.md) | 概要、導入、基本的な使い方、重要な注意 |

`AGENTS.md` はこの工程と正本を維持する作業規則であり、製品仕様そのものの正本にはしません。

## 2. 必須の開発順序

機能、外部挙動、内部構造を変更する場合は、次の順序で進めます。後段を先に変更し、前段を後付けしません。

1. **要件**
   - 依頼が既存要件の範囲内か、要件そのものを変更するかを判断する。
   - 要件変更なら `REQUIREMENTS.md` を最初に更新し、`REQ-*` を確定する。
2. **外部設計**
   - 確定した要件を、外部から観測できる contract へ落とす。
   - `EXTERNAL_DESIGN.md` の `EXT-*` と必要な `AC-*` を確定する。
3. **内部設計**
   - 外部 contract を実現する責務、処理、状態、resource ownership を決める。
   - `INTERNAL_DESIGN.md` と必要な `INT-*` を確定する。
4. **実装**
   - 確定した内部設計を production code / project / configuration へ実装する。
   - 実装都合で上位設計を黙って変更しない。変更が必要なら該当する上位段階へ戻る。
5. **テスト**
   - 完成した実装が `REQ-*` / `EXT-*` / `INT-*` / `AC-*` を満たすことを検証する。
   - 不足する regression / acceptance test を追加・更新する。
6. **横断整合性確認**
   - 要件、外部設計、内部設計、実装、テスト、README、OPERATIONS、ADR に矛盾や不要な重複がないことを確認する。

この順序は「別 commit に分ける」という意味ではありません。同一 Pull Request / commit 内でも構いませんが、**作業上の判断と編集は上流から下流へ進める**ことを要求します。

## 3. 変更分類

着手時に変更を分類します。

- **A: 要件変更** — 目的、提供機能、制約、非機能要件が変わる。
- **B: 外部設計変更** — 要件は同じだが observable contract が変わる。
- **C: 内部設計変更** — 外部 contract は同じだが内部責務・方式が変わる。
- **D: 実装のみ** — 要件・外部設計・内部設計はそのままで bug fix / refactoring を行う。
- **E: 運用のみ** — build / test / package / CI / release 手順だけが変わる。

A は必ず B/C/実装/テストへの影響を確認します。B は C/実装/テストへの影響を確認します。C は実装/テストへの影響を確認します。

D でも、実装変更前に既存 `REQUIREMENTS.md`、`EXTERNAL_DESIGN.md`、`INTERNAL_DESIGN.md` が変更後も真であることを確認します。

## 4. Stable ID

- 要件: `REQ-<AREA>-NNN`
- 外部仕様: `EXT-<AREA>-NNN`
- 受入条件: `AC-<AREA>-NNN`
- 内部設計: `INT-<AREA>-NNN`
- 未確定事項: `TBD-<AREA>-NNN`

ID は章立ての代わりではありません。変更、review、traceability に価値がある単位だけに付与します。

未確定事項は `TBD-*` として上流文書で明示し、source code、現在の test、一般的 best practice、AI の推測だけで確定しません。

## 5. Traceability

原則として次の方向で追跡できる状態を維持します。

```text
REQ -> EXT / AC -> INT -> implementation -> test
```

逆方向にも、test failure や source code から、どの internal design、external contract、requirement を実現・検証しているか説明できることを目標とします。

traceability のためだけの巨大な独立台帳は作らず、`REQUIREMENTS.md`、`EXTERNAL_DESIGN.md`、`INTERNAL_DESIGN.md` の対応表を使用します。

## 6. 文書の重複を避ける規則

- REQUIREMENTS は「何を必要とするか」を書き、具体的な UI / protocol behavior / class design を書かない。
- EXTERNAL_DESIGN は observable contract を書き、C# class や private implementation を書かない。
- INTERNAL_DESIGN は内部責務・方式を書き、外部 contract を詳細にコピーしない。
- README は利用者向け説明に限定し、詳細は外部設計へ参照する。
- OPERATIONS は実際の command / CI / release 操作に限定する。
- ADR は contract の一覧ではなく、重要な判断理由と見直し条件に限定する。
- test は同じ contract を複数階層で重複して固定しない。

重複排除の目的は文書数を減らすことではありません。**要件・外部設計・内部設計を明確に分離し、それぞれの正本を一つにすること**を優先します。

## 7. 矛盾が見つかった場合

矛盾を code に合わせて文書だけ修正しません。

1. `REQUIREMENTS.md` で現在の要求を確認する。
2. 外部 contract の問題なら `EXTERNAL_DESIGN.md` を確認する。
3. 内部方式の問題なら `INTERNAL_DESIGN.md` を確認する。
4. 上流から正しい内容を確定する。
5. その順に実装を合わせる。
6. 最後に test で検証する。

下流の現在状態から上流仕様を後付けするのではなく、常に上流から下流へ整合させます。
