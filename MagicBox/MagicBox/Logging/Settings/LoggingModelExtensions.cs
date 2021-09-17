using System;

namespace MagicBox.Logging.Settings
{
    public static class LoggingModelExtensions
    {
        private const string VerboseLiteral = "Verbose";

        public static bool AllLoggersAreDisabled(this ILoggerSettings settings)
        {
            if (settings == null) return true;

            var oneLoggerIsEnabled =
                (settings.File?.EnableFileLogging ?? false) ||
                (settings.Syslog?.EnableSyslogLogging ?? false);

            return !oneLoggerIsEnabled;
        }

        public static bool AtLeastOneVerbose(this ILoggerSettings settings)
        {
            return string.Compare(settings.File?.MinimumLevel, VerboseLiteral, StringComparison.InvariantCultureIgnoreCase) == 0 ||
                   string.Compare(settings.Syslog?.MinimumLevel, VerboseLiteral, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}