namespace TestConnection {
    internal sealed class NicConfigurationSnapshot {
        public string Path { get; private set; }
        public string Description { get; private set; }
        public bool DhcpEnabled { get; private set; }
        public string[] IpAddresses { get; private set; }
        public string[] SubnetMasks { get; private set; }
        public string[] DefaultGateways { get; private set; }
        public ushort[] GatewayMetrics { get; private set; }

        internal NicConfigurationSnapshot(string path, string description, bool dhcpEnabled,
            string[] ipAddresses, string[] subnetMasks, string[] defaultGateways, ushort[] gatewayMetrics) {
            Path = path;
            Description = description;
            DhcpEnabled = dhcpEnabled;
            IpAddresses = ipAddresses;
            SubnetMasks = subnetMasks;
            DefaultGateways = defaultGateways;
            GatewayMetrics = gatewayMetrics;
        }
    }
}
