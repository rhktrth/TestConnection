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
                //�e�X�g�p�X���b�h���쐬
                testThread = new Thread(new ThreadStart(this.TestLoop));
                testThread.IsBackground = true;
                //�e�X�g�p�X���b�h���N������
                testThread.Start();
                //�e�X�g�p�X���b�h�������オ��̂�҂�
                while (!testThread.IsAlive) {
                    Thread.Sleep(100);
                }
            }
        }

        private void TestLoop() {
            // repeatCount��0�̂Ƃ��͖����ɌJ��Ԃ�
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
            // �e�X�g���s�t���O�����낷�B
            if (clientRunningFlag == true) {
                clientRunningFlag = false;
                if (nowtester != null) {
                    nowtester.Cancel();
                }
                testThread.Abort();
                // testThread.Join();  //�X���b�h���I���܂ő҂�TCP���I���Ȃ��\��
            }
        }

        public void AllClear() {
            foreach (TesterClient tc in this) {
                tc.ClearCount();
            }
        }
    }
}
