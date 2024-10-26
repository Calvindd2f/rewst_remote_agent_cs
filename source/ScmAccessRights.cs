using System;

namespace Rewst.RemoteAgent.Calvindd2f
{
    [System.Flags]
    public enum ScmAccessRights
    {
        Connect = 1,
        CreateService = 2,
        EnumerateService = 4,
        Lock = 8,
        QueryLockStatus = 0x10,
        ModifyBootConfig = 0x20,
        StandardRightsRequired = 0xF0000,
        AllAccess = 0xF003F,
    }
}
