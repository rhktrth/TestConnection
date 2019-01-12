using System;
using System.Collections.Generic;
using System.Management;
using System.Net;
using System.Windows.Forms;


namespace TestConnection {
    delegate void SamDelegate(string str);
    delegate void successEventHandler();
    delegate void failureEventHandler();

    public partial class MainForm : Form {
        enum ResultLogProcessType { Window, File, None };

        const string CRLF = "\r\n";
        const int CS = 0;
        const int LIPADDR = 1;
        const int RIPADDR = 2;
        const int PROTO = 3;
        const int PORT = 4;
        const string DEFAULTSETTING = "res/default.csv";
        const string SUCCESSSOUNDFILE = "res/success.wav";
        const string FAILURESOUNDFILE = "res/failure.wav";

        private TesterServerList testerServerList = new TesterServerList();
        private TesterClientList testerClientList = new TesterClientList();
        private List<ManagementObject> orgNicList = new List<ManagementObject>();
        private bool ipChangedFlag = false;
        private bool formRunningFlag = false;
        private ResultLogProcessType nowlogProcess = ResultLogProcessType.Window;
        private System.IO.StreamWriter resultLogStreamWriter;
        private bool soundEnable = false;
        private System.Media.SoundPlayer successSoundPlayer = null;
        private System.Media.SoundPlayer failureSoundPlayer = null;

        public MainForm() {
            InitializeComponent();

            //デフォルト設定の読み込み
            if (System.IO.File.Exists(DEFAULTSETTING)) {
                System.IO.StreamReader fsr = new System.IO.StreamReader(DEFAULTSETTING);
                loadSetting(fsr);
                fsr.Close();
            }

            // IPが使えるNICの取得、コンボに表示
            nicComboBox.Items.Clear();
            ManagementObjectSearcher query1 = new ManagementObjectSearcher("Select * from Win32_NetworkAdapterConfiguration where IPEnabled=TRUE");
            ManagementObjectCollection queryCollection1 = query1.Get();
            foreach (ManagementObject mo in queryCollection1) {
                orgNicList.Add(mo);
                nicComboBox.Items.Add(mo["Description"]);
            }
            if (orgNicList.Count > 0) {
                //orgNicList.Sort(hikaku);
                nicComboBox.SelectedIndex = 0;
            }

            // サーバ・クライアントのプロトコルコンボボックスの初期値を設定
            sprotoComboBox.SelectedIndex = 0;
            cprotoComboBox.SelectedIndex = 0;
        }


        private int hikaku(ManagementObject x, ManagementObject y) {
            return (int)y.GetPropertyValue("NumForwardPackets") - (int)x.GetPropertyValue("NumForwardPackets");
        }

        private void start_Click(object sender, EventArgs e) {
            if (formRunningFlag == true) {
                return;
            }

            //実行すべきサーバorクライアントがあるかチェック
            if (serverBindingSource.List.Count == 0 && clientBindingSource.List.Count == 0) {
                MessageBox.Show("実行すべきアイテムがありません");
                return;
            }

            //スタートボタン等の無効化
            startButton.Enabled = false;
            nicSetButton.Enabled = false;
            nicResButton.Enabled = false;
            //ステータス表示の変更
            toolStripStatusLabel1.Text = "起動中";

            // テスト用インスタンスのクリア
            testerServerList.Clear();
            testerClientList.Clear();

            // 設定の読み込み
            foreach (TesterServer ts in serverBindingSource.List) {
                testerServerList.Add(ts);
            }
            foreach (TesterClient tc in clientBindingSource.List) {
                testerClientList.Add(tc);
            }

            //ログ出力方法の決定
            if (radioButton1.Checked == true) {
                nowlogProcess = ResultLogProcessType.Window;
            }
            else if (radioButton2.Checked == true) {
                nowlogProcess = ResultLogProcessType.File;
            }
            else if (radioButton3.Checked == true) {
                nowlogProcess = ResultLogProcessType.None;
            }

            //ログ出力ファイルのオープン
            if (nowlogProcess == ResultLogProcessType.File) {
                try {
                    resultLogStreamWriter = new System.IO.StreamWriter(logFileNameTextBox.Text);
                }
                catch (Exception ex) {
                    MessageBox.Show("ログ出力ファイル名が無効です" + CRLF + ex.Message);
                }
                resultText.AppendText(logFileNameTextBox.Text + " への出力開始" + CRLF);
            }

            // 統計の更新開始
            resultUpdateTimer.Interval = (int)resultUpdateIntervalUpDown.Value;
            resultUpdateTimer.Enabled = true;

            //ログに開始を書く
            addResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TestConnection Started.");

            //音の設定
            soundEnable = soundBox.Checked;
            if (soundEnable == true) {
                successSoundPlayer = new System.Media.SoundPlayer(SUCCESSSOUNDFILE);
                failureSoundPlayer = new System.Media.SoundPlayer(FAILURESOUNDFILE);
            }

            // サーバ、クライアントの起動
            testerServerList.AllStart();
            testerClientList.itemInterval = (int)testIntervalUpDown.Value;
            testerClientList.listInterval = (int)repeatIntervalUpDown.Value;
            testerClientList.Start((int)repeatCountUpDown.Value);

            //ステータス表示の変更、停止ボタンを有効化、表示タブを結果ログ用に
            toolStripStatusLabel1.Text = "実行中";
            stopButton.Enabled = true;
            mainTabControl.SelectedTab = resultTab;
            formRunningFlag = true;
        }


        private void stop_Click(object sender, EventArgs e) {
            if (formRunningFlag == false) {
                return;
            }

            // 停止ボタンの無効化
            stopButton.Enabled = false;
            //ステータス表示の変更
            toolStripStatusLabel1.Text = "停止中";

            // クライアント、サーバの終了
            testerClientList.Stop();
            testerServerList.AllStop();

            //更新停止
            resultUpdateTimer.Enabled = false;
            serverGridView.Refresh();
            clientGridView.Refresh();

            //音を止める
            if (successSoundPlayer != null) {
                successSoundPlayer.Stop();
                successSoundPlayer = null;
            }
            if (failureSoundPlayer != null) {
                failureSoundPlayer.Stop();
                failureSoundPlayer = null;
            }

            //ログに停止を書く
            addResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TestConnection Stopped.");

            //ログ出力ファイルのクローズ
            if (nowlogProcess == ResultLogProcessType.File) {
                try {
                    resultLogStreamWriter.Close();
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
                resultText.AppendText(logFileNameTextBox.Text + " への出力終了" + CRLF);
            }

            //ステータス表示の変更、開始ボタン等を有効化
            toolStripStatusLabel1.Text = "待機中";
            formRunningFlag = false;
            startButton.Enabled = true;
            nicSetButton.Enabled = true;
            nicResButton.Enabled = true;
        }


        private void loadSetting(System.IO.StreamReader sr) {
            int lineCount = 1;

            serverBindingSource.Clear();
            clientBindingSource.Clear();
            string rowbuffer = sr.ReadLine();
            while (rowbuffer != null) {
                string[] rowdata = rowbuffer.Split(',');
                try {
                    addTester(rowdata[CS], rowdata[LIPADDR], rowdata[RIPADDR], rowdata[PROTO], Convert.ToInt32(rowdata[PORT]));
                }
                catch {
                    MessageBox.Show(lineCount + "行: 読み込みファイルの形式が無効です。");
                    return;
                }
                rowbuffer = sr.ReadLine();
                lineCount++;
            }
        }


        private void addTester(string cs, string lipaddr, string ripaddr, string proto, int portno) {
            //ローカルIPアドレスの書式妥当性チェック クライアントのときはemptyでもオッケー
            if (lipaddr != string.Empty) {
                try {
                    IPAddress.Parse(lipaddr);
                }
                catch {
                    MessageBox.Show(lipaddr + " ローカルIPアドレスが無効です");
                    return;
                }
            }
            //リモートIPアドレスの書式妥当性チェック サーバのときは実行しない
            if (cs != CSType.Server.ToString()) {
                try {
                    IPAddress.Parse(ripaddr);
                }
                catch {
                    MessageBox.Show(ripaddr + " リモートIPアドレスが無効です");
                    return;
                }
            }
            if (proto != ProtocolName.Ping.ToString()) {
                if (portno < 0 || 65535 < portno) {
                    MessageBox.Show(portno + " ポート番号が無効です");
                    return;
                }
            }
            if (cs == CSType.Server.ToString()) {
                if (proto == ProtocolName.TCP.ToString()) {
                    TcpTesterServer temptester = new TcpTesterServer(this, lipaddr, portno);
                    temptester.timeout = (int)timeoutUpDown.Value;
                    temptester.successEvent += new successEventHandler(successPlaySound);
                    temptester.failureEvent += new failureEventHandler(failurePlaySound);
                    serverBindingSource.Add(temptester);
                }
                else if (proto == ProtocolName.UDP.ToString()) {
                    UdpTesterServer temptester = new UdpTesterServer(this, lipaddr, portno);
                    temptester.successEvent += new successEventHandler(successPlaySound);
                    temptester.failureEvent += new failureEventHandler(failurePlaySound);
                    serverBindingSource.Add(temptester);
                }
                else {
                    MessageBox.Show(proto + " is TCP/UDP type error");
                    return;
                }
            }
            else if (cs == CSType.Client.ToString()) {
                if (proto == ProtocolName.TCP.ToString()) {
                    TcpTesterClient temptester = new TcpTesterClient(this, lipaddr, ripaddr, portno);
                    temptester.timeout = (int)timeoutUpDown.Value;
                    temptester.successEvent += new successEventHandler(successPlaySound);
                    temptester.failureEvent += new failureEventHandler(failurePlaySound);
                    clientBindingSource.Add(temptester);
                }
                else if (proto == ProtocolName.UDP.ToString()) {
                    UdpTesterClient temptester = new UdpTesterClient(this, lipaddr, ripaddr, portno);
                    temptester.successEvent += new successEventHandler(successPlaySound);
                    temptester.failureEvent += new failureEventHandler(failurePlaySound);
                    clientBindingSource.Add(temptester);
                }
                else if (proto == ProtocolName.DNS.ToString()) {
                    DnsTesterClient temptester = new DnsTesterClient(this, lipaddr, ripaddr, portno);
                    temptester.timeout = (int)timeoutUpDown.Value;
                    temptester.successEvent += new successEventHandler(successPlaySound);
                    temptester.failureEvent += new failureEventHandler(failurePlaySound);
                    clientBindingSource.Add(temptester);
                }
                else if (proto == ProtocolName.Ping.ToString()) {
                    PingTesterClient temptester = new PingTesterClient(this, lipaddr, ripaddr);
                    temptester.timeout = (int)timeoutUpDown.Value;
                    temptester.successEvent += new successEventHandler(successPlaySound);
                    temptester.failureEvent += new failureEventHandler(failurePlaySound);
                    clientBindingSource.Add(temptester);
                }
                else {
                    MessageBox.Show(proto + " is TCP/UDP/DNS/Ping type error");
                    return;
                }
            }
            else {
                MessageBox.Show(cs + " is Server/Client type error");
                return;
            }
        }


        public void addResult(string msg) {
            switch (nowlogProcess) {
                case ResultLogProcessType.Window:
                    resultText.AppendText((string)msg + CRLF);
                    break;
                case ResultLogProcessType.File:
                    resultLogStreamWriter.WriteLine(msg);
                    break;
                case ResultLogProcessType.None:
                    //何もしない
                    break;
            }
        }


        private void saveResultButton_Click(object sender, EventArgs e) {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "テキストファイル(*.txt)|*.txt|すべてのファイル(*.*)|*.*";
            sfd.Title = "結果を保存";
            sfd.RestoreDirectory = true;

            //ダイアログを表示する
            if (sfd.ShowDialog() == DialogResult.OK) {
                string fileName;
                fileName = sfd.FileName;
                if (fileName != null) {
                    //ファイルに書き込む
                    resultLogStreamWriter = new System.IO.StreamWriter(fileName,
                        false, System.Text.Encoding.GetEncoding("Shift_JIS"));
                    resultLogStreamWriter.Write(resultText.Text);
                    //閉じる
                    resultLogStreamWriter.Close();
                }
            }
        }


        private void clearResultButton_Click(object sender, EventArgs e) {
            resultText.Text = string.Empty;
        }


        private void resultUpdateTimer_Tick(object sender, EventArgs e) {
            統計更新ToolStripMenuItem_Click(sender, e);
        }


        private void saddbtn_Click(object sender, EventArgs e) {
            try {
                if (clipTextBox.Text != string.Empty) {
                    IPAddress.Parse(slipTextBox.Text);
                }
            }
            catch {
                MessageBox.Show("IPアドレスが無効です");
                return;
            }
            addTester(CSType.Server.ToString(), slipTextBox.Text, "", sprotoComboBox.Text, (int)sportUpDown.Value);
        }


        private void sdelbtn_Click(object sender, EventArgs e) {
            TesterServer ts;
            ts = (TesterServer)serverBindingSource.Current;
            slipTextBox.Text = ts.localIpAddress;
            sprotoComboBox.Text = ts.protocolName.ToString();
            sportUpDown.Value = ts.portNo;
            serverBindingSource.RemoveCurrent();
        }


        private void sAdelbtn_Click(object sender, EventArgs e) {
            serverBindingSource.Clear();
        }


        private void caddbtn_Click(object sender, EventArgs e) {
            try {
                if (clipTextBox.Text != string.Empty) {
                    IPAddress.Parse(clipTextBox.Text);
                }
                IPAddress.Parse(cripTextBox.Text);
            }
            catch {
                MessageBox.Show("IPアドレスが無効です");
                return;
            }
            addTester(CSType.Client.ToString(), clipTextBox.Text, cripTextBox.Text, cprotoComboBox.Text, (int)cportUpDown.Value);
        }


        private void cdelbtn_Click(object sender, EventArgs e) {
            TesterClient tc;
            tc = (TesterClient)clientBindingSource.Current;
            clipTextBox.Text = tc.localIpAddress;
            cripTextBox.Text = tc.remoteIpAddress;
            cprotoComboBox.Text = tc.protocolName.ToString();
            cportUpDown.Value = tc.portNo;
            clientBindingSource.RemoveCurrent();
        }


        private void cAdelbtn_Click(object sender, EventArgs e) {
            clientBindingSource.Clear();
        }


        private void 新規作成ToolStripMenuItem_Click(object sender, EventArgs e) {
            stop_Click(sender, e);
            serverBindingSource.Clear();
            clientBindingSource.Clear();
            clearResultButton_Click(sender, e);
        }


        private void 開くToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "カンマ区切りファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
            ofd.Title = "設定を開く";
            ofd.RestoreDirectory = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK) {
                System.IO.Stream stream;
                stream = ofd.OpenFile();

                if (stream != null) {
                    //内容を読み込み
                    System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                    loadSetting(sr);
                    sr.Close();
                    stream.Close();
                }
            }
        }


        private void 名前を付けて保存ToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFileDialog sfd = new SaveFileDialog();

            //SaveFileDialogクラスのインスタンスを作成
            sfd.Filter = "カンマ区切りファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
            sfd.Title = "設定を保存";
            sfd.RestoreDirectory = true;

            //ダイアログを表示する
            if (sfd.ShowDialog() == DialogResult.OK) {
                System.IO.Stream stream;
                stream = sfd.OpenFile();
                if (stream != null) {
                    //ファイルに書き込む
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(stream);

                    foreach (TesterServer it in serverBindingSource.List) {
                        string sline = it.csType.ToString() + "," +
                                    it.localIpAddress + "," +
                                     "," +
                                    it.protocolName.ToString() + "," +
                                    it.portNo.ToString();
                        sw.WriteLine(sline);
                    }
                    foreach (TesterClient it in clientBindingSource.List) {
                        string sline = it.csType.ToString() + "," +
                                    it.localIpAddress + "," +
                                    it.remoteIpAddress + "," +
                                    it.protocolName.ToString() + "," +
                                    it.portNo.ToString();
                        sw.WriteLine(sline);
                    }

                    //閉じる
                    sw.Close();
                    stream.Close();
                }
            }
        }


        private void バージョン情報ToolStripMenuItem_Click(object sender, EventArgs e) {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }


        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e) {
            stop_Click(sender, e);
            nicRes_Click(sender, e);
            Application.Exit();
        }


        private void 統計クリアToolStripMenuItem_Click(object sender, EventArgs e) {
            testerClientList.AllClear();
            testerServerList.AllClear();
            統計更新ToolStripMenuItem_Click(sender, e);
        }


        private void 統計更新ToolStripMenuItem_Click(object sender, EventArgs e) {
            //GridViewをリフレッシュして成功数・失敗数の表示を更新する
            if (mainTabControl.SelectedTab == serverTab) {
                serverGridView.Refresh();
            }
            else if (mainTabControl.SelectedTab == clientTab) {
                clientGridView.Refresh();
            }
        }


        private void closingclean(object sender, FormClosingEventArgs e) {
            stop_Click(sender, e);
            nicRes_Click(sender, e);
        }


        private void saveSetting_Click(object sender, EventArgs e) {
            global::TestConnection.Properties.Settings.Default.Save();
        }


        private void loadDefaultButton_Click(object sender, EventArgs e) {
            repeatCountUpDown.Value = 0;
            testIntervalUpDown.Value = 500;
            repeatIntervalUpDown.Value = 1000;
            timeoutUpDown.Value = 1000;
            resultUpdateIntervalUpDown.Value = 500;
            soundBox.Checked = false;
            //nicCheckBox.Checked = false;
            subnetTextBox.Text = "255.255.255.0";
            defgwTextBox.Text = "0.0.0.0";
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            logFileNameTextBox.Text = ".\\tescon.log";
        }


        private void logFileNameSelectButton_Click(object sender, EventArgs e) {
            SaveFileDialog sfd = new SaveFileDialog();

            //SaveFileDialogクラスのインスタンスを作成
            sfd.Filter = "ログファイル(*.log)|*.log|すべてのファイル(*.*)|*.*";
            sfd.Title = "ログファイル名を指定";
            sfd.RestoreDirectory = true;

            //ファイル名を設定
            if (sfd.ShowDialog() == DialogResult.OK) {
                logFileNameTextBox.Text = sfd.FileName;
            }
        }


        //WAVEファイルを再生する -- 成功音
        public void successPlaySound() {
            if (soundEnable == true && successSoundPlayer != null) {
                successSoundPlayer.Stop();
                successSoundPlayer.Play();
            }
        }


        //WAVEファイルを再生する -- 失敗音
        public void failurePlaySound() {
            if (soundEnable == true && failureSoundPlayer != null) {
                failureSoundPlayer.Stop();
                failureSoundPlayer.Play();
            }
        }


        private void nicSet_Click(object sender, EventArgs e) {
            ManagementObject targetNIC;

            // コンボボックスに表示されているNICをターゲットにする
            targetNIC = (ManagementObject)orgNicList[nicComboBox.SelectedIndex].Clone(); ;

            // サブネットマスクのチェック
            try {
                IPAddress.Parse(subnetTextBox.Text);
            }
            catch {
                MessageBox.Show("サブネットマスクが無効です");
                return;
            }
            // デフォルトゲートウェイのチェック
            try {
                IPAddress.Parse(defgwTextBox.Text);
            }
            catch {
                MessageBox.Show("デフォルトゲートウェイが無効です");
                return;
            }

            // 重複の無いIPアドレス配列を作る ただしループバック以外
            List<string> ipList = new List<string>();
            List<string> subnetList = new List<string>();
            foreach (TesterServer si in serverBindingSource.List) {
                if (si.localIpAddress != string.Empty
                    && !IPAddress.IsLoopback(IPAddress.Parse(si.localIpAddress))
                    && !ipList.Contains(si.localIpAddress)) {
                    ipList.Add(si.localIpAddress);
                    subnetList.Add(subnetTextBox.Text);
                }
            }
            foreach (TesterClient si in clientBindingSource.List) {
                if (si.localIpAddress != string.Empty
                    && !IPAddress.IsLoopback(IPAddress.Parse(si.localIpAddress))
                    && !ipList.Contains(si.localIpAddress)) {
                    ipList.Add(si.localIpAddress);
                    subnetList.Add(subnetTextBox.Text);
                }
            }

            // IPアドレスの設定
            ManagementBaseObject objNewIP;
            ManagementBaseObject objNewGate;
            objNewIP = targetNIC.GetMethodParameters("EnableStatic");
            objNewIP["IPAddress"] = ipList.ToArray();
            objNewIP["SubnetMask"] = subnetList.ToArray();
            objNewGate = targetNIC.GetMethodParameters("SetGateways");
            objNewGate["DefaultIPGateway"] = new string[] { defgwTextBox.Text };
            objNewGate["GatewayCostMetric"] = new int[] { 1 };
            targetNIC.InvokeMethod("EnableStatic", objNewIP, null);
            targetNIC.InvokeMethod("SetGateways", objNewGate, null);

            //NIC情報の変更フラグをtrueに
            ipChangedFlag = true;
        }


        private void nicRes_Click(object sender, EventArgs e) {
            //ネットワークアダプタのIP設定を復元
            if (ipChangedFlag == true) {
                foreach (ManagementObject targetNIC in orgNicList) {
                    ManagementBaseObject objNewIP;
                    ManagementBaseObject objNewGate;
                    if ((bool)targetNIC.GetPropertyValue("DHCPEnabled")) {
                        targetNIC.InvokeMethod("EnableDHCP", null);
                    }
                    else {
                        objNewIP = targetNIC.GetMethodParameters("EnableStatic");
                        objNewGate = targetNIC.GetMethodParameters("SetGateways");
                        targetNIC.InvokeMethod("EnableStatic", objNewIP, null);
                        targetNIC.InvokeMethod("SetGateways", objNewGate, null);
                    }
                }
                ipChangedFlag = false;
            }
        }
    }
}