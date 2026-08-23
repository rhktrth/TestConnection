using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TestConnection {
    class TcpTesterClient : TesterClient {
        private readonly object clientLock = new object();
        private TcpClient client;

        public TcpTesterClient(Action<string> resultOutput, TesterDefinition definition)
            : base(resultOutput, definition) {
        }

        public override void RunOnce() {
            string resultMessage = null;
            IPEndPoint remoteEndPoint = null;
            StringBuilder messageBuilder = new StringBuilder();
            WaitHandle waitHandle = null;
            TcpClient currentClient = null;
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

                currentClient = new TcpClient(localEndPoint);
                lock (clientLock) {
                    if (CancellationRequested) {
                        return;
                    }
                    client = currentClient;
                }

                IAsyncResult connectAsyncResult = currentClient.BeginConnect(remoteAddress, Port, null, null);
                waitHandle = connectAsyncResult.AsyncWaitHandle;

                if (!waitHandle.WaitOne(TimeoutMilliseconds, true)) {
                    messageBuilder.Append("timeout-to ");
                    messageBuilder.Append(remoteTarget);
                    messageBuilder.Append(":");
                    messageBuilder.Append(remoteEndPoint.Port.ToString());
                    resultMessage = messageBuilder.ToString();
                    throw new TimeoutException(resultMessage);
                }

                currentClient.EndConnect(connectAsyncResult);
                localEndPoint = (IPEndPoint)currentClient.Client.LocalEndPoint;

                RecordSuccess();
                messageBuilder.Append("connected-to ");
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
                } else if (resultMessage == null) {
                    resultMessage = "failed-to " +
                        RemoteEndpointResolver.FormatResolvedTarget(RemoteIpAddress, remoteEndPoint.Address) + ":" +
                        remoteEndPoint.Port.ToString() + " " + ex.Message;
                } else {
                    resultMessage = ex.Message;
                }
                outputResult = true;
            } finally {
                if (waitHandle != null) {
                    waitHandle.Close();
                }
                ReleaseClient(currentClient);
                if (outputResult) {
                    WriteResult(resultMessage);
                }
            }
        }

        protected override void CancelCurrentAttempt() {
            TcpClient current;
            lock (clientLock) {
                current = client;
                client = null;
            }
            CloseClient(current);
        }

        private void ReleaseClient(TcpClient current) {
            lock (clientLock) {
                if (ReferenceEquals(client, current)) {
                    client = null;
                }
            }
            CloseClient(current);
        }

        private static void CloseClient(TcpClient current) {
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
