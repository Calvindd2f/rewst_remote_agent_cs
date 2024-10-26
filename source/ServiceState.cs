using System;

namespace Rewst.RemoteAgent.Calvindd2f
{
    public enum ServiceState
    {
        Unknown = -1,
        NotFound,
        Stopped,
        StartPending,
        StopPending,
        Running,
        ContinuePending,
        PausePending,
        Paused,
    }
}
