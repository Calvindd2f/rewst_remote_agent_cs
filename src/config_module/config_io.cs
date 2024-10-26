using System;
using System.Text.Json;
using NLog;
using System.IO;
using System.Environment;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using NLog.Targets;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RewstRemoteAgent
{
    public static class ConfigIO
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly ConcurrentDictionary<string, string> ConfigCache = new ConcurrentDictionary<string, string>();

        public static string GetExecutableFolder(string orgId)
        {
            var osType = RuntimeInformation.OSDescription.ToLower();
            string executablePath;

            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            switch (osType)
            {
                case var _ when osType.Contains("windows"):
                    var programFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                    executablePath = Path.Combine(programFilesDir, "RewstRemoteAgent", orgId);
                    break;
                case var _ when osType.Contains("linux") || osType.Contains("darwin"):
                    executablePath = Path.Combine(homeDir, "RewstRemoteAgent", orgId);
                    break;
                default:
                    Logger.Error($"Unsupported OS type: {osType}. Send this output to Rewst for investigation!");
                    Environment.Exit(1);
                    return null;
            }

            return executablePath;
        }

        public static string GetServiceManagerPath(string orgId)
        {
            string executableName;
            var osType = RuntimeInformation.OSDescription.ToLower();

            if (osType.Contains("windows"))
            {
                executableName = $"rewst_service_manager.win_{orgId}.exe";
            }
            else if (osType.Contains("linux"))
            {
                executableName = $"rewst_service_manager.linux_{orgId}.bin";
            }
            else if (osType.Contains("darwin"))
            {
                executableName = $"rewst_service_manager.darwin_{orgId}.nim";
            }
            else
            {
                Logger.Error($"Unsupported OS type: {osType}. Send this output to Rewst for investigation!");
                Environment.Exit(1);
                return null;
            }
            string executablePath = GetExecutableFolder(orgId) + executableName;

            return executablePath;
        }

        public static string GetAgentExecutablePath(string orgId)
        {
            string executableName;
            var osType = RuntimeInformation.OSDescription.ToLower();

            switch (osType)
            {
                case var o when o.Contains("windows"):
                    executableName = $"rewst_service_manager.win_{orgId}.exe";
                    break;
                case var o when o.Contains("linux"):
                    executableName = $"rewst_service_manager.linux_{orgId}.bin";
                    break;
                case var o when o.Contains("darwin"):
                    executableName = $"rewst_service_manager.darwin_{orgId}.nim";
                    break;
                default:
                    Logger.Error($"Unsupported OS type: {osType}. Send this output to Rewst for investigation!");
                    Environment.Exit(1);
                    return null;
            }

            return executableName;
        }

        public static string GetServiceExecutablePath(string orgId)
        {
            var osType = RuntimeInformation.OSDescription.ToLower();
            if (osType.Contains("windows"))
            {
                var executableName = $"rewst_service_manager.win_{orgId}.exe";
                var executablePath = $"{GetExecutableFolder(orgId)}{executableName}";
                return executablePath;
            }
            else
            {
                Logger.Info($"Windows Service executable is only necessary for Windows, not {osType}");
                return null;
            }
        }

        public static string GetLoggingPath(string orgId)
        {
            var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var osType = RuntimeInformation.OSDescription.ToLower();
            var logFilename = "rewst_agent.log";

            string logPath;
            if (osType.Contains("windows"))
            {
                logPath = $"{baseDir}\\RewstRemoteAgent\\{orgId}\\logs\\{logFilename}";
            }
            else if (osType.Contains("linux"))
            {
                logPath = $"{baseDir}/RewstRemoteAgent/{orgId}/logs/{logFilename}";
            }
            else if (osType.Contains("darwin"))
            {
                logPath = $"{baseDir}/RewstRemoteAgent/{orgId}/logs/{logFilename}";
            }
            else
            {
                Logger.Error($"Unsupported OS type: {osType}. Send this output to Rewst for investigation!");
                Environment.Exit(1);
                return null;
            }
            Logger.Info($"Logging to: {logPath}");

            return logPath;
        }

        public static string GetConfigFilePath(string orgId)
        {
            var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var osType = RuntimeInformation.OSDescription.ToLower();
            Logger.Info($"Returning {osType} config file path.");

            string configDir;
            switch (osType)
            {
                case var o when o.Contains("windows"):
                    configDir = $"{baseDir}\\RewstRemoteAgent\\{orgId}";
                    break;
                case var o when o.Contains("linux"):
                    configDir = $"/etc/rewst_remote_agent/{orgId}/";
                    break;
                case var o when o.Contains("darwin"):
                    configDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Library/Application Support/RewstRemoteAgent/{orgId}/";
                    break;
                default:
                    Logger.Error($"Unsupported OS type: {osType}. Send this output to Rewst for investigation!");
                    Environment.Exit(1);
                    return null;
            }

            Logger.Info($"Path: {configDir}");

            if (!Directory.Exists(configDir))
            {
                try
                {
                    Directory.CreateDirectory(configDir);
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to create directory {configDir}: {e.Message}");
                    throw;
                }
            }

            var configFilePath = Path.Combine(configDir, "config.json");
            Logger.Info($"Config File Path: {configFilePath}");
            return configFilePath;
        }

        public static async Task SaveConfigurationAsync(object configData, string configFilePath = null)
        {
            var orgId = configData.GetType().GetProperty("rewst_org_id").GetValue(configData, null).ToString();
            configFilePath = GetConfigFilePath(orgId);

            if (string.IsNullOrEmpty(configFilePath))
            {
                throw new ArgumentException("Configuration file path cannot be null or empty.", nameof(configFilePath));
            }

            var jsonData = JsonSerializer.Serialize(configData, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(configFilePath, jsonData);
            ConfigCache[configFilePath] = jsonData;
            Console.WriteLine($"Configuration saved to {configFilePath}");
        }

        public static async Task<string> LoadConfigurationAsync(string orgId = null, string configFilePath = null)
        {
            if (string.IsNullOrEmpty(configFilePath))
            {
                throw new ArgumentException("Configuration file path cannot be null or empty.", nameof(configFilePath));
            }

            if (ConfigCache.TryGetValue(configFilePath, out var cachedConfig))
            {
                return cachedConfig;
            }

            if (!File.Exists(configFilePath))
            {
                throw new FileNotFoundException("Configuration file not found.", configFilePath);
            }

            try
            {
                string jsonString = await File.ReadAllTextAsync(configFilePath);
                ConfigCache[configFilePath] = jsonString;
                return jsonString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                return null;
            }
        }

        public static string GetOrgIdFromExecutableName(string commandlineArgs)
        {
            var pattern = @"rewst_.*_(.+?)\.";
            var regex = new Regex(pattern);
            var match = regex.Match(commandlineArgs);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return null;
            }
        }

        public static bool SetupFileLogging(string orgId = null)
        {
            string logFilePath = GetLoggingPath(orgId);
            Console.WriteLine($"Configuring logging to file: {logFilePath}"); // Debug print

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

                var config = new LoggingConfiguration();
                var fileTarget = new FileTarget("logfile")
                {
                    FileName = logFilePath,
                    MaxArchiveFiles = 3,
                    ArchiveEvery = FileArchivePeriod.Day,
                    ArchiveNumbering = ArchiveNumberingMode.Date,
                    Layout = "${longdate} ${level} ${message}"
                };
                config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
                LogManager.Configuration = config;

                Logger.Info("File Logging initialized.");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception setting up file logging: {e}"); // Debug print
                return false;
            }
        }

        static void Main(string[] args)
        {
            string orgId = "YourOrgId";
            string executableFolder = GetExecutableFolder(orgId);
            Console.WriteLine($"Executable Folder: {executableFolder}");
        }
    }
}
