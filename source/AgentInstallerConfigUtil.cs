using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Rewst.RemoteAgent.Calvindd2f
{
    public class AgentInstallerConfigUtil
    {
        public static string GetConfigString(
            int? clientId,
            string tenantName = "",
            bool activityNode = false
        )
        {
            Rewst.RemoteAgent.Calvindd2f.AgentServiceInstallerEmbeddedConfig config =
                Rewst.RemoteAgent.Calvindd2f.AgentInstallerConfigUtil.GetConfig();
            string text;
            if (clientId != null)
            {
                int? num = clientId;
                int num2 = 0;
                if (num.GetValueOrDefault() >= num2 & num != null)
                {
                    text = clientId.ToString();
                    goto IL_3D;
                }
            }
            text = "0";
            IL_3D:
            string text2 = text;
            string text3 = System.Text.RegularExpressions.Regex.Replace(
                ((config != null) ? config.TenantDomain : null) ?? tenantName,
                "[^0-9\\.A-Za-z-:]",
                ""
            );
            return ""
                + "{"
                + "    \"ClientId\":  "
                + text2
                + ","
                + (activityNode ? "   \"IsActivityNode\": true," : "")
                + "    \"TenantDomain\": \""
                + text3
                + "\""
                + "}";
        }

        private static Rewst.RemoteAgent.Calvindd2f.AgentServiceInstallerEmbeddedConfig GetConfig()
        {
            Rewst.RemoteAgent.Calvindd2f.AgentServiceInstallerEmbeddedConfig result;
            using (
                System.IO.Stream manifestResourceStream = System
                    .Reflection.Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("rewst_AGENT_CONFIG_EMBED")
            )
            {
                if (manifestResourceStream == null)
                {
                    result = null;
                }
                else
                {
                    result = (Rewst.RemoteAgent.Calvindd2f.AgentServiceInstallerEmbeddedConfig)
                        new System.Xml.Serialization.XmlSerializer(
                            typeof(Rewst.RemoteAgent.Calvindd2f.AgentServiceInstallerEmbeddedConfig)
                        ).Deserialize(manifestResourceStream);
                }
            }
            return result;
        }

        public AgentInstallerConfigUtil() { }

        private const string EMBED_RESOURCE_NAME = "rewst_AGENT_CONFIG_EMBED";
    }
}
