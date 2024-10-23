using System;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace RewstRemoteAgent
{
    public static class ServiceManager
    {
        private static ILogger<ServiceManager>? _logger;

        private static void Main(string[] args)
        {
            SetupDependencyInjection(args);

            if (args.Length < 2)
            {
                Console.WriteLine(
                    "Usage: ServiceManager.exe --org-id <orgId> [--config-file <path>] [--install|--uninstall|--start|--stop|--restart|--status]"
                );
                return;
            }

            string orgId = null;
            string configFile = null;
            string action = null;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--org-id":
                        if (i + 1 < args.Length)
                            orgId = args[++i];
                        break;
                    case "--config-file":
                        if (i + 1 < args.Length)
                            configFile = args[++i];
                        break;
                    case "--install":
                    case "--uninstall":
                    case "--start":
                    case "--stop":
                    case "--restart":
                    case "--status":
                        action = args[i].Substring(2);
                        break;
                }
            }

            if (string.IsNullOrEmpty(orgId))
            {
                Console.WriteLine("Organization ID is required.");
                return;
            }

            _serviceName = $"RewstRemoteAgent_{orgId}";

            if (configFile != null)
            {
                LoadConfiguration(configFile);
            }

            switch (action)
            {
                case "install":
                    InstallService(orgId);
                    StartService();
                    break;
                case "uninstall":
                    UninstallService();
                    break;
                case "start":
                    StartService();
                    break;
                case "stop":
                    StopService();
                    break;
                case "restart":
                    RestartService();
                    break;
                case "status":
                    CheckServiceStatus();
                    break;
                default:
                    Console.WriteLine(
                        "No action specified. Use --install, --uninstall, --start, --stop, --status, or --restart."
                    );
                    break;
            }
        }

        private static void SetupDependencyInjection(string[] args)
        {
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddConfiguration(GetLoggingConfiguration(args));
            });

            var serviceProvider = services.BuildServiceProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<ServiceManager>>();
        }

        private static IConfiguration GetLoggingConfiguration(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddCommandLine(args);
            return configBuilder.Build();
        }

        private static void LoadConfiguration(string configFile)
        {
            if (File.Exists(configFile))
            {
                // Implement configuration loading logic
                _logger.LogInformation($"Loaded configuration from {configFile}");
            }
            else
            {
                _logger.LogWarning($"Configuration file {configFile} not found.");
            }
        }

        private static void InstallService(string orgId)
        {
            try
            {
                ManagedInstallerClass.InstallHelper(
                    new string[] { Assembly.GetExecutingAssembly().Location }
                );
                _logger.LogInformation($"Service {_serviceName} installed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to install service {_serviceName}");
            }
        }

        private static void UninstallService()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(
                    new string[] { "/u", Assembly.GetExecutingAssembly().Location }
                );
                _logger.LogInformation($"Service {_serviceName} uninstalled successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to uninstall service {_serviceName}");
            }
        }

        private static void StartService()
        {
            try
            {
                using (var service = new ServiceController(_serviceName))
                {
                    if (service.Status != ServiceControllerStatus.Running)
                    {
                        service.Start();
                        service.WaitForStatus(
                            ServiceControllerStatus.Running,
                            TimeSpan.FromSeconds(10)
                        );
                        _logger.LogInformation($"Service {_serviceName} started successfully.");
                    }
                    else
                    {
                        _logger.LogInformation($"Service {_serviceName} is already running.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to start service {_serviceName}");
            }
        }

        private static void StopService()
        {
            try
            {
                using (var service = new ServiceController(_serviceName))
                {
                    if (service.Status != ServiceControllerStatus.Stopped)
                    {
                        service.Stop();
                        service.WaitForStatus(
                            ServiceControllerStatus.Stopped,
                            TimeSpan.FromSeconds(10)
                        );
                        _logger.LogInformation($"Service {_serviceName} stopped successfully.");
                    }
                    else
                    {
                        _logger.LogInformation($"Service {_serviceName} is already stopped.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to stop service {_serviceName}");
            }
        }

        private static void RestartService()
        {
            StopService();
            StartService();
        }

        private static void CheckServiceStatus()
        {
            try
            {
                using (var service = new ServiceController(_serviceName))
                {
                    _logger.LogInformation($"Service {_serviceName} status: {service.Status}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to check status of service {_serviceName}");
            }
        }
    }
}
