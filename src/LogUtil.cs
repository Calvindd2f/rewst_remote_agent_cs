using System;
using System.Collections.Generic;
using System.IO;
using Rewst.RemoteAgent;

namespace Rewst.Log
{
    public static class LogUtil
    {
        private static readonly List<string> logLines = new();
        private const string LOG_FILE_NAME = "RewstAgent.log";
        private const int MAX_LOG_SIZE_MB = 10;
        private const int MAX_LOG_FILES = 5;

        public static void LogInfo(string shortMessage, string detailDescription = "")
        {
            Log("Information", shortMessage, detailDescription);
        }

        public static void LogError(string shortMessage, string detailDescription = "")
        {
            Log("Error", shortMessage, detailDescription);
        }

        public static void LogWarning(string shortMessage, string detailDescription = "")
        {
            Log("Warning", shortMessage, detailDescription);
        }

        private static void Log(string type, string shortDescription, string detailDescription = "")
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logMessage = $"[{timestamp}] [{type}] {shortDescription}";

            if (!string.IsNullOrEmpty(detailDescription))
            {
                logMessage += $"\r\nDetails: {detailDescription}";
            }

            logMessage += "\r\n";
            logLines.Add(logMessage);
            WriteToLogFile(logMessage);
        }

        private static void WriteToLogFile(string message)
        {
            try
            {
                var agentInstallationDirectory = GetAgentInstallationDirectory();
                if (!Directory.Exists(agentInstallationDirectory))
                {
                    Directory.CreateDirectory(agentInstallationDirectory);
                }

                var logFilePath = Path.Combine(agentInstallationDirectory, LOG_FILE_NAME);

                if (File.Exists(logFilePath))
                {
                    var fileInfo = new FileInfo(logFilePath);
                    if (fileInfo.Length > MAX_LOG_SIZE_MB * 1024 * 1024)
                    {
                        RotateLogs(agentInstallationDirectory);
                    }
                }

                File.AppendAllText(logFilePath, message);
            }
            catch (Exception ex) when (ex is DirectoryNotFoundException ||
                ex is UnauthorizedAccessException ||
                ex is IOException)
            {
                // In a production environment, you might want to implement a fallback logging mechanism
                // or notify an monitoring system about logging failures}
            }

        private static void RotateLogs(string logDirectory)
        {
            var oldestLog = Path.Combine(logDirectory, $"{LOG_FILE_NAME}.{MAX_LOG_FILES}");
            if (File.Exists(oldestLog))
            {
                File.Delete(oldestLog);
            }

            for (int i = MAX_LOG_FILES - 1; i >= 1; i--)
            {
                var source = Path.Combine(logDirectory, $"{LOG_FILE_NAME}.{i}");
                var destination = Path.Combine(logDirectory, $"{LOG_FILE_NAME}.{i + 1}");
                if (File.Exists(source))
                {
                    File.Move(source, destination);
                }
            }

            var currentLog = Path.Combine(logDirectory, LOG_FILE_NAME);
            var newFirstBackup = Path.Combine(logDirectory, $"{LOG_FILE_NAME}.1");
            if (File.Exists(currentLog))
            {
                File.Move(currentLog, newFirstBackup);
            }
        }

        private static string GetAgentInstallationDirectory()
        {
            var programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return Path.Combine(programDataPath, "Rewst Agent", "Logs");
        }
    }
}}