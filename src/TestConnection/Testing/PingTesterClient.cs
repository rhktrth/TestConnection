using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestConnection {
    class PingTesterClient : TesterClient {
        private const int DefaultTimeout = 1000;
        private const int PayloadLength = 32;
        private const ushort Identifier = 45;
        private const ushort SequenceNumber = 0;

        private readonly object socketLock = new object();
        private Socket socket;

        public PingTesterClient(Action<string> resultOutput, TesterDefinition definition)
            : base(resultOutput, definition) {
            TimeoutMilliseconds = DefaultTimeout;
        }

        public override void RunOnce() {
            Socket currentSocket = null;
            StringBuilder messageBuilder = new StringBuilder();
            IPAddress remoteAddress = null;

            if (CancellationRequested) {
                return;
            }

            localEndPoint = null;
            try {
                remoteAddress = RemoteEndpointResolver.ResolveIpv4(RemoteIpAddress);
                EndPoint remoteEndPoint = new IPEndPoint(remoteAddress, 0);

                localEndPoint = CreateLocalEndPoint(0);

                currentSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);

                lock (socketLock) {
                    if (CancellationRequested) {
                        return;
                    }
                    socket = currentSocket;
                }

                currentSocket.Bind(localEndPoint);
                byte[] request = IcmpEchoPacket.CreateRequest(Identifier, SequenceNumber, PayloadLength);
                currentSocket.SendTimeout = TimeoutMilliseconds;

                Stopwatch stopwatch = Stopwatch.StartNew();
                currentSocket.SendTo(request, request.Length, SocketFlags.None, remoteEndPoint);
                localEndPoint = (IPEndPoint)currentSocket.LocalEndPoint;

                byte[] receiveBuffer = new byte[1024];
                while (!CancellationRequested) {
                    int remaining = TimeoutMilliseconds - (int)stopwatch.ElapsedMilliseconds;
                    if (remaining <= 0) {
                        throw new SocketException(10060);
                    }

                    currentSocket.ReceiveTimeout = remaining;
                    EndPoint source = new IPEndPoint(IPAddress.Any, 0);
                    int received = currentSocket.ReceiveFrom(receiveBuffer, ref source);
                    IPEndPoint sourceEndPoint = source as IPEndPoint;
                    if (sourceEndPoint == null || !sourceEndPoint.Address.Equals(remoteAddress)) {
                        continue;
                    }
                    if (!IcmpEchoPacket.IsMatchingReply(receiveBuffer, received, Identifier, SequenceNumber)) {
                        continue;
                    }

                    stopwatch.Stop();
                    RecordSuccess();
                    messageBuilder.Append("ping-ok-to ");
                    messageBuilder.Append(RemoteEndpointResolver.FormatResolvedTarget(RemoteIpAddress, remoteAddress));
                    messageBuilder.Append(" ");
                    messageBuilder.Append(stopwatch.ElapsedMilliseconds);
                    messageBuilder.Append("ms");
                    WriteResult(messageBuilder.ToString());
                    return;
                }
            } catch (Exception ex) {
                if (CancellationRequested || ex is ObjectDisposedException) {
                    return;
                }

                RecordFailure();
                string target = RemoteEndpointResolver.FormatResolvedTarget(RemoteIpAddress, remoteAddress);
                messageBuilder.Append("ping-ng-to ").Append(target).Append(" ").Append(ex.Message);
                WriteResult(messageBuilder.ToString());
            } finally {
                ReleaseSocket(currentSocket);
            }
        }

        protected override void CancelCurrentAttempt() {
            Socket current;
            lock (socketLock) {
                current = socket;
                socket = null;
            }
            CloseSocket(current);
        }

        private void ReleaseSocket(Socket current) {
            lock (socketLock) {
                if (ReferenceEquals(socket, current)) {
                    socket = null;
                }
            }
            CloseSocket(current);
        }

        private static void CloseSocket(Socket current) {
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
