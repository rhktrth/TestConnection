using System.Collections.Generic;
using System.Threading;

namespace TestConnection {
    internal sealed class ClientTestRunner {
        private readonly List<TesterClient> testers;
        private readonly int itemInterval;
        private readonly int listInterval;
        private readonly object stateLock = new object();
        private bool running;
        private Thread testThread;
        private ManualResetEvent stopEvent;
        private TesterClient currentTester;

        public ClientTestRunner(IEnumerable<TesterClient> testers, int itemInterval, int listInterval) {
            this.testers = testers == null ? new List<TesterClient>() : new List<TesterClient>(testers);
            this.itemInterval = itemInterval;
            this.listInterval = listInterval;
        }

        public void Start(int repeatCount) {
            Thread newThread;
            ManualResetEvent currentStopEvent;

            lock (stateLock) {
                if (running) {
                    return;
                }

                foreach (TesterClient tester in testers) {
                    tester.ResetCancellation();
                }

                running = true;
                currentTester = null;
                currentStopEvent = new ManualResetEvent(false);
                stopEvent = currentStopEvent;
                newThread = new Thread(() => TestLoop(currentStopEvent, repeatCount));
                newThread.IsBackground = true;
                testThread = newThread;
            }

            newThread.Start();
        }

        private void TestLoop(ManualResetEvent currentStopEvent, int repeatCount) {
            try {
                // repeatCountが0のときは無限に繰り返す。
                for (int i = 0; repeatCount == 0 || i < repeatCount; i++) {
                    foreach (TesterClient tester in testers) {
                        if (currentStopEvent.WaitOne(0)) {
                            return;
                        }

                        lock (stateLock) {
                            if (!running || currentStopEvent.WaitOne(0)) {
                                return;
                            }
                            currentTester = tester;
                        }

                        try {
                            tester.RunOnce();
                        } finally {
                            lock (stateLock) {
                                if (ReferenceEquals(currentTester, tester)) {
                                    currentTester = null;
                                }
                            }
                        }

                        if (currentStopEvent.WaitOne(itemInterval)) {
                            return;
                        }
                    }

                    if (currentStopEvent.WaitOne(listInterval)) {
                        return;
                    }
                }
            } finally {
                lock (stateLock) {
                    if (ReferenceEquals(stopEvent, currentStopEvent)) {
                        running = false;
                        stopEvent = null;
                        testThread = null;
                        currentTester = null;
                    }
                }
                currentStopEvent.Close();
            }
        }

        public void Stop() {
            TesterClient tester;
            Thread currentThread;

            lock (stateLock) {
                if (!running) {
                    return;
                }

                running = false;
                tester = currentTester;
                currentThread = testThread;
                if (stopEvent != null) {
                    stopEvent.Set();
                }
            }

            if (tester != null) {
                tester.Cancel();
            }
            if (currentThread != null && !ReferenceEquals(currentThread, Thread.CurrentThread)) {
                currentThread.Join();
            }
        }
    }
}
