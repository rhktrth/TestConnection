using System;
using System.Management;

namespace TestConnection {
    public partial class MainForm {
        private bool nicSelectionInitialized;

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);

            if (nicSelectionInitialized) {
                return;
            }

            nicSelectionInitialized = true;
            restoreSelectedNic();
            nicComboBox.SelectedIndexChanged += nicComboBox_SelectedIndexChanged;
        }

        private void restoreSelectedNic() {
            if (orgNicList.Count == 0) {
                return;
            }

            string savedSettingId = Properties.Settings.Default.selectedNicSettingId;
            if (!string.IsNullOrEmpty(savedSettingId)) {
                for (int i = 0; i < orgNicList.Count; i++) {
                    string settingId = getNicSettingId(orgNicList[i]);
                    if (string.Equals(settingId, savedSettingId, StringComparison.OrdinalIgnoreCase)) {
                        nicComboBox.SelectedIndex = i;
                        return;
                    }
                }
            }

            nicComboBox.SelectedIndex = 0;
        }

        private void nicComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            int selectedIndex = nicComboBox.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= orgNicList.Count) {
                return;
            }

            string settingId = getNicSettingId(orgNicList[selectedIndex]);
            if (string.IsNullOrEmpty(settingId)) {
                return;
            }

            persistSelectedNicSettingId(settingId);
        }

        private static void persistSelectedNicSettingId(string settingId) {
            Properties.Settings settings = Properties.Settings.Default;
            settings.selectedNicSettingId = settingId;

            // NIC選択だけを即時保存し、画面上で編集中の他設定まで暗黙に保存しない。
            System.Configuration.SettingsProperty property = settings.Properties["selectedNicSettingId"];
            System.Configuration.SettingsPropertyValue value = new System.Configuration.SettingsPropertyValue(property);
            value.PropertyValue = settingId;
            value.IsDirty = true;

            System.Configuration.SettingsPropertyValueCollection values =
                new System.Configuration.SettingsPropertyValueCollection();
            values.Add(value);
            property.Provider.SetPropertyValues(settings.Context, values);
        }

        private static string getNicSettingId(ManagementObject nic) {
            if (nic == null) {
                return string.Empty;
            }

            return Convert.ToString(nic["SettingID"]);
        }
    }
}
