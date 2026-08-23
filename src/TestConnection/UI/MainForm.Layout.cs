using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestConnection {
    public partial class MainForm {
        private bool layoutInitialized;

        protected override void OnCreateControl() {
            base.OnCreateControl();
            initializeLayout();
        }

        private void initializeLayout() {
            if (layoutInitialized) {
                return;
            }

            SuspendLayout();

            Font = SystemFonts.MessageBoxFont;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimumSize = new Size(640, 440);
            ClientSize = new Size(900, 560);
            StartPosition = FormStartPosition.CenterScreen;

            rebuildMainLayout();
            rebuildServerTab();
            rebuildClientTab();
            rebuildSettingsTab();
            rebuildResultTab();
            configureDataGrids();
            configureTabControl();

            layoutInitialized = true;
            ResumeLayout(true);
        }

        private void rebuildMainLayout() {
            Controls.Clear();

            menuStrip.Padding = new Padding(8, 2, 0, 2);
            menuStrip.Dock = DockStyle.Fill;
            statusStrip.Dock = DockStyle.Fill;

            configureMainActionButton(startButton, 104);
            configureMainActionButton(stopButton, 88);
            configureMainActionButton(nicSetButton, 88);
            configureMainActionButton(nicResButton, 88);
            stopButton.Text = "停止";
            nicResButton.Text = "NIC復元";

            FlowLayoutPanel actionBar = new FlowLayoutPanel();
            actionBar.AutoSize = true;
            actionBar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            actionBar.Dock = DockStyle.Fill;
            actionBar.FlowDirection = FlowDirection.LeftToRight;
            actionBar.Padding = new Padding(12, 10, 12, 8);
            actionBar.WrapContents = true;
            actionBar.Controls.Add(startButton);
            actionBar.Controls.Add(stopButton);
            actionBar.Controls.Add(nicSetButton);
            actionBar.Controls.Add(nicResButton);

            mainTabControl.Dock = DockStyle.Fill;
            mainTabControl.Margin = new Padding(12, 0, 12, 0);

            TableLayoutPanel root = new TableLayoutPanel();
            root.ColumnCount = 1;
            root.RowCount = 4;
            root.Dock = DockStyle.Fill;
            root.Margin = new Padding(0);
            root.Padding = new Padding(0);
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.Controls.Add(menuStrip, 0, 0);
            root.Controls.Add(actionBar, 0, 1);
            root.Controls.Add(mainTabControl, 0, 2);
            root.Controls.Add(statusStrip, 0, 3);

            Controls.Add(root);
            MainMenuStrip = menuStrip;
        }

        private void rebuildServerTab() {
            serverTab.Controls.Clear();
            serverTab.Padding = new Padding(0);

            label1.Text = "ローカルIP（省略可）";
            label9.Text = "プロトコル";
            label10.Text = "ポート";

            FlowLayoutPanel inputRow = createInputRow();
            inputRow.Controls.Add(createLabeledField(label1, slipTextBox, 200));
            inputRow.Controls.Add(createLabeledField(label9, sprotoComboBox, 100));
            inputRow.Controls.Add(createLabeledField(label10, sportUpDown, 100));
            configureActionButton(saddbtn, "追加", 88);
            saddbtn.Margin = new Padding(8, 21, 8, 4);
            inputRow.Controls.Add(saddbtn);

            FlowLayoutPanel actions = createRightAlignedActionRow();
            configureActionButton(sdelbtn, "選択を削除", 104);
            configureActionButton(sAdelbtn, "すべて削除", 96);
            actions.Controls.Add(sdelbtn);
            actions.Controls.Add(sAdelbtn);

            serverGridView.Dock = DockStyle.Fill;
            serverGridView.Margin = new Padding(12, 4, 12, 12);

            TableLayoutPanel layout = createTabLayout();
            layout.Controls.Add(inputRow, 0, 0);
            layout.Controls.Add(actions, 0, 1);
            layout.Controls.Add(serverGridView, 0, 2);
            serverTab.Controls.Add(layout);
        }

        private void rebuildClientTab() {
            clientTab.Controls.Clear();
            clientTab.Padding = new Padding(0);

            label11.Text = "ローカルIP（省略可）";
            label14.Text = "リモート（IP / ホスト名）";
            label12.Text = "プロトコル";
            label13.Text = "ポート";

            FlowLayoutPanel inputRow = createInputRow();
            inputRow.Controls.Add(createLabeledField(label11, clipTextBox, 180));
            inputRow.Controls.Add(createLabeledField(label14, cripTextBox, 220));
            inputRow.Controls.Add(createLabeledField(label12, cprotoComboBox, 100));
            inputRow.Controls.Add(createLabeledField(label13, cportUpDown, 100));
            configureActionButton(caddbtn, "追加", 88);
            caddbtn.Margin = new Padding(8, 21, 8, 4);
            inputRow.Controls.Add(caddbtn);

            FlowLayoutPanel actions = createRightAlignedActionRow();
            configureActionButton(cdelbtn, "選択を削除", 104);
            configureActionButton(cAdelbtn, "すべて削除", 96);
            actions.Controls.Add(cdelbtn);
            actions.Controls.Add(cAdelbtn);

            clientGridView.Dock = DockStyle.Fill;
            clientGridView.Margin = new Padding(12, 4, 12, 12);

            TableLayoutPanel layout = createTabLayout();
            layout.Controls.Add(inputRow, 0, 0);
            layout.Controls.Add(actions, 0, 1);
            layout.Controls.Add(clientGridView, 0, 2);
            clientTab.Controls.Add(layout);
        }

        private void rebuildSettingsTab() {
            settingTab.Controls.Clear();
            settingTab.Padding = new Padding(0);

            configureActionButton(saveSettingButton, "設定を保存", 104);
            configureActionButton(loadDefaultButton, "デフォルト", 96);

            FlowLayoutPanel settingActions = new FlowLayoutPanel();
            settingActions.AutoSize = true;
            settingActions.Dock = DockStyle.Fill;
            settingActions.FlowDirection = FlowDirection.LeftToRight;
            settingActions.WrapContents = true;
            settingActions.Margin = new Padding(0, 0, 0, 8);
            settingActions.Controls.Add(saveSettingButton);
            settingActions.Controls.Add(loadDefaultButton);

            GroupBox testGroup = new GroupBox();
            testGroup.Text = "試験";
            testGroup.Dock = DockStyle.Fill;
            testGroup.AutoSize = true;
            testGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            testGroup.Padding = new Padding(12, 8, 12, 12);

            TableLayoutPanel testSettings = createTwoColumnSettingsTable();
            label8.Text = "繰り返し回数 (0 = 無限)";
            label4.Text = "テスト実行間隔 (ms)";
            label6.Text = "繰り返し間隔 (ms)";
            label7.Text = "タイムアウト (ms)";
            label3.Text = "統計更新間隔 (ms)";
            addSettingRow(testSettings, label8, repeatCountUpDown, 0);
            addSettingRow(testSettings, label4, testIntervalUpDown, 1);
            addSettingRow(testSettings, label6, repeatIntervalUpDown, 2);
            addSettingRow(testSettings, label7, timeoutUpDown, 3);
            addSettingRow(testSettings, label3, resultUpdateIntervalUpDown, 4);
            soundBox.Margin = new Padding(0, 8, 0, 0);
            testSettings.Controls.Add(soundBox, 0, 5);
            testSettings.SetColumnSpan(soundBox, 2);
            testGroup.Controls.Add(testSettings);

            rebuildLogGroup();

            GroupBox nicGroup = new GroupBox();
            nicGroup.Text = "NIC一時設定（設定保存されません）";
            nicGroup.Dock = DockStyle.Fill;
            nicGroup.AutoSize = true;
            nicGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            nicGroup.Padding = new Padding(12, 8, 12, 12);

            TableLayoutPanel nicSettings = createTwoColumnSettingsTable();
            label2.Text = "NIC";
            label5.Text = "サブネットマスク";
            label15.Text = "デフォルトゲートウェイ";
            addSettingRow(nicSettings, label2, nicComboBox, 0);
            addSettingRow(nicSettings, label5, subnetTextBox, 1);
            addSettingRow(nicSettings, label15, defgwTextBox, 2);
            nicComboBox.Dock = DockStyle.Fill;
            nicGroup.Controls.Add(nicSettings);

            TableLayoutPanel stack = new TableLayoutPanel();
            stack.AutoSize = true;
            stack.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            stack.ColumnCount = 1;
            stack.RowCount = 4;
            stack.Dock = DockStyle.Top;
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.Controls.Add(settingActions, 0, 0);
            stack.Controls.Add(testGroup, 0, 1);
            stack.Controls.Add(groupBox1, 0, 2);
            stack.Controls.Add(nicGroup, 0, 3);

            Panel scrollPanel = new Panel();
            scrollPanel.Dock = DockStyle.Fill;
            scrollPanel.AutoScroll = true;
            scrollPanel.Padding = new Padding(12);
            scrollPanel.Controls.Add(stack);

            settingTab.Controls.Add(scrollPanel);
        }

        private void rebuildLogGroup() {
            groupBox1.Controls.Clear();
            groupBox1.Text = "結果ログ（設定保存されません）";
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.AutoSize = true;
            groupBox1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox1.Padding = new Padding(12, 8, 12, 12);
            groupBox1.Margin = new Padding(0, 10, 0, 0);

            configureActionButton(logFileNameSelectButton, "参照...", 88);
            logFileNameTextBox.Dock = DockStyle.Fill;

            TableLayoutPanel fileRow = new TableLayoutPanel();
            fileRow.AutoSize = true;
            fileRow.ColumnCount = 2;
            fileRow.RowCount = 1;
            fileRow.Dock = DockStyle.Fill;
            fileRow.Margin = new Padding(24, 2, 0, 2);
            fileRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            fileRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            fileRow.Controls.Add(logFileNameTextBox, 0, 0);
            fileRow.Controls.Add(logFileNameSelectButton, 1, 0);

            TableLayoutPanel logSettings = new TableLayoutPanel();
            logSettings.AutoSize = true;
            logSettings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            logSettings.ColumnCount = 1;
            logSettings.RowCount = 4;
            logSettings.Dock = DockStyle.Top;
            logSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            logSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            logSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            logSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            radioButton1.Margin = new Padding(0, 4, 0, 4);
            radioButton2.Margin = new Padding(0, 4, 0, 0);
            radioButton3.Margin = new Padding(0, 4, 0, 4);
            logSettings.Controls.Add(radioButton1, 0, 0);
            logSettings.Controls.Add(radioButton2, 0, 1);
            logSettings.Controls.Add(fileRow, 0, 2);
            logSettings.Controls.Add(radioButton3, 0, 3);
            groupBox1.Controls.Add(logSettings);
        }

        private void rebuildResultTab() {
            resultTab.Controls.Clear();
            resultTab.Padding = new Padding(0);

            configureActionButton(saveResultButton, "結果を保存", 104);
            configureActionButton(clearResultButton, "クリア", 88);

            FlowLayoutPanel actions = new FlowLayoutPanel();
            actions.AutoSize = true;
            actions.Dock = DockStyle.Fill;
            actions.FlowDirection = FlowDirection.LeftToRight;
            actions.WrapContents = true;
            actions.Controls.Add(saveResultButton);
            actions.Controls.Add(clearResultButton);

            resultText.Dock = DockStyle.Fill;
            resultText.Margin = new Padding(0, 8, 0, 0);
            resultText.BorderStyle = BorderStyle.FixedSingle;
            resultText.WordWrap = false;

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.ColumnCount = 1;
            layout.RowCount = 2;
            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(12);
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.Controls.Add(actions, 0, 0);
            layout.Controls.Add(resultText, 0, 1);
            resultTab.Controls.Add(layout);
        }

        private static TableLayoutPanel createTabLayout() {
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.ColumnCount = 1;
            layout.RowCount = 3;
            layout.Dock = DockStyle.Fill;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            return layout;
        }

        private static FlowLayoutPanel createInputRow() {
            FlowLayoutPanel row = new FlowLayoutPanel();
            row.AutoSize = true;
            row.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            row.Dock = DockStyle.Fill;
            row.FlowDirection = FlowDirection.LeftToRight;
            row.Padding = new Padding(12, 10, 12, 2);
            row.WrapContents = true;
            return row;
        }

        private static FlowLayoutPanel createRightAlignedActionRow() {
            FlowLayoutPanel row = new FlowLayoutPanel();
            row.AutoSize = true;
            row.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            row.Dock = DockStyle.Fill;
            row.FlowDirection = FlowDirection.RightToLeft;
            row.Padding = new Padding(12, 0, 12, 4);
            row.WrapContents = false;
            return row;
        }

        private static Control createLabeledField(Label label, Control input, int width) {
            label.AutoSize = true;
            label.Margin = new Padding(0);
            input.Width = width;
            input.Margin = new Padding(0, 4, 0, 0);

            FlowLayoutPanel field = new FlowLayoutPanel();
            field.AutoSize = true;
            field.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            field.FlowDirection = FlowDirection.TopDown;
            field.Margin = new Padding(0, 0, 10, 4);
            field.WrapContents = false;
            field.Controls.Add(label);
            field.Controls.Add(input);
            return field;
        }

        private static TableLayoutPanel createTwoColumnSettingsTable() {
            TableLayoutPanel table = new TableLayoutPanel();
            table.AutoSize = true;
            table.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            table.ColumnCount = 2;
            table.Dock = DockStyle.Top;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            return table;
        }

        private static void addSettingRow(TableLayoutPanel table, Label label, Control input, int row) {
            label.AutoSize = true;
            label.Anchor = AnchorStyles.Left;
            label.Margin = new Padding(0, 5, 18, 5);
            input.Anchor = AnchorStyles.Left;
            input.Margin = new Padding(0, 4, 0, 4);
            if (!(input is ComboBox)) {
                input.Width = Math.Max(input.Width, 140);
            }
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.Controls.Add(label, 0, row);
            table.Controls.Add(input, 1, row);
        }

        private static void configureMainActionButton(Button button, int width) {
            configureActionButton(button, button.Text, width);
            button.Height = 34;
            button.Margin = new Padding(0, 0, 8, 0);
        }

        private static void configureActionButton(Button button, string text, int width) {
            button.Text = text;
            button.AutoSize = false;
            button.Width = width;
            button.Height = 30;
            button.Margin = new Padding(0, 0, 8, 4);
            button.FlatStyle = FlatStyle.System;
            button.UseVisualStyleBackColor = true;
        }

        private void configureDataGrids() {
            configureDataGrid(serverGridView);
            configureDataGrid(clientGridView);

            serverGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            localIpAddressDataGridViewTextBoxColumn.FillWeight = 38F;
            protocolNameDataGridViewTextBoxColumn.FillWeight = 18F;
            portNoDataGridViewTextBoxColumn.FillWeight = 16F;
            successCountDataGridViewTextBoxColumn.FillWeight = 14F;
            failureCountDataGridViewTextBoxColumn.FillWeight = 14F;

            clientGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            localIpAddressDataGridViewTextBoxColumn1.FillWeight = 25F;
            remoteIpAddressDataGridViewTextBoxColumn.FillWeight = 31F;
            protocolNameDataGridViewTextBoxColumn1.FillWeight = 15F;
            portNoDataGridViewTextBoxColumn1.FillWeight = 11F;
            successCount.FillWeight = 9F;
            failureCount.FillWeight = 9F;
        }

        private static void configureDataGrid(DataGridView grid) {
            grid.AllowUserToResizeRows = false;
            grid.BackgroundColor = SystemColors.Window;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersHeight = 34;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.EnableHeadersVisualStyles = true;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;
            grid.RowTemplate.Height = 30;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void configureTabControl() {
            mainTabControl.DrawMode = TabDrawMode.Normal;
            mainTabControl.ItemSize = new Size(116, 30);
            mainTabControl.SizeMode = TabSizeMode.Fixed;
        }
    }
}
