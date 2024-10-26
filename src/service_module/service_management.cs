using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Rewst.RemoteAgent
{
    public interface IServiceManager
    {
        private static readonly string OsType = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows" :
                                                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" :
                                                RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "darwin" : "unknown";

        private static readonly ILogger<IServiceManager> Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<IServiceManager>();

        public static string GetServiceName(string orgId)
        {
            return OsType == "windows" ? "RewstWindowsService" : $"RewstRemoteAgent_{orgId}";
        }

        public static bool IsServiceInstalled(string orgId)
        {
            var serviceName = GetServiceName(orgId);
            switch (OsType)
            {
                case "windows":
                    return ServiceController.GetServices().Any(s => s.ServiceName == serviceName);
                case "linux":
                    return File.Exists($"/etc/systemd/system/{serviceName}.service");
                case "darwin":
                    return File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Library/LaunchAgents/{serviceName}.plist");
                default:
                    throw new PlatformNotSupportedException($"Unsupported OS type: {OsType}");
            }
        }

        public static bool IsServiceRunning(string orgId)
        {
            var executablePath = GetAgentExecutablePath(orgId);
            var executableName = Path.GetFileName(executablePath);
            return Process.GetProcessesByName(executableName).Length > 0;
        }

        public static async Task InstallService(string orgId)
        {
            var executablePath = GetAgentExecutablePath(orgId);
            var serviceName = GetServiceName(orgId);
            var configFilePath = GetConfigFilePath(orgId);

            Logger.LogInformation($"Installing {serviceName} Service...");

            if (IsServiceInstalled(orgId))
            {
                Logger.LogInformation("Service is already installed.");
                return;
            }

            switch (OsType)
            {
                case "windows":
                    Logger.LogInformation($"Installing Windows Service: {serviceName}");
                    var windowsServicePath = GetServiceExecutablePath(orgId);
                    await Task.Run(() => Process.Start(windowsServicePath, "install").WaitForExit());
                    break;

                case "linux":
                    var systemdServiceContent = $@"
                    [Unit]
                    Description={serviceName}

                    [Service]
                    ExecStart={executablePath} --config-file {configFilePath}
                    Restart=always

                    [Install]
                    WantedBy=multi-user.target
                    ";
                    await File.WriteAllTextAsync($"/etc/systemd/system/{serviceName}.service", systemdServiceContent);
                    await Task.WhenAll(
                        Task.Run(() => Process.Start("systemctl", "daemon-reload").WaitForExit()),
                        Task.Run(() => Process.Start("systemctl", $"enable {serviceName}").WaitForExit())
                    );
                    break;

                case "darwin":
                    var launchdPlistContent = $@"
                    <?xml version=""1.0"" encoding=""UTF-8""?>
                    <!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
                    <plist version=""1.0"">
                    <dict>
                        <key>Label</key>
                        <string>{serviceName}</string>
                        <key>ProgramArguments</key>
                        <array>
                            <string>{executablePath}</string>
                            <string>--config-file</string>
                            <string>{configFilePath}</string>
                        </array>
                        <key>RunAtLoad</key>
                        <true/>
                    </dict>
                    </plist>
                    ";
                    var plistPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Library/LaunchAgents/{serviceName}.plist";
                    await File.WriteAllTextAsync(plistPath, launchdPlistContent);
                    await Task.Run(() => Process.Start("launchctl", $"load {plistPath}").WaitForExit());
                    break;

                default:
                    throw new PlatformNotSupportedException($"Unsupported OS type: {OsType}");
            }
        }

        public static async Task UninstallService(string orgId)
        {
            var serviceName = GetServiceName(orgId);
            Logger.LogInformation($"Uninstalling service {serviceName}.");

            try
            {
                await StopService(orgId);
            }
            catch (Exception e)
            {
                Logger.LogError($"Unable to stop service: {e.Message}");
            }

            switch (OsType)
            {
                case "windows":
                    try
                    {
                        await Task.Run(() => Process.Start("sc", $"delete {serviceName}").WaitForExit());
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Exception removing service: {e.Message}");
                    }
                    break;

                case "linux":
                    await Task.WhenAll(
                        Task.Run(() => Process.Start("systemctl", $"disable {serviceName}").WaitForExit()),
                        Task.Run(() => File.Delete($"/etc/systemd/system/{serviceName}.service")),
                        Task.Run(() => Process.Start("systemctl", "daemon-reload").WaitForExit())
                    );
                    break;

                case "darwin":
                    var plistPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Library/LaunchAgents/{serviceName}.plist";
                    await Task.WhenAll(
                        Task.Run(() => Process.Start("launchctl", $"unload {plistPath}").WaitForExit()),
                        Task.Run(() => File.Delete(plistPath))
                    );
                    break;

                default:
                    throw new PlatformNotSupportedException($"Unsupported OS type: {OsType}");
            }
        }

        public static async Task CheckServiceStatus(string orgId)
        {
            var serviceName = GetServiceName(orgId);

            switch (OsType)
            {
                case "windows":
                    try
                    {
                        using var service = new ServiceController(serviceName);
                        Logger.LogInformation($"Service status: {service.Status}");
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Error: {e.Message}");
                    }
                    break;

                case "linux":
                    try
                    {
                        var process = await Task.Run(() => Process.Start(new ProcessStartInfo("systemctl", $"is-active {serviceName}")
                        {
                            RedirectStandardOutput = true,
                            UseShellExecute = false
                        }));
                        Logger.LogInformation($"Service status: {await process.StandardOutput.ReadToEndAsync().Trim()}");
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Error: {e.Message}");
                    }
                    break;

                case "darwin":
                    try
                    {
                        var process = await Task.Run(() => Process.Start(new ProcessStartInfo("launchctl", $"list {serviceName}")
                        {
                            RedirectStandardOutput = true,
                            UseShellExecute = false
                        }));
                        var output = await process.StandardOutput.ReadToEndAsync();
                        Logger.LogInformation($"Service status: {(output.Contains(serviceName) ? "Running" : "Not Running")}");
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Error: {e.Message}");
                    }
                    break;

                default:
                    Logger.LogError($"Unsupported OS type: {OsType}");
                    break;
            }
        }

        public static async Task StartService(string orgId)
        {
            var serviceName = GetServiceName(orgId);
            Logger.LogInformation($"Starting Service {serviceName} for {OsType}");

            switch (OsType)
            {
                case "windows":
                    using (var service = new ServiceController(serviceName))
                    {
                        service.Start();
                        await Task.Run(() => service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30)));
                    }
                    break;

                case "linux":
                    await Task.Run(() => Process.Start("systemctl", $"start {serviceName}").WaitForExit());
                    break;

                case "darwin":
                    await Task.Run(() => Process.Start("launchctl", $"start {serviceName}").WaitForExit());
                    break;

                default:
                    throw new PlatformNotSupportedException($"Unsupported OS type: {OsType}");
            }
        }

        public static async Task StopService(string orgId)
        {
            var serviceName = GetServiceName(orgId);

            switch (OsType)
            {
                case "windows":
                    using (var service = new ServiceController(serviceName))
                    {
                        if (service.Status != ServiceControllerStatus.Stopped)
                        {
                            service.Stop();
                            await Task.Run(() => service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30)));
                        }
                    }
                    break;

                case "linux":
                    await Task.Run(() => Process.Start("systemctl", $"stop {serviceName}").WaitForExit());
                    break;

                case "darwin":
                    await Task.Run(() => Process.Start("launchctl", $"stop {serviceName}").WaitForExit());
                    break;

                default:
                    throw new PlatformNotSupportedException($"Unsupported OS type: {OsType}");
            }
        }

        public static async Task RestartService(string orgId)
        {
            await StopService(orgId);
            await StartService(orgId);
        }

        private static string GetAgentExecutablePath(string orgId) => throw new NotImplementedException();
        private static string GetConfigFilePath(string orgId) => throw new NotImplementedException();
        private static string GetServiceExecutablePath(string orgId) => throw new NotImplementedException();
    }
}
