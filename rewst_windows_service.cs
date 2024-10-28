using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rewst.RemoteAgent.Service;

namespace Rewst.RemoteAgent
{
    public class RewstWindowsService : ServiceBase
    {
        private Process _process;
        private Timer _timer;
        private string _orgId;
        private string _agentExecutablePath;
        private ILogger<RewstWindowsService> _logger;

        public RewstWindowsService()
        {
            ServiceName = "RewstRemoteAgent";
            _logger = NullLogger<RewstWindowsService>.Instance;
        }

        protected override void OnStart(string[] args)
        {
            LogUtil.LogInfo("Service is starting...");
            _orgId = GetOrgIdFromExecutableName();

            if (!string.IsNullOrEmpty(_orgId))
            {
                ServiceName = $"RewstRemoteAgent_{_orgId}";
                LogUtil.LogInfo($"Found Org ID {_orgId}");
                _agentExecutablePath = ConfigIO.GetAgentExecutablePath(_orgId);
            }
            else
            {
                LogUtil.LogWarning("Org ID not found in executable name");
                return;
            }

            StartProcess();

            _timer = new Timer(5000); // Check every 5 seconds
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        protected override void OnStop()
        {
            LogUtil.LogInfo("Service is stopping...");
            StopProcess();
            _timer?.Stop();
        }

        private void StartProcess()
        {
            try
            {
                if (IsChecksumValid(_agentExecutablePath).Result)
                {
                    LogUtil.LogInfo($"Verified that the executable {_agentExecutablePath} has a valid signature.");
                    string processName = Path.GetFileNameWithoutExtension(_agentExecutablePath);
                    LogUtil.LogInfo($"Launching process for {processName}");

                    _process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = _agentExecutablePath,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    _process.Start();
                    LogUtil.LogInfo($"Started process with PID {_process.Id}.");
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError("Failed to start external process", ex.ToString());
            }
        }

        private void StopProcess()
        {
            if (_process != null && !_process.HasExited)
            {
                LogUtil.LogInfo($"Attempting to terminate process with PID {_process.Id}");
                _process.Kill();
                _process.WaitForExit(10000);
                if (!_process.HasExited)
                {
                    LogUtil.LogWarning($"Process with PID {_process.Id} did not terminate in time. Forcing kill.");
                    _process.Kill(true);
                }
            }

            string processName = Path.GetFileNameWithoutExtension(_agentExecutablePath);
            foreach (var proc in Process.GetProcessesByName(processName))
            {
                try
                {
                    LogUtil.LogInfo($"Force killing leftover process with PID {proc.Id}.");
                    proc.Kill();
                }
                catch (Exception ex)
                {
                    LogUtil.LogError($"Failed to kill leftover process with PID {proc.Id}", ex.ToString());
                }
            }

            LogUtil.LogInfo("All processes stopped.");
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_process == null || _process.HasExited)
            {
                LogUtil.LogWarning("External process terminated unexpectedly. Restarting.");
                StartProcess();
            }
        }

        private string GetOrgIdFromExecutableName()
        {
            var pattern = @"rewst_.*_(.+?)\.";
            var regex = new Regex(pattern);
            var match = regex.Match(AppDomain.CurrentDomain.FriendlyName);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return null;
            }
        }

        private async Task<bool> IsChecksumValid(string filePath)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<ChecksumVerifier>();
            var httpClient = new HttpClient();
            var verifier = new ChecksumVerifier(logger, httpClient);

            var isValid = await verifier.IsChecksumValidAsync(filePath);
            return isValid;
        }

        public static void Main()
        {
            using (var service = new RewstWindowsService())
            {
                ServiceBase.Run(service);
            }
        }
    }
}
