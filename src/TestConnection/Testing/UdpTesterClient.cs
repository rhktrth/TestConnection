using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestConnection {
    class UdpTesterClient : TesterClient {
        const string DATASTRING = "Hello";

        private readonly object clientLock = new object();
        private UdpClient client;

        public UdpTesterClient(Action<string> resultOutput, TesterDefinition definition)
            : base(resultOutput, definition) {
        }

        public override void RunOnce() {
            string resultMessage = null;
            StringBuilder messageBuilder = new StringBuilder();
            IPEndPoint remoteEndPoint = null;
            UdpClient currentClient = null;
            bool outputResult = false;

            if (CancellationRequested) {
                return;
            }

            localEndPoint = null;
            try {
                IPAddress remoteAddress = RemoteEndpointResolver.ResolveIpv4(RemoteIpAddress);
                if (CancellationRequested) {
                    return;
                }

                remoteEndPoint = new IPEndPoint(remoteAddress, Port);
                string remoteTarget = RemoteEndpointResolver.FormatResolvedTarget(RemoteIpAddress, remoteAddress);

                localEndPoint = CreateLocalEndPoint(0);

                currentClient = new UdpClient(localEndPoint);
                lock (clientLock) {
                    if (CancellationRequested) {
                        return;
                    }
                    client = currentClient;
                }

                currentClient.Connect(remoteEndPoint);
                byte[] message = Encoding.ASCII.GetBytes(DATASTRING);
                currentClient.Send(message, message.Length);
                localEndPoint = (IPEndPoint)currentClient.Client.LocalEndPoint;
                RecordSuccess();

                messageBuilder.Append("sent-to ");
                messageBuilder.Append(remoteTarget);
                messageBuilder.Append(":");
                messageBuilder.Append(remoteEndPoint.Port.ToString());
                resultMessage = messageBuilder.ToString();
                outputResult = true;
            } catch (Exception ex) {
                if (CancellationRequested || ex is ObjectDisposedException) {
                    return;
                }

                SocketException socketException = ex as SocketException;
                if (socketException != null && socketException.SocketErrorCode == SocketError.Interrupted) {
                    return;
                }

                RecordFailure();
                if (remoteEndPoint == null) {
                    resultMessage = "resolve-failed-to " + RemoteIpAddress + " " + ex.Message;
                } else {
                    resultMessage = "failed-to " +
                        RemoteEndpointResolver.FormatResolvedTarget(RemoteIpAddress, remoteEndPoint.Address) + ":" +
                        remoteEndPoint.Port.ToString() + " " + ex.Message;
                }
                outputResult = true;
            } finally {
                ReleaseClient(currentClient);
                if (outputResult) {
                    WriteResult(resultMessage);
                }
            }
        }

        protected override void CancelCurrentAttempt() {
            UdpClient current;
            lock (clientLock) {
                current = client;
                client = null;
            }
            CloseClient(current);
        }

        private void ReleaseClient(UdpClient current) {
            lock (clientLock) {
                if (ReferenceEquals(client, current)) {
                    client = null;
                }
            }
            CloseClient(current);
        }

        private static void CloseClient(UdpClient current) {
            if (current == null) {
                return;
            }
            try {
                current.Close();
            } catch (ObjectDisposedException) {
            }
        }
    }
}
