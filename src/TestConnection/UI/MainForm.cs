using System;
using System.Collections.Generic;
using System.Management;
using System.Windows.Forms;

namespace TestConnection {
    public partial class MainForm : Form {
        enum ResultLogProcessType { Window, File, None };

        const string CRLF = "\r\n";
        const string DEFAULTSETTING = "res/default.csv";
        const string SUCCESSSOUNDFILE = "res/success.wav";
        const string FAILURESOUNDFILE = "res/failure.wav";

        private readonly TestSession testSession;
        private List<ManagementObject> orgNicList = new List<ManagementObject>();
        private ResultLogProcessType nowlogProcess = ResultLogProcessType.Window;
        private System.IO.StreamWriter resultLogStreamWriter;
        private bool soundEnable = false;
        private System.Media.SoundPlayer successSoundPlayer = null;
        private System.Media.SoundPlayer failureSoundPlayer = null;

        public MainForm() {
            InitializeComponent();
            testSession = new TestSession(outputTesterResult, successPlaySound, failurePlaySound);

            if (System.IO.File.Exists(DEFAULTSETTING)) {
                using (System.IO.StreamReader fsr = new System.IO.StreamReader(DEFAULTSETTING)) {
                    loadSetting(fsr);
                }
            }

            nicComboBox.Items.Clear();
            using (ManagementObjectSearcher query1 = new ManagementObjectSearcher(
                "Select * from Win32_NetworkAdapterConfiguration where IPEnabled=TRUE")) {
                ManagementObjectCollection queryCollection1 = query1.Get();
                foreach (ManagementObject mo in queryCollection1) {
                    orgNicList.Add(mo);
                    nicComboBox.Items.Add(mo["Description"]);
                }
            }
            if (orgNicList.Count > 0) {
                nicComboBox.SelectedIndex = 0;
            }

            sprotoComboBox.SelectedIndex = 0;
            cprotoComboBox.SelectedIndex = 0;
        }

        private void start_Click(object sender, EventArgs e) {
            if (testSession.IsRunning) {
                return;
            }

            if (serverBindingSource.List.Count == 0 && clientBindingSource.List.Count == 0) {
                MessageBox.Show("実行すべきアイテムがありません");
                return;
            }

            if (!prepareResultLog()) {
                return;
            }

            setStartingUiState();

            try {
                startRunResources();
                testSession.Start(
                    getTesters(),
                    (int)testIntervalUpDown.Value,
                    (int)repeatIntervalUpDown.Value,
                    (int)repeatCountUpDown.Value);
            }
            catch (Exception ex) {
                finishRunResources(false);
                setIdleUiState();
                MessageBox.Show("試験を開始できませんでした。" + CRLF + ex.Message);
                return;
            }

            setRunningUiState();
        }

        private IEnumerable<TesterBase> getTesters() {
            foreach (TesterServer tester in serverBindingSource.List) {
                yield return tester;
            }
            foreach (TesterClient tester in clientBindingSource.List) {
                yield return tester;
            }
        }

        private bool prepareResultLog() {
            if (radioButton1.Checked) {
                nowlogProcess = ResultLogProcessType.Window;
            }
            else if (radioButton2.Checked) {
                nowlogProcess = ResultLogProcessType.File;
            }
            else {
                nowlogProcess = ResultLogProcessType.None;
            }

            resultLogStreamWriter = null;
            if (nowlogProcess != ResultLogProcessType.File) {
                return true;
            }

            try {
                resultLogStreamWriter = new System.IO.StreamWriter(logFileNameTextBox.Text);
            }
            catch (Exception ex) {
                MessageBox.Show("ログ出力ファイル名が無効です" + CRLF + ex.Message);
                return false;
            }

            resultText.AppendText(logFileNameTextBox.Text + " への出力開始" + CRLF);
            return true;
        }

        private void startRunResources() {
            resultUpdateTimer.Interval = (int)resultUpdateIntervalUpDown.Value;
            resultUpdateTimer.Enabled = true;

            addResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TestConnection Started.");

            soundEnable = soundBox.Checked;
            if (soundEnable) {
                successSoundPlayer = new System.Media.SoundPlayer(SUCCESSSOUNDFILE);
                failureSoundPlayer = new System.Media.SoundPlayer(FAILURESOUNDFILE);
            }
        }

        private void stop_Click(object sender, EventArgs e) {
            if (!testSession.IsRunning) {
                return;
            }

            stopButton.Enabled = false;
            toolStripStatusLabel1.Text = "停止中";

            Exception stopError = null;
            try {
                testSession.Stop();
            }
            catch (Exception ex) {
                stopError = ex;
            }

            finishRunResources(true);
            setIdleUiState();

            if (stopError != null) {
                MessageBox.Show("試験の停止中にエラーが発生しました。" + CRLF + stopError.Message);
            }
        }

        private void finishRunResources(bool writeStoppedResult) {
            resultUpdateTimer.Enabled = false;
            serverGridView.Refresh();
            clientGridView.Refresh();

            disposeSoundPlayers();

            if (writeStoppedResult) {
                addResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TestConnection Stopped.");
            }

            closeResultLog();
        }

        private void disposeSoundPlayers() {
            if (successSoundPlayer != null) {
                successSoundPlayer.Stop();
                successSoundPlayer.Dispose();
                successSoundPlayer = null;
            }
            if (failureSoundPlayer != null) {
                failureSoundPlayer.Stop();
                failureSoundPlayer.Dispose();
                failureSoundPlayer = null;
            }
            soundEnable = false;
        }

        private void closeResultLog() {
            if (nowlogProcess != ResultLogProcessType.File) {
                return;
            }

            System.IO.StreamWriter writer = resultLogStreamWriter;
            resultLogStreamWriter = null;
            if (writer != null) {
                try {
                    writer.Dispose();
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
            resultText.AppendText(logFileNameTextBox.Text + " への出力終了" + CRLF);
        }

        private void setStartingUiState() {
            startButton.Enabled = false;
            stopButton.Enabled = false;
            nicSetButton.Enabled = false;
            nicResButton.Enabled = false;
            toolStripStatusLabel1.Text = "起動中";
        }

        private void setRunningUiState() {
            toolStripStatusLabel1.Text = "実行中";
            stopButton.Enabled = true;
            mainTabControl.SelectedTab = resultTab;
        }

        private void setIdleUiState() {
            toolStripStatusLabel1.Text = "待機中";
            startButton.Enabled = true;
            stopButton.Enabled = false;
            updateNicButtons();
        }

        private void loadSetting(System.IO.StreamReader sr) {
            List<TesterDefinition> definitions;
            try {
                definitions = TesterDefinitionFile.Load(sr);
            }
            catch (FormatException ex) {
                MessageBox.Show(ex.Message);
                return;
            }

            serverBindingSource.Clear();
            clientBindingSource.Clear();
            foreach (TesterDefinition definition in definitions) {
                addTester(definition);
            }
        }

        private void addTester(TesterDefinition definition) {
            TesterBase tester;
            string validationError;
            if (!testSession.TryCreateTester(
                definition, (int)timeoutUpDown.Value, out tester, out validationError)) {
                MessageBox.Show(validationError);
                return;
            }

            TesterServer serverTester = tester as TesterServer;
            if (serverTester != null) {
                serverBindingSource.Add(serverTester);
                return;
            }

            clientBindingSource.Add((TesterClient)tester);
        }

        private static bool tryParseProtocolName(string value, out ProtocolName protocol) {
            return Enum.TryParse(value, out protocol) && protocol.ToString() == value;
        }

        private void outputTesterResult(string message) {
            if (InvokeRequired) {
                Invoke(new Action<string>(outputTesterResult), message);
                return;
            }
            addResult(message);
        }

        public void addResult(string message) {
            switch (nowlogProcess) {
                case ResultLogProcessType.Window:
                    resultText.AppendText(message + CRLF);
                    break;
                case ResultLogProcessType.File:
                    if (resultLogStreamWriter != null) {
                        resultLogStreamWriter.WriteLine(message);
                    }
                    break;
                case ResultLogProcessType.None:
                    break;
            }
        }

        private void saveResultButton_Click(object sender, EventArgs e) {
            using (SaveFileDialog sfd = new SaveFileDialog()) {
                sfd.Filter = "テキストファイル(*.txt)|*.txt|すべてのファイル(*.*)|*.*";
                sfd.Title = "結果を保存";
                sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == DialogResult.OK && sfd.FileName != null) {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(
                        sfd.FileName, false, System.Text.Encoding.GetEncoding("Shift_JIS"))) {
                        writer.Write(resultText.Text);
                    }
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
            ProtocolName protocol;
            if (!tryParseProtocolName(sprotoComboBox.Text, out protocol)) {
                MessageBox.Show(sprotoComboBox.Text + " is TCP/UDP type error");
                return;
            }
            addTester(new TesterDefinition(
                TesterRole.Server, slipTextBox.Text, string.Empty, protocol, (int)sportUpDown.Value));
        }

        private void sdelbtn_Click(object sender, EventArgs e) {
            TesterServer ts = serverBindingSource.Current as TesterServer;
            if (ts == null) {
                return;
            }

            slipTextBox.Text = ts.LocalIpAddress;
            sprotoComboBox.Text = ts.Protocol.ToString();
            sportUpDown.Value = ts.Port;
            serverBindingSource.RemoveCurrent();
        }

        private void sAdelbtn_Click(object sender, EventArgs e) {
            serverBindingSource.Clear();
        }

        private void caddbtn_Click(object sender, EventArgs e) {
            ProtocolName protocol;
            if (!tryParseProtocolName(cprotoComboBox.Text, out protocol)) {
                MessageBox.Show(cprotoComboBox.Text + " is TCP/UDP/DNS/Ping type error");
                return;
            }
            addTester(new TesterDefinition(
                TesterRole.Client, clipTextBox.Text, cripTextBox.Text, protocol, (int)cportUpDown.Value));
        }

        private void cdelbtn_Click(object sender, EventArgs e) {
            TesterClient tc = clientBindingSource.Current as TesterClient;
            if (tc == null) {
                return;
            }

            clipTextBox.Text = tc.LocalIpAddress;
            cripTextBox.Text = tc.RemoteIpAddress;
            cprotoComboBox.Text = tc.Protocol.ToString();
            cportUpDown.Value = tc.Port;
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
            using (OpenFileDialog ofd = new OpenFileDialog()) {
                ofd.Filter = "カンマ区切りファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
                ofd.Title = "設定を開く";
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() == DialogResult.OK) {
                    using (System.IO.Stream stream = ofd.OpenFile()) {
                        if (stream != null) {
                            using (System.IO.StreamReader sr = new System.IO.StreamReader(stream)) {
                                loadSetting(sr);
                            }
                        }
                    }
                }
            }
        }

        private void 名前を付けて保存ToolStripMenuItem_Click(object sender, EventArgs e) {
            using (SaveFileDialog sfd = new SaveFileDialog()) {
                sfd.Filter = "カンマ区切りファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
                sfd.Title = "設定を保存";
                sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == DialogResult.OK) {
                    using (System.IO.Stream stream = sfd.OpenFile())
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(stream)) {
                        List<TesterDefinition> definitions = new List<TesterDefinition>();
                        foreach (TesterServer tester in serverBindingSource.List) {
                            definitions.Add(tester.Definition);
                        }
                        foreach (TesterClient tester in clientBindingSource.List) {
                            definitions.Add(tester.Definition);
                        }
                        TesterDefinitionFile.Save(sw, definitions);
                    }
                }
            }
        }

        private void バージョン情報ToolStripMenuItem_Click(object sender, EventArgs e) {
            using (AboutBox aboutBox = new AboutBox()) {
                aboutBox.ShowDialog();
            }
        }

        private void 統計クリアToolStripMenuItem_Click(object sender, EventArgs e) {
            testSession.ClearStatistics();
            統計更新ToolStripMenuItem_Click(sender, e);
        }

        private void 統計更新ToolStripMenuItem_Click(object sender, EventArgs e) {
            if (mainTabControl.SelectedTab == serverTab) {
                serverGridView.Refresh();
            }
            else if (mainTabControl.SelectedTab == clientTab) {
                clientGridView.Refresh();
            }
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
            subnetTextBox.Text = "255.255.255.0";
            defgwTextBox.Text = "0.0.0.0";
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            logFileNameTextBox.Text = ".\\tescon.log";
        }

        private void logFileNameSelectButton_Click(object sender, EventArgs e) {
            using (SaveFileDialog sfd = new SaveFileDialog()) {
                sfd.Filter = "ログファイル(*.log)|*.log|すべてのファイル(*.*)|*.*";
                sfd.Title = "ログファイル名を指定";
                sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == DialogResult.OK) {
                    logFileNameTextBox.Text = sfd.FileName;
                }
            }
        }

        public void successPlaySound() {
            if (soundEnable && successSoundPlayer != null) {
                successSoundPlayer.Stop();
                successSoundPlayer.Play();
            }
        }

        public void failurePlaySound() {
            if (soundEnable && failureSoundPlayer != null) {
                failureSoundPlayer.Stop();
                failureSoundPlayer.Play();
            }
        }
    }
}
