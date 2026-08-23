using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestConnection {
    class DnsTesterClient : TesterClient {
        private readonly object clientLock = new object();
        private UdpClient client;

        public DnsTesterClient(Action<string> resultOutput, TesterDefinition definition)
            : base(resultOutput, definition) {
        }

        public override void RunOnce() {
            string resultMessage = null;
            StringBuilder messageBuilder = new StringBuilder();
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(RemoteIpAddress), Port);
            UdpClient currentClient = null;
            bool outputResult = false;

            if (CancellationRequested) {
                return;
            }

            localEndPoint = null;
            try {
                localEndPoint = CreateLocalEndPoint(0);

                currentClient = new UdpClient(localEndPoint);
                lock (clientLock) {
                    if (CancellationRequested) {
                        return;
                    }
                    client = currentClient;
                }

                currentClient.Connect(remoteEndPoint);
                byte[] message = { 0x00, 0x02, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00,
                                     0x00, 0x00, 0x01, 0x41, 0x0c, 0x52, 0x4f, 0x4f, 0x54, 0x2d,
                                     0x53, 0x45, 0x52, 0x56, 0x45, 0x52, 0x53, 0x03, 0x4e, 0x45,
                                     0x54, 0x00, 0x00, 0x01, 0x00, 0x01 };

                currentClient.Send(message, message.Length);
                localEndPoint = (IPEndPoint)currentClient.Client.LocalEndPoint;

                currentClient.Client.ReceiveTimeout = TimeoutMilliseconds;
                currentClient.Receive(ref remoteEndPoint);
                if (CancellationRequested) {
                    return;
                }

                RecordSuccess();
                messageBuilder.Append("dns-response-from ");
                messageBuilder.Append(remoteEndPoint.Address.ToString());
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
                resultMessage = ex.Message;
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
