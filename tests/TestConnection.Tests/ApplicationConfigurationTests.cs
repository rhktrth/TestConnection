using System;
using System.IO;
using System.Xml;

namespace TestConnection.Tests {
    internal static class ApplicationConfigurationTests {
        public static void TestWinFormsApplicationConfiguration() {
            string configPath = findRepositoryFile("src", "TestConnection", "app.config");
            XmlDocument document = new XmlDocument();
            document.Load(configPath);

            XmlNode forbiddenDeclaration = document.SelectSingleNode(
                "/configuration/configSections/section[@name='System.Windows.Forms.ApplicationConfigurationSection']");
            TestAssert.True(
                forbiddenDeclaration == null,
                "WinForms ApplicationConfigurationSectionは組み込みセクションなのでconfigSectionsで宣言しない");

            XmlNode dpiAwareness = document.SelectSingleNode(
                "/configuration/System.Windows.Forms.ApplicationConfigurationSection/add[@key='DpiAwareness']");
            TestAssert.True(dpiAwareness != null, "DpiAwareness設定が存在する");
            TestAssert.Equal("PerMonitorV2", dpiAwareness.Attributes["value"].Value, "DpiAwarenessをPerMonitorV2にする");
        }

        private static string findRepositoryFile(params string[] relativePath) {
            string path = findFromDirectory(Directory.GetCurrentDirectory(), relativePath);
            if (path != null) {
                return path;
            }

            path = findFromDirectory(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            if (path != null) {
                return path;
            }

            throw new FileNotFoundException("repositoryのapp.configを見つけられませんでした。");
        }

        private static string findFromDirectory(string startDirectory, string[] relativePath) {
            DirectoryInfo directory = new DirectoryInfo(startDirectory);
            while (directory != null) {
                string candidate = directory.FullName;
                foreach (string part in relativePath) {
                    candidate = Path.Combine(candidate, part);
                }
                if (File.Exists(candidate)) {
                    return candidate;
                }
                directory = directory.Parent;
            }
            return null;
        }
    }
}
