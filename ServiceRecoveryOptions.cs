namespace Rewst.RemoteAgent
{
    public class ServiceRecoveryOptions
	{
		public ServiceRecoveryOptions()
		{
			this.FirstFailureAction =  ServiceRecoveryAction.TakeNoAction;
			this.SecondFailureAction =  ServiceRecoveryAction.TakeNoAction;
			this.SubsequentFailureActions =  ServiceRecoveryAction.TakeNoAction;
			this.DaysToResetFailAcount = 0;
			this.MinutesToRestartService = 1;
		}

		public ServiceRecoveryAction FirstFailureAction
        {
            [System.Runtime.CompilerServices.CompilerGenerated]
            get => this.< FirstFailureAction > k__BackingField;
            [System.Runtime.CompilerServices.CompilerGenerated]
            set => this.< FirstFailureAction > k__BackingField = value;
        }

        public  ServiceRecoveryAction SecondFailureAction
        {
            [System.Runtime.CompilerServices.CompilerGenerated]
            get => this.< SecondFailureAction > k__BackingField;
            [System.Runtime.CompilerServices.CompilerGenerated]
            set => this.< SecondFailureAction > k__BackingField = value;
        }

        public  ServiceRecoveryAction SubsequentFailureActions
        {
            [System.Runtime.CompilerServices.CompilerGenerated]
            get => this.< SubsequentFailureActions > k__BackingField;
            [System.Runtime.CompilerServices.CompilerGenerated]
            set => this.< SubsequentFailureActions > k__BackingField = value;
        }

        public int DaysToResetFailAcount
        {
            [System.Runtime.CompilerServices.CompilerGenerated]
            get => this.< DaysToResetFailAcount > k__BackingField;
            [System.Runtime.CompilerServices.CompilerGenerated]
            set => this.< DaysToResetFailAcount > k__BackingField = value;
        }

        public int MinutesToRestartService
        {
            [System.Runtime.CompilerServices.CompilerGenerated]
            get => this.< MinutesToRestartService > k__BackingField;
            [System.Runtime.CompilerServices.CompilerGenerated]
            set => this.< MinutesToRestartService > k__BackingField = value;
        }

        public string RebootMessage
        {
            [System.Runtime.CompilerServices.CompilerGenerated]
            get => this.< RebootMessage > k__BackingField;
            [System.Runtime.CompilerServices.CompilerGenerated]
            set => this.< RebootMessage > k__BackingField = value;
        }

        public string CommandToLaunchOnFailure
        {
            [System.Runtime.CompilerServices.CompilerGenerated]
            get => this.< CommandToLaunchOnFailure > k__BackingField;
            [System.Runtime.CompilerServices.CompilerGenerated]
            set => this.< CommandToLaunchOnFailure > k__BackingField = value;
        }

        private bool RecoveryActionIsDefined( ServiceRecoveryAction action)
		{
			return this.FirstFailureAction == action || this.SecondFailureAction == action || this.SubsequentFailureActions == action;
		}

		public override bool Equals(object other)
		{
			return other != null && (this == other || (!(other.GetType() != typeof( ServiceRecoveryOptions)) && this.Equals(( ServiceRecoveryOptions)other)));
		}

		public bool Equals( ServiceRecoveryOptions other)
		{
			return other != null && (this == other || (object.Equals(other.FirstFailureAction, this.FirstFailureAction) && object.Equals(other.SecondFailureAction, this.SecondFailureAction) && object.Equals(other.SubsequentFailureActions, this.SubsequentFailureActions) && other.DaysToResetFailAcount == this.DaysToResetFailAcount && other.MinutesToRestartService == this.MinutesToRestartService && object.Equals(other.RebootMessage, this.RebootMessage) && object.Equals(other.CommandToLaunchOnFailure, this.CommandToLaunchOnFailure)));
		}

		public override int GetHashCode()
		{
			return (((((this.FirstFailureAction.GetHashCode() * 0x18D ^ this.SecondFailureAction.GetHashCode()) * 0x18D ^ this.SubsequentFailureActions.GetHashCode()) * 0x18D ^ this.DaysToResetFailAcount) * 0x18D ^ this.MinutesToRestartService) * 0x18D ^ ((this.RebootMessage != null) ? this.RebootMessage.GetHashCode() : 0)) * 0x18D ^ ((this.CommandToLaunchOnFailure != null) ? this.CommandToLaunchOnFailure.GetHashCode() : 0);
		}

		public static  ServiceRecoveryOptions FromConfiguration( RecoveryOptionsElement recoveryOptionsElement)
		{
			return new  ServiceRecoveryOptions
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
		private  ServiceRecoveryAction <FirstFailureAction>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private  ServiceRecoveryAction <SecondFailureAction>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private  ServiceRecoveryAction <SubsequentFailureActions>k__BackingField;

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


