using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestConnection {
    class TcpTesterServer : TesterServer {
        private TcpListener listener;

        public TcpTesterServer(Action<string> resultOutput, TesterDefinition definition)
            : base(resultOutput, definition) {
        }

        protected override void Listen() {
            TcpListener currentListener = null;
            try {
                localEndPoint = CreateLocalEndPoint(Port);
                currentListener = new TcpListener(localEndPoint);
                currentListener.Server.ReceiveTimeout = TimeoutMilliseconds;
                currentListener.Server.SendTimeout = TimeoutMilliseconds;
                currentListener.Start(maxConnection);
                listener = currentListener;
                SignalStartupCompleted();

                WriteResult("listen-start");
                while (serverRunningFlag) {
                    TcpClient acceptedClient = null;
                    try {
                        try {
                            acceptedClient = currentListener.AcceptTcpClient();
                            acceptedClient.ReceiveTimeout = TimeoutMilliseconds;
                            acceptedClient.SendTimeout = TimeoutMilliseconds;
                        } catch (SocketException se) {
                            if (se.SocketErrorCode == SocketError.Interrupted && !serverRunningFlag) {
                                break;
                            }
                            throw;
                        }

                        if (!serverRunningFlag) {
                            break;
                        }
                        if (acceptedClient.Connected) {
                            StringBuilder messageBuilder = new StringBuilder();
                            IPEndPoint remoteEndPoint = (IPEndPoint)acceptedClient.Client.RemoteEndPoint;
                            RecordSuccess();

                            messageBuilder.Append("connected-from ");
                            messageBuilder.Append(remoteEndPoint.Address.ToString());
                            messageBuilder.Append(":");
                            messageBuilder.Append(remoteEndPoint.Port.ToString());
                            WriteResult(messageBuilder.ToString());
                        }
                    } catch (Exception ex) {
                        if (!serverRunningFlag && (ex is ObjectDisposedException || ex is SocketException)) {
                            break;
                        }
                        RecordFailure();
                        WriteResult(ex.Message);
                        return;
                    } finally {
                        if (acceptedClient != null) {
                            acceptedClient.Close();
                        }
                    }
                }
            } finally {
                if (currentListener != null) {
                    currentListener.Stop();
                }
                if (ReferenceEquals(listener, currentListener)) {
                    listener = null;
                }
            }
        }

        protected override void StopListening() {
            TcpListener currentListener = listener;
            if (currentListener != null) {
                currentListener.Stop();
            }
        }
    }
}
