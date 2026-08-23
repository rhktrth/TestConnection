# ADR-0002: 疎通判定は protocol の最小成立点に限定する

## Decision

TestConnection の success は、各 protocol で現在観測している最小の成立点に限定します。application protocol 固有の正常性まで暗黙に拡張しません。

- TCP client: connection が確立した時点で success とし、application data を送信せず切断する。
- TCP server: connection を accept した時点で success とし、application data を交換せず切断する。
- UDP client: datagram の送信 API が成功した時点で success とする。遠端到達は意味しない。
- UDP server: datagram を受信した時点で success とする。
- DNS client: 固定 query を指定 DNS server へ送信し、UDP response を受信した時点で success とする。transaction ID、RCODE、answer 内容は判定しない。
- Ping client: ICMP Echo Request を送信し、同じ宛先から type=Echo Reply、identifier、sequence number が対応する response を受信した時点で success とする。

local IP address が指定された tester はその address に bind し、未指定の場合は OS に送信元 address の選択を委ねます。

## Rationale

TestConnection の主用途は、router、firewall、経路制御、冗長構成等がある環境で、指定した送信元から指定した宛先への通信がどこまで成立するかを観測することです。

application 固有の handshake、認証、業務データ、長時間 session 維持まで success 条件へ含めると、試験対象サービスへの依存と副作用が増えます。また TCP、UDP、DNS、ICMP では success が技術的に意味する範囲が異なるため、一律に「相手が正常」と表現しません。

Ping については、raw ICMP socket が無関係な ICMP packet も受信し得るため、「何らかのpacketを受信した」だけではfalse positiveになり得ます。Echo Replyのtype / identifier / sequence numberと送信先addressを照合することはapplication-level判定への拡張ではなく、ICMP Echo成立点そのものを正しく観測するための条件とします。

## Consequences

- TCP success は対象 application の正常性を保証しない。
- UDP client の success だけでは遠端到達を証明できない。UDP server、packet capture、firewall log 等の別観測点が必要になる。
- DNS success は UDP response の受信を示すが、名前解決結果の意味的妥当性を保証しない。
- Ping success は送信した Echo Request に対応する Echo Reply の受信を示すが、application port や上位applicationの正常性は保証しない。
- success semantics を強化・変更する場合は、既存の意味を暗黙に変えず、この ADR と利用者向け説明を先に更新する。
