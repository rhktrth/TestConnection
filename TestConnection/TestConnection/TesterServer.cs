using System.Threading;

namespace TestConnection {
    abstract class TesterServer : TesterBase {
        const int DEFAULTMAXCONNECTION = 10;

        protected int maxConnection = DEFAULTMAXCONNECTION;
        protected bool serverRunningFlag = false;
        protected Thread listenThread = null;

        public TesterServer(MainForm fo, string lip, int po)
            : base(fo) {
            csType = CSType.Server;
            localIpAddress = lip;
            remoteIpAddress = null;
            portNo = po;
        }

        public void Start() {
            // 現在テスト実行中で無ければ、テストを開始。
            if (serverRunningFlag == false) {
                listenThread = new Thread(new ThreadStart(this.Listen));
                listenThread.IsBackground = true;
                listenThread.Start();
                while (!listenThread.IsAlive) ;
            }
        }

        public abstract void Listen();

        public abstract void Stop();
    }
}
