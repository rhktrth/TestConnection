namespace TestConnection {
    internal sealed class TesterDefinition {
        public TesterRole Role { get; private set; }
        public string LocalIpAddress { get; private set; }
        public string RemoteIpAddress { get; private set; }
        public ProtocolName Protocol { get; private set; }
        public int Port { get; private set; }

        public TesterDefinition(TesterRole role, string localIpAddress, string remoteIpAddress,
            ProtocolName protocol, int port) {
            Role = role;
            LocalIpAddress = localIpAddress;
            RemoteIpAddress = role == TesterRole.Server ? string.Empty : remoteIpAddress;
            Protocol = protocol;
            Port = port;
        }
    }
}
