using System;
using System.Runtime.CompilerServices;

namespace MagicBox.Logging
{
    public class EmptyProjectLogger : IProjectLogger
    {
        public void Verbose(string message, [CallerMemberName] string memberName = "")
        {
            // Do nothing because of the null-object pattern
        }

        public void Debug(string message, [CallerMemberName] string memberName = "")
        {
            // Do nothing because of the null-object pattern
        }

        public void Info(string message, [CallerMemberName] string memberName = "")
        {
            // Do nothing because of the null-object pattern
        }

        public void Warning(string message, [CallerMemberName] string memberName = "")
        {
            // Do nothing because of the null-object pattern
        }

        public void Error(string message, Exception ex, [CallerMemberName] string memberName = "")
        {
            // Do nothing because of the null-object pattern
        }

        public void Fatal(string message, Exception ex, [CallerMemberName] string memberName = "")
        {
            // Do nothing because of the null-object pattern
        }

        public IDisposable PushProperty(string name, object value)
        {
            return new DisposableNullObject();
        }

        #region class DisposableNullObject

        private class DisposableNullObject : IDisposable
        {
            public void Dispose()
            {
            }
        }

        #endregion
    }
}