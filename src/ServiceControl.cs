namespace Rewst.RemoteAgent
{
    public enum ServiceControl
    {
        Stop = 1,
        Pause,
        Continue,
        Interrogate,
        Shutdown,
        ParamChange,
        NetBindAdd,
        NetBindRemove,
        NetBindEnable,
        NetBindDisable,
    }
}
