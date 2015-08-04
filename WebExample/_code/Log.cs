using System.IO;
using CodeFirstConfig;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

// ReSharper disable once CheckNamespace
namespace WebExample
{
    public class Log
    {
        private static ulong _id;
        private static readonly ILog Logger;
        private static readonly string FileName;
        private static readonly object LockObj = new object();

        static Log()
        {
            var layout = new PatternLayout("%property{id}. %logger %-5level %date - %message%newline%exception%newline");
            FileName = Path.Combine(App.Config.DataFolder, "LOG.TXT");
            var appender = new RollingFileAppender
            {
                File = FileName,
                AppendToFile = true,
                RollingStyle = RollingFileAppender.RollingMode.Composite,
                MaxSizeRollBackups = 0,                
                DatePattern = ".yyyy-MM-dd",
                Layout = layout,
                LockingModel = new FileAppender.MinimalLock()
            };
            layout.ActivateOptions();
            appender.ActivateOptions();
            BasicConfigurator.Configure(appender);
            Logger = LogManager.GetLogger(App.Config.InstanceId);
            _id = 0;
        }

        public static string Content() => File.ReadAllText(FileName);

        //public static string[] Lines() => File.ReadAllLines(FileName);

        public static void Info(object message)
        {
            lock (LockObj)
            {
                ThreadContext.Properties["id"] = _id++;
                Logger.Info(message);
            }                       
        }

        public static void Error(object message)
        {
            lock (LockObj)
            {
                ThreadContext.Properties["id"] = _id++;
                Logger.Error(message);
            }
        }
    }
}
