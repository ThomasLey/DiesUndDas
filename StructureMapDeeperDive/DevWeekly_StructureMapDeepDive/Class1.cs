using System;
using NUnit.Framework;
using StructureMap;

namespace DevWeekly_StructureMapDeepDive
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void ContainerTest1()
        {
            var container = new Container(_ =>
            {
                _.For<ILogger>().Use<LoggerImpl1>();
                _.For<IMailer>().Use<OutlookMailer>();
                _.For<ITaskMananger>().Use<SpTaskManager>();
            });

            Console.WriteLine("Resolve Logger");
            container.GetInstance<ILogger>();

            Console.WriteLine("Resolve Mailer");
            container.GetInstance<IMailer>();

            Console.WriteLine("Resolve Tasker");
            var taskManager = container.GetInstance<ITaskMananger>();
            taskManager.CreateTask("This is my injected task");
        }

        [Test]
        public void ContainerTest1_Singleton()
        {
            var container = new Container(_ =>
            {
                _.For<ILogger>().Use<LoggerImpl1>().Singleton();
                _.For<IMailer>().Use<OutlookMailer>().Singleton();
                _.For<ITaskMananger>().Use<SpTaskManager>().Singleton(); ;
            });

            Console.WriteLine("Resolve Logger");
            container.GetInstance<ILogger>();

            Console.WriteLine("Resolve Mailer");
            container.GetInstance<IMailer>();

            Console.WriteLine("Resolve Tasker");
            var taskManager = container.GetInstance<ITaskMananger>();
            taskManager.CreateTask("This is my injected task");
        }

        [Test]
        public void ContainerTest1_AlwaysUnique()
        {
            var container = new Container(_ =>
            {
                _.For<ILogger>().Use<LoggerImpl1>().AlwaysUnique();
                _.For<IMailer>().Use<OutlookMailer>().AlwaysUnique();
                _.For<ITaskMananger>().Use<SpTaskManager>().AlwaysUnique();
            });

            Console.WriteLine("Resolve Logger");
            container.GetInstance<ILogger>();

            Console.WriteLine("Resolve Mailer");
            container.GetInstance<IMailer>();

            Console.WriteLine("Resolve Tasker");
            var taskManager = container.GetInstance<ITaskMananger>();
            taskManager.CreateTask("This is my injected task");
        }

        [Test]
        public void ContainerTest2()
        {
            var container = new Container(_ =>
            {
                _.For<ILogger>().Use<LoggerImpl1>();
                _.For<IMailer>().Use<OutlookMailer>();
                _.For<ITaskMananger>().Use<SpTaskManager>();
            });

            container.GetInstance<ILogger>();
            container.GetInstance<ILogger>();

        }

        [Test]
        public void ContainerTest3()
        {
            var container = new Container(_ =>
            {
                _.For<ILogger>().Use<LoggerImpl1>().Singleton();
                _.For<IMailer>().Use<OutlookMailer>();
                _.For<ITaskMananger>().Use<SpTaskManager>();
            });

            //var l1 = container.GetInstance<ILogger>();
            //var l2 = container.GetInstance<ILogger>();

            //Console.WriteLine(l1.GetHashCode());
            //Console.WriteLine(l2.GetHashCode());

            container.GetInstance<ITaskMananger>();
            Console.WriteLine("---");
            container.GetInstance<ITaskMananger>();
        }

        [Test]
        public void ContainerTest_MixEmUp()
        {
            var container = new Container(_ =>
            {
                _.For<ILogger>().Use<LoggerImpl1>().AlwaysUnique();
                _.For<IMailer>().Use<OutlookMailer>().Singleton();
                _.For<ITaskMananger>().Use<SpTaskManager>().Transient(); // per "GetInstance""
                _.For<IEngine>().Use<Engine>().Transient();
            });

            Console.WriteLine("Resolve Logger 1");
            container.GetInstance<ILogger>();

            Console.WriteLine("Resolve Mailer 1");
            container.GetInstance<IMailer>();

            Console.WriteLine("Resolve Mailer 2");
            container.GetInstance<IMailer>();

            Console.WriteLine("Resolve Tasker 1");
            container.GetInstance<ITaskMananger>();

            Console.WriteLine("Resolve Tasker 2");
            container.GetInstance<ITaskMananger>();

            Console.WriteLine("Resolve Engine 1");
            container.GetInstance<IEngine>();

            Console.WriteLine("Resolve Engine 2");
            container.GetInstance<IEngine>();
        }

        [Test]
        public void TaskMgr()
        {
            var lgr = new LoggerImpl1();
            var mlr = new OutlookMailer(lgr);
            var mgr = new SpTaskManager(mlr, lgr);
            var eng = new Engine(mgr, lgr);

            mgr.CreateTask("go to work");
        }
    }

    public interface IEngine
    {
    }

    public class Engine : IEngine
    {
        private readonly ITaskMananger _tasker;
        private readonly ILogger _logger;

        public Engine(ITaskMananger tasker, ILogger logger)
        {
            _tasker = tasker ?? throw new ArgumentNullException(nameof(tasker));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            Console.WriteLine($"Constructor of {GetType().Name} with #{GetHashCode()}");
            Console.WriteLine($"Constructor of {GetType().Name} with ILogger #{_logger.GetHashCode()}");
            Console.WriteLine($"Constructor of {GetType().Name} with ITasker #{_tasker.GetHashCode()}");
        }
    }

    public class SpTaskManager : ITaskMananger
    {
        private readonly IMailer _mailer;
        private readonly ILogger _logger;

        public SpTaskManager(IMailer mailer, ILogger logger)
        {
            _mailer = mailer ?? throw new ArgumentNullException(nameof(mailer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // ReSharper disable once VirtualMemberCallInConstructor
            Console.WriteLine($"Constructor of {GetType().Name} with #{GetHashCode()}");
            Console.WriteLine($"Constructor of {GetType().Name} with ILogger #{_logger.GetHashCode()}");
            Console.WriteLine($"Constructor of {GetType().Name} with IMailer #{_mailer.GetHashCode()} (Logger is #{_mailer.Logger.GetHashCode()})");
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

        public OutlookMailer(ILogger logger)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Console.WriteLine($"Constructor of {GetType().Name} with #{GetHashCode()}");

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Console.WriteLine($"Constructor of {GetType().Name} with ILogger #{_logger.GetHashCode()}");
        }

        public ILogger Logger => _logger;

        public void SendMail(string message)
        {
            _logger.Log($"outlook email sent with message {message}");
            Console.WriteLine($"Email '{message}' was sent to /dev/null");
        }
    }

    public class NotesMailer : IMailer
    {
        public NotesMailer()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Console.WriteLine($"Constructor of {GetType().Name} with #{GetHashCode()}");
        }

        public ILogger Logger => null;

        public void SendMail(string message)
        {
            Console.WriteLine($"Message '{message}' was sent to /dev/null");
        }
    }

    public class DummyMailer : IMailer
    {
        public DummyMailer()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Console.WriteLine($"Constructor of {GetType().Name} with #{GetHashCode()}");
        }

        public void SendMail(string message)
        {
            SendCounter++;
        }

        public ILogger Logger => null;

        public int SendCounter { get; set; }
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
