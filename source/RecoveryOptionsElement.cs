using System;
using System.Configuration;

namespace Rewst.RemoteAgent.Calvindd2f
{
    public class RecoveryOptionsElement : System.Configuration.ConfigurationElement
    {
        public RecoveryOptionsElement()
        {
            this.FirstFailureAction = Rewst
                .RemoteAgent
                .Calvindd2f
                .ServiceRecoveryAction
                .TakeNoAction;
            this.SecondFailureAction = Rewst
                .RemoteAgent
                .Calvindd2f
                .ServiceRecoveryAction
                .TakeNoAction;
            this.SubsequentFailureActions = Rewst
                .RemoteAgent
                .Calvindd2f
                .ServiceRecoveryAction
                .TakeNoAction;
            this.DaysToResetFailAcount = 0;
            this.MinutesToRestartService = 1;
        }

        [System.Configuration.ConfigurationProperty("firstFailureAction")]
        public Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction FirstFailureAction
        {
            get
            {
                return (Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction)
                    base["firstFailureAction"];
            }
            set { base["firstFailureAction"] = value; }
        }

        [System.Configuration.ConfigurationProperty("secondFailureAction")]
        public Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction SecondFailureAction
        {
            get
            {
                return (Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction)
                    base["secondFailureAction"];
            }
            set { base["secondFailureAction"] = value; }
        }

        [System.Configuration.ConfigurationProperty("subsequentFailureAction")]
        public Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction SubsequentFailureActions
        {
            get
            {
                return (Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction)
                    base["subsequentFailureAction"];
            }
            set { base["subsequentFailureAction"] = value; }
        }

        [System.Configuration.ConfigurationProperty("daysToResetFailAcount")]
        public int DaysToResetFailAcount
        {
            get { return (int)base["daysToResetFailAcount"]; }
            set { base["daysToResetFailAcount"] = value; }
        }

        [System.Configuration.ConfigurationProperty("minutesToRestartService")]
        public int MinutesToRestartService
        {
            get { return (int)base["minutesToRestartService"]; }
            set { base["minutesToRestartService"] = value; }
        }

        [System.Configuration.ConfigurationProperty("rebootMessage")]
        public string RebootMessage
        {
            get { return (string)base["rebootMessage"]; }
            set { base["rebootMessage"] = value; }
        }

        [System.Configuration.ConfigurationProperty("commandToLaunchOnFailure")]
        public string CommandToLaunchOnFailure
        {
            get { return (string)base["commandToLaunchOnFailure"]; }
            set { base["commandToLaunchOnFailure"] = value; }
        }
    }
}
