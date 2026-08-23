using System;
using System.Collections.Generic;
using System.IO;
using TestConnection;

namespace TestConnection.Tests {
    internal static class ConfigurationTests {
        public static void TestTesterDefinitionFormat() {
            TesterDefinition client = LoadOne("Client,192.0.2.10,198.51.100.20,TCP,443");
            TestAssert.Equal(TesterRole.Client, client.Role, "client role");
            TestAssert.Equal("192.0.2.10", client.LocalIpAddress, "client local address");
            TestAssert.Equal("198.51.100.20", client.RemoteIpAddress, "client remote address");
            TestAssert.Equal(ProtocolName.TCP, client.Protocol, "client protocol");
            TestAssert.Equal(443, client.Port, "client port");
            TestAssert.Equal("Client,192.0.2.10,198.51.100.20,TCP,443" + Environment.NewLine,
                SaveOne(client), "client serialize");

            TesterDefinition server = LoadOne("Server,192.0.2.10,,UDP,53");
            TestAssert.Equal(TesterRole.Server, server.Role, "server role");
            TestAssert.Equal(ProtocolName.UDP, server.Protocol, "server protocol");
            TestAssert.Equal(string.Empty, server.RemoteIpAddress, "server empty remote address");
            TestAssert.Equal("Server,192.0.2.10,,UDP,53" + Environment.NewLine,
                SaveOne(server), "server serialize");

            TesterDefinition extraColumn = LoadOne("Client,,203.0.113.1,Ping,0,legacy-extra");
            TestAssert.Equal(ProtocolName.Ping, extraColumn.Protocol, "legacy extra column protocol");
            TestAssert.Equal(0, extraColumn.Port, "legacy extra column port");

            TesterDefinition hostname = LoadOne("Client,,service.example.test,TCP,443");
            TestAssert.Equal("service.example.test", hostname.RemoteIpAddress, "hostname remote endpoint");
            TestAssert.Equal("Client,,service.example.test,TCP,443" + Environment.NewLine,
                SaveOne(hostname), "hostname remains configured value");

            TestAssert.Throws<FormatException>(delegate {
                LoadOne("Client,192.0.2.10,198.51.100.20,TCP");
            }, "missing column");
            TestAssert.Throws<FormatException>(delegate {
                LoadOne("Client,192.0.2.10,198.51.100.20,TCP,not-a-number");
            }, "invalid port");
            TestAssert.Throws<FormatException>(delegate {
                LoadOne("Unknown,192.0.2.10,198.51.100.20,TCP,443");
            }, "invalid role");
            TestAssert.Throws<FormatException>(delegate {
                LoadOne("Client,192.0.2.10,198.51.100.20,Unknown,443");
            }, "invalid protocol");
        }

        public static void TestTesterDefinitionFile() {
            string source = "# comment\n" +
                "Client,192.0.2.10,198.51.100.20,TCP,443\n" +
                "Server,192.0.2.11,,UDP,53\n";
            List<TesterDefinition> definitions = TesterDefinitionFile.Load(new StringReader(source));
            TestAssert.Equal(2, definitions.Count, "definition count");
            TestAssert.Equal(TesterRole.Client, definitions[0].Role, "first role");
            TestAssert.Equal(TesterRole.Server, definitions[1].Role, "second role");

            StringWriter writer = new StringWriter();
            TesterDefinitionFile.Save(writer, definitions);
            string expected = "Client,192.0.2.10,198.51.100.20,TCP,443" + Environment.NewLine +
                "Server,192.0.2.11,,UDP,53" + Environment.NewLine;
            TestAssert.Equal(expected, writer.ToString(), "multi-line serialize");

            TestAssert.Equal(0, TesterDefinitionFile.Load(new StringReader(string.Empty)).Count, "empty file");

            try {
                TesterDefinitionFile.Load(new StringReader(
                    "Client,,198.51.100.20,TCP,443\ninvalid-row\n"));
                throw new InvalidOperationException("Assertion failed: invalid middle row did not fail.");
            } catch (FormatException ex) {
                TestAssert.True(ex.Message.StartsWith("2行:", StringComparison.Ordinal), "invalid row line number");
            }

            TestAssert.Throws<FormatException>(delegate {
                TesterDefinitionFile.Load(new StringReader("Client,,2001:db8::1,TCP,443\n"));
            }, "invalid definition validation");
        }

        private static TesterDefinition LoadOne(string line) {
            return TesterDefinitionFile.Load(new StringReader(line + "\n"))[0];
        }

        private static string SaveOne(TesterDefinition definition) {
            StringWriter writer = new StringWriter();
            TesterDefinitionFile.Save(writer, new TesterDefinition[] { definition });
            return writer.ToString();
        }
    }
}
