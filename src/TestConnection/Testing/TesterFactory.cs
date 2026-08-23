using System;

namespace TestConnection {
    internal static class TesterFactory {
        public static TesterBase Create(
            TesterDefinition definition, Action<string> resultOutput, int timeoutMilliseconds) {
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }
            if (resultOutput == null) {
                throw new ArgumentNullException("resultOutput");
            }

            TesterBase tester;
            if (definition.Role == TesterRole.Server) {
                tester = CreateServer(definition, resultOutput);
            }
            else if (definition.Role == TesterRole.Client) {
                tester = CreateClient(definition, resultOutput);
            }
            else {
                throw new InvalidOperationException("roleが無効です。");
            }

            tester.TimeoutMilliseconds = timeoutMilliseconds;
            return tester;
        }

        private static TesterServer CreateServer(TesterDefinition definition, Action<string> resultOutput) {
            switch (definition.Protocol) {
                case ProtocolName.TCP:
                    return new TcpTesterServer(resultOutput, definition);
                case ProtocolName.UDP:
                    return new UdpTesterServer(resultOutput, definition);
                default:
                    throw new InvalidOperationException("server protocolが無効です。");
            }
        }

        private static TesterClient CreateClient(TesterDefinition definition, Action<string> resultOutput) {
            switch (definition.Protocol) {
                case ProtocolName.TCP:
                    return new TcpTesterClient(resultOutput, definition);
                case ProtocolName.UDP:
                    return new UdpTesterClient(resultOutput, definition);
                case ProtocolName.DNS:
                    return new DnsTesterClient(resultOutput, definition);
                case ProtocolName.Ping:
                    return new PingTesterClient(resultOutput, definition);
                default:
                    throw new InvalidOperationException("client protocolが無効です。");
            }
        }
    }
}
