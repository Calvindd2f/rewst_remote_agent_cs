using System;
using System.Runtime.CompilerServices;

namespace Rewst.RemoteAgent.Calvindd2f
{
	public class ServiceRecoveryOptions
	{
		public ServiceRecoveryOptions()
		{
			this.FirstFailureAction =  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.TakeNoAction;
			this.SecondFailureAction =  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.TakeNoAction;
			this.SubsequentFailureActions =  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.TakeNoAction;
			this.DaysToResetFailAcount = 0;
			this.MinutesToRestartService = 1;
		}

		public  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction FirstFailureAction
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<FirstFailureAction>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<FirstFailureAction>k__BackingField = value;
			}
		}

		public  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction SecondFailureAction
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<SecondFailureAction>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<SecondFailureAction>k__BackingField = value;
			}
		}

		public  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction SubsequentFailureActions
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<SubsequentFailureActions>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<SubsequentFailureActions>k__BackingField = value;
			}
		}

		public int DaysToResetFailAcount
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<DaysToResetFailAcount>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<DaysToResetFailAcount>k__BackingField = value;
			}
		}

		public int MinutesToRestartService
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<MinutesToRestartService>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<MinutesToRestartService>k__BackingField = value;
			}
		}

		public string RebootMessage
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<RebootMessage>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<RebootMessage>k__BackingField = value;
			}
		}

		public string CommandToLaunchOnFailure
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<CommandToLaunchOnFailure>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<CommandToLaunchOnFailure>k__BackingField = value;
			}
		}

		private bool RecoveryActionIsDefined( Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction action)
		{
			return this.FirstFailureAction == action || this.SecondFailureAction == action || this.SubsequentFailureActions == action;
		}

		public override bool Equals(object other)
		{
			return other != null && (this == other || (!(other.GetType() != typeof( Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryOptions)) && this.Equals(( Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryOptions)other)));
		}

		public bool Equals( Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryOptions other)
		{
			return other != null && (this == other || (object.Equals(other.FirstFailureAction, this.FirstFailureAction) && object.Equals(other.SecondFailureAction, this.SecondFailureAction) && object.Equals(other.SubsequentFailureActions, this.SubsequentFailureActions) && other.DaysToResetFailAcount == this.DaysToResetFailAcount && other.MinutesToRestartService == this.MinutesToRestartService && object.Equals(other.RebootMessage, this.RebootMessage) && object.Equals(other.CommandToLaunchOnFailure, this.CommandToLaunchOnFailure)));
		}

		public override int GetHashCode()
		{
			return (((((this.FirstFailureAction.GetHashCode() * 0x18D ^ this.SecondFailureAction.GetHashCode()) * 0x18D ^ this.SubsequentFailureActions.GetHashCode()) * 0x18D ^ this.DaysToResetFailAcount) * 0x18D ^ this.MinutesToRestartService) * 0x18D ^ ((this.RebootMessage != null) ? this.RebootMessage.GetHashCode() : 0)) * 0x18D ^ ((this.CommandToLaunchOnFailure != null) ? this.CommandToLaunchOnFailure.GetHashCode() : 0);
		}

		public static  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryOptions FromConfiguration( Rewst.RemoteAgent.Calvindd2f.RecoveryOptionsElement recoveryOptionsElement)
		{
			return new  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryOptions
			{
				FirstFailureAction = recoveryOptionsElement.FirstFailureAction,
				SecondFailureAction = recoveryOptionsElement.SecondFailureAction,
				SubsequentFailureActions = recoveryOptionsElement.SubsequentFailureActions,
				DaysToResetFailAcount = recoveryOptionsElement.DaysToResetFailAcount,
				CommandToLaunchOnFailure = recoveryOptionsElement.CommandToLaunchOnFailure,
				MinutesToRestartService = recoveryOptionsElement.MinutesToRestartService,
				RebootMessage = recoveryOptionsElement.RebootMessage
			};
		}

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction <FirstFailureAction>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction <SecondFailureAction>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction <SubsequentFailureActions>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private int <DaysToResetFailAcount>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private int <MinutesToRestartService>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private string <RebootMessage>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private string <CommandToLaunchOnFailure>k__BackingField;
	}
}


