namespace Rewst.RemoteAgent
{
    public class AgentInstallerConfigUtil
    {
        public static string GetConfigString(
            int? clientId,
            string tenantName = "",
            bool activityNode = false
        )
        {
            AgentServiceInstallerEmbeddedConfig config = AgentInstallerConfigUtil.GetConfig();
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

        private static AgentServiceInstallerEmbeddedConfig GetConfig()
        {
            AgentServiceInstallerEmbeddedConfig result;
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
                    result = (AgentServiceInstallerEmbeddedConfig)
                        new System.Xml.Serialization.XmlSerializer(
                            typeof(AgentServiceInstallerEmbeddedConfig)
                        ).Deserialize(manifestResourceStream);
                }
            }
            return result;
        }

        public AgentInstallerConfigUtil() { }

        private const string EMBED_RESOURCE_NAME = "rewst_AGENT_CONFIG_EMBED";
    }

    public class AgentServiceInstallerEmbeddedConfig
    {
        public string TenantDomain { get; set; }
    }
}
