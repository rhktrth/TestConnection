using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestConnection {
    class UdpTesterServer : TesterServer {
        private UdpClient client;

        public UdpTesterServer(MainForm fo, string lip, int po)
            : base(fo, lip, po) {
            protocolName = ProtocolName.UDP;
        }

        public override void Listen() {
            if (localIpAddress == string.Empty) {
                localIPEndPoint = new IPEndPoint(0, portNo);
            } else {
                localIPEndPoint = new IPEndPoint(IPAddress.Parse(localIpAddress), portNo);
            }
            try {
                client = new UdpClient(localIPEndPoint);
            } catch (Exception ex) {
                outputMessage(ex.Message);
                return;
            }
            outputMessage("listen-start");

            serverRunningFlag = true;
            while (true) {
                IPEndPoint rep = new IPEndPoint(IPAddress.Any, 0);
                try {
                    StringBuilder sb = new StringBuilder();
                    try {
                        client.Receive(ref rep);
                    } catch (SocketException se) {
                        // WSABlockは無視
                        if (se.ErrorCode != 10004) {
                            throw;
                        }
                    }
                    if (serverRunningFlag == false) {
                        break;
                    }
                    success();

                    sb.Append("received-from ");
                    sb.Append(rep.Address.ToString());
                    sb.Append(":");
                    sb.Append(rep.Port.ToString());
                    sb.Append("/");
                    sb.Append(protocolName.ToString());
                    outputMessage(sb.ToString());
                } catch (Exception ex) {
                    failure();
                    outputMessage(ex.Message);
                }
            }
        }

        public override void Stop() {
            // 現在テスト実行中であれば、実行フラグを下ろす。
            if (serverRunningFlag == true) {
                serverRunningFlag = false;
                if (client != null) {
                    client.Close();
                }
                listenThread.Join();
                outputMessage("listen-stop");
            }
        }
    }
}
