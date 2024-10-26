using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Rewst.RemoteAgent.Calvindd2f.Helpers;
using Rewst.RemoteAgent.Calvindd2f.Properties;

namespace Rewst.RemoteAgent.Calvindd2f
{
    public class AgentInstallController
    {
        public static bool CheckIfInstalled()
        {
            return System.IO.File.Exists(
                System.IO.Path.Combine(
                    System.Environment.GetFolderPath(0x26),
                    " Agent",
                    " Agent.exe"
                )
            );
        }

        private static void Install_StopService()
        {
            try
            {
                Rewst.RemoteAgent.Calvindd2f.ServiceState serviceStatus =
                    Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.GetServiceStatus(" Agent");
                if (
                    serviceStatus != Rewst.RemoteAgent.Calvindd2f.ServiceState.Stopped
                    && serviceStatus != Rewst.RemoteAgent.Calvindd2f.ServiceState.NotFound
                )
                {
                    Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.StopService(" Agent");
                    Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo("Service has been stopped.", "");
                }
                else
                {
                    Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                        "Agent service does not need to be stopped, service state: "
                            + System.Enum.GetName(
                                typeof(Rewst.RemoteAgent.Calvindd2f.ServiceState),
                                serviceStatus
                            ),
                        ""
                    );
                }
            }
            catch (System.Exception)
            {
                throw new Rewst.RemoteAgent.Calvindd2f.AgentInstallException(
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
                if (System.IO.Directory.Exists(agentInstallDir))
                {
                    if (System.IO.Directory.Exists(agentUpdateBackupDir))
                    {
                        try
                        {
                            Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                                "Previous agent backup directory detected, deleting agent backup directory.",
                                ""
                            );
                            System.IO.Directory.Delete(agentUpdateBackupDir, true);
                        }
                        catch (System.Exception)
                        {
                            throw new System.Exception(
                                "Failed to delete the backup directory in the installation path. Perhaps there is a file open?"
                            );
                        }
                    }
                    System.Collections.Generic.IEnumerable<string> enumerable =
                        System.IO.Directory.EnumerateFiles(agentInstallDir, "*.dll", 0);
                    System.Collections.Generic.IEnumerable<string> enumerable2 =
                        System.IO.Directory.EnumerateFiles(agentInstallDir, "*.exe", 0);
                    try
                    {
                        Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                            "Create new update backup directory: " + agentUpdateBackupDir,
                            ""
                        );
                        System.IO.Directory.CreateDirectory(agentUpdateBackupDir);
                        Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                            "Copy agent service files to backup directory",
                            ""
                        );
                        foreach (string text in enumerable)
                        {
                            System.IO.File.Copy(
                                text,
                                System.IO.Path.Combine(
                                    agentUpdateBackupDir,
                                    System.IO.Path.GetFileName(text)
                                )
                            );
                        }
                        foreach (string text2 in enumerable2)
                        {
                            if (
                                System.IO.Path.GetFileName(text2).ToLower()
                                != "rewst.Rewst.RemoteAgent.Calvindd2f.exe".ToLower()
                            )
                            {
                                System.IO.File.Copy(
                                    text2,
                                    System.IO.Path.Combine(
                                        agentUpdateBackupDir,
                                        System.IO.Path.GetFileName(text2)
                                    )
                                );
                            }
                        }
                    }
                    catch (System.Exception)
                    {
                        throw new Rewst.RemoteAgent.Calvindd2f.AgentInstallException(
                            "Failed backup installation files prior to update."
                        );
                    }
                    try
                    {
                        Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                            "Delete previous agent service executable:  Agent.exe",
                            ""
                        );
                        if (
                            System.IO.File.Exists(
                                System.IO.Path.Combine(agentInstallDir, " Agent.exe")
                            )
                        )
                        {
                            System.IO.File.Delete(
                                System.IO.Path.Combine(agentInstallDir, " Agent.exe")
                            );
                        }
                        Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                            "Delete service agent files",
                            ""
                        );
                        foreach (string text3 in enumerable)
                        {
                            System.IO.File.Delete(text3);
                        }
                        foreach (string text4 in enumerable2)
                        {
                            System.IO.File.Delete(text4);
                        }
                        goto IL_1D1;
                    }
                    catch (System.Exception)
                    {
                        throw new Rewst.RemoteAgent.Calvindd2f.AgentInstallException(
                            "Failed to delete the currently installed agent files in preparation for update."
                        );
                    }
                }
                Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                    "Agent service directory does not exist, creating a new one: "
                        + agentInstallDir,
                    ""
                );
                System.IO.Directory.CreateDirectory(agentInstallDir);
                IL_1D1:
                ;
            }
            catch (Rewst.RemoteAgent.Calvindd2f.AgentInstallException ex)
            {
                throw ex;
            }
            catch (System.Exception)
            {
                throw new Rewst.RemoteAgent.Calvindd2f.AgentInstallException(
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
                    System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(
                        Rewst.RemoteAgent.Calvindd2f.Properties.Resources.AgentService
                    )
                )
                {
                    new System.IO.Compression.ZipArchive(memoryStream).ExtractToDirectory(
                        agentInstallDir,
                        true
                    );
                }
                Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                    "Copy installer file into the agent service directory.",
                    ""
                );
                System.IO.File.Copy(
                    System.Reflection.Assembly.GetExecutingAssembly().Location,
                    System.IO.Path.Combine(
                        agentInstallDir,
                        "rewst.Rewst.RemoteAgent.Calvindd2f.exe"
                    ),
                    true
                );
                if (writeConfig)
                {
                    Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                        "Write config.json file to installation directory.",
                        ""
                    );
                    string configString =
                        Rewst.RemoteAgent.Calvindd2f.AgentInstallerConfigUtil.GetConfigString(
                            clientId,
                            tenantName,
                            activityNode
                        );
                    System.IO.File.WriteAllText(
                        System.IO.Path.Combine(agentInstallDir, "config.json"),
                        configString
                    );
                }
            }
            catch (System.Exception ex)
            {
                Rewst.RemoteAgent.Calvindd2f.AgentInstallController.DisplayMessage(
                    true,
                    ex.ToString(),
                    ex.ToString()
                );
                throw new Rewst.RemoteAgent.Calvindd2f.AgentInstallException(
                    "Could not extract installation files to agent directory."
                );
            }
        }

        private static void Install_Rollback(string agentInstallDir, string agentUpdateBackupDir)
        {
            Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                "Begin rollback of unsuccessful installation.",
                ""
            );
            try
            {
                Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                    "Delete agent service files from installation directory.",
                    ""
                );
                System.Collections.Generic.IEnumerable<string> enumerable =
                    System.IO.Directory.EnumerateFiles(agentInstallDir, "*.dll", 0);
                System.Collections.Generic.IEnumerable<string> enumerable2 =
                    System.IO.Directory.EnumerateFiles(agentInstallDir, "*.exe", 0);
                System.IO.File.Delete(System.IO.Path.Combine(agentInstallDir, " Agent.exe"));
                foreach (string text in enumerable)
                {
                    System.IO.File.Delete(text);
                }
                foreach (string text2 in enumerable2)
                {
                    if (
                        System.IO.Path.GetFileName(text2).ToLower()
                        != "rewst.Rewst.RemoteAgent.Calvindd2f.exe".ToLower()
                    )
                    {
                        System.IO.File.Delete(text2);
                    }
                }
                Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                    "Restore the previously backed up files to the installation directory.",
                    ""
                );
                foreach (
                    string text3 in System.IO.Directory.EnumerateFiles(
                        agentUpdateBackupDir,
                        "*.dll",
                        0
                    )
                )
                {
                    System.IO.File.Copy(
                        text3,
                        System.IO.Path.Combine(agentInstallDir, System.IO.Path.GetFileName(text3))
                    );
                }
                System.IO.File.Copy(
                    System.IO.Path.Combine(agentUpdateBackupDir, " Agent.exe"),
                    System.IO.Path.Combine(agentInstallDir, " Agent.exe")
                );
                enumerable2 = System.IO.Directory.EnumerateFiles(agentUpdateBackupDir, "*.exe", 0);
                foreach (string text4 in enumerable2)
                {
                    if (
                        System.IO.Path.GetFileName(text4).ToLower()
                        != "rewst.Rewst.RemoteAgent.Calvindd2f.exe".ToLower()
                    )
                    {
                        System.IO.File.Copy(
                            text4,
                            System.IO.Path.Combine(
                                agentInstallDir,
                                System.IO.Path.GetFileName(text4)
                            )
                        );
                    }
                }
            }
            catch (System.Exception)
            {
                throw new Rewst.RemoteAgent.Calvindd2f.AgentInstallException("ROLLBACK_FAILED.");
            }
        }

        private static void Install_EnsureServiceInstalled(string agentInstallDir)
        {
            try
            {
                Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.Install(
                    " Agent",
                    " Agent",
                    "\"" + System.IO.Path.Combine(agentInstallDir, " Agent.exe") + "\""
                );
            }
            catch (System.InvalidOperationException ex)
            {
                throw new Rewst.RemoteAgent.Calvindd2f.AgentInstallException(ex.Message, true);
            }
            catch (System.Exception e)
            {
                throw new Rewst.RemoteAgent.Calvindd2f.AgentInstallException(
                    "Failed to install windows service for the agent."
                        + Rewst.RemoteAgent.Calvindd2f.ExceptionLoggingUtil.GetExceptionInformation(
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
                    Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.GetServiceStatus(" Agent")
                    != Rewst.RemoteAgent.Calvindd2f.ServiceState.Running
                )
                {
                    Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.StartService(" Agent");
                }
            }
            catch (System.Exception)
            {
                throw new Rewst.RemoteAgent.Calvindd2f.AgentInstallException(
                    "Failed to start windows service for the agent."
                );
            }
        }

        public static void Update(bool isInteractive)
        {
            Rewst.RemoteAgent.Calvindd2f.AgentInstallController.IsInteractiveMode = isInteractive;
            string text = System.IO.Path.Combine(System.Environment.GetFolderPath(0x26), " Agent");
            string agentUpdateBackupDir = System.IO.Path.Combine(text, "UpdateBackup");
            Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                "Ensure service is stopped prior to update.",
                ""
            );
            Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install_StopService();
            Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo("Prepare agent install directory.", "");
            Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install_PrepareAgentDirectory(
                text,
                agentUpdateBackupDir
            );
            try
            {
                Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                    "Extract agent executables to installation directory",
                    ""
                );
                Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install_ExtractToAgentDirectory(
                    text,
                    default(int?),
                    false,
                    "",
                    false
                );
                Rewst.RemoteAgent.Calvindd2f.AgentInstallController.FixVulnerablePathForExistingAgents();
                Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo("Start the agent service.", "");
                Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install_StartService();
            }
            catch (System.Exception ex)
            {
                try
                {
                    Rewst.RemoteAgent.Calvindd2f.AgentInstallController.DisplayMessage(
                        true,
                        ex.ToString(),
                        "Update"
                    );
                    Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install_Rollback(
                        text,
                        agentUpdateBackupDir
                    );
                    throw new System.Exception(ex.Message + " Update has been rolled back.");
                }
                catch (System.Exception ex2)
                {
                    throw new System.Exception(ex.Message + " " + ex2.Message);
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
            Rewst.RemoteAgent.Calvindd2f.AgentInstallController.IsInteractiveMode = isInteractive;
            string text = System.IO.Path.Combine(System.Environment.GetFolderPath(0x26), " Agent");
            string agentUpdateBackupDir = System.IO.Path.Combine(text, "UpdateBackup");
            Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Uninstall(false, false);
            Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo("Prepare agent install directory.", "");
            Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install_PrepareAgentDirectory(
                text,
                agentUpdateBackupDir
            );
            try
            {
                Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                    "Extract agent executables to installation directory",
                    ""
                );
                Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install_ExtractToAgentDirectory(
                    text,
                    clientId,
                    true,
                    tenantName,
                    activityNode
                );
                Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo("Install the windows service.", "");
                Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install_EnsureServiceInstalled(
                    text
                );
                Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo("Start the agent service.", "");
                Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install_StartService();
            }
            catch (System.Exception ex)
            {
                try
                {
                    Rewst.RemoteAgent.Calvindd2f.AgentInstallController.DisplayMessage(
                        true,
                        ex.ToString(),
                        "Install"
                    );
                    Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install_Rollback(
                        text,
                        agentUpdateBackupDir
                    );
                    throw new System.Exception("Install has been rolled back.");
                }
                catch (System.Exception ex2)
                {
                    throw new Rewst.RemoteAgent.Calvindd2f.AgentInstallException(
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
                    Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.GetServiceStatus(" Agent")
                    != Rewst.RemoteAgent.Calvindd2f.ServiceState.NotFound
                )
                {
                    Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.Uninstall(" Agent");
                }
            }
            catch (System.Exception)
            {
                throw new Rewst.RemoteAgent.Calvindd2f.AgentInstallException(
                    "Uninstall Failed. Could not remove Windows Service."
                );
            }
        }

        private static void Install_DeleteAgentFiles()
        {
            string text = System.IO.Path.Combine(System.Environment.GetFolderPath(0x26), " Agent");
            try
            {
                if (System.IO.Directory.Exists(text))
                {
                    bool flag = false;
                    int num = 0;
                    if (
                        System.IO.File.Exists(System.IO.Path.Combine(text, "LogFile_Installer.txt"))
                    )
                    {
                        string text2 = System.IO.Path.Combine(
                            System.IO.Path.GetTempPath(),
                            " UninstallLog_LogFile_Installer.txt"
                        );
                        System.IO.File.Copy(
                            System.IO.Path.Combine(text, "LogFile_Installer.txt"),
                            text2,
                            true
                        );
                        Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                            "Copied log file to temp: " + text2,
                            ""
                        );
                    }
                    if (System.IO.File.Exists(System.IO.Path.Combine(text, "LogFile.txt")))
                    {
                        string text3 = System.IO.Path.Combine(
                            System.IO.Path.GetTempPath(),
                            " UninstallLog_LogFile.txt"
                        );
                        System.IO.File.Copy(
                            System.IO.Path.Combine(text, "LogFile.txt"),
                            text3,
                            true
                        );
                        Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                            "Copied log file to temp: " + text3,
                            ""
                        );
                    }
                    while (!flag && num < 0xA)
                    {
                        try
                        {
                            System.IO.Directory.Delete(text, true);
                            flag = true;
                        }
                        catch (System.UnauthorizedAccessException)
                        {
                            Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                                "Issue with deleting files (unauthorised) - typically this occurs if a file is open / being accessed - retry "
                                    + (num + 1).ToString()
                                    + " of 10.",
                                ""
                            );
                            num++;
                            System.Threading.Thread.Sleep(0x3E8);
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                throw new Rewst.RemoteAgent.Calvindd2f.AgentInstallException(
                    "Partial uninstall: Windows Service removed but failed to remove agent install directory."
                );
            }
        }

        public static bool RemoveCertificate()
        {
            bool result;
            using (
                System.IO.Pipes.NamedPipeClientStream namedPipeClientStream =
                    new System.IO.Pipes.NamedPipeClientStream(".", " servicenamedpipe", 3)
            )
            {
                try
                {
                    Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                        "Connecting to agent service.",
                        ""
                    );
                    namedPipeClientStream.Connect(0x3E8);
                    if (!namedPipeClientStream.IsConnected)
                    {
                        return false;
                    }
                    Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                        "Sending uninstall command to agent service.",
                        ""
                    );
                    byte[] bytes = System.Text.Encoding.ASCII.GetBytes("UNINSTALLrewstAGENT   ");
                    namedPipeClientStream.Write(bytes, 0, bytes.Length);
                    Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                        "Read uninstall response from agent service.",
                        ""
                    );
                    byte[] array = new byte[8];
                    namedPipeClientStream.Read(array, 0, 8);
                    string @string = System.Text.Encoding.ASCII.GetString(array, 0, 7);
                    if (!string.IsNullOrEmpty(@string) && @string != "success")
                    {
                        return false;
                    }
                    Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                        "Agent Service certificate removal successful.",
                        ""
                    );
                }
                catch (System.Exception)
                {
                    return false;
                }
                result = true;
            }
            return result;
        }

        public static bool Uninstall(bool isInteractive = true, bool removeCertificate = false)
        {
            Rewst.RemoteAgent.Calvindd2f.AgentInstallController.IsInteractiveMode = isInteractive;
            if (
                removeCertificate
                && !Rewst.RemoteAgent.Calvindd2f.AgentInstallController.RemoveCertificate()
            )
            {
                Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                    "Could not verify certificate removal.",
                    ""
                );
                Rewst.RemoteAgent.Calvindd2f.AgentInstallController.DisplayMessage(
                    false,
                    "Could not verify certificate removal.",
                    ""
                );
            }
            Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo("Stop agent service.", "");
            Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install_StopService();
            Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo("Remove  Agent windows service.", "");
            Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install_Uninstall();
            Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(
                "Delete agent service files from installation directory.",
                ""
            );
            Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install_DeleteAgentFiles();
            return true;
        }

        private static void DisplayMessage(bool isError, string message, string title = "")
        {
            if (!isError)
            {
                Rewst.RemoteAgent.Calvindd2f.LogUtil.LogInfo(message, "");
            }
            if (Rewst.RemoteAgent.Calvindd2f.AgentInstallController.IsInteractiveMode)
            {
                if (string.IsNullOrEmpty(title))
                {
                    System.Windows.Forms.MessageBox.Show(message, " Agent");
                    return;
                }
                System.Windows.Forms.MessageBox.Show(message, title);
            }
        }

        private static void FixVulnerablePathForExistingAgents()
        {
            try
            {
                using (
                    System.Management.ManagementObject managementObject =
                        new System.Management.ManagementObject("Win32_Service.Name=' Agent'")
                )
                {
                    string text = managementObject["PathName"].ToString();
                    if (string.IsNullOrEmpty(text))
                    {
                        Rewst.RemoteAgent.Calvindd2f.AgentInstallController.DisplayMessage(
                            true,
                            "Empty Image path of the service was empty.Try running the installer as an Administrator.",
                            "Empty Image path."
                        );
                    }
                    else
                    {
                        if (
                            Rewst.RemoteAgent.Calvindd2f.AgentInstallController.IsPathVulnerable(
                                text
                            )
                        )
                        {
                            string text2 = "\"" + text.Trim(new char[] { '"' }) + "\"";
                            using (
                                System.Management.ManagementBaseObject methodParameters =
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
                                    Rewst.RemoteAgent.Calvindd2f.AgentInstallController.DisplayMessage(
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
                                    Rewst.RemoteAgent.Calvindd2f.AgentInstallController.DisplayMessage(
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
                        Rewst.RemoteAgent.Calvindd2f.AgentInstallController.DisplayMessage(
                            false,
                            "Existing imagePath is valid. " + text + ".",
                            ""
                        );
                        IL_102:
                        ;
                    }
                }
            }
            catch (System.Exception e)
            {
                Rewst.RemoteAgent.Calvindd2f.AgentInstallController.DisplayMessage(
                    true,
                    "Failed updating imagePath. "
                        + Rewst.RemoteAgent.Calvindd2f.ExceptionLoggingUtil.GetExceptionInformation(
                            e
                        ),
                    ""
                );
            }
        }

        private static bool IsPathVulnerable(string path)
        {
            return (path.StartsWith("\"") && !path.EndsWith("\""))
                || (!path.StartsWith("\"") && path.EndsWith("\""))
                || (!path.StartsWith("\"") && !path.EndsWith("\""));
        }

        public AgentInstallController() { }

        // Note: this type is marked as 'beforefieldinit'.
        static AgentInstallController() { }

        private static bool IsInteractiveMode = true;

        public const string AgentInstallFolder = " Agent";

        public const string InstallerLogName = "LogFile_Installer.txt";

        public const string LogName = "LogFile.txt";

        public const string AgentExecuteableName = " Agent.exe";

        public const string ServiceName = " Agent";

        public const string UpdateBackupDir = "UpdateBackup";

        public const string NamedPipeServer = " servicenamedpipe";

        public const string InstallerName = "rewst.Rewst.RemoteAgent.Calvindd2f.exe";
    }
}
