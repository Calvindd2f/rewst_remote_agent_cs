using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace RewstRemoteAgent
{
    public class ConfigurationFetcher
    {
        private readonly ILogger<ConfigurationFetcher> _logger;
        private static readonly HashSet<string> RequiredKeys = new HashSet<string>
        {
            "azure_iot_hub_host",
            "device_id",
            "shared_access_key",
            "rewst_engine_host",
            "rewst_org_id",
        };

        private static readonly ConcurrentDictionary<string, Dictionary<string, string>> ConfigCache = new ConcurrentDictionary<string, Dictionary<string, string>>();

        public ConfigurationFetcher(ILogger<ConfigurationFetcher> logger)
        {
            _logger = logger;
        }

        public async Task<Dictionary<string, string>> FetchConfigurationAsync(
            string configUrl,
            string secret = null,
            string orgId = null
        )
        {
            if (ConfigCache.TryGetValue(configUrl, out var cachedConfig))
            {
                _logger.LogInformation("Returning cached configuration.");
                return cachedConfig;
            }

            var hostInfo = BuildHostTags(orgId);

            var headers = new Dictionary<string, string>();
            if (secret != null)
            {
                headers["x-rewst-secret"] = secret;
            }

            _logger.LogDebug(
                $"Sending host information to {configUrl}: {JsonSerializer.Serialize(hostInfo)}"
            );

            var retryIntervals = new List<(int interval, int maxRetries)>
            {
                (5, 12),
                (60, 60),
                (300, int.MaxValue),
            };

            foreach (var (interval, maxRetries) in retryIntervals)
            {
                for (int retries = 0; retries < maxRetries; retries++)
                {
                    using (var client = new HttpClient())
                    {
                        foreach (var header in headers)
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }

                        try
                        {
                            var response = await client.PostAsJsonAsync(configUrl, hostInfo);
                            if (response.StatusCode == System.Net.HttpStatusCode.Redirect)
                            {
                                _logger.LogInformation(
                                    "Waiting while Rewst processes Agent Registration..."
                                ); // Custom message for 303
                            }
                            else if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var data = await response.Content.ReadAsStringAsync();
                                var configData =
                                    JsonSerializer.Deserialize<Dictionary<string, object>>(data)[
                                        "configuration"
                                    ] as Dictionary<string, object>;

                                if (configData != null && RequiredKeys.IsSubsetOf(configData.Keys))
                                {
                                    var configDict = new Dictionary<string, string>();
                                    foreach (var kvp in configData)
                                    {
                                        configDict[kvp.Key] = kvp.Value.ToString();
                                    }

                                    ConfigCache[configUrl] = configDict;
                                    return configDict;
                                }
                                else
                                {
                                    _logger.LogWarning(
                                        $"Attempt {retries + 1}: Missing required keys in configuration data. Retrying..."
                                    );
                                }
                            }
                            else if (
                                response.StatusCode == System.Net.HttpStatusCode.BadRequest
                                || response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                            )
                            {
                                _logger.LogError(
                                    $"Attempt {retries + 1}: Not authorized. Check your config secret."
                                );
                            }
                            else
                            {
                                _logger.LogWarning(
                                    $"Attempt {retries + 1}: Received status code {response.StatusCode}. Retrying..."
                                );
                            }
                        }
                        catch (TaskCanceledException)
                        {
                            _logger.LogWarning(
                                $"Attempt {retries + 1}: Request timed out. Retrying..."
                            );
                            continue; // Skip the rest of the loop and retry
                        }
                        catch (HttpRequestException e)
                        {
                            _logger.LogWarning(
                                $"Attempt {retries + 1}: Network error: {e.Message}. Retrying..."
                            );
                            continue;
                        }

                        _logger.LogInformation(
                            $"Attempt {retries + 1}: Waiting {interval}s before retrying..."
                        );
                        await Task.Delay(interval * 1000); // Convert seconds to milliseconds
                    }
                }
            }

            _logger.LogInformation("This process will end when the service is installed.");
            return null;
        }

        private Dictionary<string, string> BuildHostTags(string orgId)
        {
            return new Dictionary<string, string>();
        }
    }
}
