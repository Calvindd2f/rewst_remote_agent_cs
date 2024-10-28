using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using Rewst.RemoteAgent.Service;
using Rewst.RemoteAgent.IOIHUB;
using Rewst.Log;

namespace Rewst.RemoteAgent
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var serviceProvider = ConfigureServices();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            logger.LogInformation($"Running on {Environment.OSVersion}");
            logger.LogInformation($"Rewst Agent Configuration Tool v{GetVersion()}");

            var httpClient = new HttpClient();
            var verifier = new ChecksumVerifier(logger, httpClient);

            Logger.SetupLogging("YourAppName");
            Logger.LogInfo("This is an informational message");
            Logger.LogError("An error occurred: " + exception.Message);

            ServiceManager.InstallService("your_org_id");

            ServiceManager.StartService("your_org_id");

            ServiceManager.CheckServiceStatus("your_org_id");

            var isValid = await verifier.IsChecksumValidAsync("path/to/your/executable");
            Console.WriteLine($"Is checksum valid: {isValid}");

            try
            {
                var configUrl = GetArgumentValue(args, "--config-url");
                var configSecret = GetArgumentValue(args, "--config-secret");
                var orgId = GetArgumentValue(args, "--org-id");

                if (string.IsNullOrEmpty(configUrl) || string.IsNullOrEmpty(configSecret) || string.IsNullOrEmpty(orgId))
                {
                    logger.LogError("Error: Missing required parameters.");
                    logger.LogError("Please make sure '--config-url', '--org-id' and '--config-secret' are provided.");
                    Environment.Exit(1);
                }

                if (!IsValidUrl(configUrl))
                {
                    logger.LogError("The config URL provided is not valid.");
                    Environment.Exit(1);
                }

                if (!IsBase64(configSecret))
                {
                    logger.LogError("The config secret provided is not a valid base64 string.");
                    Environment.Exit(1);
                }

                var configManager = serviceProvider.GetRequiredService<ConfigManager>();
                var config = await configManager.FetchConfiguration(configUrl, configSecret, orgId);

                if (config == null)
                {
                    logger.LogError("Failed to fetch configuration.");
                    Environment.Exit(2);
                }

                logger.LogInformation($"Configuration: {config}");
                logger.LogInformation($"Organization ID: {config.RewstOrgId}");

                var connectionManager = serviceProvider.GetRequiredService<ConnectionManager>();
                await connectionManager.Connect();
                await connectionManager.SetMessageHandler();

                var fileManager = serviceProvider.GetRequiredService<FileManager>();
                await fileManager.RemoveOldFiles(config.RewstOrgId);
                await fileManager.WaitForFiles(config.RewstOrgId);

                await connectionManager.Disconnect();
                await Task.Delay(4000);
                logger.LogInformation("Disconnected from IoT Hub.");

                var serviceManager = serviceProvider.GetRequiredService<ServiceManager>();
                while (!serviceManager.IsServiceRunning(config.RewstOrgId))
                {
                    logger.LogInformation("Waiting for the service to start...");
                    await Task.Delay(5000);
                }

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred");
                Environment.Exit(1);
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddLogging(configure => configure.AddConsole());
            services.AddSingleton<ConfigManager>();
            services.AddSingleton<ConnectionManager>();
            services.AddSingleton<FileManager>();
            services.AddSingleton<ServiceManager>();
            return services.BuildServiceProvider();
        }

        private static string GetVersion()
        {
            return "1.0.0";
        }

        private static string GetArgumentValue(string[] args, string argName)
        {
            int index = Array.IndexOf(args, argName);
            return (index != -1 && index < args.Length - 1) ? args[index + 1] : null;
        }

        private static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private static bool IsBase64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0 || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
                return false;

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

    public class ConfigManager
    {
        private readonly ILogger<ConfigManager> _logger;
        private readonly HttpClient _httpClient;

        public ConfigManager(ILogger<ConfigManager> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<Config> FetchConfiguration(string configUrl, string configSecret, string orgId)
        {
            _logger.LogInformation("Fetching configuration from Rewst...");

            var request = new HttpRequestMessage(HttpMethod.Get, configUrl);
            request.Headers.Add("X-Rewst-Config-Secret", configSecret);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to fetch configuration. Status code: {response.StatusCode}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var config = JsonSerializer.Deserialize<Config>(content);

            if (config.RewstOrgId != orgId)
            {
                _logger.LogError("Organization ID mismatch");
                return null;
            }

            SaveConfiguration(config);

            return config;
        }

        private void SaveConfiguration(Config config)
        {
            _logger.LogInformation("Saving configuration to file...");
            // Implement file saving logic here
        }
    }

    public class Config
    {
        public string? RewstOrgId { get; set; }
    }

    public class ConnectionManager
    {
        private readonly ILogger<ConnectionManager> _logger;
        private DeviceClient _deviceClient;

        public ConnectionManager(ILogger<ConnectionManager> logger)
        {
            _logger = logger;
        }

        public async Task Connect()
        {
            _logger.LogInformation("Connecting to IoT Hub...");
            // Implement IoT Hub connection logic here
            // _deviceClient = DeviceClient.CreateFromConnectionString("your_connection_string");
            await Task.CompletedTask;
        }

        public async Task SetMessageHandler()
        {
            _logger.LogInformation("Setting up method handler...");
            await _deviceClient.SetMethodHandlerAsync("MethodName", MethodHandler, null);
        }


        public async Task Disconnect()
        {
            _logger.LogInformation("Disconnecting from IoT Hub...");
            if (_deviceClient != null)
            {
                await _deviceClient.CloseAsync();
            }
        }

        private Task<MethodResponse> MethodHandler(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);

            _logger.LogInformation($"Received method: {methodRequest.Name} with data: {data}");

            switch (methodRequest.Name.ToLower())
            {
                case "restart":
                    return HandleRestartMethod(data);
                case "firmware_update":
                    return HandleFirmwareUpdateMethod(data);
                // Add more cases as needed
                default:
                    return Task.FromResult(new MethodResponse(400));
            }
        }

        private Task<MethodResponse> HandleRestartMethod(string payload)
        {
            _logger.LogInformation("Handling restart method");
            // Implement restart logic here
            var response = new { result = "Restarting device" };
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response)), 200));
        }

        private Task<MethodResponse> HandleFirmwareUpdateMethod(string payload)
        {
            _logger.LogInformation("Handling firmware update method");
            // Implement firmware update logic here
            var response = new { result = "Updating firmware" };
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response)), 200));
        }
    }

}
