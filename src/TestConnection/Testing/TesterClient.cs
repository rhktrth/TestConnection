using System;

namespace TestConnection {
    abstract class TesterClient : TesterBase {
        private volatile bool cancellationRequested;

        protected TesterClient(Action<string> resultOutput, TesterDefinition definition)
            : base(resultOutput, definition) {
        }

        protected bool CancellationRequested {
            get { return cancellationRequested; }
        }

        internal void ResetCancellation() {
            cancellationRequested = false;
        }

        public abstract void RunOnce();

        public void Cancel() {
            cancellationRequested = true;
            CancelCurrentAttempt();
        }

        protected abstract void CancelCurrentAttempt();
    }
}
