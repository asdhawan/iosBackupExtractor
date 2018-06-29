namespace CommonUtils {
    public static class CommonEnums {
        public enum LogMessageType {
            FATAL = 1,
            ERROR,
            WARN,
            INFO,
            DEBUG
        }

        public enum DBChangeType {
            None,
            New,
            Dirty,
            Deleted
        }
    }
}
