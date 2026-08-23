using System;
using System.Management;

namespace TestConnection {
    internal static class NicConfigurationService {
        public static NicConfigurationSnapshot Capture(ManagementObject nic) {
            if (nic == null) {
                throw new ArgumentNullException("nic");
            }

            nic.Get();
            return new NicConfigurationSnapshot(
                nic.Path.Path,
                Convert.ToString(nic["Description"]),
                Convert.ToBoolean(nic["DHCPEnabled"]),
                CloneStringArray(nic["IPAddress"]),
                CloneStringArray(nic["IPSubnet"]),
                CloneStringArray(nic["DefaultIPGateway"]),
                CloneUshortArray(nic["GatewayCostMetric"]));
        }

        public static void ApplyStatic(ManagementObject targetNic, string[] addresses,
            string[] subnets, string gateway) {
            if (targetNic == null) {
                throw new ArgumentNullException("targetNic");
            }

            targetNic.Get();
            ManagementBaseObject ipParameters = targetNic.GetMethodParameters("EnableStatic");
            ipParameters["IPAddress"] = addresses;
            ipParameters["SubnetMask"] = subnets;
            InvokeChecked(targetNic, "EnableStatic", ipParameters, true);

            ManagementBaseObject gatewayParameters = targetNic.GetMethodParameters("SetGateways");
            gatewayParameters["DefaultIPGateway"] = new string[] { gateway };
            gatewayParameters["GatewayCostMetric"] = new ushort[] { 1 };
            InvokeChecked(targetNic, "SetGateways", gatewayParameters, false);
        }

        public static void Restore(NicConfigurationSnapshot snapshot) {
            if (snapshot == null) {
                throw new ArgumentNullException("snapshot");
            }

            ManagementObject targetNic = new ManagementObject(snapshot.Path);
            targetNic.Get();

            if (snapshot.DhcpEnabled) {
                ManagementBaseObject dhcpParameters = targetNic.GetMethodParameters("EnableDHCP");
                InvokeChecked(targetNic, "EnableDHCP", dhcpParameters, false);
                return;
            }

            if (snapshot.IpAddresses.Length == 0 || snapshot.IpAddresses.Length != snapshot.SubnetMasks.Length) {
                throw new InvalidOperationException("変更前の静的IP設定を復元できる情報がありません。");
            }

            ManagementBaseObject ipParameters = targetNic.GetMethodParameters("EnableStatic");
            ipParameters["IPAddress"] = snapshot.IpAddresses;
            ipParameters["SubnetMask"] = snapshot.SubnetMasks;
            InvokeChecked(targetNic, "EnableStatic", ipParameters, true);

            ManagementBaseObject gatewayParameters = targetNic.GetMethodParameters("SetGateways");
            if (snapshot.DefaultGateways.Length != 0) {
                gatewayParameters["DefaultIPGateway"] = snapshot.DefaultGateways;
                gatewayParameters["GatewayCostMetric"] = NormalizeMetrics(
                    snapshot.DefaultGateways.Length, snapshot.GatewayMetrics);
            } else {
                // WMIのSetGateways仕様では、既定ゲートウェイをクリアする場合は
                // EnableStaticで設定したIP自身をgatewayとして指定する。
                gatewayParameters["DefaultIPGateway"] = new string[] { snapshot.IpAddresses[0] };
                gatewayParameters["GatewayCostMetric"] = new ushort[] { 1 };
            }
            InvokeChecked(targetNic, "SetGateways", gatewayParameters, false);
        }

        private static ushort[] NormalizeMetrics(int gatewayCount, ushort[] metrics) {
            ushort[] result = new ushort[gatewayCount];
            for (int i = 0; i < gatewayCount; i++) {
                result[i] = metrics != null && i < metrics.Length && metrics[i] != 0 ? metrics[i] : (ushort)1;
            }
            return result;
        }

        private static void InvokeChecked(ManagementObject targetNic, string methodName,
            ManagementBaseObject parameters, bool allowEnableStaticCode81) {
            ManagementBaseObject result = targetNic.InvokeMethod(methodName, parameters, null);
            if (result == null || result["ReturnValue"] == null) {
                return;
            }

            uint returnValue = Convert.ToUInt32(result["ReturnValue"]);
            if (returnValue == 0 || returnValue == 1 || (allowEnableStaticCode81 && returnValue == 81)) {
                return;
            }

            throw new InvalidOperationException(methodName + " failed (WMI return code " + returnValue + ").");
        }

        private static string[] CloneStringArray(object value) {
            string[] source = value as string[];
            return source == null ? new string[0] : (string[])source.Clone();
        }

        private static ushort[] CloneUshortArray(object value) {
            ushort[] source = value as ushort[];
            return source == null ? new ushort[0] : (ushort[])source.Clone();
        }
    }
}
