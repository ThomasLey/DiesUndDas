using System;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using MagicBox.Logging.Settings;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Syslog;

namespace MagicBox.Logging.Serilog
{
    public class SerilogFactory : ILoggerFactory
    {
        private const string AppName = "eGov360 CDS";

        public IProjectLogger Build(ILoggerSettings settings)
        {
            if (settings.AllLoggersAreDisabled())
                return new EmptyProjectLogger();

            var builder = CreateBuilder(settings);

            builder = AddSyslogUdp(settings, builder);
            builder = AddSyslogTcp(settings, builder);
            builder = AddFile(settings, builder);

            return new SerilogAdapter(builder.CreateLogger());
        }

        private LoggerConfiguration AddFile(ILoggerSettings settings, LoggerConfiguration builder)
        {
            if (settings.File?.EnableFileLogging ?? false)
                builder = builder.WriteTo.File(
                    settings.File.FilePath,
                    GetLevel(settings.File.MinimumLevel),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: settings.File.RetainedFileCountLimit,
                    outputTemplate: settings.File.OutputTemplate,
                    shared: settings.File.Async,
                    fileSizeLimitBytes: settings.File.FileSizeLimitBytes,
                    rollOnFileSizeLimit: settings.File.RollOnFileSizeLimit
                );
            return builder;
        }

        private static LoggerConfiguration AddSyslogTcp(ILoggerSettings settings, LoggerConfiguration builder)
        {
            //Use syslog over tcp for logging if enabled
            if (settings.Syslog.EnableSyslogLogging && !settings.Syslog.UseUdp)
            {
                var tcpConfig = new SyslogTcpConfig
                {
                    Host = settings.Syslog.SysLogServerIp,
                    Port = settings.Syslog.SyslogServerPort == 0 ? 601 : settings.Syslog.SyslogServerPort,
                    Formatter = new Rfc5424Formatter(),
                    Framer = new MessageFramer(FramingType.OCTET_COUNTING)
                };

                if (!string.IsNullOrWhiteSpace(settings.Syslog.CertificatePath))
                {
                    tcpConfig.Port = settings.Syslog.SyslogServerPort == 0 ? 6514 : settings.Syslog.SyslogServerPort;
                    tcpConfig.SecureProtocols = SslProtocols.Tls11 | SslProtocols.Tls12;
                    tcpConfig.CertProvider = new CertificateFileProvider(settings.Syslog.CertificatePath, settings.Syslog.CertificatePassword);
                    tcpConfig.CertValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        //Verify the certificate here
                        var localCert = new X509Certificate2(certificate);
                        return localCert.Verify();
                    };
                }

                builder = builder.WriteTo.TcpSyslog(tcpConfig);
            }

            return builder;
        }

        private static LoggerConfiguration AddSyslogUdp(ILoggerSettings settings, LoggerConfiguration builder)
        {
            if (settings.Syslog.EnableSyslogLogging && settings.Syslog.UseUdp)
                builder = builder.WriteTo.UdpSyslog(
                    settings.Syslog.SysLogServerIp,
                    settings.Syslog.SyslogServerPort == 0 ? 514 : settings.Syslog.SyslogServerPort,
                    AppName);

            return builder;
        }

        private static LoggerConfiguration CreateBuilder(ILoggerSettings settings)
        {
            var builder = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProcessId();

            if (settings.AtLeastOneVerbose()) builder = builder.MinimumLevel.Verbose();

            // MachineName, Caller and Exception Details only for Debug
            if (ModeDetector.IsDebug)
                builder = builder
                    .Enrich.WithExceptionDetails()
                    .Enrich.WithMachineName()
                    .WriteTo.Sink(new ConsoleSink());

            return builder;
        }

        private LogEventLevel GetLevel(string level)
        {
            return (LogEventLevel)Enum.Parse(typeof(LogEventLevel), level);
        }
    }
}