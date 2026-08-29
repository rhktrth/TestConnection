using System;
using System.Net;
using System.Net.Sockets;

namespace TestConnection {
    internal static class TesterDefinitionValidator {
        public static string Validate(TesterDefinition definition) {
            if (definition == null) {
                return "試験定義がありません";
            }

            if (definition.LocalIpAddress != string.Empty && !IsIpv4Address(definition.LocalIpAddress)) {
                return definition.LocalIpAddress + " ローカルIPv4アドレスが無効です";
            }

            if (definition.Role == TesterRole.Server) {
                return ValidateServer(definition);
            }
            if (definition.Role == TesterRole.Client) {
                return ValidateClient(definition);
            }
            return definition.Role + " is Server/Client type error";
        }

        private static string ValidateServer(TesterDefinition definition) {
            string portError = ValidatePort(definition);
            if (portError != null) {
                return portError;
            }

            if (definition.Protocol != ProtocolName.TCP && definition.Protocol != ProtocolName.UDP) {
                return definition.Protocol + " is TCP/UDP type error";
            }
            return null;
        }

        private static string ValidateClient(TesterDefinition definition) {
            if (definition.Protocol == ProtocolName.DNS) {
                if (!IsIpv4Address(definition.RemoteIpAddress)) {
                    return definition.RemoteIpAddress + " DNSサーバIPv4アドレスが無効です";
                }
            }
            else if (!RemoteEndpointResolver.IsSupportedInput(definition.RemoteIpAddress)) {
                return definition.RemoteIpAddress + " リモートendpointが無効です";
            }

            string portError = ValidatePort(definition);
            if (portError != null) {
                return portError;
            }

            if (!Enum.IsDefined(typeof(ProtocolName), definition.Protocol)) {
                return definition.Protocol + " is TCP/UDP/DNS/Ping type error";
            }
            return null;
        }

        private static string ValidatePort(TesterDefinition definition) {
            if (definition.Protocol != ProtocolName.Ping &&
                (definition.Port < 1 || 65535 < definition.Port)) {
                return definition.Port + " ポート番号が無効です";
            }
            return null;
        }

        private static bool IsIpv4Address(string value) {
            IPAddress address;
            return IPAddress.TryParse(value, out address) &&
                address.AddressFamily == AddressFamily.InterNetwork;
        }
    }
}
