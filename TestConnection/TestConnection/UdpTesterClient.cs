using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestConnection {
    class UdpTesterClient : TesterClient {
        const string DATASTRING = "Hello";
        private UdpClient client;

        public UdpTesterClient(MainForm fo, string lip, string rip, int po)
            : base(fo, lip, rip, po) {
            protocolName = ProtocolName.UDP;
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
                byte[] msg = Encoding.ASCII.GetBytes(DATASTRING);

                client.Send(msg, msg.Length);
                localIPEndPoint = (IPEndPoint)client.Client.LocalEndPoint;
                success();

                sb.Append("sent-to ");
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
            // ‰½‚à‚µ‚È‚¢
        }
    }
}
