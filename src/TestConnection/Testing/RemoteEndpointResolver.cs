using System;
using System.Net;
using System.Net.Sockets;

namespace TestConnection {
    internal static class RemoteEndpointResolver {
        public static bool IsSupportedInput(string target) {
            if (string.IsNullOrWhiteSpace(target)) {
                return false;
            }

            IPAddress literal;
            if (IPAddress.TryParse(target, out literal)) {
                return literal.AddressFamily == AddressFamily.InterNetwork;
            }

            // hostname/FQDNの名前解決可否は登録時ではなく各試行時に判定する。
            return true;
        }

        public static IPAddress ResolveIpv4(string target) {
            if (string.IsNullOrWhiteSpace(target)) {
                throw new ArgumentException("remote endpointが空です。", "target");
            }

            IPAddress literal;
            if (IPAddress.TryParse(target, out literal)) {
                if (literal.AddressFamily != AddressFamily.InterNetwork) {
                    throw new InvalidOperationException(target + " はIPv4 addressではありません。");
                }
                return literal;
            }

            IPAddress[] addresses = Dns.GetHostAddresses(target);
            IPAddress selected = SelectLowestIpv4(addresses);
            if (selected == null) {
                throw new InvalidOperationException(target + " のIPv4 addressを解決できませんでした。");
            }
            return selected;
        }

        public static string FormatResolvedTarget(string configuredTarget, IPAddress resolvedAddress) {
            if (resolvedAddress == null) {
                return configuredTarget;
            }

            string resolved = resolvedAddress.ToString();
            if (string.Equals(configuredTarget, resolved, StringComparison.OrdinalIgnoreCase)) {
                return resolved;
            }
            return configuredTarget + "(" + resolved + ")";
        }

        internal static IPAddress SelectLowestIpv4(IPAddress[] addresses) {
            IPAddress selected = null;
            if (addresses == null) {
                return null;
            }

            foreach (IPAddress address in addresses) {
                if (address == null || address.AddressFamily != AddressFamily.InterNetwork) {
                    continue;
                }
                if (selected == null || compareIpv4(address, selected) < 0) {
                    selected = address;
                }
            }
            return selected;
        }

        private static int compareIpv4(IPAddress left, IPAddress right) {
            byte[] leftBytes = left.GetAddressBytes();
            byte[] rightBytes = right.GetAddressBytes();
            for (int i = 0; i < 4; i++) {
                int comparison = leftBytes[i].CompareTo(rightBytes[i]);
                if (comparison != 0) {
                    return comparison;
                }
            }
            return 0;
        }
    }
}
