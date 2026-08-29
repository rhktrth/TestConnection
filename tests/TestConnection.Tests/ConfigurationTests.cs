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
            TestAssert.Equal("Client,,203.0.113.1,Ping,0" + Environment.NewLine,
                SaveOne(extraColumn), "legacy extra column is not serialized");

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
            }, "invalid remote IPv6 validation");
        }

        public static void TestTesterDefinitionValidation() {
            TestAssert.Equal<string>(null, TesterDefinitionValidator.Validate(new TesterDefinition(
                TesterRole.Client, "192.0.2.10", "198.51.100.20", ProtocolName.TCP, 1)),
                "minimum TCP port");
            TestAssert.Equal<string>(null, TesterDefinitionValidator.Validate(new TesterDefinition(
                TesterRole.Server, "192.0.2.10", string.Empty, ProtocolName.UDP, 65535)),
                "maximum UDP port");
            TestAssert.Equal<string>(null, TesterDefinitionValidator.Validate(new TesterDefinition(
                TesterRole.Client, string.Empty, "198.51.100.20", ProtocolName.Ping, 0)),
                "Ping port is unused");

            TestAssert.True(TesterDefinitionValidator.Validate(new TesterDefinition(
                TesterRole.Client, "2001:db8::10", "198.51.100.20", ProtocolName.TCP, 443)) != null,
                "local IPv6 rejected");
            TestAssert.True(TesterDefinitionValidator.Validate(new TesterDefinition(
                TesterRole.Client, string.Empty, "2001:db8::53", ProtocolName.DNS, 53)) != null,
                "DNS IPv6 rejected");
            TestAssert.True(TesterDefinitionValidator.Validate(new TesterDefinition(
                TesterRole.Server, "127.0.0.1", string.Empty, ProtocolName.TCP, 0)) != null,
                "server port zero rejected");
            TestAssert.True(TesterDefinitionValidator.Validate(new TesterDefinition(
                TesterRole.Client, string.Empty, "127.0.0.1", ProtocolName.UDP, 0)) != null,
                "client port zero rejected");
            TestAssert.True(TesterDefinitionValidator.Validate(new TesterDefinition(
                TesterRole.Client, string.Empty, "127.0.0.1", ProtocolName.DNS, 0)) != null,
                "DNS port zero rejected");
        }

        public static void TestResultLogSettings() {
            TestConnection.Properties.Settings settings = new TestConnection.Properties.Settings();

            settings.logradio1 = true;
            settings.logradio2 = false;
            settings.logradio3 = false;
            TestAssert.Equal(ResultLogProcessType.Window, ResultLogSettings.Load(settings), "window load");

            settings.logradio1 = false;
            settings.logradio2 = true;
            settings.logradio3 = false;
            TestAssert.Equal(ResultLogProcessType.File, ResultLogSettings.Load(settings), "file load");

            settings.logradio1 = false;
            settings.logradio2 = false;
            settings.logradio3 = true;
            TestAssert.Equal(ResultLogProcessType.None, ResultLogSettings.Load(settings), "none load");

            ResultLogSettings.Store(settings, ResultLogProcessType.File);
            TestAssert.False(settings.logradio1, "stored window flag");
            TestAssert.True(settings.logradio2, "stored file flag");
            TestAssert.False(settings.logradio3, "stored none flag");
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
