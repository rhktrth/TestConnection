using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestConnection {
    class DnsTesterClient : TesterClient {
        private UdpClient client;

        public DnsTesterClient(MainForm fo, string lip, string rip, int po)
            : base(fo, lip, rip, po) {
            protocolName = ProtocolName.DNS;
        }

        public override void Try() {
            string remsg = null;
            StringBuilder sb = new StringBuilder();
            IPEndPoint rep = new IPEndPoint(IPAddress.Parse(remoteIpAddress), portNo);

            try {
                if (localIpAddress == string.Empty) {
                    localIPEndPoint = new IPEndPoint(0, 0);
                } else {
                    localIPEndPoint = new IPEndPoint(IPAddress.Parse(localIpAddress), 0);
                }
                client = new UdpClient(localIPEndPoint);
                client.Connect(rep);
                byte[] msg = { 0x00, 0x02, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00,
                                 0x00, 0x00, 0x01, 0x41, 0x0c, 0x52, 0x4f, 0x4f, 0x54, 0x2d,
                                 0x53, 0x45, 0x52, 0x56, 0x45, 0x52, 0x53, 0x03, 0x4e, 0x45,
                                 0x54, 0x00, 0x00, 0x01, 0x00, 0x01 };

                client.Send(msg, msg.Length);
                localIPEndPoint = (IPEndPoint)client.Client.LocalEndPoint;

                client.Client.ReceiveTimeout = timeout;
                try {
                    client.Receive(ref rep);
                } catch (SocketException se) {
                    // WSABlock‚Í–³Ž‹
                    if (se.ErrorCode != 10004) {
                        throw;
                    }
                }
                success();

                sb.Append("dns-responsed-from ");
                sb.Append(rep.Address.ToString());
                sb.Append(":");
                sb.Append(rep.Port.ToString());
                sb.Append("/");
                sb.Append(protocolName.ToString());
                remsg = sb.ToString();
            } catch (Exception ex) {
                failure();
                remsg = ex.Message;
            } finally {
                outputMessage(remsg);
            }
        }

        public override void Cancel() {
            if (client != null) {
                client.Close();
            }
        }
    }
}
