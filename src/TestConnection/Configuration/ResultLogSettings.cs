namespace TestConnection {
    internal enum ResultLogProcessType {
        Window,
        File,
        None
    }

    internal static class ResultLogSettings {
        public static ResultLogProcessType Load(Properties.Settings settings) {
            if (settings.logradio2) {
                return ResultLogProcessType.File;
            }
            if (settings.logradio3) {
                return ResultLogProcessType.None;
            }
            return ResultLogProcessType.Window;
        }

        public static void Store(Properties.Settings settings, ResultLogProcessType processType) {
            settings.logradio1 = processType == ResultLogProcessType.Window;
            settings.logradio2 = processType == ResultLogProcessType.File;
            settings.logradio3 = processType == ResultLogProcessType.None;
        }
    }
}
