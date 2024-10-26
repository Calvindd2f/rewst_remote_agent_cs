using System;
using System.Collections.Generic;
using System.IO;

namespace Rewst.RemoteAgent.Calvindd2f
{
    public static class LogUtil
    {
        public static void LogInfo(string shortMessage, string detailDescription = "")
        {
            Rewst.RemoteAgent.Calvindd2f.LogUtil.Log(
                "Informational",
                shortMessage,
                detailDescription
            );
        }

        public static void LogError(string shortMessage, string detailDescription = "")
        {
            Rewst.RemoteAgent.Calvindd2f.LogUtil.Log("Error", shortMessage, detailDescription);
        }

        public static void LogWarning(string shortMessage, string detailDescription = "")
        {
            Rewst.RemoteAgent.Calvindd2f.LogUtil.Log("Warning", shortMessage, detailDescription);
        }

        private static void Log(string type, string shortDescription, string detailDescription = "")
        {
            string text =
                System.DateTime.Now.ToString("ddd MMM  d H:mm:ss yyyy:") + "\t" + type + "\t";
            if (type == "Error")
            {
                text += "\t";
            }
            string text2 = text + shortDescription;
            if (detailDescription != "")
            {
                text2 += " For more information please see log file";
            }
            System.Console.WriteLine(text2);
            if (detailDescription != "")
            {
                text2 = text + detailDescription;
                if (type != "Error")
                {
                    text2 = text + shortDescription + "\r\n";
                    text2 = text2 + "Detail description: " + detailDescription + "\r\n";
                }
            }
            Rewst.RemoteAgent.Calvindd2f.LogUtil.logLines.Add(text2);
            string agentInstallationDirectory =
                Rewst.RemoteAgent.Calvindd2f.LogUtil.GetAgentInstallationDirectory();
            try
            {
                if (System.IO.Directory.Exists(agentInstallationDirectory))
                {
                    System.IO.File.AppendAllLines(
                        System.IO.Path.Combine(agentInstallationDirectory, "LogFile_Installer.txt"),
                        Rewst.RemoteAgent.Calvindd2f.LogUtil.logLines
                    );
                    Rewst.RemoteAgent.Calvindd2f.LogUtil.logLines.Clear();
                }
            }
            catch (System.IO.DirectoryNotFoundException) { }
            catch (System.UnauthorizedAccessException) { }
        }

        private static string GetAgentInstallationDirectory()
        {
            string folderPath = System.Environment.GetFolderPath(0x26);
            string text = System.IO.Path.Combine(folderPath, " Agent");
            return System.IO.Path.Combine(folderPath, text);
        }

        // Note: this type is marked as 'beforefieldinit'.
        static LogUtil() { }

        private static System.Collections.Generic.List<string> logLines =
            new System.Collections.Generic.List<string>();
    }
}
