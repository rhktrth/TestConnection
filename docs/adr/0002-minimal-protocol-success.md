# ADR-0002: 疎通判定は protocol の最小成立点に限定する

## Decision

TestConnection の success は、各 protocol で疎通確認に必要な**最小の成立点**に限定し、application protocol 固有の正常性まで暗黙に拡張しません。

現在の外部 contract は [`../SPEC.md`](../SPEC.md) の次を正本とします。

- `EXT-TCP-001`, `EXT-TCP-002`
- `EXT-UDP-001`, `EXT-UDP-002`
- `EXT-DNS-001`
- `EXT-PING-001`

この ADR は各 success 条件の詳細を重複定義せず、「最小成立点までを TestConnection の責務境界とする」という判断理由だけを保持します。

## Rationale

TestConnection の主用途は、router、firewall、経路制御、冗長構成等がある環境で、指定した送信元から指定した宛先への通信がどこまで成立するかを観測することです。

application 固有の handshake、認証、業務 data、長時間 session 維持まで success 条件へ含めると、試験対象 service への dependency と副作用が増えます。また TCP、UDP、DNS、ICMP では protocol 上観測できる成立点が異なるため、一律に「相手が正常」と表現しません。

Ping は raw ICMP socket が無関係な packet も受信し得ます。そのため送信した Echo Request と reply の相関を確認することは application-level 判定への拡張ではなく、ICMP Echo の成立点そのものを正しく観測するために必要です。

## Consequences

- TCP success は接続先 application の handshake、認証、業務処理の成功を保証しない。
- UDP client の success だけでは遠端到達を証明できず、別の観測点を必要とする。
- DNS success は UDP response の受信を示すが、response 内容の意味的妥当性までは保証しない。
- Ping success は対応する Echo Reply の受信を示すが、application port や上位 application の正常性は保証しない。
- 将来 application-level check を追加する場合は、既存 protocol tester の success の意味を黙って強化せず、別 contract として `SPEC.md` で先に定義する。
- protocol ごとの正確な条件を変更する場合は、この ADR の文章をコピー修正するのではなく、まず `SPEC.md` と acceptance を更新する。
