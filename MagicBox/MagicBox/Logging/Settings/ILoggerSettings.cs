namespace MagicBox.Logging.Settings
{
    public interface ILoggerSettings
    {
        SyslogLogging Syslog { get; set; }
        FileLogging File { get; set; }
    }
}