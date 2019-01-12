using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestConnection {
    class PingTesterClient : TesterClient {
        const int SOCKET_ERROR = -1;
        const int ICMP_ECHO = 8;
        const int DEFAULTTIMEOUT = 1000;

        private Socket socket;

        public PingTesterClient(MainForm fo, string lip, string rip)
            : base(fo, lip, rip, 0) {
            protocolName = ProtocolName.Ping;
            timeout = DEFAULTTIMEOUT;
        }

        public override void Try() {
            int nBytes;
            int dwStart, dwStop;
            IPEndPoint remoteIPEndPoint;
            EndPoint lep, rep;
            StringBuilder sb = new StringBuilder();

            try {
                //ローカル側のエンドポイントを設定
                if (localIpAddress == string.Empty) {
                    localIPEndPoint = new IPEndPoint(0, 0);
                } else {
                    localIPEndPoint = new IPEndPoint(IPAddress.Parse(localIpAddress), 0);
                }

                //リモート側のエンドポイントを設定
                remoteIPEndPoint = new IPEndPoint(IPAddress.Parse(remoteIpAddress), 0);
                rep = (EndPoint)remoteIPEndPoint;

                //ソケットの初期化 ICMPで
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);

                //送信元ソケットをバインド
                socket.Bind(localIPEndPoint);
                lep = (EndPoint)localIPEndPoint;
            } catch (Exception ex) {
                failure();
                sb.Append("ping-ng-to ").Append(remoteIpAddress).Append(" ").Append(ex.Message);
                outputMessage(sb.ToString());
                return;
            }

            int PacketSize = 0;
            IcmpPacket packet = new IcmpPacket();
            // Construct the packet to send
            packet.Type = ICMP_ECHO; //8
            packet.SubCode = 0;
            packet.CheckSum = UInt16.Parse("0");
            packet.Identifier = UInt16.Parse("45");
            packet.SequenceNumber = UInt16.Parse("0");
            int PingData = 32; // sizeof(IcmpPacket) - 8;
            packet.Data = new Byte[PingData];
            //Initilize the Packet.Data
            for (int i = 0; i < PingData; i++) {
                packet.Data[i] = (byte)'A';
            }

            //Variable to hold the total Packet size
            PacketSize = PingData + 8;
            Byte[] icmp_pkt_buffer = new Byte[PacketSize];
            Int32 Index = 0;
            //Call a Method Serialize which counts
            //The total number of Bytes in the Packet
            Index = Serialize(packet, icmp_pkt_buffer, PacketSize, PingData);
            //Error in Packet Size
            if (Index == -1) {
                failure();
                sb.Append("ping-ng-to ").Append(remoteIpAddress).Append(" ").Append("Error in Making Packet (base)");
                outputMessage(sb.ToString());
                return;
            }

            // now get this critter into a UInt16 array

            //Get the Half size of the Packet
            Double double_length = Convert.ToDouble(Index);
            Double dtemp = Math.Ceiling(double_length / 2);
            int cksum_buffer_length = Convert.ToInt32(dtemp);
            //Create a Byte Array
            UInt16[] cksum_buffer = new UInt16[cksum_buffer_length];
            //Code to initialize the Uint16 array 
            int icmp_header_buffer_index = 0;
            for (int i = 0; i < cksum_buffer_length; i++) {
                cksum_buffer[i] =
                      BitConverter.ToUInt16(icmp_pkt_buffer, icmp_header_buffer_index);
                icmp_header_buffer_index += 2;
            }
            //Call a method which will return a checksum             
            UInt16 u_cksum = checksum(cksum_buffer, cksum_buffer_length);
            //Save the checksum to the Packet
            packet.CheckSum = u_cksum;

            // Now that we have the checksum, serialize the packet again
            Byte[] sendbuf = new Byte[PacketSize];
            //again check the packet size
            Index = Serialize(packet, sendbuf, PacketSize, PingData);
            //if there is a error report it
            if (Index == -1) {
                failure();
                sb.Append("ping-ng-to ").Append(remoteIpAddress).Append(" ").Append("Error in Making Packet (checksum)");
                outputMessage(sb.ToString());
                return;
            }

            //送信
            dwStart = System.Environment.TickCount; // Start timing
            socket.SendTimeout = timeout;

            /*            if ((nBytes = socket.SendTo(sendbuf, PacketSize, SocketFlags.None, rep)) == SOCKET_ERROR) {
                            failure();
                            outputMessage("ping-ng-to " + remoteIpAddress + " " + "Socket Error cannot Send Packet");
                        } */
            try {
                nBytes = socket.SendTo(sendbuf, PacketSize, SocketFlags.None, rep);
            } catch (Exception ex) {
                failure();
                sb.Append("ping-ng-to ").Append(remoteIpAddress).Append(" ").Append(ex.Message);
                outputMessage(sb.ToString());
                return;
            }
            localIPEndPoint = (IPEndPoint)socket.LocalEndPoint;

            //受信
            Byte[] ReceiveBuffer = new Byte[1024];
            nBytes = 0;
            socket.ReceiveTimeout = timeout;
            try {
                nBytes = socket.ReceiveFrom(ReceiveBuffer, ref rep);
            } catch (Exception ex) {
                failure();
                sb.Append("ping-ng-to ").Append(remoteIpAddress).Append(" ").Append(ex.Message);
                outputMessage(sb.ToString());
                return;
            }
            dwStop = System.Environment.TickCount - dwStart; // stop timing

            success();
            sb.Append("ping-ok-to ");
            sb.Append(remoteIpAddress);
            sb.Append(":");
            sb.Append(portNo);
            sb.Append("/");
            sb.Append(protocolName.ToString());
            sb.Append(" ");
            sb.Append(dwStop);
            sb.Append("ms");
            outputMessage(sb.ToString());

            socket.Close();
        }
        /// <summary>
        ///  This method get the Packet and calculates the total size 
        ///  of the Pack by converting it to byte array
        /// </summary>
        private static Int32 Serialize(IcmpPacket packet, Byte[] Buffer,
              Int32 PacketSize, Int32 PingData) {
            Int32 cbReturn = 0;
            // serialize the struct into the array
            int Index = 0;

            Byte[] b_type = new Byte[1];
            b_type[0] = (packet.Type);

            Byte[] b_code = new Byte[1];
            b_code[0] = (packet.SubCode);

            Byte[] b_cksum = BitConverter.GetBytes(packet.CheckSum);
            Byte[] b_id = BitConverter.GetBytes(packet.Identifier);
            Byte[] b_seq = BitConverter.GetBytes(packet.SequenceNumber);

            // Console.WriteLine("Serialize type ");
            Array.Copy(b_type, 0, Buffer, Index, b_type.Length);
            Index += b_type.Length;

            // Console.WriteLine("Serialize code ");
            Array.Copy(b_code, 0, Buffer, Index, b_code.Length);
            Index += b_code.Length;

            // Console.WriteLine("Serialize cksum ");
            Array.Copy(b_cksum, 0, Buffer, Index, b_cksum.Length);
            Index += b_cksum.Length;

            // Console.WriteLine("Serialize id ");
            Array.Copy(b_id, 0, Buffer, Index, b_id.Length);
            Index += b_id.Length;

            Array.Copy(b_seq, 0, Buffer, Index, b_seq.Length);
            Index += b_seq.Length;

            // copy the data	        
            Array.Copy(packet.Data, 0, Buffer, Index, PingData);
            Index += PingData;
            if (Index != PacketSize/* sizeof(IcmpPacket)  */) {
                cbReturn = -1;
                return cbReturn;
            }

            cbReturn = Index;
            return cbReturn;
        }
        /// <summary>
        ///		This Method has the algorithm to make a checksum 
        /// </summary>
        private static UInt16 checksum(UInt16[] buffer, int size) {
            Int32 cksum = 0;
            int counter = 0;

            while (size > 0) {
                //                UInt16 val = buffer[counter];

                cksum += Convert.ToInt32(buffer[counter]);
                counter++;
                size--;
            }

            cksum = (cksum >> 16) + (cksum & 0xffff);
            cksum += (cksum >> 16);
            return (UInt16)(~cksum);
        }

        public override void Cancel() {
            if (socket != null) {
                socket.Close();
            }
        }

        private class IcmpPacket {
            public Byte Type;    // type of message
            public Byte SubCode;    // type of sub code
            public UInt16 CheckSum;   // ones complement checksum of struct
            public UInt16 Identifier;      // identifier
            public UInt16 SequenceNumber;     // sequence number  
            public Byte[] Data;
        }

    }
}
