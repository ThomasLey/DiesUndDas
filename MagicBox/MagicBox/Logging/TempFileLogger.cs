using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MagicBox.Logging
{
    /// <summary>
    ///     Logger implementation to write log messages directly into a file in a temp folder.
    ///     Naming convention is "project_yyyy-mm-dd.log".
    /// </summary>
    public class TempFileLogger : IProjectLogger
    {
        private const string PrefixDefault = "ProjectName";

        private readonly string _dateFormat;
        private readonly Func<DateTime> _now;
        private readonly string _prefix;
        private readonly IDictionary<string, object> _pushProps = new Dictionary<string, object>();
        private readonly string _tempPath;

        public TempFileLogger(string dateFormat = null, Func<DateTime> now = null, string tempPath = null,
            string prefix = null)
        {
            _prefix = prefix ?? PrefixDefault;
            _now = now ?? (() => DateTime.UtcNow);
            _dateFormat = dateFormat ?? "yyyy-MM-dd";
            _tempPath = tempPath ?? Path.GetTempPath();
        }

        public void Verbose(string message, [CallerMemberName] string memberName = "")
        {
            LogAsync("verbose", message);
        }

        public void Debug(string message, [CallerMemberName] string memberName = "")
        {
            LogAsync("debug", message);
        }


        public void Info(string message, [CallerMemberName] string memberName = "")
        {
            LogAsync("info", message);
        }

        public void Warning(string message, [CallerMemberName] string memberName = "")
        {
            LogAsync("warning", message);
        }

        public void Error(string message, Exception ex, [CallerMemberName] string memberName = "")
        {
            LogAsync("error", message, ex);
        }

        public void Fatal(string message, Exception ex, [CallerMemberName] string memberName = "")
        {
            LogAsync("fatal", message, ex);
        }

        public IDisposable PushProperty(string name, object value)
        {
            _pushProps[name] = value;
            return new DisposableLambda(() =>
            {
                if (_pushProps.ContainsKey(name)) _pushProps.Remove(name);
            });
        }

        private void LogAsync(string severity, string message, Exception ex = null)
        {
            var now = _now();
            var fileName = Path.Combine(_tempPath, _prefix + "_" + now.ToString(_dateFormat) + ".log");
            var stackTrace = ex?.StackTrace ?? Environment.StackTrace;
            var formattedStackTrace = string.Join(
                $"{Environment.NewLine}{severity.ToUpper()}\t{now:yyyy-MM-dd HH:mm:ss}\t",
                stackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Skip(2));

            File.AppendAllText(fileName,
                $"{severity.ToUpper()}\t{now:yyyy-MM-dd HH:mm:ss}\t{message}{Environment.NewLine}");

            foreach (var prop in _pushProps)
                File.AppendAllText(fileName,
                    $"\t{severity.ToUpper()}\t{now:yyyy-MM-dd HH:mm:ss}\t[Key] {prop.Key} [Value] {prop.Value}{Environment.NewLine}");

            if (ex != null)
                File.AppendAllText(fileName,
                    $"\t{severity.ToUpper()}\t{now:yyyy-MM-dd HH:mm:ss}\t[{ex.GetType().Name}] {ex.Message}{Environment.NewLine}");

            File.AppendAllText(fileName,
                $"\t{severity.ToUpper()}\t{now:yyyy-MM-dd HH:mm:ss}\t{formattedStackTrace}{Environment.NewLine}");
        }

        // ReSharper disable once ClassWithVirtualMembersNeverInherited.Local
        private class DisposableLambda : IDisposable
        {
            private readonly Action _disposeLogic;

            public DisposableLambda(Action disposeLogic)
            {
                _disposeLogic = disposeLogic ?? (() => { }); // null object for simplicity
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                    _disposeLogic();
            }

            ~DisposableLambda()
            {
                Dispose(false);
            }
        }
    }
}