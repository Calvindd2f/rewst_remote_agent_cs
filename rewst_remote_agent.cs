using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rewst.Log;

namespace Rewst.RemoteAgent
{
    internal class Program
    {
        private static ILogger<Program>? _logger;
        private static CancellationTokenSource _cts = new CancellationTokenSource();

        private static async Task Main(string[] args)
        {
            SetupDependencyInjection(args);

            _logger.LogInformation($"Version: {Assembly.GetEntryAssembly().GetName().Version}");
            _logger.LogInformation($"Running on {RuntimeInformation.OSDescription}");

            string configFile = null;
            string orgId = null;

            try
            {
                _logger.LogInformation("Loading Configuration");
                if (!string.IsNullOrEmpty(configFile))
                {
                    _logger.LogInformation($"Using config file {configFile}.");
                    var config = LoadConfiguration(null, configFile);
                    orgId = config["rewst_org_id"];
                }
                else
                {
                    orgId = GetOrgIdFromExecutableName(args);
                    if (!string.IsNullOrEmpty(orgId))
                    {
                        _logger.LogInformation($"Found Org ID {orgId} via executable name.");
                        var config = LoadConfiguration(orgId);
                    }
                    else
                    {
                        _logger.LogWarning("Did not find guid in executable name.");
                    }
                }

                if (string.IsNullOrEmpty(orgId))
                {
                    throw new ConfigurationException("No configuration was found.");
                }
            }
            catch (ConfigurationException e)
            {
                _logger.LogError(e.Message);
                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception Caught during self-configuration");
                return;
            }

            _logger.LogInformation($"Running for Org ID {orgId}");

            _logger.LogInformation("Setting up file logging");
            try
            {
                SetupFileLogging(orgId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred setting up file-based logging");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    _logger.LogInformation("Shutting down gracefully.");
                    _cts.Cancel();
                    eventArgs.Cancel = true;
                };
            }
            else
            {
                // For non-Windows platforms, you might want to use a library like Mono.Posix for signal handling
                // This is a simplified example
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    _logger.LogInformation("Shutting down gracefully.");
                    _cts.Cancel();
                    eventArgs.Cancel = true;
                };
            }

            await IoTHubConnectionLoop(_cts.Token);
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
            _logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        }

        private static IConfiguration GetLoggingConfiguration(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddCommandLine(args);
            return configBuilder.Build();
        }

        private static IConfiguration LoadConfiguration(string orgId, string configFile = null)
        {
            var builder = new ConfigurationBuilder();

            if (!string.IsNullOrEmpty(configFile))
            {
                builder.AddJsonFile(configFile, optional: false, reloadOnChange: true);
            }
            else if (!string.IsNullOrEmpty(orgId))
            {
                // Implement logic to load configuration based on orgId
                // This might involve loading from a specific file or a database
            }

            return builder.Build();
        }

        private static string GetOrgIdFromExecutableName(string[] args)
        {
            // Implement logic to extract org ID from executable name
            return "";
        }

        private static void SetupFileLogging(string orgId)
        {
            // Implement file logging setup
        }

        private static async Task IoTHubConnectionLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Implement IoT Hub connection logic here
                    await Task.Delay(1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("IoT Hub connection loop cancelled.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in IoT Hub connection loop");
                    await Task.Delay(5000, cancellationToken);
                }
            }
        }
    }

    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message) { }
    }
}
