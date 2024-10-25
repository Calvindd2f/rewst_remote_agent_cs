using System.Configuration;
using Rewst.Log;

namespace Rewst.RemoteAgent
{
    public class RecoveryOptionsElement : ConfigurationElement
    {
        public RecoveryOptionsElement()
        {
            this.FirstFailureAction = ServiceRecoveryAction.TakeNoAction;
            this.SecondFailureAction = ServiceRecoveryAction.TakeNoAction;
            this.SubsequentFailureActions = ServiceRecoveryAction.TakeNoAction;
            this.DaysToResetFailAcount = 0;
            this.MinutesToRestartService = 1;
        }

        [ConfigurationProperty("firstFailureAction")]
        public ServiceRecoveryAction FirstFailureAction
        {
            get
            {
                return (ServiceRecoveryAction)
                    base["firstFailureAction"];
            }
            set { base["firstFailureAction"] = value; }
        }

        [ConfigurationProperty("secondFailureAction")]
        public ServiceRecoveryAction SecondFailureAction
        {
            get
            {
                return (ServiceRecoveryAction)
                    base["secondFailureAction"];
            }
            set { base["secondFailureAction"] = value; }
        }

        [ConfigurationProperty("subsequentFailureAction")]
        public ServiceRecoveryAction SubsequentFailureActions
        {
            get
            {
                return (ServiceRecoveryAction)
                    base["subsequentFailureAction"];
            }
            set { base["subsequentFailureAction"] = value; }
        }

        [ConfigurationProperty("daysToResetFailAcount")]
        public int DaysToResetFailAcount
        {
            get { return (int)base["daysToResetFailAcount"]; }
            set { base["daysToResetFailAcount"] = value; }
        }

        [ConfigurationProperty("minutesToRestartService")]
        public int MinutesToRestartService
        {
            get { return (int)base["minutesToRestartService"]; }
            set { base["minutesToRestartService"] = value; }
        }

        [ConfigurationProperty("rebootMessage")]
        public string RebootMessage
        {
            get { return (string)base["rebootMessage"]; }
            set { base["rebootMessage"] = value; }
        }

        [ConfigurationProperty("commandToLaunchOnFailure")]
        public string CommandToLaunchOnFailure
        {
            get { return (string)base["commandToLaunchOnFailure"]; }
            set { base["commandToLaunchOnFailure"] = value; }
        }
    }
}
