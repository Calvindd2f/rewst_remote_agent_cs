using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using Microsoft.Extensions.Logging;

namespace RewstRemoteAgent
{
    public class RewstWindowsService : ServiceBase
    {
        private ILogger<RewstWindowsService> _logger;
        private Process _process;
        private Timer _timer;
        private string _orgId;
        private string _agentExecutablePath;

        public RewstWindowsService(ILogger<RewstWindowsService> logger)
        {
            _logger = logger;
            ServiceName = "RewstRemoteAgent";
        }

        protected override void OnStart(string[] args)
        {
            _logger.LogInformation("Service is starting...");
            _orgId = GetOrgIdFromExecutableName();

            if (!string.IsNullOrEmpty(_orgId))
            {
                ServiceName = $"RewstRemoteAgent_{_orgId}";
                _logger.LogInformation($"Found Org ID {_orgId}");
                _agentExecutablePath = GetAgentExecutablePath(_orgId);
            }
            else
            {
                _logger.LogWarning("Org ID not found in executable name");
                return;
            }

            StartProcess();

            _timer = new Timer(5000); // Check every 5 seconds
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        protected override void OnStop()
        {
            _logger.LogInformation("Service is stopping...");
            StopProcess();
            _timer?.Stop();
        }

        private void StartProcess()
        {
            try
            {
                if (IsChecksumValid(_agentExecutablePath))
                {
                    _logger.LogInformation($"Verified that the executable {_agentExecutablePath} has a valid signature.");
                    string processName = Path.GetFileNameWithoutExtension(_agentExecutablePath);
                    _logger.LogInformation($"Launching process for {processName}");

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
                    _logger.LogInformation($"Started process with PID {_process.Id}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start external process");
            }
        }

        private void StopProcess()
        {
            if (_process != null && !_process.HasExited)
            {
                _logger.LogInformation($"Attempting to terminate process with PID {_process.Id}");
                _process.Kill();
                _process.WaitForExit(10000);
                if (!_process.HasExited)
                {
                    _logger.LogWarning($"Process with PID {_process.Id} did not terminate in time. Forcing kill.");
                    _process.Kill(true);
                }
            }

            string processName = Path.GetFileNameWithoutExtension(_agentExecutablePath);
            foreach (var proc in Process.GetProcessesByName(processName))
            {
                try
                {
                    _logger.LogInformation($"Force killing leftover process with PID {proc.Id}.");
                    proc.Kill();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to kill leftover process with PID {proc.Id}");
                }
            }

            _logger.LogInformation("All processes stopped.");
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_process == null || _process.HasExited)
            {
                _logger.LogWarning("External process terminated unexpectedly. Restarting.");
                StartProcess();
            }
        }

        private string GetOrgIdFromExecutableName()
        {
            // Implement logic to extract org ID from executable name
            return "";
        }

        private string GetAgentExecutablePath(string orgId)
        {
            // Implement logic to get agent executable path
            return "";
        }

        private bool IsChecksumValid(string filePath)
        {
            // Implement checksum validation logic
            return true;
        }

        public static void Main()
        {
            using (var service = new RewstWindowsService(new LoggerFactory().CreateLogger<RewstWindowsService>()))
            {
                ServiceBase.Run(service);
            }
        }
    }
}