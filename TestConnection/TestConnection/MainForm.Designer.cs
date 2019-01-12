namespace TestConnection
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.resultTab = new System.Windows.Forms.TabPage();
            this.clearResultButton = new System.Windows.Forms.Button();
            this.resultText = new System.Windows.Forms.TextBox();
            this.saveResultButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.settingTab = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.logFileNameTextBox = new System.Windows.Forms.TextBox();
            this.logFileNameSelectButton = new System.Windows.Forms.Button();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.loadDefaultButton = new System.Windows.Forms.Button();
            this.saveSettingButton = new System.Windows.Forms.Button();
            this.defgwTextBox = new System.Windows.Forms.TextBox();
            this.subnetTextBox = new System.Windows.Forms.TextBox();
            this.soundBox = new System.Windows.Forms.CheckBox();
            this.repeatCountUpDown = new System.Windows.Forms.NumericUpDown();
            this.repeatIntervalUpDown = new System.Windows.Forms.NumericUpDown();
            this.timeoutUpDown = new System.Windows.Forms.NumericUpDown();
            this.testIntervalUpDown = new System.Windows.Forms.NumericUpDown();
            this.resultUpdateIntervalUpDown = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nicComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.serverTab = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.sportUpDown = new System.Windows.Forms.NumericUpDown();
            this.sprotoComboBox = new System.Windows.Forms.ComboBox();
            this.sdelbtn = new System.Windows.Forms.Button();
            this.sAdelbtn = new System.Windows.Forms.Button();
            this.saddbtn = new System.Windows.Forms.Button();
            this.slipTextBox = new System.Windows.Forms.TextBox();
            this.serverGridView = new System.Windows.Forms.DataGridView();
            this.localIpAddressDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.protocolNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.portNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.successCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.failureCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.serverBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.clientTab = new System.Windows.Forms.TabPage();
            this.label13 = new System.Windows.Forms.Label();
            this.cportUpDown = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.cprotoComboBox = new System.Windows.Forms.ComboBox();
            this.cdelbtn = new System.Windows.Forms.Button();
            this.cAdelbtn = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.caddbtn = new System.Windows.Forms.Button();
            this.cripTextBox = new System.Windows.Forms.TextBox();
            this.clipTextBox = new System.Windows.Forms.TextBox();
            this.clientGridView = new System.Windows.Forms.DataGridView();
            this.localIpAddressDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.remoteIpAddressDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.protocolNameDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.portNoDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.successCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.failureCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clientBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.resultUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.ファイルToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新規作成ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.開くToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.上書き保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ツールToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.統計クリアToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.統計更新ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ヘルプToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.バージョン情報ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nicSetButton = new System.Windows.Forms.Button();
            this.nicResButton = new System.Windows.Forms.Button();
            this.resultTab.SuspendLayout();
            this.settingTab.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.repeatCountUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repeatIntervalUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testIntervalUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultUpdateIntervalUpDown)).BeginInit();
            this.serverTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sportUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverBindingSource)).BeginInit();
            this.mainTabControl.SuspendLayout();
            this.clientTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cportUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.clientGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.clientBindingSource)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // resultTab
            // 
            this.resultTab.Controls.Add(this.clearResultButton);
            this.resultTab.Controls.Add(this.resultText);
            this.resultTab.Controls.Add(this.saveResultButton);
            this.resultTab.Location = new System.Drawing.Point(4, 22);
            this.resultTab.Name = "resultTab";
            this.resultTab.Padding = new System.Windows.Forms.Padding(3);
            this.resultTab.Size = new System.Drawing.Size(618, 300);
            this.resultTab.TabIndex = 3;
            this.resultTab.Text = "結果";
            this.resultTab.UseVisualStyleBackColor = true;
            // 
            // clearResultButton
            // 
            this.clearResultButton.Location = new System.Drawing.Point(92, 6);
            this.clearResultButton.Name = "clearResultButton";
            this.clearResultButton.Size = new System.Drawing.Size(80, 20);
            this.clearResultButton.TabIndex = 402;
            this.clearResultButton.Text = "結果クリア";
            this.clearResultButton.UseVisualStyleBackColor = true;
            this.clearResultButton.Click += new System.EventHandler(this.clearResultButton_Click);
            // 
            // resultText
            // 
            this.resultText.AcceptsReturn = true;
            this.resultText.AcceptsTab = true;
            this.resultText.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.resultText.Location = new System.Drawing.Point(3, 31);
            this.resultText.Multiline = true;
            this.resultText.Name = "resultText";
            this.resultText.ReadOnly = true;
            this.resultText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.resultText.Size = new System.Drawing.Size(612, 266);
            this.resultText.TabIndex = 410;
            // 
            // saveResultButton
            // 
            this.saveResultButton.Location = new System.Drawing.Point(6, 6);
            this.saveResultButton.Name = "saveResultButton";
            this.saveResultButton.Size = new System.Drawing.Size(80, 20);
            this.saveResultButton.TabIndex = 401;
            this.saveResultButton.Text = "結果保存";
            this.saveResultButton.UseVisualStyleBackColor = true;
            this.saveResultButton.Click += new System.EventHandler(this.saveResultButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(90, 27);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(80, 20);
            this.stopButton.TabIndex = 2;
            this.stopButton.Text = "テスト停止";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stop_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(4, 27);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(80, 20);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "テスト実行";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.start_Click);
            // 
            // settingTab
            // 
            this.settingTab.Controls.Add(this.groupBox1);
            this.settingTab.Controls.Add(this.loadDefaultButton);
            this.settingTab.Controls.Add(this.saveSettingButton);
            this.settingTab.Controls.Add(this.defgwTextBox);
            this.settingTab.Controls.Add(this.subnetTextBox);
            this.settingTab.Controls.Add(this.soundBox);
            this.settingTab.Controls.Add(this.repeatCountUpDown);
            this.settingTab.Controls.Add(this.repeatIntervalUpDown);
            this.settingTab.Controls.Add(this.timeoutUpDown);
            this.settingTab.Controls.Add(this.testIntervalUpDown);
            this.settingTab.Controls.Add(this.resultUpdateIntervalUpDown);
            this.settingTab.Controls.Add(this.label8);
            this.settingTab.Controls.Add(this.label7);
            this.settingTab.Controls.Add(this.label17);
            this.settingTab.Controls.Add(this.label16);
            this.settingTab.Controls.Add(this.label6);
            this.settingTab.Controls.Add(this.label4);
            this.settingTab.Controls.Add(this.label15);
            this.settingTab.Controls.Add(this.label5);
            this.settingTab.Controls.Add(this.label3);
            this.settingTab.Controls.Add(this.nicComboBox);
            this.settingTab.Controls.Add(this.label2);
            this.settingTab.Location = new System.Drawing.Point(4, 22);
            this.settingTab.Name = "settingTab";
            this.settingTab.Padding = new System.Windows.Forms.Padding(3);
            this.settingTab.Size = new System.Drawing.Size(618, 300);
            this.settingTab.TabIndex = 1;
            this.settingTab.Text = "環境設定";
            this.settingTab.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.logFileNameTextBox);
            this.groupBox1.Controls.Add(this.logFileNameSelectButton);
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Location = new System.Drawing.Point(336, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 110);
            this.groupBox1.TabIndex = 321;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "結果ログの処理（設定保存されません）";
            // 
            // logFileNameTextBox
            // 
            this.logFileNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TestConnection.Properties.Settings.Default, "logfilename", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.logFileNameTextBox.Location = new System.Drawing.Point(23, 62);
            this.logFileNameTextBox.Name = "logFileNameTextBox";
            this.logFileNameTextBox.Size = new System.Drawing.Size(175, 19);
            this.logFileNameTextBox.TabIndex = 353;
            this.logFileNameTextBox.Text = global::TestConnection.Properties.Settings.Default.logfilename;
            // 
            // logFileNameSelectButton
            // 
            this.logFileNameSelectButton.Location = new System.Drawing.Point(204, 62);
            this.logFileNameSelectButton.Name = "logFileNameSelectButton";
            this.logFileNameSelectButton.Size = new System.Drawing.Size(50, 20);
            this.logFileNameSelectButton.TabIndex = 354;
            this.logFileNameSelectButton.Text = "選択";
            this.logFileNameSelectButton.UseVisualStyleBackColor = true;
            this.logFileNameSelectButton.Click += new System.EventHandler(this.logFileNameSelectButton_Click);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(6, 87);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(76, 16);
            this.radioButton3.TabIndex = 355;
            this.radioButton3.Text = "出力しない";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(6, 18);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(214, 16);
            this.radioButton1.TabIndex = 351;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "ウィンドウに表示（メモリの使用量に注意）";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(6, 40);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(90, 16);
            this.radioButton2.TabIndex = 352;
            this.radioButton2.Text = "ファイルに保存";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // loadDefaultButton
            // 
            this.loadDefaultButton.Location = new System.Drawing.Point(92, 6);
            this.loadDefaultButton.Name = "loadDefaultButton";
            this.loadDefaultButton.Size = new System.Drawing.Size(80, 20);
            this.loadDefaultButton.TabIndex = 302;
            this.loadDefaultButton.Text = "デフォルト";
            this.loadDefaultButton.UseVisualStyleBackColor = true;
            this.loadDefaultButton.Click += new System.EventHandler(this.loadDefaultButton_Click);
            // 
            // saveSettingButton
            // 
            this.saveSettingButton.Location = new System.Drawing.Point(6, 6);
            this.saveSettingButton.Name = "saveSettingButton";
            this.saveSettingButton.Size = new System.Drawing.Size(80, 20);
            this.saveSettingButton.TabIndex = 301;
            this.saveSettingButton.Text = "設定を保存";
            this.saveSettingButton.UseVisualStyleBackColor = true;
            this.saveSettingButton.Click += new System.EventHandler(this.saveSetting_Click);
            // 
            // defgwTextBox
            // 
            this.defgwTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TestConnection.Properties.Settings.Default, "defaultgateway", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.defgwTextBox.Location = new System.Drawing.Point(141, 258);
            this.defgwTextBox.Name = "defgwTextBox";
            this.defgwTextBox.Size = new System.Drawing.Size(111, 19);
            this.defgwTextBox.TabIndex = 321;
            this.defgwTextBox.Text = global::TestConnection.Properties.Settings.Default.defaultgateway;
            // 
            // subnetTextBox
            // 
            this.subnetTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TestConnection.Properties.Settings.Default, "subnetmask", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.subnetTextBox.Location = new System.Drawing.Point(141, 234);
            this.subnetTextBox.Name = "subnetTextBox";
            this.subnetTextBox.Size = new System.Drawing.Size(111, 19);
            this.subnetTextBox.TabIndex = 320;
            this.subnetTextBox.Text = global::TestConnection.Properties.Settings.Default.subnetmask;
            // 
            // soundBox
            // 
            this.soundBox.AutoSize = true;
            this.soundBox.Checked = global::TestConnection.Properties.Settings.Default.sound;
            this.soundBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TestConnection.Properties.Settings.Default, "sound", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.soundBox.Location = new System.Drawing.Point(6, 164);
            this.soundBox.Name = "soundBox";
            this.soundBox.Size = new System.Drawing.Size(150, 16);
            this.soundBox.TabIndex = 317;
            this.soundBox.Text = "成功・失敗時に音を鳴らす";
            this.soundBox.UseVisualStyleBackColor = true;
            // 
            // repeatCountUpDown
            // 
            this.repeatCountUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::TestConnection.Properties.Settings.Default, "repeatcount", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.repeatCountUpDown.Location = new System.Drawing.Point(207, 34);
            this.repeatCountUpDown.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.repeatCountUpDown.Name = "repeatCountUpDown";
            this.repeatCountUpDown.Size = new System.Drawing.Size(95, 19);
            this.repeatCountUpDown.TabIndex = 312;
            this.repeatCountUpDown.Value = global::TestConnection.Properties.Settings.Default.repeatcount;
            // 
            // repeatIntervalUpDown
            // 
            this.repeatIntervalUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::TestConnection.Properties.Settings.Default, "repeatInterval", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.repeatIntervalUpDown.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.repeatIntervalUpDown.Location = new System.Drawing.Point(207, 84);
            this.repeatIntervalUpDown.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.repeatIntervalUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.repeatIntervalUpDown.Name = "repeatIntervalUpDown";
            this.repeatIntervalUpDown.Size = new System.Drawing.Size(95, 19);
            this.repeatIntervalUpDown.TabIndex = 314;
            this.repeatIntervalUpDown.Value = global::TestConnection.Properties.Settings.Default.repeatInterval;
            // 
            // timeoutUpDown
            // 
            this.timeoutUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::TestConnection.Properties.Settings.Default, "timeout", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.timeoutUpDown.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.timeoutUpDown.Location = new System.Drawing.Point(207, 109);
            this.timeoutUpDown.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.timeoutUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.timeoutUpDown.Name = "timeoutUpDown";
            this.timeoutUpDown.Size = new System.Drawing.Size(95, 19);
            this.timeoutUpDown.TabIndex = 315;
            this.timeoutUpDown.Value = global::TestConnection.Properties.Settings.Default.timeout;
            // 
            // testIntervalUpDown
            // 
            this.testIntervalUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::TestConnection.Properties.Settings.Default, "testinterval", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.testIntervalUpDown.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.testIntervalUpDown.Location = new System.Drawing.Point(207, 59);
            this.testIntervalUpDown.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.testIntervalUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.testIntervalUpDown.Name = "testIntervalUpDown";
            this.testIntervalUpDown.Size = new System.Drawing.Size(95, 19);
            this.testIntervalUpDown.TabIndex = 313;
            this.testIntervalUpDown.Value = global::TestConnection.Properties.Settings.Default.testinterval;
            // 
            // resultUpdateIntervalUpDown
            // 
            this.resultUpdateIntervalUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::TestConnection.Properties.Settings.Default, "resultupdateinterval", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.resultUpdateIntervalUpDown.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.resultUpdateIntervalUpDown.Location = new System.Drawing.Point(207, 134);
            this.resultUpdateIntervalUpDown.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.resultUpdateIntervalUpDown.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.resultUpdateIntervalUpDown.Name = "resultUpdateIntervalUpDown";
            this.resultUpdateIntervalUpDown.Size = new System.Drawing.Size(95, 19);
            this.resultUpdateIntervalUpDown.TabIndex = 316;
            this.resultUpdateIntervalUpDown.Value = global::TestConnection.Properties.Settings.Default.resultupdateinterval;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(191, 12);
            this.label8.TabIndex = 125;
            this.label8.Text = "クライアントテスト繰り返し回数(0=無限)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 111);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 12);
            this.label7.TabIndex = 125;
            this.label7.Text = "タイムアウト(ms)";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(493, 211);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(113, 12);
            this.label17.TabIndex = 125;
            this.label17.Text = "（設定保存されません）";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 190);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(101, 12);
            this.label16.TabIndex = 125;
            this.label16.Text = "設定するNICの情報";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(170, 12);
            this.label6.TabIndex = 125;
            this.label6.Text = "クライアントテスト繰り返し間隔(ms)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(153, 12);
            this.label4.TabIndex = 125;
            this.label4.Text = "クライアントテスト実行間隔(ms)";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(33, 261);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(102, 12);
            this.label15.TabIndex = 125;
            this.label15.Text = "デフォルトゲートウェイ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 237);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 12);
            this.label5.TabIndex = 125;
            this.label5.Text = "サブネットマスク";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 12);
            this.label3.TabIndex = 125;
            this.label3.Text = "統計更新間隔(ms)";
            // 
            // nicComboBox
            // 
            this.nicComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.nicComboBox.Location = new System.Drawing.Point(141, 208);
            this.nicComboBox.Name = "nicComboBox";
            this.nicComboBox.Size = new System.Drawing.Size(346, 20);
            this.nicComboBox.TabIndex = 319;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 211);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 12);
            this.label2.TabIndex = 127;
            this.label2.Text = "NIC";
            // 
            // serverTab
            // 
            this.serverTab.Controls.Add(this.label10);
            this.serverTab.Controls.Add(this.label9);
            this.serverTab.Controls.Add(this.label1);
            this.serverTab.Controls.Add(this.sportUpDown);
            this.serverTab.Controls.Add(this.sprotoComboBox);
            this.serverTab.Controls.Add(this.sdelbtn);
            this.serverTab.Controls.Add(this.sAdelbtn);
            this.serverTab.Controls.Add(this.saddbtn);
            this.serverTab.Controls.Add(this.slipTextBox);
            this.serverTab.Controls.Add(this.serverGridView);
            this.serverTab.Location = new System.Drawing.Point(4, 22);
            this.serverTab.Name = "serverTab";
            this.serverTab.Padding = new System.Windows.Forms.Padding(3);
            this.serverTab.Size = new System.Drawing.Size(618, 300);
            this.serverTab.TabIndex = 0;
            this.serverTab.Text = "サーバ";
            this.serverTab.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(238, 18);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 12);
            this.label10.TabIndex = 111;
            this.label10.Text = "ポート番号";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(146, 18);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 12);
            this.label9.TabIndex = 111;
            this.label9.Text = "プロトコル";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 12);
            this.label1.TabIndex = 111;
            this.label1.Text = "ローカルIPアドレス（省略可）";
            // 
            // sportUpDown
            // 
            this.sportUpDown.Location = new System.Drawing.Point(240, 34);
            this.sportUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.sportUpDown.Name = "sportUpDown";
            this.sportUpDown.Size = new System.Drawing.Size(85, 19);
            this.sportUpDown.TabIndex = 103;
            this.sportUpDown.Value = new decimal(new int[] {
            49152,
            0,
            0,
            0});
            // 
            // sprotoComboBox
            // 
            this.sprotoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sprotoComboBox.FormattingEnabled = true;
            this.sprotoComboBox.Items.AddRange(new object[] {
            "TCP",
            "UDP"});
            this.sprotoComboBox.Location = new System.Drawing.Point(148, 33);
            this.sprotoComboBox.Name = "sprotoComboBox";
            this.sprotoComboBox.Size = new System.Drawing.Size(86, 20);
            this.sprotoComboBox.TabIndex = 102;
            // 
            // sdelbtn
            // 
            this.sdelbtn.Location = new System.Drawing.Point(532, 32);
            this.sdelbtn.Name = "sdelbtn";
            this.sdelbtn.Size = new System.Drawing.Size(80, 20);
            this.sdelbtn.TabIndex = 105;
            this.sdelbtn.Text = "削除";
            this.sdelbtn.UseVisualStyleBackColor = true;
            this.sdelbtn.Click += new System.EventHandler(this.sdelbtn_Click);
            // 
            // sAdelbtn
            // 
            this.sAdelbtn.Location = new System.Drawing.Point(532, 6);
            this.sAdelbtn.Name = "sAdelbtn";
            this.sAdelbtn.Size = new System.Drawing.Size(80, 20);
            this.sAdelbtn.TabIndex = 106;
            this.sAdelbtn.Text = "全削除";
            this.sAdelbtn.UseVisualStyleBackColor = true;
            this.sAdelbtn.Click += new System.EventHandler(this.sAdelbtn_Click);
            // 
            // saddbtn
            // 
            this.saddbtn.Location = new System.Drawing.Point(446, 32);
            this.saddbtn.Name = "saddbtn";
            this.saddbtn.Size = new System.Drawing.Size(80, 20);
            this.saddbtn.TabIndex = 104;
            this.saddbtn.Text = "追加";
            this.saddbtn.UseVisualStyleBackColor = true;
            this.saddbtn.Click += new System.EventHandler(this.saddbtn_Click);
            // 
            // slipTextBox
            // 
            this.slipTextBox.Location = new System.Drawing.Point(8, 33);
            this.slipTextBox.Name = "slipTextBox";
            this.slipTextBox.Size = new System.Drawing.Size(135, 19);
            this.slipTextBox.TabIndex = 101;
            // 
            // serverGridView
            // 
            this.serverGridView.AllowUserToAddRows = false;
            this.serverGridView.AllowUserToDeleteRows = false;
            this.serverGridView.AllowUserToResizeRows = false;
            this.serverGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serverGridView.AutoGenerateColumns = false;
            this.serverGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.serverGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.localIpAddressDataGridViewTextBoxColumn,
            this.protocolNameDataGridViewTextBoxColumn,
            this.portNoDataGridViewTextBoxColumn,
            this.successCountDataGridViewTextBoxColumn,
            this.failureCountDataGridViewTextBoxColumn});
            this.serverGridView.DataSource = this.serverBindingSource;
            this.serverGridView.Location = new System.Drawing.Point(0, 59);
            this.serverGridView.Name = "serverGridView";
            this.serverGridView.ReadOnly = true;
            this.serverGridView.RowTemplate.Height = 21;
            this.serverGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.serverGridView.Size = new System.Drawing.Size(618, 242);
            this.serverGridView.TabIndex = 110;
            // 
            // localIpAddressDataGridViewTextBoxColumn
            // 
            this.localIpAddressDataGridViewTextBoxColumn.DataPropertyName = "localIpAddress";
            this.localIpAddressDataGridViewTextBoxColumn.HeaderText = "ローカルIPアドレス";
            this.localIpAddressDataGridViewTextBoxColumn.Name = "localIpAddressDataGridViewTextBoxColumn";
            this.localIpAddressDataGridViewTextBoxColumn.ReadOnly = true;
            this.localIpAddressDataGridViewTextBoxColumn.Width = 130;
            // 
            // protocolNameDataGridViewTextBoxColumn
            // 
            this.protocolNameDataGridViewTextBoxColumn.DataPropertyName = "protocolName";
            this.protocolNameDataGridViewTextBoxColumn.HeaderText = "プロトコル";
            this.protocolNameDataGridViewTextBoxColumn.Name = "protocolNameDataGridViewTextBoxColumn";
            this.protocolNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.protocolNameDataGridViewTextBoxColumn.Width = 80;
            // 
            // portNoDataGridViewTextBoxColumn
            // 
            this.portNoDataGridViewTextBoxColumn.DataPropertyName = "portNo";
            this.portNoDataGridViewTextBoxColumn.HeaderText = "ポート番号";
            this.portNoDataGridViewTextBoxColumn.Name = "portNoDataGridViewTextBoxColumn";
            this.portNoDataGridViewTextBoxColumn.ReadOnly = true;
            this.portNoDataGridViewTextBoxColumn.Width = 80;
            // 
            // successCountDataGridViewTextBoxColumn
            // 
            this.successCountDataGridViewTextBoxColumn.DataPropertyName = "successCount";
            this.successCountDataGridViewTextBoxColumn.HeaderText = "成功数";
            this.successCountDataGridViewTextBoxColumn.Name = "successCountDataGridViewTextBoxColumn";
            this.successCountDataGridViewTextBoxColumn.ReadOnly = true;
            this.successCountDataGridViewTextBoxColumn.Width = 50;
            // 
            // failureCountDataGridViewTextBoxColumn
            // 
            this.failureCountDataGridViewTextBoxColumn.DataPropertyName = "failureCount";
            this.failureCountDataGridViewTextBoxColumn.HeaderText = "失敗数";
            this.failureCountDataGridViewTextBoxColumn.Name = "failureCountDataGridViewTextBoxColumn";
            this.failureCountDataGridViewTextBoxColumn.ReadOnly = true;
            this.failureCountDataGridViewTextBoxColumn.Width = 50;
            // 
            // serverBindingSource
            // 
            this.serverBindingSource.AllowNew = false;
            this.serverBindingSource.DataSource = typeof(TestConnection.TesterServer);
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.serverTab);
            this.mainTabControl.Controls.Add(this.clientTab);
            this.mainTabControl.Controls.Add(this.settingTab);
            this.mainTabControl.Controls.Add(this.resultTab);
            this.mainTabControl.Location = new System.Drawing.Point(4, 53);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(626, 326);
            this.mainTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.mainTabControl.TabIndex = 5;
            // 
            // clientTab
            // 
            this.clientTab.Controls.Add(this.label13);
            this.clientTab.Controls.Add(this.cportUpDown);
            this.clientTab.Controls.Add(this.label12);
            this.clientTab.Controls.Add(this.cprotoComboBox);
            this.clientTab.Controls.Add(this.cdelbtn);
            this.clientTab.Controls.Add(this.cAdelbtn);
            this.clientTab.Controls.Add(this.label14);
            this.clientTab.Controls.Add(this.label11);
            this.clientTab.Controls.Add(this.caddbtn);
            this.clientTab.Controls.Add(this.cripTextBox);
            this.clientTab.Controls.Add(this.clipTextBox);
            this.clientTab.Controls.Add(this.clientGridView);
            this.clientTab.Location = new System.Drawing.Point(4, 22);
            this.clientTab.Name = "clientTab";
            this.clientTab.Padding = new System.Windows.Forms.Padding(3);
            this.clientTab.Size = new System.Drawing.Size(618, 300);
            this.clientTab.TabIndex = 5;
            this.clientTab.Text = "クライアント";
            this.clientTab.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(354, 18);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(57, 12);
            this.label13.TabIndex = 111;
            this.label13.Text = "ポート番号";
            // 
            // cportUpDown
            // 
            this.cportUpDown.Location = new System.Drawing.Point(356, 34);
            this.cportUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.cportUpDown.Name = "cportUpDown";
            this.cportUpDown.Size = new System.Drawing.Size(85, 19);
            this.cportUpDown.TabIndex = 204;
            this.cportUpDown.Value = new decimal(new int[] {
            49152,
            0,
            0,
            0});
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(262, 18);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 12);
            this.label12.TabIndex = 111;
            this.label12.Text = "プロトコル";
            // 
            // cprotoComboBox
            // 
            this.cprotoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cprotoComboBox.FormattingEnabled = true;
            this.cprotoComboBox.Items.AddRange(new object[] {
            "TCP",
            "UDP",
            "DNS",
            "Ping"});
            this.cprotoComboBox.Location = new System.Drawing.Point(264, 33);
            this.cprotoComboBox.Name = "cprotoComboBox";
            this.cprotoComboBox.Size = new System.Drawing.Size(86, 20);
            this.cprotoComboBox.TabIndex = 203;
            // 
            // cdelbtn
            // 
            this.cdelbtn.Location = new System.Drawing.Point(532, 32);
            this.cdelbtn.Name = "cdelbtn";
            this.cdelbtn.Size = new System.Drawing.Size(80, 20);
            this.cdelbtn.TabIndex = 206;
            this.cdelbtn.Text = "削除";
            this.cdelbtn.UseVisualStyleBackColor = true;
            this.cdelbtn.Click += new System.EventHandler(this.cdelbtn_Click);
            // 
            // cAdelbtn
            // 
            this.cAdelbtn.Location = new System.Drawing.Point(532, 6);
            this.cAdelbtn.Name = "cAdelbtn";
            this.cAdelbtn.Size = new System.Drawing.Size(80, 20);
            this.cAdelbtn.TabIndex = 207;
            this.cAdelbtn.Text = "全削除";
            this.cAdelbtn.UseVisualStyleBackColor = true;
            this.cAdelbtn.Click += new System.EventHandler(this.cAdelbtn_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(149, 18);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(85, 12);
            this.label14.TabIndex = 111;
            this.label14.Text = "リモートIPアドレス";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 18);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(137, 12);
            this.label11.TabIndex = 111;
            this.label11.Text = "ローカルIPアドレス（省略可）";
            // 
            // caddbtn
            // 
            this.caddbtn.Location = new System.Drawing.Point(446, 32);
            this.caddbtn.Name = "caddbtn";
            this.caddbtn.Size = new System.Drawing.Size(80, 20);
            this.caddbtn.TabIndex = 205;
            this.caddbtn.Text = "追加";
            this.caddbtn.UseVisualStyleBackColor = true;
            this.caddbtn.Click += new System.EventHandler(this.caddbtn_Click);
            // 
            // cripTextBox
            // 
            this.cripTextBox.Location = new System.Drawing.Point(151, 33);
            this.cripTextBox.Name = "cripTextBox";
            this.cripTextBox.Size = new System.Drawing.Size(107, 19);
            this.cripTextBox.TabIndex = 202;
            // 
            // clipTextBox
            // 
            this.clipTextBox.Location = new System.Drawing.Point(8, 33);
            this.clipTextBox.Name = "clipTextBox";
            this.clipTextBox.Size = new System.Drawing.Size(137, 19);
            this.clipTextBox.TabIndex = 201;
            // 
            // clientGridView
            // 
            this.clientGridView.AllowUserToAddRows = false;
            this.clientGridView.AllowUserToDeleteRows = false;
            this.clientGridView.AllowUserToResizeRows = false;
            this.clientGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.clientGridView.AutoGenerateColumns = false;
            this.clientGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.clientGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.localIpAddressDataGridViewTextBoxColumn1,
            this.remoteIpAddressDataGridViewTextBoxColumn,
            this.protocolNameDataGridViewTextBoxColumn1,
            this.portNoDataGridViewTextBoxColumn1,
            this.successCount,
            this.failureCount});
            this.clientGridView.DataSource = this.clientBindingSource;
            this.clientGridView.Location = new System.Drawing.Point(0, 59);
            this.clientGridView.Name = "clientGridView";
            this.clientGridView.ReadOnly = true;
            this.clientGridView.RowTemplate.Height = 21;
            this.clientGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.clientGridView.Size = new System.Drawing.Size(618, 242);
            this.clientGridView.TabIndex = 210;
            // 
            // localIpAddressDataGridViewTextBoxColumn1
            // 
            this.localIpAddressDataGridViewTextBoxColumn1.DataPropertyName = "localIpAddress";
            this.localIpAddressDataGridViewTextBoxColumn1.HeaderText = "ローカルIPアドレス";
            this.localIpAddressDataGridViewTextBoxColumn1.Name = "localIpAddressDataGridViewTextBoxColumn1";
            this.localIpAddressDataGridViewTextBoxColumn1.ReadOnly = true;
            this.localIpAddressDataGridViewTextBoxColumn1.Width = 130;
            // 
            // remoteIpAddressDataGridViewTextBoxColumn
            // 
            this.remoteIpAddressDataGridViewTextBoxColumn.DataPropertyName = "remoteIpAddress";
            this.remoteIpAddressDataGridViewTextBoxColumn.HeaderText = "リモートIPアドレス";
            this.remoteIpAddressDataGridViewTextBoxColumn.Name = "remoteIpAddressDataGridViewTextBoxColumn";
            this.remoteIpAddressDataGridViewTextBoxColumn.ReadOnly = true;
            this.remoteIpAddressDataGridViewTextBoxColumn.Width = 130;
            // 
            // protocolNameDataGridViewTextBoxColumn1
            // 
            this.protocolNameDataGridViewTextBoxColumn1.DataPropertyName = "protocolName";
            this.protocolNameDataGridViewTextBoxColumn1.HeaderText = "プロトコル";
            this.protocolNameDataGridViewTextBoxColumn1.Name = "protocolNameDataGridViewTextBoxColumn1";
            this.protocolNameDataGridViewTextBoxColumn1.ReadOnly = true;
            this.protocolNameDataGridViewTextBoxColumn1.Width = 80;
            // 
            // portNoDataGridViewTextBoxColumn1
            // 
            this.portNoDataGridViewTextBoxColumn1.DataPropertyName = "portNo";
            this.portNoDataGridViewTextBoxColumn1.HeaderText = "ポート番号";
            this.portNoDataGridViewTextBoxColumn1.Name = "portNoDataGridViewTextBoxColumn1";
            this.portNoDataGridViewTextBoxColumn1.ReadOnly = true;
            this.portNoDataGridViewTextBoxColumn1.Width = 80;
            // 
            // successCount
            // 
            this.successCount.DataPropertyName = "successCount";
            this.successCount.HeaderText = "成功数";
            this.successCount.Name = "successCount";
            this.successCount.ReadOnly = true;
            this.successCount.Width = 50;
            // 
            // failureCount
            // 
            this.failureCount.DataPropertyName = "failureCount";
            this.failureCount.HeaderText = "失敗数";
            this.failureCount.Name = "failureCount";
            this.failureCount.ReadOnly = true;
            this.failureCount.Width = 50;
            // 
            // clientBindingSource
            // 
            this.clientBindingSource.DataSource = typeof(TestConnection.TesterClient);
            // 
            // resultUpdateTimer
            // 
            this.resultUpdateTimer.Tick += new System.EventHandler(this.resultUpdateTimer_Tick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "タイプ";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "ローカルIPアドレス";
            this.columnHeader2.Width = 140;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "リモートIPアドレス";
            this.columnHeader3.Width = 140;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "プロトコル";
            this.columnHeader4.Width = 80;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "ポート";
            this.columnHeader5.Width = 80;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip.Location = new System.Drawing.Point(0, 381);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(630, 23);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 403;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(44, 18);
            this.toolStripStatusLabel1.Text = "待機中";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルToolStripMenuItem,
            this.ツールToolStripMenuItem,
            this.ヘルプToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(630, 26);
            this.menuStrip.TabIndex = 404;
            this.menuStrip.Text = "menuStrip1";
            // 
            // ファイルToolStripMenuItem
            // 
            this.ファイルToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新規作成ToolStripMenuItem,
            this.開くToolStripMenuItem,
            this.toolStripSeparator,
            this.上書き保存ToolStripMenuItem,
            this.toolStripSeparator2,
            this.終了ToolStripMenuItem});
            this.ファイルToolStripMenuItem.Name = "ファイルToolStripMenuItem";
            this.ファイルToolStripMenuItem.Size = new System.Drawing.Size(68, 22);
            this.ファイルToolStripMenuItem.Text = "ファイル";
            // 
            // 新規作成ToolStripMenuItem
            // 
            this.新規作成ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("新規作成ToolStripMenuItem.Image")));
            this.新規作成ToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.新規作成ToolStripMenuItem.Name = "新規作成ToolStripMenuItem";
            this.新規作成ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.新規作成ToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.新規作成ToolStripMenuItem.Text = "新規作成";
            this.新規作成ToolStripMenuItem.Click += new System.EventHandler(this.新規作成ToolStripMenuItem_Click);
            // 
            // 開くToolStripMenuItem
            // 
            this.開くToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("開くToolStripMenuItem.Image")));
            this.開くToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.開くToolStripMenuItem.Name = "開くToolStripMenuItem";
            this.開くToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.開くToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.開くToolStripMenuItem.Text = "開く";
            this.開くToolStripMenuItem.Click += new System.EventHandler(this.開くToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(216, 6);
            // 
            // 上書き保存ToolStripMenuItem
            // 
            this.上書き保存ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("上書き保存ToolStripMenuItem.Image")));
            this.上書き保存ToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.上書き保存ToolStripMenuItem.Name = "上書き保存ToolStripMenuItem";
            this.上書き保存ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.上書き保存ToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.上書き保存ToolStripMenuItem.Text = "名前を付けて保存";
            this.上書き保存ToolStripMenuItem.Click += new System.EventHandler(this.名前を付けて保存ToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(216, 6);
            // 
            // 終了ToolStripMenuItem
            // 
            this.終了ToolStripMenuItem.Name = "終了ToolStripMenuItem";
            this.終了ToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.終了ToolStripMenuItem.Text = "終了";
            this.終了ToolStripMenuItem.Click += new System.EventHandler(this.終了ToolStripMenuItem_Click);
            // 
            // ツールToolStripMenuItem
            // 
            this.ツールToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.統計クリアToolStripMenuItem,
            this.統計更新ToolStripMenuItem});
            this.ツールToolStripMenuItem.Name = "ツールToolStripMenuItem";
            this.ツールToolStripMenuItem.Size = new System.Drawing.Size(56, 22);
            this.ツールToolStripMenuItem.Text = "ツール";
            // 
            // 統計クリアToolStripMenuItem
            // 
            this.統計クリアToolStripMenuItem.Name = "統計クリアToolStripMenuItem";
            this.統計クリアToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.統計クリアToolStripMenuItem.Text = "統計クリア";
            this.統計クリアToolStripMenuItem.Click += new System.EventHandler(this.統計クリアToolStripMenuItem_Click);
            // 
            // 統計更新ToolStripMenuItem
            // 
            this.統計更新ToolStripMenuItem.Name = "統計更新ToolStripMenuItem";
            this.統計更新ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.統計更新ToolStripMenuItem.Text = "統計更新";
            this.統計更新ToolStripMenuItem.Click += new System.EventHandler(this.統計更新ToolStripMenuItem_Click);
            // 
            // ヘルプToolStripMenuItem
            // 
            this.ヘルプToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.バージョン情報ToolStripMenuItem});
            this.ヘルプToolStripMenuItem.Name = "ヘルプToolStripMenuItem";
            this.ヘルプToolStripMenuItem.Size = new System.Drawing.Size(56, 22);
            this.ヘルプToolStripMenuItem.Text = "ヘルプ";
            // 
            // バージョン情報ToolStripMenuItem
            // 
            this.バージョン情報ToolStripMenuItem.Name = "バージョン情報ToolStripMenuItem";
            this.バージョン情報ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.バージョン情報ToolStripMenuItem.Text = "バージョン情報...";
            this.バージョン情報ToolStripMenuItem.Click += new System.EventHandler(this.バージョン情報ToolStripMenuItem_Click);
            // 
            // nicSetButton
            // 
            this.nicSetButton.Location = new System.Drawing.Point(223, 27);
            this.nicSetButton.Name = "nicSetButton";
            this.nicSetButton.Size = new System.Drawing.Size(80, 20);
            this.nicSetButton.TabIndex = 3;
            this.nicSetButton.Text = "NIC設定";
            this.nicSetButton.UseVisualStyleBackColor = true;
            this.nicSetButton.Click += new System.EventHandler(this.nicSet_Click);
            // 
            // nicResButton
            // 
            this.nicResButton.Location = new System.Drawing.Point(309, 27);
            this.nicResButton.Name = "nicResButton";
            this.nicResButton.Size = new System.Drawing.Size(80, 20);
            this.nicResButton.TabIndex = 4;
            this.nicResButton.Text = "NIC戻し";
            this.nicResButton.UseVisualStyleBackColor = true;
            this.nicResButton.Click += new System.EventHandler(this.nicRes_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.startButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.stopButton;
            this.ClientSize = new System.Drawing.Size(630, 404);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.nicResButton);
            this.Controls.Add(this.nicSetButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "TestConnection";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.closingclean);
            this.resultTab.ResumeLayout(false);
            this.resultTab.PerformLayout();
            this.settingTab.ResumeLayout(false);
            this.settingTab.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.repeatCountUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repeatIntervalUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testIntervalUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultUpdateIntervalUpDown)).EndInit();
            this.serverTab.ResumeLayout(false);
            this.serverTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sportUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverBindingSource)).EndInit();
            this.mainTabControl.ResumeLayout(false);
            this.clientTab.ResumeLayout(false);
            this.clientTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cportUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.clientGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.clientBindingSource)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabPage resultTab;
        private System.Windows.Forms.TabPage settingTab;
        private System.Windows.Forms.TabPage serverTab;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.ComboBox nicComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button saveResultButton;
        private System.Windows.Forms.TextBox resultText;
        private System.Windows.Forms.Button clearResultButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown testIntervalUpDown;
        private System.Windows.Forms.NumericUpDown resultUpdateIntervalUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer resultUpdateTimer;
        private System.Windows.Forms.TextBox subnetTextBox;
        private System.Windows.Forms.NumericUpDown repeatIntervalUpDown;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown timeoutUpDown;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown repeatCountUpDown;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TabPage clientTab;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.DataGridView serverGridView;
        private System.Windows.Forms.DataGridView clientGridView;
        private System.Windows.Forms.BindingSource clientBindingSource;
        private System.Windows.Forms.BindingSource serverBindingSource;
        private System.Windows.Forms.Button sdelbtn;
        private System.Windows.Forms.Button saddbtn;
        private System.Windows.Forms.TextBox slipTextBox;
        private System.Windows.Forms.ComboBox sprotoComboBox;
        private System.Windows.Forms.NumericUpDown sportUpDown;
        private System.Windows.Forms.NumericUpDown cportUpDown;
        private System.Windows.Forms.Button cdelbtn;
        private System.Windows.Forms.Button caddbtn;
        private System.Windows.Forms.TextBox clipTextBox;
        private System.Windows.Forms.TextBox cripTextBox;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem ファイルToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 新規作成ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 開くToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem 上書き保存ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem 終了ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ツールToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ヘルプToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem バージョン情報ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 統計クリアToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 統計更新ToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn localIpAddressDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn remoteIpAddressDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn protocolNameDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn portNoDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn successCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn failureCount;
        private System.Windows.Forms.ComboBox cprotoComboBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DataGridViewTextBoxColumn localIpAddressDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn protocolNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn portNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn successCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn failureCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button sAdelbtn;
        private System.Windows.Forms.Button cAdelbtn;
        private System.Windows.Forms.TextBox defgwTextBox;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button saveSettingButton;
        private System.Windows.Forms.Button loadDefaultButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.TextBox logFileNameTextBox;
        private System.Windows.Forms.Button logFileNameSelectButton;
        private System.Windows.Forms.CheckBox soundBox;
        private System.Windows.Forms.Button nicSetButton;
        private System.Windows.Forms.Button nicResButton;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
    }
}

