using System;
using System.Net;
using System.Text;

namespace TestConnection {
    enum TesterRole { Server, Client };
    enum ProtocolName { TCP, UDP, DNS, Ping };

    abstract class TesterBase {
        public TesterDefinition Definition { get; private set; }
        public string RemoteIpAddress { get { return Definition.RemoteIpAddress; } }
        public string LocalIpAddress { get { return Definition.LocalIpAddress; } }
        public int Port { get { return Definition.Port; } }
        public int SuccessCount { get; protected set; }
        public int FailureCount { get; protected set; }
        public int TimeoutMilliseconds { get; set; }
        public event Action Succeeded;
        public event Action Failed;
        public TesterRole Role { get { return Definition.Role; } }
        public ProtocolName Protocol { get { return Definition.Protocol; } }
        protected IPEndPoint localEndPoint;

        private readonly Action<string> resultOutput;

        protected TesterBase(Action<string> resultOutput, TesterDefinition definition) {
            if (resultOutput == null) {
                throw new ArgumentNullException("resultOutput");
            }
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }

            this.resultOutput = resultOutput;
            Definition = definition;
            ClearCount();
        }

        public void ClearCount() {
            SuccessCount = 0;
            FailureCount = 0;
        }

        protected IPEndPoint CreateLocalEndPoint(int port) {
            IPAddress address = LocalIpAddress == string.Empty
                ? IPAddress.Any
                : IPAddress.Parse(LocalIpAddress);
            return new IPEndPoint(address, port);
        }

        protected void WriteResult(string message) {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss "));
            messageBuilder.Append(Role.ToString());
            messageBuilder.Append(" ");
            if (localEndPoint != null) {
                messageBuilder.Append(localEndPoint.Address.ToString());
                messageBuilder.Append(":");
                messageBuilder.Append(localEndPoint.Port.ToString());
                messageBuilder.Append("/");
            }
            messageBuilder.Append(Protocol.ToString());
            if (!string.IsNullOrEmpty(message)) {
                messageBuilder.Append(" ");
                messageBuilder.Append(message);
            }
            resultOutput(messageBuilder.ToString());
        }

        protected void RecordSuccess() {
            SuccessCount++;
            Action handler = Succeeded;
            if (handler != null) {
                handler();
            }
        }

        protected void RecordFailure() {
            FailureCount++;
            Action handler = Failed;
            if (handler != null) {
                handler();
            }
        }
    }
}
