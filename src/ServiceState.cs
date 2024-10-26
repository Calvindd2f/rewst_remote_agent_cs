namespace Rewst.RemoteAgent
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
