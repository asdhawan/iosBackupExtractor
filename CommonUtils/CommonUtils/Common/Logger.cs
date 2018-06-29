using System;
using System.Configuration;
using System.Threading.Tasks;

namespace CommonUtils
{
    public static class Logger
    {
        private static readonly string ApplicationName = ConfigurationManager.AppSettings["ApplicationName"];

        public static void LogInfo(string source, string message, bool bAsync = false, string loginId = null)
        { LogInfoMessage(CommonEnums.LogMessageType.INFO, source, message, bAsync, loginId); }

        public static void LogDebug(string source, string message, bool bAsync = false, string loginId = null)
        { LogInfoMessage(CommonEnums.LogMessageType.DEBUG, source, message, bAsync, loginId); }

        public static void LogWarn(string source, string message, bool bAsync = false, string loginId = null)
        { LogInfoMessage(CommonEnums.LogMessageType.WARN, source, message, bAsync, loginId); }

        public static void LogError(string source, string message, Exception ex, bool bAsync = false, string loginId = null)
        { LogErrorMessage(CommonEnums.LogMessageType.ERROR, source, message, ex, bAsync, loginId); }

        public static void LogFatal(string source, string message, Exception ex, bool bAsync = false, string loginId = null)
        { LogErrorMessage(CommonEnums.LogMessageType.FATAL, source, message, ex, bAsync, loginId); }

        private static void LogInfoMessage(CommonEnums.LogMessageType messageType, string source, string message, bool bAsync = false, string loginId = null)
        {
            DateTime now = DateTime.Now;
            if (bAsync)
                Task.Factory.StartNew(() => LogInfoMessage(now, messageType, source, message, loginId));
            else
                LogInfoMessage(now, messageType, source, message, loginId);
        }
        private static void LogInfoMessage(DateTime timestamp, CommonEnums.LogMessageType messageType, string source, string message, string loginId = null)
        {
            //prepend application name to source
            source = string.Format("{0}: {1}", ApplicationName, source);
            if (!string.IsNullOrEmpty(loginId))
                source += string.Format(" [{0}]", loginId);

            Utils.WriteToEventLog(source, message, System.Diagnostics.EventLogEntryType.Information);
        }
        private static void LogErrorMessage(CommonEnums.LogMessageType messageType, string source, string message, Exception ex, bool bAsync = false, string loginId = null)
        {
            DateTime now = DateTime.Now;
            if (bAsync)
                Task.Factory.StartNew(() => LogErrorMessage(now, messageType, source, message, ex, loginId));
            else
                LogErrorMessage(now, messageType, source, message, ex, loginId);
        }
        private static void LogErrorMessage(DateTime timestamp, CommonEnums.LogMessageType messageType, string source, string message, Exception ex, string loginId = null)
        {
            //prepend application name to source
            source = string.Format("{0}: {1}", ApplicationName, source);
            if (!string.IsNullOrEmpty(loginId))
                source += string.Format(" [{0}]", loginId);

            Utils.WriteToEventLog(
                source,
                string.Format("{0}: {1} {2} Stack Trace: {3}", message, ex.Message, Environment.NewLine, ex.StackTrace),
                System.Diagnostics.EventLogEntryType.Error);
            if (ex.InnerException != null)
                Utils.WriteToEventLog(
                    source,
                    string.Format("{0}: {1} {2} Stack Trace: {3}", message + " [Inner Exception]", ex.InnerException.Message, Environment.NewLine, ex.InnerException.StackTrace),
                    System.Diagnostics.EventLogEntryType.Error);
        }
    }
}
