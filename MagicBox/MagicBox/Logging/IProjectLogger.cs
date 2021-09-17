using System;
using System.Runtime.CompilerServices;

namespace MagicBox.Logging
{
    public interface IProjectLogger
    {
        void Verbose(string message, [CallerMemberName] string memberName = "");

        void Debug(string message, [CallerMemberName] string memberName = "");

        void Info(string message, [CallerMemberName] string memberName = "");

        void Warning(string message, [CallerMemberName] string memberName = "");

        void Error(string message, Exception ex, [CallerMemberName] string memberName = "");

        void Fatal(string message, Exception ex, [CallerMemberName] string memberName = "");

        IDisposable PushProperty(string name, object value);
    }
}