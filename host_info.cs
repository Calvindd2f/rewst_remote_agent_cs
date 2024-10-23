using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace RewstRemoteAgent
{
    public static class HostInfoHelper
    {
        private static readonly string[] PotentialServiceNames = { "ADSync", "Azure AD Sync", "EntraConnectSync", "OtherFutureName" };

        public static string GetMacAddress()
        {
            var macAddr = NetworkInterface
                .GetAllNetworkInterfaces()
                .FirstOrDefault(nic => nic.OperationalStatus == OperationalStatus.Up)?
                .GetPhysicalAddress()
                .ToString();
            return macAddr?.Replace(":", string.Empty);
        }

        public static string RunPowerShellCommand(string command)
        {
            try
            {
                var processInfo = new ProcessStartInfo("powershell", $"-Command {command}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(processInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return output.Trim();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error executing PowerShell command: {ex}");
                return null;
            }
        }

        public static bool IsDomainController()
        {
            string command = "(Get-WmiObject Win32_ComputerSystem).DomainRole";
            string output = RunPowerShellCommand(command);
            bool isDomainController = output == "4" || output == "5";
            Console.WriteLine($"Is domain controller?: {isDomainController}");
            return isDomainController;
        }

        public static string GetAdDomainName()
        {
            string command = "(Get-WmiObject Win32_ComputerSystem).Domain";
            string adDomainName = RunPowerShellCommand(command);
            Console.WriteLine($"AD domain name: {adDomainName}");
            return adDomainName != "WORKGROUP" ? adDomainName : null;
        }

        public static string GetEntraDomain()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return null;
            }
            else
            {
                try
                {
                    var result = RunPowerShellCommand("dsregcmd /status");
                    if (result.Contains("AzureAdJoined") && result.Contains("YES"))
                    {
                        var lines = result.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in lines)
                        {
                            if (line.Contains("DomainName"))
                            {
                                return line.Split(':')[1].Trim();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected issue querying for Entra Domain: {ex}");
                }
                return null;
            }
        }

        public static bool IsEntraConnectServer()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return false;
            }
            else
            {
                return PotentialServiceNames.Any(IsServiceRunning);
            }
        }

        public static bool IsServiceRunning(string serviceName)
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service"))
            {
                foreach (var service in searcher.Get())
                {
                    if (service["Name"].ToString().Equals(serviceName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static Dictionary<string, object> BuildHostTags(string orgId = null)
        {
            var adDomain = GetAdDomainName();
            bool isDc = adDomain != null && IsDomainController();

            var hostInfo = new Dictionary<string, object>
        {
            { "agent_version", typeof(HostInfoHelper).Assembly.GetName().Version.ToString() },
            { "agent_executable_path", GetAgentExecutablePath(orgId) },
            { "service_executable_path", GetServiceExecutablePath(orgId) },
            { "hostname", Environment.MachineName },
            { "mac_address", GetMacAddress() },
            { "operating_system", Environment.OSVersion },
            { "cpu_model", GetCpuModel() },
            { "ram_gb", Math.Round(GetRamGb(), 1) },
            { "ad_domain", adDomain },
            { "is_ad_domain_controller", isDc },
            { "is_entra_connect_server", IsEntraConnectServer() },
            { "entra_domain", GetEntraDomain() },
            { "org_id", orgId }
        };
            return hostInfo;
        }

        private static string GetAgentExecutablePath(string orgId) => throw new NotImplementedException();
        private static string GetServiceExecutablePath(string orgId) => throw new NotImplementedException();
        private static string GetCpuModel() => throw new NotImplementedException();
        private static double GetRamGb() => throw new NotImplementedException();
    }
}