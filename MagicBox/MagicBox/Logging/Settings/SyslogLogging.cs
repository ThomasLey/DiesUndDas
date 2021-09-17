namespace MagicBox.Logging.Settings
{
    public class SyslogLogging
    {
        public bool EnableSyslogLogging { get; set; }

        public string SysLogServerIp { get; set; }

        public int SyslogServerPort { get; set; }

        public string CertificatePath { get; set; }

        public string CertificatePassword { get; set; }

        public string MinimumLevel { get; set; }

        public bool UseUdp { get; set; }
    }
}