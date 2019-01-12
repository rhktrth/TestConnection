using System;
using System.Net;
using System.Text;

namespace TestConnection {
    enum CSType { Server, Client };
    enum ProtocolName { TCP, UDP, DNS, Ping };

    abstract class TesterBase {

        public string remoteIpAddress { get; protected set; }
        public string localIpAddress { get; protected set; }
        public int portNo { get; protected set; }
        public int successCount { get; protected set; }
        public int failureCount { get; protected set; }
        public int timeout { get; set; }                             // ms
        public event successEventHandler successEvent;
        public event failureEventHandler failureEvent;
        public CSType csType { get; protected set; }
        public ProtocolName protocolName { get; protected set; }
        protected IPEndPoint localIPEndPoint;
        private MainForm parentForm;

        public TesterBase(MainForm fo) {
            parentForm = fo;
            remoteIpAddress = string.Empty;
            localIpAddress = string.Empty;
            portNo = 0;
            ClearCount();
        }

        public void ClearCount() {
            successCount = 0;
            failureCount = 0;
        }

        protected void outputMessage(string msg) {
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss "));
            sb.Append(csType.ToString());
            sb.Append(" ");
            if (localIPEndPoint != null) { sb.Append(localIPEndPoint.Address.ToString()); }
            sb.Append(":");
            if (localIPEndPoint != null) { sb.Append(localIPEndPoint.Port.ToString()); }
            sb.Append("/");
            sb.Append(protocolName.ToString());
            sb.Append(" ");
            if (msg != null) { sb.Append(msg); }
            SamDelegate deledele = parentForm.addResult;
            parentForm.Invoke(deledele, sb.ToString());
        }

        protected void success() {
            successCount++;
            if (successEvent != null) {
                successEvent();
            }
        }
        protected void failure() {
            failureCount++;
            if (failureEvent != null) {
                failureEvent();
            }
        }
    }
}
