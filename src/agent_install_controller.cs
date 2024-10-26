using System.IO.Compression;
using System.IO.Pipes;
using Rewst.RemoteAgent.Helpers;

namespace Rewst.RemoteAgent
{
    public class AgentInstallController
    {
        public static bool CheckIfInstalled()
        {
            return File.Exists(
                Path.Combine(
                    Environment.GetFolderPath((Environment.SpecialFolder)0x26),
                    " Agent",
                    " Agent.exe"
                )
            );
        }

        private static void Install_StopService()
        {
            try
            {
                ServiceState serviceStatus = ServiceInstaller.GetServiceStatus(" Agent");
                if (
                    serviceStatus != ServiceState.Stopped
                    && serviceStatus != ServiceState.NotFound
                )
                {
                    ServiceInstaller.StopService(" Agent");
                    LogUtil.LogInfo("Service has been stopped.", "");
                }
                else
                {
                    LogUtil.LogInfo(
                        "Agent service does not need to be stopped, service state: "
                            + Enum.GetName(
                                typeof(ServiceState),
                                serviceStatus
                            ),
                        ""
                    );
                }
            }
            catch (Exception)
            {
                throw new AgentInstallException(
                    "Failed to stop existing installed service. You may need to try again in a few seconds."
                );
            }
        }

        private static void Install_PrepareAgentDirectory(
            string agentInstallDir,
            string agentUpdateBackupDir
        )
        {
            try
            {
                if (Directory.Exists(agentInstallDir))
                {
                    if (Directory.Exists(agentUpdateBackupDir))
                    {
                        try
                        {
                            LogUtil.LogInfo(
                                "Previous agent backup directory detected, deleting agent backup directory.",
                                ""
                            );
                            Directory.Delete(agentUpdateBackupDir, true);
                        }
                        catch (Exception)
                        {
                            throw new Exception(
                                "Failed to delete the backup directory in the installation path. Perhaps there is a file open?"
                            );
                        }
                    }
                    System.Collections.Generic.IEnumerable<string> enumerable =
                        Directory.EnumerateFiles(agentInstallDir, "*.dll", 0);
                    System.Collections.Generic.IEnumerable<string> enumerable2 =
                        Directory.EnumerateFiles(agentInstallDir, "*.exe", 0);
                    try
                    {
                        LogUtil.LogInfo(
                            "Create new update backup directory: " + agentUpdateBackupDir,
                            ""
                        );
                        Directory.CreateDirectory(agentUpdateBackupDir);
                        LogUtil.LogInfo(
                            "Copy agent service files to backup directory",
                            ""
                        );
                        foreach (string text in enumerable)
                        {
                            File.Copy(
                                text,
                                Path.Combine(
                                    agentUpdateBackupDir,
                                    Path.GetFileName(text)
                                )
                            );
                        }
                        foreach (string text2 in enumerable2)
                        {
                            if (
                                !Path.GetFileName(text2).Equals("rewst.exe", StringComparison.CurrentCultureIgnoreCase)
                            )
                            {
                                File.Copy(
                                    text2,
                                    Path.Combine(
                                        agentUpdateBackupDir,
                                        Path.GetFileName(text2)
                                    )
                                );
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw new AgentInstallException(
                            "Failed backup installation files prior to update."
                        );
                    }
                    try
                    {
                        LogUtil.LogInfo(
                            "Delete previous agent service executable:  Agent.exe",
                            ""
                        );
                        if (
                            File.Exists(
                                Path.Combine(agentInstallDir, " Agent.exe")
                            )
                        )
                        {
                            File.Delete(
                                Path.Combine(agentInstallDir, " Agent.exe")
                            );
                        }
                        LogUtil.LogInfo(
                            "Delete service agent files",
                            ""
                        );
                        foreach (string text3 in enumerable)
                        {
                            File.Delete(text3);
                        }
                        foreach (string text4 in enumerable2)
                        {
                            File.Delete(text4);
                        }
                        goto IL_1D1;
                    }
                    catch (Exception)
                    {
                        throw new AgentInstallException(
                            "Failed to delete the currently installed agent files in preparation for update."
                        );
                    }
                }
                LogUtil.LogInfo(
                    "Agent service directory does not exist, creating a new one: "
                        + agentInstallDir,
                    ""
                );
                Directory.CreateDirectory(agentInstallDir);
                IL_1D1:
                ;
            }
            catch (AgentInstallException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                throw new AgentInstallException(
                    "An error occured while preparing agent directory."
                );
            }
        }

        private static void Install_ExtractToAgentDirectory(
            string agentInstallDir,
            int? clientId,
            bool writeConfig,
            string tenantName = "",
            bool activityNode = false
        )
        {
            try
            {
                using (
                    MemoryStream memoryStream = new MemoryStream(
                        Properties.Resources.AgentService
                    )
                )
                {
                    new System.IO.Compression.ZipArchive(memoryStream).ExtractToDirectory(
                        agentInstallDir,
                        true
                    );
                }
                LogUtil.LogInfo(
                    "Copy installer file into the agent service directory.",
                    ""
                );
                File.Copy(
                    System.Reflection.Assembly.GetExecutingAssembly().Location,
                    Path.Combine(
                        agentInstallDir,
                        "rewst.exe"
                    ),
                    true
                );
                if (writeConfig)
                {
                    LogUtil.LogInfo(
                        "Write config.json file to installation directory.",
                        ""
                    );
                    string configString =
                        AgentInstallerConfigUtil.GetConfigString(
                            clientId,
                            tenantName,
                            activityNode
                        );
                    File.WriteAllText(
                        Path.Combine(agentInstallDir, "config.json"),
                        configString
                    );
                }
            }
            catch (Exception ex)
            {
                AgentInstallController.DisplayMessage(
                    true,
                    ex.ToString(),
                    ex.ToString()
                );
                throw new AgentInstallException(
                    "Could not extract installation files to agent directory."
                );
            }
        }

        private static void Install_Rollback(string agentInstallDir, string agentUpdateBackupDir)
        {
            LogUtil.LogInfo(
                "Begin rollback of unsuccessful installation.",
                ""
            );
            try
            {
                LogUtil.LogInfo(
                    "Delete agent service files from installation directory.",
                    ""
                );
                System.Collections.Generic.IEnumerable<string> enumerable =
                    Directory.EnumerateFiles(agentInstallDir, "*.dll", 0);
                System.Collections.Generic.IEnumerable<string> enumerable2 =
                    Directory.EnumerateFiles(agentInstallDir, "*.exe", 0);
                File.Delete(Path.Combine(agentInstallDir, " Agent.exe"));
                foreach (string text in enumerable)
                {
                    File.Delete(text);
                }
                foreach (string text2 in enumerable2)
                {
                    if (
                        !Path.GetFileName(text2).Equals("rewst.exe", StringComparison.CurrentCultureIgnoreCase)
                    )
                    {
                        File.Delete(text2);
                    }
                }
                LogUtil.LogInfo(
                    "Restore the previously backed up files to the installation directory.",
                    ""
                );
                foreach (
                    string text3 in Directory.EnumerateFiles(
                        agentUpdateBackupDir,
                        "*.dll",
                        0
                    )
                )
                {
                    File.Copy(
                        text3,
                        Path.Combine(agentInstallDir, Path.GetFileName(text3))
                    );
                }
                File.Copy(
                    Path.Combine(agentUpdateBackupDir, " Agent.exe"),
                    Path.Combine(agentInstallDir, " Agent.exe")
                );
                enumerable2 = Directory.EnumerateFiles(agentUpdateBackupDir, "*.exe", 0);
                foreach (string text4 in enumerable2)
                {
                    if (
                        !Path.GetFileName(text4).Equals("rewst.exe", StringComparison.CurrentCultureIgnoreCase)
                    )
                    {
                        File.Copy(
                            text4,
                            Path.Combine(
                                agentInstallDir,
                                Path.GetFileName(text4)
                            )
                        );
                    }
                }
            }
            catch (Exception)
            {
                throw new AgentInstallException("ROLLBACK_FAILED.");
            }
        }

        private static void Install_EnsureServiceInstalled(string agentInstallDir)
        {
            try
            {
                ServiceInstaller.Install(
                    " Agent",
                    " Agent",
                    "\"" + Path.Combine(agentInstallDir, " Agent.exe") + "\""
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AgentInstallException(ex.Message, true);
            }
            catch (Exception e)
            {
                throw new AgentInstallException(
                    "Failed to install windows service for the agent."
                        + ExceptionLoggingUtil.GetExceptionInformation(
                            e
                        )
                );
            }
        }

        private static void Install_StartService()
        {
            try
            {
                if (
                    ServiceInstaller.GetServiceStatus(" Agent")
                    != ServiceState.Running
                )
                {
                    ServiceInstaller.StartService(" Agent");
                }
            }
            catch (Exception)
            {
                throw new AgentInstallException(
                    "Failed to start windows service for the agent."
                );
            }
        }

        public static void Update(bool isInteractive)
        {
            AgentInstallController.IsInteractiveMode = isInteractive;
            string text = Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder)0x26), " Agent");
            string agentUpdateBackupDir = Path.Combine(text, "UpdateBackup");
            LogUtil.LogInfo(
                "Ensure service is stopped prior to update.",
                ""
            );
            AgentInstallController.Install_StopService();
            LogUtil.LogInfo("Prepare agent install directory.", "");
            AgentInstallController.Install_PrepareAgentDirectory(
                text,
                agentUpdateBackupDir
            );
            try
            {
                LogUtil.LogInfo(
                    "Extract agent executables to installation directory",
                    ""
                );
                AgentInstallController.Install_ExtractToAgentDirectory(
                    text,
                    default(int?),
                    false,
                    "",
                    false
                );
                AgentInstallController.FixVulnerablePathForExistingAgents();
                LogUtil.LogInfo("Start the agent service.", "");
                AgentInstallController.Install_StartService();
            }
            catch (Exception ex)
            {
                try
                {
                    AgentInstallController.DisplayMessage(
                        true,
                        ex.ToString(),
                        "Update"
                    );
                    AgentInstallController.Install_Rollback(
                        text,
                        agentUpdateBackupDir
                    );
                    throw new Exception(ex.Message + " Update has been rolled back.");
                }
                catch (Exception ex2)
                {
                    throw new Exception(ex.Message + " " + ex2.Message);
                }
            }
        }

        public static void Install(
            bool isInteractive,
            int? clientId,
            string tenantName = "",
            bool activityNode = false
        )
        {
            AgentInstallController.IsInteractiveMode = isInteractive;
            string text = Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder)0x26), " Agent");
            string agentUpdateBackupDir = Path.Combine(text, "UpdateBackup");
            AgentInstallController.Uninstall(false, false);
            LogUtil.LogInfo("Prepare agent install directory.", "");
            AgentInstallController.Install_PrepareAgentDirectory(
                text,
                agentUpdateBackupDir
            );
            try
            {
                LogUtil.LogInfo(
                    "Extract agent executables to installation directory",
                    ""
                );
                AgentInstallController.Install_ExtractToAgentDirectory(
                    text,
                    clientId,
                    true,
                    tenantName,
                    activityNode
                );
                LogUtil.LogInfo("Install the windows service.", "");
                AgentInstallController.Install_EnsureServiceInstalled(
                    text
                );
                LogUtil.LogInfo("Start the agent service.", "");
                AgentInstallController.Install_StartService();
            }
            catch (Exception ex)
            {
                try
                {
                    AgentInstallController.DisplayMessage(
                        true,
                        ex.ToString(),
                        "Install"
                    );
                    AgentInstallController.Install_Rollback(
                        text,
                        agentUpdateBackupDir
                    );
                    throw new Exception("Install has been rolled back.");
                }
                catch (Exception ex2)
                {
                    throw new AgentInstallException(
                        ex.Message + " " + ex2.Message
                    );
                }
            }
        }

        private static void Install_Uninstall()
        {
            try
            {
                if (
                    ServiceInstaller.GetServiceStatus(" Agent")
                    != ServiceState.NotFound
                )
                {
                    ServiceInstaller.Uninstall(" Agent");
                }
            }
            catch (Exception)
            {
                throw new AgentInstallException(
                    "Uninstall Failed. Could not remove Windows Service."
                );
            }
        }

        private static void Install_DeleteAgentFiles()
        {
            string text = Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder)0x26), " Agent");
            try
            {
                if (Directory.Exists(text))
                {
                    bool flag = false;
                    int num = 0;
                    if (
                        File.Exists(Path.Combine(text, "LogFile_Installer.txt"))
                    )
                    {
                        string text2 = Path.Combine(
                            Path.GetTempPath(),
                            " UninstallLog_LogFile_Installer.txt"
                        );
                        File.Copy(
                            Path.Combine(text, "LogFile_Installer.txt"),
                            text2,
                            true
                        );
                        LogUtil.LogInfo(
                            "Copied log file to temp: " + text2,
                            ""
                        );
                    }
                    if (File.Exists(Path.Combine(text, "LogFile.txt")))
                    {
                        string text3 = Path.Combine(
                            Path.GetTempPath(),
                            " UninstallLog_LogFile.txt"
                        );
                        File.Copy(
                            Path.Combine(text, "LogFile.txt"),
                            text3,
                            true
                        );
                        LogUtil.LogInfo(
                            "Copied log file to temp: " + text3,
                            ""
                        );
                    }
                    while (!flag && num < 0xA)
                    {
                        try
                        {
                            Directory.Delete(text, true);
                            flag = true;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            LogUtil.LogInfo(
                                "Issue with deleting files (unauthorised) - typically this occurs if a file is open / being accessed - retry "
                                    + (num + 1).ToString()
                                    + " of 10.",
                                ""
                            );
                            num++;
                            Threading.Thread.Sleep(0x3E8);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new AgentInstallException(
                    "Partial uninstall: Windows Service removed but failed to remove agent install directory."
                );
            }
        }

        public static bool RemoveCertificate()
        {
            bool result;
            using (
                Pipes.NamedPipeClientStream namedPipeClientStream =
                    new Pipes.NamedPipeClientStream(".", " servicenamedpipe", (PipeDirection)3)
            )
            {
                try
                {
                    LogUtil.LogInfo(
                        "Connecting to agent service.",
                        ""
                    );
                    namedPipeClientStream.Connect(0x3E8);
                    if (!namedPipeClientStream.IsConnected)
                    {
                        return false;
                    }
                    LogUtil.LogInfo(
                        "Sending uninstall command to agent service.",
                        ""
                    );
                    byte[] bytes = Text.Encoding.ASCII.GetBytes("UNINSTALLrewstAGENT   ");
                    namedPipeClientStream.Write(bytes, 0, bytes.Length);
                    LogUtil.LogInfo(
                        "Read uninstall response from agent service.",
                        ""
                    );
                    byte[] array = new byte[8];
                    namedPipeClientStream.Read(array, 0, 8);
                    string @string = Text.Encoding.ASCII.GetString(array, 0, 7);
                    if (!string.IsNullOrEmpty(@string) && @string != "success")
                    {
                        return false;
                    }
                    LogUtil.LogInfo(
                        "Agent Service certificate removal successful.",
                        ""
                    );
                }
                catch (Exception)
                {
                    return false;
                }
                result = true;
            }
            return result;
        }

        public static bool Uninstall(bool isInteractive = true, bool removeCertificate = false)
        {
            AgentInstallController.IsInteractiveMode = isInteractive;
            if (
                removeCertificate
                && !AgentInstallController.RemoveCertificate()
            )
            {
                LogUtil.LogInfo(
                    "Could not verify certificate removal.",
                    ""
                );
                AgentInstallController.DisplayMessage(
                    false,
                    "Could not verify certificate removal.",
                    ""
                );
            }
            LogUtil.LogInfo("Stop agent service.", "");
            AgentInstallController.Install_StopService();
            LogUtil.LogInfo("Remove  Agent windows service.", "");
            AgentInstallController.Install_Uninstall();
            LogUtil.LogInfo(
                "Delete agent service files from installation directory.",
                ""
            );
            AgentInstallController.Install_DeleteAgentFiles();
            return true;
        }

        private static void DisplayMessage(bool isError, string message, string title = "")
        {
            if (!isError)
            {
                LogUtil.LogInfo(message, "");
            }
            if (AgentInstallController.IsInteractiveMode)
            {
                if (string.IsNullOrEmpty(title))
                {
                    Windows.Forms.MessageBox.Show(message, " Agent");
                    return;
                }
                Windows.Forms.MessageBox.Show(message, title);
            }
        }

        private static void FixVulnerablePathForExistingAgents()
        {
            try
            {
                using (
                    Management.ManagementObject managementObject =
                        new Management.ManagementObject("Win32_Service.Name=' Agent'")
                )
                {
                    string text = managementObject["PathName"].ToString();
                    if (string.IsNullOrEmpty(text))
                    {
                        AgentInstallController.DisplayMessage(
                            true,
                            "Empty Image path of the service was empty.Try running the installer as an Administrator.",
                            "Empty Image path."
                        );
                    }
                    else
                    {
                        if (
                            AgentInstallController.IsPathVulnerable(
                                text
                            )
                        )
                        {
                            string text2 = "\"" + text.Trim(new char[] { '"' }) + "\"";
                            using (
                                Management.ManagementBaseObject methodParameters =
                                    managementObject.GetMethodParameters("Change")
                            )
                            {
                                methodParameters["PathName"] = text2;
                                uint num = (uint)
                                    managementObject.InvokeMethod("Change", methodParameters, null)[
                                        "ReturnValue"
                                    ];
                                if (num == 0U)
                                {
                                    AgentInstallController.DisplayMessage(
                                        false,
                                        "Imagepath has been updated for  Agent. old path: "
                                            + text
                                            + " new path; "
                                            + text2,
                                        ""
                                    );
                                }
                                else
                                {
                                    AgentInstallController.DisplayMessage(
                                        true,
                                        string.Format(
                                            "Imagepath was not updated. Result code {0}. Old path: {1} new path; {2}",
                                            num,
                                            text,
                                            text2
                                        ),
                                        ""
                                    );
                                }
                                goto IL_102;
                            }
                        }
                        AgentInstallController.DisplayMessage(
                            false,
                            "Existing imagePath is valid. " + text + ".",
                            ""
                        );
                    IL_102:
                        ;
                    }
                }
            }
            catch (Exception e)
            {
                AgentInstallController.DisplayMessage(
                    true,
                    "Failed updating imagePath. "
                        + ExceptionLoggingUtil.GetExceptionInformation(
                            e
                        ),
                    ""
                );
            }
        };

        private static bool IsPathVulnerable(string path)
        {
            return (path.StartsWith("\"") && !path.EndsWith("\""))
                || (!path.StartsWith("\"") && path.EndsWith("\""))
                || (!path.StartsWith("\"") && !path.EndsWith("\""));
        };

        public AgentInstallController() { };

        static AgentInstallController() { };

        private static bool IsInteractiveMode = true;

        public const string AgentInstallFolder = "Rewst Agent";

        public const string InstallerLogName = "LogFile_Installer.txt";

        public const string LogName = "LogFile.txt";

        public const string AgentExecuteableName = "RewstAgent.exe";

        public const string ServiceName = "Rewst Agent";

        public const string UpdateBackupDir = "UpdateBackup";

        public const string NamedPipeServer = " servicenamedpipe";

        public const string InstallerName = "rewst.exe";
    }
}
