using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace RewstRemoteAgent
{
    public class ConnectionManager
    {
        private readonly ConfigData _configData;
        private readonly string _connectionString;
        private readonly DeviceClient _client;
        private readonly string _osType;

        public ConnectionManager(ConfigData configData)
        {
            _configData = configData;
            _connectionString = GetConnectionString();
            _osType =
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows"
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "darwin"
                : "linux";
            _client = DeviceClient.CreateFromConnectionString(_connectionString);
        }

        private string GetConnectionString()
        {
            return $"HostName={_configData.AzureIotHubHost};"
                + $"DeviceId={_configData.DeviceId};"
                + $"SharedAccessKey={_configData.SharedAccessKey}";
        }

        public async Task ConnectAsync()
        {
            try
            {
                await _client.OpenAsync();
                Console.WriteLine("Connected to IoT Hub");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception in connection to the IoT Hub: {e}");
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                await _client.CloseAsync();
                Console.WriteLine("Disconnected from IoT Hub");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception in disconnecting from the IoT Hub: {e}");
            }
        }

        public async Task SendMessageAsync(object messageData)
        {
            var messageJson = JsonConvert.SerializeObject(messageData);
            var message = new Message(Encoding.UTF8.GetBytes(messageJson));
            await _client.SendEventAsync(message);
        }

        public async Task SetMessageHandlerAsync()
        {
            await _client.SetReceiveMessageHandlerAsync(HandleMessageAsync, null);
        }

        private async Task<MessageResponse> HandleMessageAsync(Message message, object userContext)
        {
            Console.WriteLine("Received IoT Hub message in HandleMessageAsync.");
            try
            {
                var messageData = JsonConvert.DeserializeObject<dynamic>(
                    Encoding.UTF8.GetString(message.GetBytes())
                );
                bool getInstallationInfo = messageData.get_installation;
                string commands = messageData.commands;
                string postId = messageData.post_id;
                string interpreterOverride = messageData.interpreter_override;

                string postUrl = null;
                if (!string.IsNullOrEmpty(postId))
                {
                    var postPath = postId.Replace(":", "/");
                    var rewstEngineHost = _configData.RewstEngineHost;
                    postUrl = $"https://{rewstEngineHost}/webhooks/custom/action/{postPath}";
                    Console.WriteLine($"Will POST results to {postUrl}");
                }

                if (!string.IsNullOrEmpty(commands))
                {
                    Console.WriteLine("Received commands in message");
                    try
                    {
                        await ExecuteCommandsAsync(commands, postUrl, interpreterOverride);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Exception running commands: {e}");
                    }
                }

                if (getInstallationInfo)
                {
                    Console.WriteLine("Received request for installation paths");
                    try
                    {
                        await GetInstallationAsync(postUrl);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Exception getting installation info: {e}");
                    }
                }
            }
            catch (JsonException e)
            {
                Console.WriteLine($"Error decoding message data as JSON: {e}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An unexpected error occurred: {e}");
            }

            return MessageResponse.Completed;
        }

        private async Task ExecuteCommandsAsync(
            string commands,
            string postUrl,
            string interpreterOverride
        )
        {
            var interpreter = interpreterOverride ?? GetDefaultInterpreter();
            Console.WriteLine($"Using interpreter: {interpreter}");

            var decodedCommands = Encoding.UTF8.GetString(Convert.FromBase64String(commands));
            var scriptExtension = interpreter.ToLower().Contains("powershell") ? ".ps1" : ".sh";

            var tempFilePath = Path.GetTempFileName();
            File.Move(tempFilePath, Path.ChangeExtension(tempFilePath, scriptExtension));
            tempFilePath = Path.ChangeExtension(tempFilePath, scriptExtension);

            await File.WriteAllTextAsync(tempFilePath, decodedCommands);

            var shellCommand = interpreter.ToLower().Contains("powershell")
                ? $"{interpreter} -File \"{tempFilePath}\""
                : $"{interpreter} \"{tempFilePath}\"";

            try
            {
                Console.WriteLine($"Running process via command line: {shellCommand}");
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = interpreter,
                    Arguments = $"-File \"{tempFilePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using var process = new Process { StartInfo = processStartInfo };
                process.Start();
                var stdout = await process.StandardOutput.ReadToEndAsync();
                var stderr = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                var exitCode = process.ExitCode;
                Console.WriteLine($"Command completed with exit code {exitCode}");

                dynamic outputMessageData;
                if (exitCode != 0 || !string.IsNullOrEmpty(stderr))
                {
                    var errorMessage = $"Script execution failed with exit code {exitCode}. Error: {stderr}";
                    Console.WriteLine(errorMessage);
                    outputMessageData = new { output = stdout, error = errorMessage };
                }
                else
                {
                    outputMessageData = new { output = stdout, error = "" };
                }

                if (!string.IsNullOrEmpty(postUrl))
                {
                    await PostResultsAsync(postUrl, outputMessageData);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An unexpected error occurred: {e}");
            }
            finally
            {
                File.Delete(tempFilePath);
            }
        }

        private async Task GetInstallationAsync(string postUrl)
        {
            // Implement the logic to get installation paths
            // This is a placeholder and needs to be adjusted based on your specific requirements
            var pathsData = new
            {
                service_executable_path = $"get_service_executable_path{orgId}",
                agent_executable_path = $"get_agent_executable_path{orgId}",
                config_file_path = $"get_config_file_path{orgId}",
                service_manager_path = $"get_service_manager_path{orgId}",
                tags = new[] { "tag1", "tag2" },
            };

            await PostResultsAsync(postUrl, pathsData);
        }

        private async Task PostResultsAsync(string postUrl, object data)
        {
            using var client = new System.Net.Http.HttpClient();
            var content = new StringContent(
                JsonConvert.SerializeObject(data),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await client.PostAsync(postUrl, content);
                response.EnsureSuccessStatusCode();
                Console.WriteLine($"POST request status: {response.StatusCode}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error posting to {postUrl}: {e}");
            }
        }

        private string GetDefaultInterpreter()
        {
            return _osType switch
            {
                "windows" => "powershell",
                "darwin" => "/bin/zsh",
                _ => "/bin/bash",
            };
        }
    }

    public class ConfigData
    {
        public string AzureIotHubHost { get; set; }
        public string DeviceId { get; set; }
        public string SharedAccessKey { get; set; }
        public string RewstEngineHost { get; set; }
    }
}