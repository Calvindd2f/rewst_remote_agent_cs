using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace Rewst.RemoteAgent
{
    // service_management.cs
    public interface IServiceManager
    {
        private static readonly string OsType = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows" :
                                                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" :
                                                RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "darwin" : "unknown";

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

        public static void InstallService(string orgId)
        {
            var executablePath = GetAgentExecutablePath(orgId);
            var serviceName = GetServiceName(orgId);
            var configFilePath = GetConfigFilePath(orgId);

            Console.WriteLine($"Installing {serviceName} Service...");

            if (IsServiceInstalled(orgId))
            {
                Console.WriteLine("Service is already installed.");
                return;
            }

            switch (OsType)
            {
                case "windows":
                    Console.WriteLine($"Installing Windows Service: {serviceName}");
                    var windowsServicePath = GetServiceExecutablePath(orgId);
                    Process.Start(windowsServicePath, "install").WaitForExit();
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
                    File.WriteAllText($"/etc/systemd/system/{serviceName}.service", systemdServiceContent);
                    Process.Start("systemctl", "daemon-reload").WaitForExit();
                    Process.Start("systemctl", $"enable {serviceName}").WaitForExit();
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
                    File.WriteAllText(plistPath, launchdPlistContent);
                    Process.Start("launchctl", $"load {plistPath}").WaitForExit();
                    break;

                default:
                    throw new PlatformNotSupportedException($"Unsupported OS type: {OsType}");
            }
        }

        public static void UninstallService(string orgId)
        {
            var serviceName = GetServiceName(orgId);
            Console.WriteLine($"Uninstalling service {serviceName}.");

            try
            {
                StopService(orgId);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to stop service: {e.Message}");
            }

            switch (OsType)
            {
                case "windows":
                    try
                    {
                        Process.Start("sc", $"delete {serviceName}").WaitForExit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Exception removing service: {e.Message}");
                    }
                    break;

                case "linux":
                    Process.Start("systemctl", $"disable {serviceName}").WaitForExit();
                    File.Delete($"/etc/systemd/system/{serviceName}.service");
                    Process.Start("systemctl", "daemon-reload").WaitForExit();
                    break;

                case "darwin":
                    var plistPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Library/LaunchAgents/{serviceName}.plist";
                    Process.Start("launchctl", $"unload {plistPath}").WaitForExit();
                    File.Delete(plistPath);
                    break;

                default:
                    throw new PlatformNotSupportedException($"Unsupported OS type: {OsType}");
            }
        }

        public static void CheckServiceStatus(string orgId)
        {
            var serviceName = GetServiceName(orgId);

            switch (OsType)
            {
                case "windows":
                    try
                    {
                        using var service = new ServiceController(serviceName);
                        Console.WriteLine($"Service status: {service.Status}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                    break;

                case "linux":
                    try
                    {
                        var process = Process.Start(new ProcessStartInfo("systemctl", $"is-active {serviceName}")
                        {
                            RedirectStandardOutput = true,
                            UseShellExecute = false
                        });
                        Console.WriteLine($"Service status: {process.StandardOutput.ReadToEnd().Trim()}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                    break;

                case "darwin":
                    try
                    {
                        var process = Process.Start(new ProcessStartInfo("launchctl", $"list {serviceName}")
                        {
                            RedirectStandardOutput = true,
                            UseShellExecute = false
                        });
                        var output = process.StandardOutput.ReadToEnd();
                        Console.WriteLine($"Service status: {(output.Contains(serviceName) ? "Running" : "Not Running")}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                    break;

                default:
                    Console.WriteLine($"Unsupported OS type: {OsType}");
                    break;
            }
        }

        public static void StartService(string orgId)
        {
            var serviceName = GetServiceName(orgId);
            Console.WriteLine($"Starting Service {serviceName} for {OsType}");

            switch (OsType)
            {
                case "windows":
                    using (var service = new ServiceController(serviceName))
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    }
                    break;

                case "linux":
                    Process.Start("systemctl", $"start {serviceName}").WaitForExit();
                    break;

                case "darwin":
                    Process.Start("launchctl", $"start {serviceName}").WaitForExit();
                    break;

                default:
                    throw new PlatformNotSupportedException($"Unsupported OS type: {OsType}");
            }
        }

        public static void StopService(string orgId)
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
                            service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                        }
                    }
                    break;

                case "linux":
                    Process.Start("systemctl", $"stop {serviceName}").WaitForExit();
                    break;

                case "darwin":
                    Process.Start("launchctl", $"stop {serviceName}").WaitForExit();
                    break;

                default:
                    throw new PlatformNotSupportedException($"Unsupported OS type: {OsType}");
            }
        }

        public static void RestartService(string orgId)
        {
            StopService(orgId);
            StartService(orgId);
        }

        // These methods need to be implemented based on your specific configuration logic
        private static string GetAgentExecutablePath(string orgId) => throw new NotImplementedException();
        private static string GetConfigFilePath(string orgId) => throw new NotImplementedException();
        private static string GetServiceExecutablePath(string orgId) => throw new NotImplementedException();
    };

    // rewst_service_manager.cs
    internal class ServiceController
    {
    }
}