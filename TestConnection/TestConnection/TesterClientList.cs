using System.Collections.Generic;
using System.Threading;

namespace TestConnection {
    class TesterClientList : List<TesterClient> {
        const int DEFAULTINTERVAL = 1000;

        public int itemInterval { set; get; }
        public int listInterval { set; get; }
        private bool clientRunningFlag;
        private Thread testThread;
        private int repeatCount;
        private TesterClient nowtester;

        public TesterClientList()
            : base() {
            itemInterval = DEFAULTINTERVAL;
            listInterval = DEFAULTINTERVAL;
            clientRunningFlag = false;
            repeatCount = 0;
        }

        public void Start() {
            Start(0);
        }

        public void Start(int ct) {
            if (clientRunningFlag == false) {
                repeatCount = ct;
                clientRunningFlag = true;
                nowtester = null;
                //テスト用スレッドを作成
                testThread = new Thread(new ThreadStart(this.TestLoop));
                testThread.IsBackground = true;
                //テスト用スレッドを起動する
                testThread.Start();
                //テスト用スレッドが立ち上がるのを待つ
                while (!testThread.IsAlive) {
                    Thread.Sleep(100);
                }
            }
        }

        private void TestLoop() {
            // repeatCountが0のときは無限に繰り返す
            for (int i = 0; clientRunningFlag && (i < repeatCount || repeatCount == 0); i++) {
                foreach (TesterClient tc in this) {
                    if (clientRunningFlag == false) {
                        break;
                    }
                    nowtester = tc;
                    nowtester.Try();
                    Thread.Sleep(itemInterval);
                }
                if (clientRunningFlag == false) {
                    break;
                }
                Thread.Sleep(listInterval);
            }
        }

        public void Stop() {
            // テスト実行フラグを下ろす。
            if (clientRunningFlag == true) {
                clientRunningFlag = false;
                if (nowtester != null) {
                    nowtester.Cancel();
                }
                testThread.Abort();
                // testThread.Join();  //スレッドが終わるまで待つとTCPが終わらない可能性
            }
        }

        public void AllClear() {
            foreach (TesterClient tc in this) {
                tc.ClearCount();
            }
        }
    }
}
