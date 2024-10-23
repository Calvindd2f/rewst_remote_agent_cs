using System;
using System.Text.Json;
using NLog;
using System.IO;
using System.Environment;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using NLog.Targets;

namespace RewstRemoteAgent
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(){};

    public static string get_executable_folder(string orgId)
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

    public static string get_service_manager_path(string orgId)
    {
        string executable_name;
        if (osType.Contains("windows"))
        {
            executable_name = $"rewst_service_manager.win_{orgId}.exe";
        }
        else if (osType.Contains("linux"))
        {
            executable_name = $"rewst_service_manager.linux_{orgId}.bin";
        }
        else if (osType.Contains("darwin"))
        {
            executable_name = $"rewst_service_manager.darwin_{orgId}.nim";
        }
        else
        {
            Logger.Error($"Unsupported OS type: {osType}. Send this output to Rewst for investigation!");
            Environment.Exit(1);
            return null;
        }
        string executable_path = get_executable_folder(orgId) + executable_name;

        return executable_path;
    }

    public static string get_agent_executable_path(string orgId)
    {
        string executableName;

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

    public static string get_service_executable_path(string orgId)
    {
        if (osType.Contains("windows"))
        {
            var executable_name = $"rewst_service_manager.win_{org_id}.exe"
            var executable_path = $"{get_executable_folder(org_id)}{executable_name}"
        }
        else
        {
            logging.info($"Windows Service executable is only necessary for Windows, not {os_type}")

            executable_path = None
        }
        return executable_path
    }

    public static string get_logging_path(string orgId)
    {
        var base_dir = site_config_dir()

        var log_filename = "rewst_agent.log"

        if (osType.Contains("windows"))
        {
            var log_path = $"{base_dir}\\RewstRemoteAgent\\{org_id}\\logs\\{log_filename}"
        }
        elseif(osType.Contains("linux"))
        {
            var log_path = $"{base_dir}\\RewstRemoteAgent\\{org_id}\\logs\\{log_filename}"
        }
        elseif(osType.Contains("darwin"))
        {
            var log_path = $"{base_dir}\\RewstRemoteAgent\\{org_id}\\logs\\{log_filename}"
        }
        else
        {
            logging.error($"Unsupported OS type: {os_type}. Send this output to Rewst for investigation!")
            exit(1)
        }
        logging.info($"Logging to: {log_path}")

        return log_path
    };

    public static string get_config_file_path(string orgId)
    {
        var baseDir = site_config_dir();
        Logging.Info($"Returning {osType} config file path.");

        string configDir;
        switch (osType)
        {
            case "windows":
                configDir = $"{baseDir}//RewstRemoteAgent//{orgId}";
                break;
            case "linux":
                configDir = "/etc/rewst_remote_agent/{orgId}/";
                break;
            case "darwin":
                configDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Library/Application Support/RewstRemoteAgent/{orgId}/";
                break;
            default:
                Logging.Error($"Unsupported OS type: {osType}. Send this output to Rewst for investigation!");
                Environment.Exit(1);
                return null; // Unreachable code, added for completeness
        }

        Logging.Info($"Path: {configDir}");

        if (!Directory.Exists(configDir))
        {
            try
            {
                Directory.CreateDirectory(configDir);
            }
            catch (Exception e)
            {
                Logging.Error($"Failed to create directory {configDir}: {e.Message}");
                throw;
            }
        }

        var configFilePath = Path.Combine(configDir, "config.json");
        Logging.Info($"Config File Path: {configFilePath}");
        return configFilePath;
    };

    public static string save_configuration(string config_data, string config_file= null)
    {
        var org_id  = config_data.rewst_org_id;
        var config_file_path =  get_config_file_path(org_id);

        if (string.IsNullOrEmpty(configData))
        {
            throw new ArgumentException("Configuration file path cannot be null or empty.", nameof(configFilePath));
        }

        File.WriteAllText(configFilePath, JsonConvert.SerializeObject(configData, Formatting.Indented));
        Console.WriteLine($"Configuration saved to {configFilePath}");
    };

    public static async Task<string> loaf_configuration(string orgId = null, string configFilePath = null)
    {
        if (string.IsNullOrEmpty(configFilePath))
        {
            throw new ArgumentException("Configuration file path cannot be null or empty.", nameof(configFilePath));
        }

        if (!File.Exists(configFilePath))
        {
            throw new FileNotFoundException("Configuration file not found.", configFilePath);
        }

        try
        {
            string jsonString = await File.ReadAllTextAsync(configFilePath);

            // Optionally, you can deserialize the JSON string into a specific object
            // var configData = JsonSerializer.Deserialize<YourConfigType>(jsonString);

            return jsonString;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration: {ex.Message}");
            return null;
        }
    };

    public static string get_org_id_from_executable_name(string commandline_args)
    {
        var pattern = @"rewst_.*_(.+?)\.";
        var regex = new Regex(pattern);
        var match = regex.Match(executablePath);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        else
        {
            return null;
        }
    };

    public static string setup_file_logging(string org_id=null)
    {
        string log_file_path = get_logging_path(org_id)
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

            logger.Info("File Logging initialized.");
            return true;
        }
        catch (System.Exception e)
        {
            Console.WriteLine($"Exception setting up file logging: {e}"); // Debug print
            return false;
        }
    };

    static void Main(string[] args)
    {
        string orgId = "YourOrgId";
        string executableFolder = GetExecutableFolder(orgId);
        Console.WriteLine($"Executable Folder: {executableFolder}");
    }
}