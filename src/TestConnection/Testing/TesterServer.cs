using System;
using System.Threading;

namespace TestConnection {
    abstract class TesterServer : TesterBase {
        const int DEFAULTMAXCONNECTION = 10;

        protected int maxConnection = DEFAULTMAXCONNECTION;
        protected volatile bool serverRunningFlag;
        protected Thread listenThread;

        private readonly object stateLock = new object();
        private ManualResetEvent startupEvent;

        protected TesterServer(Action<string> resultOutput, TesterDefinition definition)
            : base(resultOutput, definition) {
        }

        public void Start() {
            Thread newThread;
            ManualResetEvent currentStartupEvent;

            lock (stateLock) {
                if (serverRunningFlag) {
                    return;
                }

                serverRunningFlag = true;
                currentStartupEvent = new ManualResetEvent(false);
                startupEvent = currentStartupEvent;
                newThread = new Thread(ListenWorker);
                newThread.IsBackground = true;
                listenThread = newThread;
            }

            newThread.Start();
            currentStartupEvent.WaitOne();

            lock (stateLock) {
                if (ReferenceEquals(startupEvent, currentStartupEvent)) {
                    startupEvent = null;
                }
            }
            currentStartupEvent.Close();
        }

        public void Stop() {
            if (!serverRunningFlag) {
                return;
            }

            serverRunningFlag = false;
            try {
                StopListening();
            } finally {
                JoinListenThread();
            }
            WriteResult("listen-stop");
        }

        private void ListenWorker() {
            try {
                Listen();
            } catch (Exception ex) {
                WriteResult(ex.Message);
            } finally {
                lock (stateLock) {
                    serverRunningFlag = false;
                    if (startupEvent != null) {
                        startupEvent.Set();
                    }
                    if (ReferenceEquals(listenThread, Thread.CurrentThread)) {
                        listenThread = null;
                    }
                }
            }
        }

        protected void SignalStartupCompleted() {
            lock (stateLock) {
                if (startupEvent != null) {
                    startupEvent.Set();
                }
            }
        }

        protected void JoinListenThread() {
            Thread currentThread = listenThread;
            if (currentThread != null && !ReferenceEquals(currentThread, Thread.CurrentThread)) {
                currentThread.Join();
            }
        }

        protected abstract void Listen();

        protected abstract void StopListening();
    }
}
