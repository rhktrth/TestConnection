using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestConnection {
    class TcpTesterClient : TesterClient {
        private TcpClient client;

        public TcpTesterClient(MainForm fo, string lip, string rip, int po)
            : base(fo, lip, rip, po) {
            protocolName = ProtocolName.TCP;
        }

        public override void Try() {
            string remsg = null;
            IPEndPoint rep = new IPEndPoint(IPAddress.Parse(remoteIpAddress), portNo);
            StringBuilder sb = new StringBuilder();
            IAsyncResult result;

            try {
                if (localIpAddress == string.Empty) {
                    localIPEndPoint = new IPEndPoint(0, 0);
                } else {
                    localIPEndPoint = new IPEndPoint(IPAddress.Parse(localIpAddress), 0);
                }

                client = new TcpClient(localIPEndPoint);

                result = client.BeginConnect(IPAddress.Parse(remoteIpAddress), portNo, null, null);

                if (result.AsyncWaitHandle.WaitOne(timeout, true) == false) {
                    client.Close();
                    client = null;
                    sb.Append("timeout-to ");
                    sb.Append(rep.Address.ToString());
                    sb.Append(":");
                    sb.Append(rep.Port.ToString());
                    sb.Append("/");
                    sb.Append(protocolName.ToString());
                    remsg = sb.ToString();
                    throw new Exception(remsg);
                }

                client.EndConnect(result);

                localIPEndPoint = (IPEndPoint)client.Client.LocalEndPoint;
                NetworkStream networkStream = client.GetStream();
                networkStream.Close();
                client.Close();
                client = null;

                success();
                sb.Append("connected-to ");
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
                client = null;
            }
        }
    }
}
