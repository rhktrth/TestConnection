namespace TestConnection {
    abstract class TesterClient : TesterBase {
        public TesterClient(MainForm fo, string lip, string rip, int po)
            : base(fo) {
            csType = CSType.Client;
            localIpAddress = lip;
            remoteIpAddress = rip;
            portNo = po;
        }

        public abstract void Try();

        public abstract void Cancel();
    }
}
