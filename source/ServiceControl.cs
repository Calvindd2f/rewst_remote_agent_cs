using System;

namespace Rewst.RemoteAgent.Calvindd2f
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
