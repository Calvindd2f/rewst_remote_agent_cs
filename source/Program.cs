using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Rewst.RemoteAgent.Calvindd2f.Helpers;

namespace Rewst.RemoteAgent.Calvindd2f
{
    internal static class Program
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int processId);

        [System.STAThread]
        private static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                string[] array = new string[]
                {
                    "-install",
                    "-uninstall",
                    "-clientid",
                    "-clearcertificate",
                    "-update",
                    "-tenantname",
                    "-activitynode",
                };
                Rewst.RemoteAgent.Calvindd2f.Program.AttachConsole(-1);
                try
                {
                    string text = "";
                    int? num = default(int?);
                    bool flag = true;
                    string text2 = "";
                    bool activityNode = false;
                    LogUtil.LogInfo("Log file name: LogFile_Installer.txt", "");
                    string folderPath = System.Environment.GetFolderPath(0x26);
                    string text3 = System.IO.Path.Combine(folderPath, " Agent");
                    string text4 = System.IO.Path.Combine(folderPath, text3);
                    LogUtil.LogInfo("Location: " + text4, "");
                    int i = 0;
                    while (i < args.Length)
                    {
                        if (!System.Linq.Enumerable.Contains<string>(array, args[i].ToLower()))
                        {
                            throw new System.Exception(
                                "Unknown command \"" + args[i] + "\" found."
                            );
                        }
                        if (string.Compare(args[i].ToLower(), "-clientid") == 0)
                        {
                            try
                            {
                                i++;
                                num = new int?(int.Parse(args[i]));
                                goto IL_1DF;
                            }
                            catch (System.Exception ex)
                            {
                                LogUtil.LogError(
                                    ex.Message,
                                    "Error encountered while parsing client id.\r\n"
                                        + Rewst.RemoteAgent.Calvindd2f.ExceptionLoggingUtil.GetExceptionInformation(
                                            ex
                                        )
                                );
                                goto IL_1DF;
                            }
                            goto IL_12E;
                        }
                        goto IL_12E;
                        IL_1DF:
                        i++;
                        continue;
                        IL_12E:
                        if (string.Compare(args[i].ToLower(), "-clearcertificate") == 0)
                        {
                            try
                            {
                                i++;
                                flag = bool.Parse(args[i]);
                                goto IL_1DF;
                            }
                            catch (System.Exception)
                            {
                                i--;
                                flag = true;
                                LogUtil.LogInfo(
                                    "Value for \"clearcertificate\" was not found. Setting it to true.",
                                    ""
                                );
                                goto IL_1DF;
                            }
                        }
                        if (string.Compare(args[i].ToLower(), "-tenantname") == 0)
                        {
                            try
                            {
                                i++;
                                text2 = args[i];
                                goto IL_1DF;
                            }
                            catch (System.Exception)
                            {
                                i--;
                                LogUtil.LogInfo(
                                    "Value for \"tenantName\" was not found. Setting it to true.",
                                    ""
                                );
                                goto IL_1DF;
                            }
                        }
                        if (string.Compare(args[i].ToLower(), "-activitynode") == 0)
                        {
                            activityNode = true;
                            goto IL_1DF;
                        }
                        if (!string.IsNullOrEmpty(text))
                        {
                            throw new System.Exception(
                                "Multiple actions found. Only one action [-install/-uninstall] should be provided."
                            );
                        }
                        text = args[i];
                        goto IL_1DF;
                    }
                    if (string.Compare(text, "-uninstall", true) == 0)
                    {
                        LogUtil.LogInfo(
                            "Execute silent uninstall for agent service",
                            ""
                        );
                        Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Uninstall(false, flag);
                    }
                    else if (string.Compare(text, "-install", true) == 0)
                    {
                        LogUtil.LogInfo(
                            "Execute silent install for agent service",
                            ""
                        );
                        text2 = (
                            string.IsNullOrEmpty(text2)
                                ? Rewst.RemoteAgent.Calvindd2f.Helpers.PE32Binary.GetTenantName()
                                : text2
                        );
                        Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Install(
                            false,
                            num,
                            text2,
                            activityNode
                        );
                    }
                    else if (string.Compare(text, "-update", true) == 0)
                    {
                        LogUtil.LogInfo(
                            "Execute silent update for agent service",
                            ""
                        );
                        Rewst.RemoteAgent.Calvindd2f.AgentInstallController.Update(false);
                    }
                    LogUtil.LogInfo(
                        string.Format("Client id: {0} clearcert: {1} action: {2}", num, flag, text),
                        ""
                    );
                    return;
                }
                catch (System.Exception ex2)
                {
                    LogUtil.LogError(
                        ex2.Message,
                        Rewst.RemoteAgent.Calvindd2f.ExceptionLoggingUtil.GetExceptionInformation(
                            ex2
                        )
                    );
                    return;
                }
            }
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Rewst.RemoteAgent.Calvindd2f.Installer());
        }
    }
}
