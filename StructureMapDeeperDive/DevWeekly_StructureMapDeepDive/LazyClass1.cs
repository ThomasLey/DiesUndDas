using System;
using NUnit.Framework;
using StructureMap;

namespace DevWeekly_StructureMapDeepDive1
{
    [TestFixture]
    public class LazyClass1
    {
        [Test]
        public void ContainerTest_MixEmUp()
        {
            var container = new Container(_ =>
            {
                _.For<ILogger>().Use<LoggerImpl1>().AlwaysUnique();
                _.For<Func<ILogger>>().Use(new Func<ILogger>(() => new LoggerImpl1())).AlwaysUnique();
                _.For<IMailer>().Use<OutlookMailer>().Singleton();
                _.For<ITaskMananger>().Use<SpTaskManager>().Transient(); // per "GetInstance""
                _.For<IEngine>().Use<Engine>().Transient();
            });

            //Console.WriteLine("Resolve Logger 1");
            //container.GetInstance<ILogger>();

            //Console.WriteLine("Resolve Mailer 1");
            //container.GetInstance<IMailer>();

            //Console.WriteLine("Resolve Mailer 2");
            //container.GetInstance<IMailer>();

            //Console.WriteLine("Resolve Tasker 1");
            //container.GetInstance<ITaskMananger>();

            //Console.WriteLine("Resolve Tasker 2");
            //container.GetInstance<ITaskMananger>();

            //Console.WriteLine("Resolve Engine 1");
            //container.GetInstance<IEngine>();

            Console.WriteLine("Resolve Engine 2");
            container.GetInstance<IEngine>();
        }
    }

    public interface IEngine
    {
    }

    public class Engine : IEngine
    {
        private readonly ITaskMananger _tasker;
        private readonly ILogger _logger;

        public Engine(Lazy<ITaskMananger> tasker, Func<ILogger> logger)
        {
            _tasker = tasker.Value ?? throw new ArgumentNullException(nameof(tasker));
            _logger = logger() ?? throw new ArgumentNullException(nameof(logger));

            Console.WriteLine($"Constructor of {GetType().Name} with #{GetHashCode()}");
            Console.WriteLine($"Constructor of {GetType().Name} with ILogger #{_logger.GetHashCode()}");
            Console.WriteLine($"Constructor of {GetType().Name} with ITasker #{_tasker.GetHashCode()}");

            Console.WriteLine("--New logger");
            Console.WriteLine($"Constructor of {GetType().Name} with ILogger #{logger().GetHashCode()}");
        }
    }

    public class SpTaskManager : ITaskMananger
    {
        private readonly IMailer _mailer;
        private readonly ILogger _logger;

        public SpTaskManager(IMailer mailer, Func<ILogger> logger)
        {
            _mailer = mailer ?? throw new ArgumentNullException(nameof(mailer));
            _logger = logger() ?? throw new ArgumentNullException(nameof(logger));

            // ReSharper disable once VirtualMemberCallInConstructor
            Console.WriteLine($"Constructor of {GetType().Name} with #{GetHashCode()}");
            Console.WriteLine($"Constructor of {GetType().Name} with ILogger #{_logger.GetHashCode()}");
            Console.WriteLine($"Constructor of {GetType().Name} with IMailer #{_mailer.GetHashCode()} (Logger is #{_mailer.Logger.GetHashCode()})");

            Console.WriteLine("--New logger");
            Console.WriteLine($"Constructor of {GetType().Name} with ILogger #{logger().GetHashCode()}");
        }

        public void CreateTask(string taskTitle)
        {
            // send mail
            _mailer.SendMail($"you got task: {taskTitle}");

            // log that it was created
            _logger.Log($"task was created with {taskTitle}");

            // do something cool e.g. Add to database, show notification
            Console.WriteLine($"I created a task: {taskTitle} ");
        }
    }

    public interface ITaskMananger
    {
        void CreateTask(string taskTitle);
    }

    public class OutlookMailer : IMailer
    {
        private readonly ILogger _logger;

        public OutlookMailer(Func<ILogger> logger)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Console.WriteLine($"Constructor of {GetType().Name} with #{GetHashCode()}");

            _logger = logger() ?? throw new ArgumentNullException(nameof(logger));
            Console.WriteLine($"Constructor of {GetType().Name} with ILogger #{_logger.GetHashCode()}");

            Console.WriteLine("--New logger");
            Console.WriteLine($"Constructor of {GetType().Name} with ILogger #{logger().GetHashCode()}");
        }

        public ILogger Logger => _logger;

        public void SendMail(string message)
        {
            _logger.Log($"outlook email sent with message {message}");
            Console.WriteLine($"Email '{message}' was sent to /dev/null");
        }
    }

    public interface IMailer
    {
        void SendMail(string message);
        ILogger Logger { get; }
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public class LoggerImpl1 : ILogger
    {
        public LoggerImpl1()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Console.WriteLine($"Constructor of {GetType().Name} with #{GetHashCode()}");
        }

        public void Log(string message)
        {
            Console.WriteLine($"Log:{message}");
        }
    }
}
