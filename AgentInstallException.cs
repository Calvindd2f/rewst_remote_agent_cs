using System.Runtime.CompilerServices;

namespace Rewst.RemoteAgent
{
    public class AgentInstallException : System.Exception
    {
        public bool DisplayNotification
        {
            [CompilerGenerated]
            get
            {
                return this.<DisplayNotification>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<DisplayNotification>k__BackingField = value;
            }
        }

        public AgentInstallException(string message, bool displayNotification) : base(message)
        {
            this.DisplayNotification = displayNotification;
        }

        public AgentInstallException(string message) : base(message)
        {
        }

        [CompilerGenerated]
        private bool <DisplayNotification>k__BackingField;
    }
}