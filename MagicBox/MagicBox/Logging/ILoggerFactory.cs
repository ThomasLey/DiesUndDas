using MagicBox.Logging.Settings;

namespace MagicBox.Logging
{
    public interface ILoggerFactory
    {
        IProjectLogger Build(ILoggerSettings settings);
    }
}