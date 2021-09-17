using System;
using System.Diagnostics;

namespace MagicBox
{
    /// <summary>
    ///     Used to detect the current build compiling symbols
    /// </summary>
    public class ModeDetector
    {
        public static IModeDetector Current { get; private set; } = new DefaultModeDetector();

        public static bool IsDebug => Current.IsDebug;
        public static bool IsTrace => Current.IsTrace;
        public static bool IsRelease => Current.IsRelease;
        public static bool IsNcrunch => Current.IsNcrunch;
        public static bool IsResharper => Current.IsResharper;
        public static bool IsNUnit => Current.IsNUnit;
        public static bool UnitTesting => Current.UnitTesting;

        public static void AttachDebugger(Func<bool> when = null)
        {
            Current.AttachDebugger(when);
        }

        #region class DefaultModeDetector

        public class DefaultModeDetector : IModeDetector
        {
            private readonly string _processName = Process.GetCurrentProcess().ProcessName;

            public bool IsDebug
            {
                get
                {
#if (DEBUG)
                    // ReSharper disable once ConvertPropertyToExpressionBody
                    return true;
#else // ReSharper disable once ConvertPropertyToExpressionBody
                return false;
#endif
                }
            }

            public bool IsTrace
            {
                get
                {
#if (TRACE)
                    // ReSharper disable once ConvertPropertyToExpressionBody
                    return true;
#else // ReSharper disable once ConvertPropertyToExpressionBody
                return false;
#endif
                }
            }

            public bool IsRelease => !IsDebug;


            public bool IsNcrunch
            {
                get
                {
#if (NCRUNCH) // ReSharper disable once ConvertPropertyToExpressionBody
                return true;
#else
                    // ReSharper disable once ConvertPropertyToExpressionBody
                    return false;
#endif
                }
            }

            public bool IsResharper => _processName.StartsWith("JetBrains.ReSharper");

            public bool IsNUnit => _processName.StartsWith("nunit");

            public bool UnitTesting => IsNcrunch || IsNUnit || IsResharper;

            public void AttachDebugger(Func<bool> when = null)
            {
                var safeWhen = when ?? (() => !Debugger.IsAttached && IsDebug && !UnitTesting);
                if (safeWhen())
                    Debugger.Launch();
            }
        }

        #endregion

        #region interface IModeDetector

        public interface IModeDetector
        {
            bool IsDebug { get; }
            bool IsTrace { get; }
            bool IsRelease { get; }
            bool IsNcrunch { get; }
            bool IsResharper { get; }
            bool IsNUnit { get; }
            bool UnitTesting { get; }
            void AttachDebugger(Func<bool> when = null);
        }

        #endregion

        #region class TestWith

        public class TestWith : IDisposable
        {
            private readonly IModeDetector _currentTemp;

            // ReSharper disable ArrangeStaticMemberQualifier
            public TestWith(IModeDetector mock)
            {
                _currentTemp = Current;
                Current = mock;
            }

            public void Dispose()
            {
                Current = _currentTemp;
            }
            // ReSharper restore ArrangeStaticMemberQualifier
        }

        #endregion
    }
}