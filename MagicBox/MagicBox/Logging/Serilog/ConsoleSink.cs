using System;
using Serilog.Core;
using Serilog.Events;

namespace MagicBox.Logging.Serilog
{
    public class ConsoleSink : ILogEventSink
    {
        #region Implementation of ILogEventSink

        public void Emit(LogEvent logEvent)
        {
            Console.WriteLine(logEvent.RenderMessage());
        }

        #endregion Implementation of ILogEventSink
    }
}