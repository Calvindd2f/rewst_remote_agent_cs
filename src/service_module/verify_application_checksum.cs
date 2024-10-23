using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RewstRemoteAgent
{
    public class ChecksumVerifier
    {
        private readonly ILogger<ChecksumVerifier> _logger;
        private readonly HttpClient _httpClient;
        private const string Version = "1.0.0"; // Replace with your actual version

        public ChecksumVerifier(ILogger<ChecksumVerifier> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<bool> IsChecksumValidAsync(string executablePath)
        {
            var checksumFileName = Path.GetFileName(executablePath) + ".sha256";
            checksumFileName = Regex.Replace(checksumFileName, @"_[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}", "");

            var githubChecksum = await FetchChecksumFromGithubAsync(checksumFileName);
            _logger.LogInformation($"GH Checksum: {githubChecksum}");

            var localChecksum = CalculateLocalFileChecksum(executablePath);
            _logger.LogInformation($"Local Checksum: {localChecksum}");

            if (string.IsNullOrEmpty(githubChecksum) || string.IsNullOrEmpty(localChecksum))
            {
                _logger.LogError("Failed to get one or both of the checksums.");
                return false;
            }

            return string.Equals(githubChecksum, localChecksum, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<ReleaseInfo> GetReleaseInfoByTagAsync(string repo, string tag)
        {
            var url = $"https://api.github.com/repos/{repo}/releases/tags/{tag}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ReleaseInfo>(content);
        }

        private async Task<string> FetchChecksumFromGithubAsync(string checksumFileName)
        {
            var repo = "rewstapp/rewst_remote_agent";
            var versionTag = $"v{Version}";

            var checksumFileUrl = await GetChecksumFileUrlAsync(repo, versionTag, checksumFileName);

            if (string.IsNullOrEmpty(checksumFileUrl))
            {
                _logger.LogError($"Checksum file URL not found for {checksumFileName}");
                return null;
            }

            try
            {
                var response = await _httpClient.GetAsync(checksumFileUrl);
                response.EnsureSuccessStatusCode();
                var checksumData = await response.Content.ReadAsStringAsync();

                foreach (var line in checksumData.Split('\n'))
                {
                    if (line.StartsWith("Hash", StringComparison.OrdinalIgnoreCase))
                    {
                        return line.Split(':')[1].Trim();
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to fetch checksum from GitHub: {e.Message}");
                return null;
            }
        }

        private async Task<string> GetChecksumFileUrlAsync(string repo, string tag, string fileName)
        {
            var releaseInfo = await GetReleaseInfoByTagAsync(repo, tag);
            return releaseInfo.Assets
                .FirstOrDefault(asset => asset.Name == fileName)
                ?.BrowserDownloadUrl;
        }

        private string CalculateLocalFileChecksum(string filePath)
        {
            try
            {
                using var sha256 = SHA256.Create();
                using var stream = File.OpenRead(filePath);
                var hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to calculate local file checksum: {e.Message}");
                return null;
            }
        }
    }

    public class ReleaseInfo
    {
        public Asset[]? Assets { get; set; }
    }

    public class Asset
    {
        public string? Name { get; set; }
        public string? BrowserDownloadUrl { get; set; }
    }
}