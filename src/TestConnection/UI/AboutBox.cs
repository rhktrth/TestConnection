using System.Reflection;
using System.Windows.Forms;

namespace TestConnection {
    partial class AboutBox : Form {
        public AboutBox() {
            InitializeComponent();
            Text = string.Format("{0} のバージョン情報", AssemblyTitle);
            productNameLabel.Text = AssemblyProduct;
            versionLabel.Text = string.Format("バージョン {0}", AssemblyVersion);
            descriptionTextBox.Text = AssemblyDescription;
        }

        private string AssemblyTitle {
            get {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0) {
                    AssemblyTitleAttribute title = (AssemblyTitleAttribute)attributes[0];
                    if (!string.IsNullOrEmpty(title.Title)) {
                        return title.Title;
                    }
                }
                return Assembly.GetExecutingAssembly().GetName().Name;
            }
        }

        private string AssemblyVersion {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        private string AssemblyDescription {
            get {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        private string AssemblyProduct {
            get {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }
    }
}
