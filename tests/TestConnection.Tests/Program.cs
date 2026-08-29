using System;

namespace TestConnection.Tests {
    internal static class Program {
        private static int failures;

        private static int Main() {
            Run("ICMP Echo Requestを正しく生成する", IcmpEchoPacketTests.TestCreateEchoRequest);
            Run("対応するICMP Echo Replyだけを受理する", IcmpEchoPacketTests.TestMatchingEchoReply);
            Run("不正なICMP packetを拒否する", IcmpEchoPacketTests.TestMalformedEchoReply);
            Run("TCP client/serverがloopbackで接続できる", ConnectivityTests.TestTcpLoopback);
            Run("UDP client/serverがloopbackで送受信できる", ConnectivityTests.TestUdpLoopback);
            Run("DNS clientがloopback responseをsuccessにする", ConnectivityTests.TestDnsLoopback);
            Run("DNS clientのCancelが受信待ちをfailureなしで終了する", ConnectivityTests.TestDnsCancellation);
            Run("有限回数のclient runnerを登録順に実行する", RunnerTests.TestFiniteClientLoop);
            Run("client runnerのStopが現在試行をCancelしてworker終了を待つ", RunnerTests.TestClientLoopStop);
            Run("TestSessionが開始停止順序と再Startを管理する", RunnerTests.TestSessionLifecycle);
            Run("TestSessionが定義からtesterを生成する", RunnerTests.TestSessionCreatesTester);
            Run("server Startがlisten準備完了を待つ", RunnerTests.TestServerStartWaitsForReady);
            Run("結果ログprefixが空endpointへ区切り文字を付けない", ConnectivityTests.TestResultLogFormat);
            Run("既存5列CSVを互換にparse/serializeする", ConfigurationTests.TestTesterDefinitionFormat);
            Run("設定ファイルを全件検証して読み書きする", ConfigurationTests.TestTesterDefinitionFile);
            Run("remote endpointのIPv4選択規則を固定する", ConnectivityTests.TestRemoteEndpointResolver);
            Run("WinForms High DPI構成が起動可能な形式である", ApplicationConfigurationTests.TestWinFormsApplicationConfiguration);

            if (failures != 0) {
                Console.Error.WriteLine(failures + " regression test(s) failed.");
                return 1;
            }

            Console.WriteLine("All regression tests passed.");
            return 0;
        }

        private static void Run(string name, Action test) {
            try {
                test();
                Console.WriteLine("PASS: " + name);
            } catch (Exception ex) {
                failures++;
                Console.Error.WriteLine("FAIL: " + name);
                Console.Error.WriteLine(ex.ToString());
            }
        }
    }
}
