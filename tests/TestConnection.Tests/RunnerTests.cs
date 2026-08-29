using System;
using System.Collections.Generic;
using System.Threading;
using TestConnection;

namespace TestConnection.Tests {
    internal static class RunnerTests {
        public static void TestFiniteClientLoop() {
            int total = 0;
            using (ManualResetEvent runnerCompleted = new ManualResetEvent(false)) {
                FakeTesterClient first = new FakeTesterClient(delegate {
                    Interlocked.Increment(ref total);
                });
                FakeTesterClient second = new FakeTesterClient(delegate {
                    Interlocked.Increment(ref total);
                });

                ClientTestRunner runner = new ClientTestRunner(
                    new TesterClient[] { first, second }, 0, 0,
                    delegate { runnerCompleted.Set(); });
                runner.Start(2);

                TestAssert.True(runnerCompleted.WaitOne(3000), "finite loop completion callback");
                TestAssert.Equal(4, total, "total finite attempts");
                TestAssert.Equal(2, first.TryCount, "first tester count");
                TestAssert.Equal(2, second.TryCount, "second tester count");
                runner.Stop();
            }
        }

        public static void TestClientLoopStop() {
            using (ManualResetEvent started = new ManualResetEvent(false))
            using (ManualResetEvent released = new ManualResetEvent(false))
            using (ManualResetEvent completed = new ManualResetEvent(false)) {
                BlockingTesterClient tester = new BlockingTesterClient(started, released, completed);
                ClientTestRunner runner = new ClientTestRunner(
                    new TesterClient[] { tester }, 1000, 1000);
                runner.Start(0);

                TestAssert.True(started.WaitOne(3000), "tester start");
                runner.Stop();
                TestAssert.True(released.WaitOne(0), "tester cancel");
                TestAssert.True(completed.WaitOne(0), "worker completion before Stop returns");
                TestAssert.Equal(1, tester.CancelCount, "cancel count");
            }

            using (ManualResetEvent completed = new ManualResetEvent(false)) {
                CancellationProbeTesterClient tester = new CancellationProbeTesterClient(completed);
                tester.Cancel();
                tester.RunOnce();
                TestAssert.Equal(0, tester.TryCount, "pre-cancelled tester does not start");

                ClientTestRunner runner = new ClientTestRunner(
                    new TesterClient[] { tester }, 0, 0);
                runner.Start(1);

                TestAssert.True(completed.WaitOne(3000), "runner resets cancellation before new run");
                runner.Stop();
                TestAssert.Equal(1, tester.TryCount, "tester starts after runner reset");
            }
        }

        public static void TestSessionLifecycle() {
            List<string> order = new List<string>();
            TestSession session = CreateSession();

            using (ManualResetEvent firstStarted = new ManualResetEvent(false))
            using (ManualResetEvent firstReleased = new ManualResetEvent(false)) {
                SessionTesterServer firstServer = new SessionTesterServer(order, "first");
                SessionTesterClient firstClient = new SessionTesterClient(order, "first", firstStarted, firstReleased);

                session.Start(
                    new TesterBase[] { firstServer, firstClient },
                    0, 0, 0);
                TestAssert.True(firstStarted.WaitOne(3000), "first client start");
                TestAssert.True(session.IsRunning, "session running");
                session.Stop();
                TestAssert.False(session.IsRunning, "session stopped");
                TestAssert.True(order.IndexOf("first-server-start") < order.IndexOf("first-client-try"),
                    "server starts before client");
                TestAssert.True(order.IndexOf("first-client-cancel") < order.IndexOf("first-server-stop"),
                    "client stops before server");
                TestAssert.Equal(1, firstServer.StopCount, "first server stop count");
                TestAssert.Equal(1, firstClient.CancelCount, "first client cancel count");

                session.Stop();
                TestAssert.Equal(1, firstServer.StopCount, "double Stop server count");
                TestAssert.Equal(1, firstClient.CancelCount, "double Stop client count");
            }

            using (ManualResetEvent secondStarted = new ManualResetEvent(false))
            using (ManualResetEvent secondReleased = new ManualResetEvent(false)) {
                SessionTesterServer secondServer = new SessionTesterServer(order, "second");
                SessionTesterClient secondClient = new SessionTesterClient(order, "second", secondStarted, secondReleased);

                session.Start(
                    new TesterBase[] { secondServer, secondClient },
                    0, 0, 0);
                TestAssert.True(secondStarted.WaitOne(3000), "second client start");
                session.Stop();
                TestAssert.Equal(1, secondServer.StopCount, "second server stop count");
                TestAssert.Equal(1, secondClient.CancelCount, "second client cancel count");
            }
        }

        public static void TestFiniteSessionCompletes() {
            List<string> order = new List<string>();
            using (ManualResetEvent completed = new ManualResetEvent(false)) {
                TestSession session = new TestSession(
                    delegate { }, null, null, delegate { completed.Set(); });
                SessionTesterServer server = new SessionTesterServer(order, "finite");
                FiniteSessionTesterClient client = new FiniteSessionTesterClient();

                session.Start(new TesterBase[] { server, client }, 0, 0, 2);

                TestAssert.True(completed.WaitOne(3000), "finite session completion notification");
                TestAssert.False(session.IsRunning, "finite session stopped automatically");
                TestAssert.Equal(2, client.TryCount, "finite session client count");
                TestAssert.Equal(1, server.StopCount, "finite session server stopped");

                SessionTesterServer serverOnly = new SessionTesterServer(order, "server-only");
                session.Start(new TesterBase[] { serverOnly }, 0, 0, 1);
                Thread.Sleep(100);
                TestAssert.True(session.IsRunning, "server-only session remains running");
                session.Stop();
                TestAssert.Equal(1, serverOnly.StopCount, "server-only session manual stop");
            }
        }

        public static void TestSessionCreatesTester() {
            int successCount = 0;
            int failureCount = 0;
            TestSession session = new TestSession(
                delegate { },
                delegate { successCount++; },
                delegate { failureCount++; });

            TesterDefinition definition = ClientDefinition();
            TesterBase tester;
            string error;
            TestAssert.True(session.TryCreateTester(definition, 1234, out tester, out error),
                "valid tester creation");
            TestAssert.Equal<string>(null, error, "valid tester error");
            TestAssert.True(tester is TcpTesterClient, "factory selects TCP client");
            TestAssert.Equal(definition, tester.Definition, "tester keeps canonical definition");
            TestAssert.Equal(1234, tester.TimeoutMilliseconds, "timeout assigned");

            tester.ClearCount();
            TestAssert.Equal(0, successCount, "success callback not called by creation");
            TestAssert.Equal(0, failureCount, "failure callback not called by creation");

            TesterDefinition invalid = new TesterDefinition(
                TesterRole.Server, "not-an-ip", string.Empty, ProtocolName.TCP, 1);
            TestAssert.False(session.TryCreateTester(invalid, 1000, out tester, out error),
                "invalid tester rejected");
            TestAssert.True(error != null, "invalid tester error");
            TestAssert.Equal<TesterBase>(null, tester, "invalid tester result");
        }

        public static void TestServerStartWaitsForReady() {
            using (ManualResetEvent listenEntered = new ManualResetEvent(false))
            using (ManualResetEvent allowReady = new ManualResetEvent(false))
            using (ManualResetEvent stopRequested = new ManualResetEvent(false)) {
                DelayedTesterServer server = new DelayedTesterServer(listenEntered, allowReady, stopRequested);
                Thread startThread = new Thread(server.Start);
                startThread.Start();

                TestAssert.True(listenEntered.WaitOne(3000), "Listen entered");
                TestAssert.False(startThread.Join(100), "Start returned before listen ready");
                allowReady.Set();
                TestAssert.True(startThread.Join(3000), "Start completion after listen ready");
                server.Stop();
            }
        }

        private static TestSession CreateSession() {
            return new TestSession(delegate { }, null, null);
        }

        private static TesterDefinition ClientDefinition() {
            return new TesterDefinition(TesterRole.Client, string.Empty, "127.0.0.1", ProtocolName.TCP, 1);
        }

        private static TesterDefinition ServerDefinition() {
            return new TesterDefinition(TesterRole.Server, string.Empty, string.Empty, ProtocolName.TCP, 1);
        }

        private sealed class FakeTesterClient : TesterClient {
            private readonly Action onTry;
            public int TryCount { get; private set; }

            public FakeTesterClient(Action onTry)
                : base(delegate { }, ClientDefinition()) {
                this.onTry = onTry;
            }

            public override void RunOnce() {
                TryCount++;
                onTry();
            }

            protected override void CancelCurrentAttempt() {
            }
        }

        private sealed class BlockingTesterClient : TesterClient {
            private readonly ManualResetEvent started;
            private readonly ManualResetEvent released;
            private readonly ManualResetEvent completed;
            public int CancelCount { get; private set; }

            public BlockingTesterClient(ManualResetEvent started, ManualResetEvent released, ManualResetEvent completed)
                : base(delegate { }, ClientDefinition()) {
                this.started = started;
                this.released = released;
                this.completed = completed;
            }

            public override void RunOnce() {
                started.Set();
                released.WaitOne(3000);
                Thread.Sleep(100);
                completed.Set();
            }

            protected override void CancelCurrentAttempt() {
                CancelCount++;
                released.Set();
            }
        }

        private sealed class CancellationProbeTesterClient : TesterClient {
            private readonly ManualResetEvent completed;
            public int TryCount { get; private set; }

            public CancellationProbeTesterClient(ManualResetEvent completed)
                : base(delegate { }, ClientDefinition()) {
                this.completed = completed;
            }

            public override void RunOnce() {
                if (CancellationRequested) {
                    return;
                }
                TryCount++;
                completed.Set();
            }

            protected override void CancelCurrentAttempt() {
            }
        }

        private sealed class FiniteSessionTesterClient : TesterClient {
            public int TryCount { get; private set; }

            public FiniteSessionTesterClient()
                : base(delegate { }, ClientDefinition()) {
            }

            public override void RunOnce() {
                TryCount++;
            }

            protected override void CancelCurrentAttempt() {
            }
        }

        private sealed class SessionTesterClient : TesterClient {
            private readonly List<string> order;
            private readonly string name;
            private readonly ManualResetEvent started;
            private readonly ManualResetEvent released;
            public int CancelCount { get; private set; }

            public SessionTesterClient(List<string> order, string name, ManualResetEvent started, ManualResetEvent released)
                : base(delegate { }, ClientDefinition()) {
                this.order = order;
                this.name = name;
                this.started = started;
                this.released = released;
            }

            public override void RunOnce() {
                order.Add(name + "-client-try");
                started.Set();
                released.WaitOne(3000);
            }

            protected override void CancelCurrentAttempt() {
                CancelCount++;
                order.Add(name + "-client-cancel");
                released.Set();
            }
        }

        private sealed class SessionTesterServer : TesterServer {
            private readonly List<string> order;
            private readonly string name;
            public int StopCount { get; private set; }

            public SessionTesterServer(List<string> order, string name)
                : base(delegate { }, ServerDefinition()) {
                this.order = order;
                this.name = name;
            }

            protected override void Listen() {
                order.Add(name + "-server-start");
                SignalStartupCompleted();
                while (serverRunningFlag) {
                    Thread.Sleep(10);
                }
            }

            protected override void StopListening() {
                StopCount++;
                order.Add(name + "-server-stop");
            }
        }

        private sealed class DelayedTesterServer : TesterServer {
            private readonly ManualResetEvent listenEntered;
            private readonly ManualResetEvent allowReady;
            private readonly ManualResetEvent stopRequested;

            public DelayedTesterServer(ManualResetEvent listenEntered, ManualResetEvent allowReady,
                ManualResetEvent stopRequested)
                : base(delegate { }, ServerDefinition()) {
                this.listenEntered = listenEntered;
                this.allowReady = allowReady;
                this.stopRequested = stopRequested;
            }

            protected override void Listen() {
                listenEntered.Set();
                allowReady.WaitOne(3000);
                SignalStartupCompleted();
                stopRequested.WaitOne(3000);
            }

            protected override void StopListening() {
                allowReady.Set();
                stopRequested.Set();
            }
        }
    }
}
