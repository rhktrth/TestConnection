using System;
using System.Collections.Generic;

namespace TestConnection {
    internal sealed class TestSession {
        private readonly Action<string> resultOutput;
        private readonly Action successHandler;
        private readonly Action failureHandler;
        private readonly Action completedHandler;
        private readonly object stateLock = new object();
        private List<TesterBase> activeTesters = new List<TesterBase>();
        private List<TesterServer> activeServers = new List<TesterServer>();
        private ClientTestRunner clientRunner;
        private bool running;

        public TestSession(Action<string> resultOutput, Action successHandler, Action failureHandler,
            Action completedHandler = null) {
            if (resultOutput == null) {
                throw new ArgumentNullException("resultOutput");
            }

            this.resultOutput = resultOutput;
            this.successHandler = successHandler;
            this.failureHandler = failureHandler;
            this.completedHandler = completedHandler;
        }

        public bool IsRunning {
            get {
                lock (stateLock) {
                    return running;
                }
            }
        }

        public bool TryCreateTester(TesterDefinition definition, int timeoutMilliseconds,
            out TesterBase tester, out string validationError) {
            validationError = TesterDefinitionValidator.Validate(definition);
            if (validationError != null) {
                tester = null;
                return false;
            }

            tester = TesterFactory.Create(definition, resultOutput, timeoutMilliseconds);
            if (successHandler != null) {
                tester.Succeeded += successHandler;
            }
            if (failureHandler != null) {
                tester.Failed += failureHandler;
            }
            return true;
        }

        public void Start(IEnumerable<TesterBase> testers,
            int itemInterval, int listInterval, int repeatCount) {
            ClientTestRunner newClientRunner;
            List<TesterServer> newServers;

            lock (stateLock) {
                if (running) {
                    return;
                }

                activeTesters = testers == null
                    ? new List<TesterBase>()
                    : new List<TesterBase>(testers);
                newServers = new List<TesterServer>();
                List<TesterClient> clients = new List<TesterClient>();

                foreach (TesterBase tester in activeTesters) {
                    TesterServer server = tester as TesterServer;
                    if (server != null) {
                        newServers.Add(server);
                        continue;
                    }

                    TesterClient client = tester as TesterClient;
                    if (client != null) {
                        clients.Add(client);
                        continue;
                    }

                    throw new InvalidOperationException("tester typeが無効です。");
                }

                newClientRunner = new ClientTestRunner(
                    clients, itemInterval, listInterval, OnClientRunnerCompleted);
                activeServers = newServers;
                clientRunner = newClientRunner;
                running = true;
            }

            try {
                foreach (TesterServer server in newServers) {
                    server.Start();
                }
                newClientRunner.Start(repeatCount);
            } catch {
                Stop();
                throw;
            }
        }

        public void Stop() {
            ClientTestRunner currentClients;
            List<TesterServer> currentServers;

            lock (stateLock) {
                if (!running) {
                    return;
                }

                running = false;
                currentClients = clientRunner;
                currentServers = new List<TesterServer>(activeServers);
            }

            if (currentClients != null) {
                currentClients.Stop();
            }
            StopServers(currentServers);
        }

        private void OnClientRunnerCompleted() {
            List<TesterServer> currentServers;

            lock (stateLock) {
                if (!running) {
                    return;
                }

                running = false;
                currentServers = new List<TesterServer>(activeServers);
            }

            StopServers(currentServers);
            if (completedHandler != null) {
                completedHandler();
            }
        }

        private static void StopServers(IEnumerable<TesterServer> servers) {
            foreach (TesterServer server in servers) {
                server.Stop();
            }
        }

        public void ClearStatistics() {
            List<TesterBase> currentTesters;
            lock (stateLock) {
                currentTesters = new List<TesterBase>(activeTesters);
            }

            foreach (TesterBase tester in currentTesters) {
                tester.ClearCount();
            }
        }
    }
}
