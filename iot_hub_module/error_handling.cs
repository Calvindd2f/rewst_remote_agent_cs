using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Runtime.InteropServices;
using LogLevel = NLog.LogLevel;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Rewst.RemoteAgent.IOIHUB
{
    public static class Logger
    {
        private static ILogger? _logger;

        public static void SetupLogging(string appName)
        {
            var config = new LoggingConfiguration();

            var logLevel = LogLevel.Info;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var eventLogTarget = new EventLogTarget("EventLog")
                {
                    Source = appName,
                    Log = "Application",
                    Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}"
                };
                config.AddTarget(eventLogTarget);
                config.AddRule(logLevel, LogLevel.Fatal, eventLogTarget);
            }
            else
            {
                var consoleTarget = new ConsoleTarget("console")
                {
                    Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}"
                };
                config.AddTarget(consoleTarget);
                config.AddRule(logLevel, LogLevel.Fatal, consoleTarget);

                var fileTarget = new FileTarget("file")
                {
                    FileName = $"{appName}.log",
                    Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}"
                };
                config.AddTarget(fileTarget);
                config.AddRule(logLevel, LogLevel.Fatal, fileTarget);
            }

            LogManager.Configuration = config;

            var factory = LoggerFactory.Create(builder =>
            {
                builder.AddNLog(config);
            });

            _logger = factory.CreateLogger(appName);
        }

        public static void LogError(string errorMessage)
        {
            _logger.LogError(errorMessage);
        }

        public static void LogInfo(string infoMessage)
        {
            _logger.LogInformation(infoMessage);
        }

        public static void LogWarning(string warningMessage)
        {
            _logger.LogWarning(warningMessage);
        }

        public static void LogDebug(string debugMessage)
        {
            _logger.LogDebug(debugMessage);
        }
    }
}
