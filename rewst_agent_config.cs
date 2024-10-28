using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Rewst.RemoteAgent
{
    public static class RewstAgentConfig
    {
        private static readonly ILogger<RewstAgentConfig> Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<RewstAgentConfig>();

        private static string OsType()
        {
            var platform = Environment.OSVersion.Platform;
            return platform.ToString().ToLower();
        }

        private static readonly string OsType = OsType();

        public static void OutputEnvironmentInfo()
        {
            string osInfo = Environment.OSVersion.ToString();
            LogUtil.LogInfo($"Running on {osInfo}");

            string versionString = $"v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
            LogUtil.LogInfo("Rewst Agent Configuration Tool " + versionString);
        }

        public static bool IsValidUrl(string url)
        {
            try
            {
                var result = new Uri(url);
                return !string.IsNullOrEmpty(result.Scheme) && !string.IsNullOrEmpty(result.Host);
            }
            catch (UriFormatException)
            {
                LogUtil.LogError($"The provided string {url} is not a valid URL");
                return false;
            }
        }

        public static bool IsBase64(string sb)
        {
            try
            {
                if (!string.IsNullOrEmpty(sb))
                {
                    sb = sb.Trim();
                }
                return Regex.IsMatch(sb, "^[A-Za-z0-9+/]+={0,2}$");
            }
            catch (Exception e)
            {
                LogUtil.LogError(e, "Error checking if string is Base64");
                return false;
            }
        }

        public static async Task RemoveOldFiles(string orgId)
        {
            string serviceManagerPath = ConfigIO.GetServiceManagerPath(orgId);
            string agentExecutablePath = ConfigIO.GetAgentExecutablePath(orgId);
            List<string> filePaths = new List<string> { serviceManagerPath, agentExecutablePath };

            if (OsType == "win32")
            {
                string serviceExecutablePath = ConfigIO.GetServiceExecutablePath(orgId);
                filePaths.Add(serviceExecutablePath);
            }

            foreach (var filePath in filePaths)
            {
                if (File.Exists(filePath))
                {
                    string newFilePath = $"{filePath}_oldver";
                    try
                    {
                        if (File.Exists(newFilePath))
                        {
                            File.Delete(newFilePath);
                        }
                        File.Move(filePath, newFilePath);
                        LogUtil.LogInfo($"Renamed {filePath} to {newFilePath}");
                    }
                    catch (IOException e)
                    {
                        LogUtil.LogError(e, $"Error renaming file {filePath}");
                    }
                }
            }
        }

        public static async Task<bool> WaitForFiles(string orgId, int timeout = 3600)
        {
            LogUtil.LogInfo("Waiting for files to be written...");
            string serviceManagerPath = ConfigIO.GetServiceManagerPath(orgId);
            LogUtil.LogInfo($"Awaiting Service Manager File: {serviceManagerPath} ...");

            string agentExecutablePath = ConfigIO.GetAgentExecutablePath(orgId);
            LogUtil.LogInfo($"Awaiting Agent Service File: {agentExecutablePath} ...");

            List<string> filePaths = new List<string> { serviceManagerPath, agentExecutablePath };

            if (OsType == "win32")
            {
                string serviceExecutablePath = ConfigIO.GetServiceExecutablePath(orgId);
                LogUtil.LogInfo($"Awaiting Service Executable File: {serviceExecutablePath} ...");
                filePaths.Add(serviceExecutablePath);
            }

            DateTime startTime = DateTime.Now;

            while (true)
            {
                bool allFilesExist = true;
                foreach (var filePath in filePaths)
                {
                    if (!File.Exists(filePath))
                    {
                        allFilesExist = false;
                        break;
                    }
                }

                if (allFilesExist)
                {
                    await Task.Delay(20000);
                    LogUtil.LogInfo("All files have been written.");
                    return true;
                }

                if ((DateTime.Now - startTime).TotalSeconds > timeout)
                {
                    LogUtil.LogError("Timeout reached while waiting for files.");
                    return false;
                }

                await Task.Delay(5000);
            }
        }

        public static async Task<bool> InstallAndStartService(string orgId)
        {
            string serviceManagerPath = ConfigIO.GetServiceManagerPath(orgId);

            var installCommand = new ProcessStartInfo
            {
                FileName = serviceManagerPath,
                Arguments = $"--org-id {orgId} --install",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            try
            {
                using (var process = Process.Start(installCommand))
                {
                    var output = await process.StandardOutput.ReadToEndAsync();
                    var error = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    if (process.ExitCode == 0)
                    {
                        LogUtil.LogInfo("Service installed successfully.");
                    }
                    else
                    {
                        LogUtil.LogError($"Failed to install the service: {error}");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.LogError(e, "Failed to install the service");
                return false;
            }

            var startCommand = new ProcessStartInfo
            {
                FileName = serviceManagerPath,
                Arguments = $"--org-id {orgId} --start",
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            try
            {
                using (var process = Process.Start(startCommand))
                {
                    await process.WaitForExitAsync();
                    if (process.ExitCode == 0)
                    {
                        LogUtil.LogInfo("Service started successfully.");
                    }
                    else
                    {
                        LogUtil.LogError("Failed to start the service.");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.LogError(e, "Failed to start the service");
                return false;
            }

            return true;
        }

        public static async Task<bool> CheckServiceStatus(string orgId)
        {
            string serviceManagerPath = ConfigIO.GetServiceManagerPath(orgId);

            var statusCommand = new ProcessStartInfo
            {
                FileName = serviceManagerPath,
                Arguments = $"--org-id {orgId} --status",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            try
            {
                using (var process = Process.Start(statusCommand))
                {
                    var output = await process.StandardOutput.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    LogUtil.LogInfo($"Service status: {output}");
                    return output.ToLower().Contains("running");
                }
            }
            catch (Exception e)
            {
                LogUtil.LogError(e, "Failed to check the service status");
                return false;
            }
        }

        public static void EndProgram(int exitLevel = 1)
        {
            LogUtil.LogInfo($"Agent configuration is exiting with exit level {exitLevel}.");
            Environment.Exit(exitLevel);
        }

        public static async Task Main(string[] args)
        {
            string configUrl = null;
            string configSecret = null;
            string orgId = null;

            foreach (var arg in args)
            {
                if (arg.StartsWith("--config-url="))
                    configUrl = arg.Substring("--config-url=".Length);
                else if (arg.StartsWith("--config-secret="))
                    configSecret = arg.Substring("--config-secret=".Length);
                else if (arg.StartsWith("--org-id="))
                    orgId = arg.Substring("--org-id=".Length);
            }

            OutputEnvironmentInfo();

            if (!IsValidUrl(configUrl))
            {
                LogUtil.LogError("The config URL provided is not valid.");
                EndProgram(1);
            }
            if (!IsBase64(configSecret))
            {
                LogUtil.LogError("The config secret provided is not a valid base64 string.");
                EndProgram(1);
            }

            try
            {
                if (
                    string.IsNullOrEmpty(configUrl)
                    || string.IsNullOrEmpty(configSecret)
                    || string.IsNullOrEmpty(orgId)
                )
                {
                    LogUtil.LogError("Error: Missing required parameters.");
                    LogUtil.LogError(
                        "Please make sure '--config-url', '--org-id' and '--config-secret' are provided."
                    );
                    EndProgram(1);
                }

                LogUtil.LogInfo("Fetching configuration from Rewst...");
                var urlOrgId = configUrl.Split('/').Last();
                var configData = await FetchConfiguration(configUrl, configSecret, orgId);
                if (configData == null)
                {
                    LogUtil.LogError("Failed to fetch configuration.");
                    EndProgram(2);
                }

                LogUtil.LogInfo("Saving configuration to file...");
                SaveConfiguration(configData);

                LogUtil.LogInfo($"Configuration: {JsonSerializer.Serialize(configData)}");

                orgId = configData["rewst_org_id"].ToString();
                LogUtil.LogInfo($"Organization ID: {orgId}");

                var connectionManager = new ConnectionManager(configData);

                LogUtil.LogInfo("Connecting to IoT Hub...");
                await connectionManager.Connect();

                LogUtil.LogInfo("Setting up message handler...");
                await connectionManager.SetMessageHandler();

                await RemoveOldFiles(orgId);

                await WaitForFiles(orgId);

                LogUtil.LogInfo("Disconnecting from IoT Hub...");
                await connectionManager.Disconnect();
                await Task.Delay(4000);
                LogUtil.LogInfo("Disconnected from IoT Hub.");

                while (!await CheckServiceStatus(orgId))
                {
                    LogUtil.LogInfo("Waiting for the service to start...");
                    await Task.Delay(5000);
                }

                EndProgram(0);
            }
            catch (Exception e)
            {
                LogUtil.LogError(e, "An error occurred");
            }
        }

        private static async Task<Dictionary<string, object>> FetchConfiguration(string configUrl, string configSecret, string orgId)
        {
            var fetcher = new ConfigurationFetcher(Logger);
            return await fetcher.FetchConfigurationAsync(configUrl, configSecret, orgId);
        }

        private static void SaveConfiguration(Dictionary<string, object> configData)
        {
            var configFilePath = ConfigIO.GetConfigFilePath(configData["rewst_org_id"].ToString());
            ConfigIO.SaveConfigurationAsync(configData, configFilePath).Wait();
        }
    }
}
