namespace Rewst.RemoteAgent
{
    public class AgentInstallException :  System.Exception
	{
		public bool DisplayNotification
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<DisplayNotification>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
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

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private bool <DisplayNotification>k__BackingField;
	}
}