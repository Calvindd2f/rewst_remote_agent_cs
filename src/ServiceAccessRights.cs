namespace Rewst.RemoteAgent
{
    [System.Flags]
    public enum ServiceAccessRights
    {
        QueryConfig = 1,
        ChangeConfig = 2,
        QueryStatus = 4,
        EnumerateDependants = 8,
        Start = 0x10,
        Stop = 0x20,
        PauseContinue = 0x40,
        Interrogate = 0x80,
        UserDefinedControl = 0x100,
        Delete = 0x10000,
        StandardRightsRequired = 0xF0000,
        AllAccess = 0xF01FF,
    }
}
