using Bugsnag;

namespace NLog.Targets.Bugsnag
{
    internal static class SeverityConverter
    {
        public static Severity ToSeverity(this LogLevel logLevel)
        {
            return logLevel >= LogLevel.Error
                ? Severity.Error
                : logLevel >= LogLevel.Warn
                    ? Severity.Warning
                    : Severity.Info;
        }
    }
}
