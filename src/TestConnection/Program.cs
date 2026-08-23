using System;
using System.Windows.Forms;

namespace TestConnection {

    static class Program {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {
            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception ex) {
                MessageBox.Show(
                    "TestConnection の起動に失敗しました。\r\n\r\n" + ex,
                    "TestConnection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }

}
