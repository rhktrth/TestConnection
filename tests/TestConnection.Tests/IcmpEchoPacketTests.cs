using TestConnection;

namespace TestConnection.Tests {
    internal static class IcmpEchoPacketTests {
        public static void TestCreateEchoRequest() {
            byte[] packet = IcmpEchoPacket.CreateRequest(0x1234, 0x5678, 32);
            TestAssert.Equal(40, packet.Length, "packet length");
            TestAssert.Equal((byte)8, packet[0], "ICMP type");
            TestAssert.Equal((byte)0, packet[1], "ICMP code");
            TestAssert.Equal((byte)0x12, packet[4], "identifier high byte");
            TestAssert.Equal((byte)0x34, packet[5], "identifier low byte");
            TestAssert.Equal((byte)0x56, packet[6], "sequence high byte");
            TestAssert.Equal((byte)0x78, packet[7], "sequence low byte");
            TestAssert.Equal(0xffff, ComputeOnesComplementSum(packet), "checksum");
        }

        public static void TestMatchingEchoReply() {
            byte[] normal = CreateIpv4EchoReply(5, 0x1234, 0x5678, 0);
            TestAssert.True(IcmpEchoPacket.IsMatchingReply(normal, normal.Length, 0x1234, 0x5678),
                "normal IPv4 reply");

            byte[] withOptions = CreateIpv4EchoReply(6, 0x1234, 0x5678, 0);
            TestAssert.True(IcmpEchoPacket.IsMatchingReply(withOptions, withOptions.Length, 0x1234, 0x5678),
                "IPv4 options reply");

            byte[] wrongType = CreateIpv4EchoReply(5, 0x1234, 0x5678, 3);
            TestAssert.False(IcmpEchoPacket.IsMatchingReply(wrongType, wrongType.Length, 0x1234, 0x5678),
                "wrong ICMP type");

            byte[] wrongId = CreateIpv4EchoReply(5, 0x9999, 0x5678, 0);
            TestAssert.False(IcmpEchoPacket.IsMatchingReply(wrongId, wrongId.Length, 0x1234, 0x5678),
                "wrong identifier");

            byte[] wrongSequence = CreateIpv4EchoReply(5, 0x1234, 0x9999, 0);
            TestAssert.False(IcmpEchoPacket.IsMatchingReply(wrongSequence, wrongSequence.Length, 0x1234, 0x5678),
                "wrong sequence");
        }

        public static void TestMalformedEchoReply() {
            TestAssert.False(IcmpEchoPacket.IsMatchingReply(null, 0, 1, 1), "null packet");
            TestAssert.False(IcmpEchoPacket.IsMatchingReply(new byte[10], 10, 1, 1), "short packet");

            byte[] invalidVersion = CreateIpv4EchoReply(5, 1, 1, 0);
            invalidVersion[0] = 0x65;
            TestAssert.False(IcmpEchoPacket.IsMatchingReply(invalidVersion, invalidVersion.Length, 1, 1),
                "IPv6-like header");

            byte[] invalidIhl = CreateIpv4EchoReply(5, 1, 1, 0);
            invalidIhl[0] = 0x44;
            TestAssert.False(IcmpEchoPacket.IsMatchingReply(invalidIhl, invalidIhl.Length, 1, 1), "too short IHL");

            byte[] valid = CreateIpv4EchoReply(5, 1, 1, 0);
            TestAssert.False(IcmpEchoPacket.IsMatchingReply(valid, valid.Length + 1, 1, 1), "length beyond buffer");
        }

        private static byte[] CreateIpv4EchoReply(int ihlWords, ushort identifier, ushort sequence, byte type) {
            int ipHeaderLength = ihlWords * 4;
            byte[] packet = new byte[ipHeaderLength + 8];
            packet[0] = (byte)(0x40 | ihlWords);
            packet[ipHeaderLength] = type;
            packet[ipHeaderLength + 1] = 0;
            packet[ipHeaderLength + 4] = (byte)(identifier >> 8);
            packet[ipHeaderLength + 5] = (byte)identifier;
            packet[ipHeaderLength + 6] = (byte)(sequence >> 8);
            packet[ipHeaderLength + 7] = (byte)sequence;
            return packet;
        }

        private static int ComputeOnesComplementSum(byte[] packet) {
            uint sum = 0;
            for (int i = 0; i < packet.Length; i += 2) {
                ushort word = (ushort)(packet[i] << 8);
                if (i + 1 < packet.Length) {
                    word |= packet[i + 1];
                }
                sum += word;
                while ((sum >> 16) != 0) {
                    sum = (sum & 0xffff) + (sum >> 16);
                }
            }
            return (int)sum;
        }
    }
}
