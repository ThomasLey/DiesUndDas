using System;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Context;

namespace MagicBox.Logging.Serilog
{
    public class SerilogAdapter : IProjectLogger
    {
        private readonly ILogger _serilogLogger;

        public SerilogAdapter(ILogger serilogLogger)
        {
            _serilogLogger = serilogLogger ?? throw new ArgumentNullException(nameof(serilogLogger));
        }

        public void Verbose(string message, [CallerMemberName] string memberName = "")
        {
            _serilogLogger.Verbose(message);
        }

        public void Debug(string message, [CallerMemberName] string memberName = "")
        {
            _serilogLogger.Debug(message);
        }

        public void Info(string message, [CallerMemberName] string memberName = "")
        {
            _serilogLogger.Information(message);
        }

        public void Warning(string message, [CallerMemberName] string memberName = "")
        {
            _serilogLogger.Warning(message);
        }

        public void Error(string message, Exception ex, [CallerMemberName] string memberName = "")
        {
            _serilogLogger.Error(ex, message);
        }

        public void Fatal(string message, Exception ex, [CallerMemberName] string memberName = "")
        {
            _serilogLogger.Fatal(ex, message);
        }

        public IDisposable PushProperty(string name, object value)
        {
            return LogContext.PushProperty(name, value);
        }
    }
}