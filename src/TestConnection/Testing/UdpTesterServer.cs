using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestConnection {
    class UdpTesterServer : TesterServer {
        private UdpClient client;

        public UdpTesterServer(Action<string> resultOutput, TesterDefinition definition)
            : base(resultOutput, definition) {
        }

        protected override void Listen() {
            UdpClient currentClient = null;
            try {
                localEndPoint = CreateLocalEndPoint(Port);
                currentClient = new UdpClient(localEndPoint);
                client = currentClient;
                SignalStartupCompleted();

                WriteResult("listen-start");
                while (serverRunningFlag) {
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    try {
                        StringBuilder messageBuilder = new StringBuilder();
                        try {
                            currentClient.Receive(ref remoteEndPoint);
                        } catch (SocketException se) {
                            if (se.SocketErrorCode == SocketError.Interrupted && !serverRunningFlag) {
                                break;
                            }
                            throw;
                        }

                        if (!serverRunningFlag) {
                            break;
                        }
                        RecordSuccess();

                        messageBuilder.Append("received-from ");
                        messageBuilder.Append(remoteEndPoint.Address.ToString());
                        messageBuilder.Append(":");
                        messageBuilder.Append(remoteEndPoint.Port.ToString());
                        WriteResult(messageBuilder.ToString());
                    } catch (Exception ex) {
                        if (!serverRunningFlag && (ex is ObjectDisposedException || ex is SocketException)) {
                            break;
                        }
                        RecordFailure();
                        WriteResult(ex.Message);
                    }
                }
            } finally {
                if (currentClient != null) {
                    currentClient.Close();
                }
                if (ReferenceEquals(client, currentClient)) {
                    client = null;
                }
            }
        }

        protected override void StopListening() {
            UdpClient currentClient = client;
            if (currentClient != null) {
                currentClient.Close();
            }
        }
    }
}
