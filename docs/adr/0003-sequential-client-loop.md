# ADR-0003: client 試験は順次反復し協調的に停止する

## Decision

client 試験は一つの background worker で登録順に実行し、item interval と list interval を挟んで反復します。

- client tester を一斉並列実行せず、一項目ずつ `Try()` する。
- repeat count が 0 の場合は停止要求まで反復する。
- interval 待機は停止 event を監視し、停止要求で解除できるようにする。
- 停止は `Thread.Abort()` 等の強制終了を使わず、停止 event と現在実行中 tester の `Cancel()` を組み合わせる。
- server tester は client loop から分離し、それぞれ background thread で継続待受する。

protocol resource の close / dispose は各 tester が担当します。TCP / UDP / DNS / Ping client は normal / error / timeout / cancel の cleanup を `finally` 等の共通経路へ収束させ、blocking I/O を `Cancel()` で解除できる構造を維持します。server の thread lifecycle は `TesterServer` が共通管理し、具象 server は protocol 固有の listener / socket close だけを担当します。

## Rationale

TestConnection は複数の疎通試験を一定間隔で繰り返し、経路や冗長機器の切替前後に成功・失敗の変化を観測する用途を持ちます。この用途では大量の同時接続を生成することより、試験順序と間隔が安定し、時系列で結果を追いやすいことを優先します。

TCP connect、DNS receive、Ping receive 等には blocking I/O があるため、worker thread を強制終了すると protocol resource や状態の後始末が不明確になります。停止要求と protocol 固有の `Cancel()` を分けることで、実行モデルと I/O 解放責務を分離します。

## Consequences

- client の実行順と試行間隔を追いやすい。
- load test や多数 endpoint の同時性能測定には向かない。
- 一つの client 試行が timeout まで掛かると、後続項目もその分遅れる。
- client の並列化が必要になった場合は、既存 loop へ場当たり的に追加せず、result の時系列 semantics と停止モデルを含めて設計判断を更新する。
- protocol tester を変更する場合は、停止時の blocking I/O 解除と resource ownership を明確にする。
