# ADR-0003: client 試験は順次反復し協調的に停止する

## Decision

client 試験は、同時性能測定ではなく時系列での疎通変化を観測する用途を優先し、**登録順の逐次実行と協調的な停止**を採用します。

現在の外部 contract は [`../SPEC.md`](../SPEC.md) の `EXT-RUN-001`、内部設計は [`../ARCHITECTURE.md`](../ARCHITECTURE.md) の `INT-SESSION-001`, `INT-RUNNER-001`, `INT-SERVER-001`, `INT-TESTER-001` を正本とします。

この ADR は具体的な start / stop sequence や method 名を重複定義せず、「逐次実行を選ぶこと」「blocking I/O を強制的な thread termination ではなく resource owner の cancel / close で解除すること」の判断理由を保持します。

## Rationale

TestConnection は複数の疎通試験を一定間隔で繰り返し、経路や冗長機器の切替前後に success / failure の変化を観測する用途を持ちます。この用途では多数 endpoint への同時接続より、試験順序と間隔が安定し、result を時系列で追いやすいことを優先します。

TCP connect、DNS receive、Ping receive 等には blocking I/O があります。worker thread を強制終了すると socket 等の resource ownership と cleanup が不明確になるため、実行モデルの停止要求と protocol-specific resource の解放責務を分けます。

## Consequences

- client の実行順と試行間隔を追いやすい。
- load test や多数 endpoint の同時性能測定には向かない。
- 一つの client 試行が timeout まで掛かると、後続項目もその分遅れる。
- client の並列化が必要になった場合は場当たり的に worker を増やさず、result の時系列 semantics、停止 contract、resource ownership を含めて `SPEC.md` / `ARCHITECTURE.md` とこの判断を見直す。
- start / stop sequence や cancellation の具体的な contract を変更する場合は、この ADR へ詳細を追加するのではなく、まず正本の `EXT-*` / `INT-*` を更新する。
