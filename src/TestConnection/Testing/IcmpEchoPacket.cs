using System;

namespace TestConnection {
    internal static class IcmpEchoPacket {
        private const byte EchoRequest = 8;
        private const byte EchoReply = 0;
        private const int IcmpHeaderLength = 8;
        private const int MinimumIpv4HeaderLength = 20;

        public static byte[] CreateRequest(ushort identifier, ushort sequenceNumber, int payloadLength) {
            if (payloadLength < 0) {
                throw new ArgumentOutOfRangeException("payloadLength");
            }

            byte[] packet = new byte[IcmpHeaderLength + payloadLength];
            packet[0] = EchoRequest;
            packet[1] = 0;
            WriteUInt16(packet, 4, identifier);
            WriteUInt16(packet, 6, sequenceNumber);

            for (int i = IcmpHeaderLength; i < packet.Length; i++) {
                packet[i] = (byte)'A';
            }

            WriteUInt16(packet, 2, ComputeChecksum(packet));
            return packet;
        }

        public static bool IsMatchingReply(byte[] packet, int length, ushort identifier, ushort sequenceNumber) {
            if (packet == null || length < MinimumIpv4HeaderLength || length > packet.Length) {
                return false;
            }
            if ((packet[0] >> 4) != 4) {
                return false;
            }

            int ipHeaderLength = (packet[0] & 0x0f) * 4;
            if (ipHeaderLength < MinimumIpv4HeaderLength || length < ipHeaderLength + IcmpHeaderLength) {
                return false;
            }

            return packet[ipHeaderLength] == EchoReply
                && packet[ipHeaderLength + 1] == 0
                && ReadUInt16(packet, ipHeaderLength + 4) == identifier
                && ReadUInt16(packet, ipHeaderLength + 6) == sequenceNumber;
        }

        private static ushort ComputeChecksum(byte[] packet) {
            uint sum = 0;
            int index = 0;

            while (index + 1 < packet.Length) {
                sum += (uint)((packet[index] << 8) | packet[index + 1]);
                index += 2;
            }
            if (index < packet.Length) {
                sum += (uint)(packet[index] << 8);
            }

            while ((sum >> 16) != 0) {
                sum = (sum & 0xffff) + (sum >> 16);
            }

            return (ushort)~sum;
        }

        private static void WriteUInt16(byte[] buffer, int offset, ushort value) {
            buffer[offset] = (byte)(value >> 8);
            buffer[offset + 1] = (byte)value;
        }

        private static ushort ReadUInt16(byte[] buffer, int offset) {
            return (ushort)((buffer[offset] << 8) | buffer[offset + 1]);
        }
    }
}
