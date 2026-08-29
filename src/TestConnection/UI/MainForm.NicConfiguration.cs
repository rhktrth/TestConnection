using System;
using System.Collections.Generic;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace TestConnection {
    public partial class MainForm {
        private readonly Dictionary<string, NicConfigurationSnapshot> nicSnapshots =
            new Dictionary<string, NicConfigurationSnapshot>(StringComparer.OrdinalIgnoreCase);

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            groupBox1.Text = "結果ログ";
            updateNicButtons();
        }

        private void nicSet_Click(object sender, EventArgs e) {
            if (nicComboBox.SelectedIndex < 0 || nicComboBox.SelectedIndex >= orgNicList.Count) {
                MessageBox.Show("設定できるNICがありません。");
                updateNicButtons();
                return;
            }

            if (!isIpv4Address(subnetTextBox.Text)) {
                MessageBox.Show("IPv4サブネットマスクが無効です");
                return;
            }
            if (!isIpv4Address(defgwTextBox.Text)) {
                MessageBox.Show("IPv4デフォルトゲートウェイが無効です");
                return;
            }

            List<string> ipList = new List<string>();
            List<string> subnetList = new List<string>();
            foreach (TesterServer tester in serverBindingSource.List) {
                addNicAddress(tester.LocalIpAddress, ipList, subnetList);
            }
            foreach (TesterClient tester in clientBindingSource.List) {
                addNicAddress(tester.LocalIpAddress, ipList, subnetList);
            }

            if (ipList.Count == 0) {
                MessageBox.Show("NICへ設定するローカルIPv4アドレスがありません。");
                return;
            }

            ManagementObject targetNic = orgNicList[nicComboBox.SelectedIndex];
            string path = targetNic.Path.Path;

            NicConfigurationSnapshot snapshot;
            if (!nicSnapshots.TryGetValue(path, out snapshot)) {
                snapshot = NicConfigurationService.Capture(targetNic);
                nicSnapshots.Add(path, snapshot);
            }

            try {
                NicConfigurationService.ApplyStatic(
                    targetNic, ipList.ToArray(), subnetList.ToArray(), defgwTextBox.Text);
            } catch (Exception ex) {
                string message = "NIC設定に失敗しました。" + CRLF + ex.Message;
                try {
                    NicConfigurationService.Restore(snapshot);
                    nicSnapshots.Remove(path);
                    message += CRLF + "変更前の設定へ復元しました。";
                } catch (Exception restoreEx) {
                    message += CRLF + "自動復元にも失敗しました。" + CRLF + restoreEx.Message;
                }
                MessageBox.Show(message);
            }

            updateNicButtons();
        }

        private void nicRes_Click(object sender, EventArgs e) {
            restoreNicSettings(true);
            updateNicButtons();
        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }

        private void closingclean(object sender, FormClosingEventArgs e) {
            stop_Click(sender, e);
            if (!restoreNicSettings(true)) {
                e.Cancel = true;
            }
        }

        private void updateNicButtons() {
            bool hasNic = orgNicList.Count != 0;
            nicComboBox.Enabled = hasNic;

            if (testSession.IsRunning) {
                nicSetButton.Enabled = false;
                nicResButton.Enabled = false;
                return;
            }

            nicSetButton.Enabled = hasNic;
            nicResButton.Enabled = hasNic && nicSnapshots.Count != 0;
        }

        private void addNicAddress(string address, List<string> ipList, List<string> subnetList) {
            if (string.IsNullOrEmpty(address)) {
                return;
            }

            IPAddress parsed = IPAddress.Parse(address);
            if (parsed.AddressFamily != AddressFamily.InterNetwork || IPAddress.IsLoopback(parsed)) {
                return;
            }
            if (ipList.Contains(address)) {
                return;
            }

            ipList.Add(address);
            subnetList.Add(subnetTextBox.Text);
        }

        private static bool isIpv4Address(string value) {
            IPAddress address;
            return IPAddress.TryParse(value, out address) &&
                address.AddressFamily == AddressFamily.InterNetwork;
        }

        private bool restoreNicSettings(bool showError) {
            List<NicConfigurationSnapshot> snapshots = new List<NicConfigurationSnapshot>(nicSnapshots.Values);
            List<string> errors = new List<string>();

            foreach (NicConfigurationSnapshot snapshot in snapshots) {
                try {
                    NicConfigurationService.Restore(snapshot);
                    nicSnapshots.Remove(snapshot.Path);
                } catch (Exception ex) {
                    errors.Add(snapshot.Description + ": " + ex.Message);
                }
            }

            if (showError && errors.Count != 0) {
                MessageBox.Show("NIC設定を完全に復元できませんでした。" + CRLF + string.Join(CRLF, errors.ToArray()));
            }

            return errors.Count == 0;
        }
    }
}
