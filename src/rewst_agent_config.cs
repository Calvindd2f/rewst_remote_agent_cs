using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Rewst.RemoteAgent
{
    private static string Os_type()
    {
        var platform = Environment.OSVersion.Platform;
        return platform.ToString().ToLower();
    }

    private var OsType = os_type();
    public static void OutputEnvironmentInfo()
    {
        string osInfo = Environment.OSVersion.ToString();
        Debug.WriteLine($"Running on {osInfo}");

        string versionString = $"v{Assembly.GetExecutingAssembly().GetName().Version}";
        Debug.WriteLine("Rewst Agent Configuration Tool " + versionString);
    };


    public static bool IsValidUrl(string url)
    {
        try
        {
            var result = new Uri(url);
            return !string.IsNullOrEmpty(result.Scheme) && !string.IsNullOrEmpty(result.Host);
        }
        catch (UriFormatException)
        {
            Debug.WriteLine($"The provided string {url} is not a valid URL");
            return false;
        }
    };

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
            Console.WriteLine(e);
            return false;
        }
    };

    public static async Task RemoveOldFiles(string orgId)
    {
        string serviceManagerPath = GetServiceManagerPath(orgId);
        string agentExecutablePath = GetAgentExecutablePath(orgId);
        List<string> filePaths = new List<string> { serviceManagerPath, agentExecutablePath };

        if (osType == "win32")
        {
            string serviceExecutablePath = GetServiceExecutablePath(orgId);
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
                    Console.WriteLine($"Renamed {filePath} to {newFilePath}");
                }
                catch (IOException e)
                {
                    Console.WriteLine($"Error renaming file {filePath}: {e.Message}");
                }
            }
        }
    }

    public static async Task<bool> WaitForFiles(string orgId, int timeout = 3600)
    {
        Console.WriteLine("Waiting for files to be written...");
        string serviceManagerPath = GetServiceManagerPath(orgId);
        Console.WriteLine($"Awaiting Service Manager File: {serviceManagerPath} ...");

        string agentExecutablePath = GetAgentExecutablePath(orgId);
        Console.WriteLine($"Awaiting Agent Service File: {agentExecutablePath} ...");

        List<string> filePaths = new List<string> { serviceManagerPath, agentExecutablePath };

        if (osType == "win32")
        {
            string serviceExecutablePath = GetServiceExecutablePath(orgId);
            Console.WriteLine($"Awaiting Service Executable File: {serviceExecutablePath} ...");
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
                Console.WriteLine("All files have been written.");
                return true;
            }

            if ((DateTime.Now - startTime).TotalSeconds > timeout)
            {
                Console.WriteLine("Timeout reached while waiting for files.");
                return false;
            }

            await Task.Delay(5000);
        }
    }

    public static async Task<bool> InstallAndStartService(string orgId)
    {
        string serviceManagerPath = GetServiceManagerPath(orgId);

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
                    Console.WriteLine("Service installed successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to install the service: {error}");
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to install the service: {e.Message}");
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
                    Console.WriteLine("Service started successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to start the service.");
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to start the service: {e.Message}");
            return false;
        }

        return true;
    }

    public static async Task<bool> CheckServiceStatus(string orgId)
    {
        string serviceManagerPath = GetServiceManagerPath(orgId);

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

                Console.WriteLine($"Service status: {output}");
                return output.ToLower().Contains("running");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to check the service status: {e.Message}");
            return false;
        }
    }

    public static void EndProgram(int exitLevel = 1)
    {
        Console.WriteLine($"Agent configuration is exiting with exit level {exitLevel}.");
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
            Console.WriteLine("The config URL provided is not valid.");
            EndProgram(1);
        }
        if (!IsBase64(configSecret))
        {
            Console.WriteLine("The config secret provided is not a valid base64 string.");
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
                Console.WriteLine("Error: Missing required parameters.");
                Console.WriteLine(
                    "Please make sure '--config-url', '--org-id' and '--config-secret' are provided."
                );
                EndProgram(1);
            }

            Console.WriteLine("Fetching configuration from Rewst...");
            var urlOrgId = configUrl.Split('/').Last();
            var configData = await FetchConfiguration(configUrl, configSecret, orgId);
            if (configData == null)
            {
                Console.WriteLine("Failed to fetch configuration.");
                EndProgram(2);
            }

            Console.WriteLine("Saving configuration to file...");
            SaveConfiguration(configData);

            Console.WriteLine($"Configuration: {JsonSerializer.Serialize(configData)}");

            orgId = configData["rewst_org_id"].ToString();
            Console.WriteLine($"Organization ID: {orgId}");

            var connectionManager = new ConnectionManager(configData);

            Console.WriteLine("Connecting to IoT Hub...");
            await connectionManager.Connect();

            Console.WriteLine("Setting up message handler...");
            await connectionManager.SetMessageHandler();

            await RemoveOldFiles(orgId);

            await WaitForFiles(orgId);

            Console.WriteLine("Disconnecting from IoT Hub...");
            await connectionManager.Disconnect();
            await Task.Delay(4000);
            Console.WriteLine("Disconnected from IoT Hub.");

            while (!await CheckServiceStatus(orgId))
            {
                Console.WriteLine("Waiting for the service to start...");
                await Task.Delay(5000);
            }

            EndProgram(0);
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }

    public static void Main(string[] args)
    {
        Task.Run(() => MainAsync(args)).GetAwaiter().GetResult();
    }
}