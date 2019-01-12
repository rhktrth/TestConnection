using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestConnection {
    class TcpTesterServer : TesterServer {
        private TcpListener listener;
        private TcpClient client;

        public TcpTesterServer(MainForm fo, string lip, int po)
            : base(fo, lip, po) {
            protocolName = ProtocolName.TCP;
        }

        public override void Listen() {
            if (localIpAddress == string.Empty) {
                localIPEndPoint = new IPEndPoint(0, portNo);
            } else {
                localIPEndPoint = new IPEndPoint(IPAddress.Parse(localIpAddress), portNo);
            }
            try {
                listener = new TcpListener(localIPEndPoint);
                listener.Server.ReceiveTimeout = timeout;
                listener.Server.SendTimeout = timeout;
                listener.Start(maxConnection);
            } catch (Exception ex) {
                outputMessage(ex.Message);
                return;
            }
            outputMessage("listen-start");

            serverRunningFlag = true;
            while (true) {
                try {
                    try {
                        client = listener.AcceptTcpClient();
                        client.ReceiveTimeout = timeout;
                        client.SendTimeout = timeout;
                    } catch (SocketException se) {
                        // WSABlockは無視
                        if (se.ErrorCode != 10004) {
                            System.Windows.Forms.MessageBox.Show(se.ErrorCode.ToString());
                            throw;
                        }
                    }
                    if (serverRunningFlag == false) {
                        break;
                    }
                    if (client.Connected) {
                        StringBuilder sb = new StringBuilder();
                        IPEndPoint rep = (IPEndPoint)client.Client.RemoteEndPoint;
                        NetworkStream networkStream = client.GetStream();
                        networkStream.Close();
                        client.Close();
                        client = null;
                        success();

                        sb.Append("connected-from ");
                        sb.Append(rep.Address.ToString());
                        sb.Append(":");
                        sb.Append(rep.Port.ToString());
                        sb.Append("/");
                        sb.Append(protocolName.ToString());
                        outputMessage(sb.ToString());
                    }
                } catch (Exception ex) {
                    failure();
                    outputMessage(ex.Message);
                    return;
                }
            }
        }

        public override void Stop() {
            // 現在テスト実行中であれば、実行フラグを下ろす。
            if (serverRunningFlag == true) {
                serverRunningFlag = false;
                listener.Stop();
                listenThread.Join();
                outputMessage("listen-stop");
            }
        }
    }
}
