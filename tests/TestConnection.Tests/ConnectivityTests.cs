using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TestConnection;

namespace TestConnection.Tests {
    internal static class ConnectivityTests {
        public static void TestTcpLoopback() {
            int port = GetAvailableTcpPort();
            TcpTesterServer server = new TcpTesterServer(
                delegate { },
                new TesterDefinition(TesterRole.Server, "127.0.0.1", string.Empty, ProtocolName.TCP, port));
            TcpTesterClient client = new TcpTesterClient(
                delegate { },
                new TesterDefinition(TesterRole.Client, "127.0.0.1", "127.0.0.1", ProtocolName.TCP, port));
            server.TimeoutMilliseconds = 1000;
            client.TimeoutMilliseconds = 2000;

            try {
                server.Start();
                client.RunOnce();
                TestAssert.Equal(1, client.SuccessCount, "TCP client success count");
                TestAssert.Equal(0, client.FailureCount, "TCP client failure count");
                TestAssert.True(WaitFor(delegate { return server.SuccessCount == 1; }, 3000), "TCP server accept");
                TestAssert.Equal(0, server.FailureCount, "TCP server failure count");
            } finally {
                server.Stop();
            }
        }

        public static void TestUdpLoopback() {
            int port = GetAvailableUdpPort();
            UdpTesterServer server = new UdpTesterServer(
                delegate { },
                new TesterDefinition(TesterRole.Server, "127.0.0.1", string.Empty, ProtocolName.UDP, port));
            UdpTesterClient client = new UdpTesterClient(
                delegate { },
                new TesterDefinition(TesterRole.Client, "127.0.0.1", "127.0.0.1", ProtocolName.UDP, port));

            try {
                server.Start();
                client.RunOnce();
                TestAssert.Equal(1, client.SuccessCount, "UDP client success count");
                TestAssert.Equal(0, client.FailureCount, "UDP client failure count");
                TestAssert.True(WaitFor(delegate { return server.SuccessCount == 1; }, 3000), "UDP server receive");
                TestAssert.Equal(0, server.FailureCount, "UDP server failure count");
            } finally {
                server.Stop();
            }
        }

        public static void TestDnsLoopback() {
            List<string> outputs = new List<string>();
            using (UdpClient dnsServer = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0))) {
                int port = ((IPEndPoint)dnsServer.Client.LocalEndPoint).Port;
                dnsServer.Client.ReceiveTimeout = 3000;
                DnsTesterClient tester = new DnsTesterClient(
                    delegate(string message) { outputs.Add(message); },
                    new TesterDefinition(
                        TesterRole.Client, "127.0.0.1", "127.0.0.1", ProtocolName.DNS, port));
                tester.TimeoutMilliseconds = 3000;

                Thread tryThread = new Thread(tester.RunOnce);
                tryThread.Start();

                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] query = dnsServer.Receive(ref clientEndPoint);
                TestAssert.True(query.Length != 0, "DNS query received by loopback server");

                byte[] response = { 0x00 };
                dnsServer.Send(response, response.Length, clientEndPoint);

                TestAssert.True(tryThread.Join(3000), "DNS Try exits after response");
                TestAssert.Equal(1, tester.SuccessCount, "DNS response success count");
                TestAssert.Equal(0, tester.FailureCount, "DNS response failure count");
                TestAssert.Equal(1, outputs.Count, "DNS response output count");
                TestAssert.True(
                    outputs[0].Contains("dns-response-from 127.0.0.1:" + port),
                    "DNS response endpoint output");
            }
        }

        public static void TestDnsCancellation() {
            List<string> outputs = new List<string>();
            using (UdpClient sink = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0))) {
                int port = ((IPEndPoint)sink.Client.LocalEndPoint).Port;
                sink.Client.ReceiveTimeout = 3000;
                DnsTesterClient tester = new DnsTesterClient(
                    delegate(string message) { outputs.Add(message); },
                    new TesterDefinition(
                        TesterRole.Client, "127.0.0.1", "127.0.0.1", ProtocolName.DNS, port));
                tester.TimeoutMilliseconds = 10000;

                Thread tryThread = new Thread(tester.RunOnce);
                tryThread.Start();

                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                byte[] query = sink.Receive(ref sender);
                TestAssert.True(query.Length != 0, "DNS query received by loopback sink");

                tester.Cancel();
                TestAssert.True(tryThread.Join(3000), "DNS Try exits after Cancel");
                TestAssert.Equal(0, tester.SuccessCount, "DNS cancel success count");
                TestAssert.Equal(0, tester.FailureCount, "DNS cancel failure count");
                TestAssert.Equal(0, outputs.Count, "DNS cancel result output count");
            }
        }

        public static void TestResultLogFormat() {
            string output = null;
            LogTester tester = new LogTester(delegate(string message) { output = message; });

            tester.Emit(null, "startup-error");
            TestAssert.True(output.EndsWith("Client TCP startup-error"), "prefix without local endpoint");
            TestAssert.False(output.Contains(":/TCP"), "no empty endpoint separators");

            tester.Emit(new IPEndPoint(IPAddress.Parse("192.0.2.10"), 12345),
                "connected-to 198.51.100.20:443");
            TestAssert.True(
                output.EndsWith("Client 192.0.2.10:12345/TCP connected-to 198.51.100.20:443"),
                "prefix with local endpoint");
        }

        public static void TestRemoteEndpointResolver() {
            TestAssert.True(RemoteEndpointResolver.IsSupportedInput("192.0.2.1"), "IPv4 literal input");
            TestAssert.True(RemoteEndpointResolver.IsSupportedInput("service.example.test"), "hostname input");
            TestAssert.False(RemoteEndpointResolver.IsSupportedInput("2001:db8::1"), "IPv6 literal input");
            TestAssert.False(RemoteEndpointResolver.IsSupportedInput("   "), "blank input");
            TestAssert.Equal(IPAddress.Parse("192.0.2.1"), RemoteEndpointResolver.ResolveIpv4("192.0.2.1"),
                "IPv4 literal resolution");

            IPAddress selected = RemoteEndpointResolver.SelectLowestIpv4(new IPAddress[] {
                IPAddress.Parse("203.0.113.10"),
                IPAddress.Parse("2001:db8::1"),
                IPAddress.Parse("192.0.2.200"),
                IPAddress.Parse("192.0.2.2")
            });
            TestAssert.Equal(IPAddress.Parse("192.0.2.2"), selected, "lowest IPv4 selection");
            TestAssert.Equal<IPAddress>(null, RemoteEndpointResolver.SelectLowestIpv4(new IPAddress[] {
                IPAddress.Parse("2001:db8::1")
            }), "no IPv4 selection");
        }

        private static int GetAvailableTcpPort() {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            try {
                return ((IPEndPoint)listener.LocalEndpoint).Port;
            } finally {
                listener.Stop();
            }
        }

        private static int GetAvailableUdpPort() {
            using (UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0))) {
                return ((IPEndPoint)client.Client.LocalEndPoint).Port;
            }
        }

        private static bool WaitFor(Func<bool> condition, int timeoutMilliseconds) {
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);
            while (DateTime.UtcNow < deadline) {
                if (condition()) {
                    return true;
                }
                Thread.Sleep(10);
            }
            return condition();
        }

        private sealed class LogTester : TesterBase {
            public LogTester(Action<string> output)
                : base(output, new TesterDefinition(
                    TesterRole.Client, string.Empty, "127.0.0.1", ProtocolName.TCP, 1)) {
            }

            public void Emit(IPEndPoint endpoint, string message) {
                localEndPoint = endpoint;
                WriteResult(message);
            }
        }
    }
}
