using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace IOCPServer
{
    class Program : Socket
    {
        ILog logger;

        public Program() :
        base
        (
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
        )
        {
            // Log Manager
            log4net.Repository.ILoggerRepository repository = LogManager.GetRepository();
            repository.Configured = true;

            // Console Log Setting
            ConsoleAppender consoleAppender = new ConsoleAppender();
            consoleAppender.Name = "Console";

            // Console Log Pattern
            consoleAppender.Layout = new PatternLayout("%d [%t] %-5p %c - %m%n");

            // File Log Pattern Setting
            RollingFileAppender rollingAppender = new RollingFileAppender();
            rollingAppender.Name = "RollingFile";
            rollingAppender.AppendToFile = true;
            rollingAppender.DatePattern = "-yyyy-MM-dd";
            rollingAppender.File = "IOCPServerLog.log";
            rollingAppender.RollingStyle = RollingFileAppender.RollingMode.Date;

            // File Log Pattern
            rollingAppender.Layout = new PatternLayout("%d [%t] %-5p %c - %m%n");

            Hierarchy hierarchy = (Hierarchy)repository;
            hierarchy.Root.AddAppender(consoleAppender);
            hierarchy.Root.AddAppender(rollingAppender);
            rollingAppender.ActivateOptions();

            // Log Level Setting
            hierarchy.Root.Level = log4net.Core.Level.All;
            logger = LogManager.GetLogger(this.GetType());

            logger.Info("Server Started.");

            Bind(new IPEndPoint(IPAddress.Any, 10000));
            logger.Info("Server Bind (Port - 10000)");

            Listen(20); // Queue length - 20
            logger.Info("Server Listen Start (Queue length - 20)");
            
            AcceptAsync(new Server(this));
        }

        static void Main(string[] args)
        {
            new Program();

            Console.WriteLine("Press the q key to exit.");

            while (true)
            {
                string k = Console.ReadLine();
                if (k.Equals("q") == true)
                {
                    break;
                }
            }
        }
    }
}
