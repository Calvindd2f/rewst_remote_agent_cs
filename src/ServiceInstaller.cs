using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rewst.RemoteAgent
{
    public static class ServiceInstaller
    {
        public static string GetAllowedUserAccountUserName
        {
            [CompilerGenerated]
            get
            {
                return ServiceInstaller.< GetAllowedUserAccountUserName > k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                ServiceInstaller.< GetAllowedUserAccountUserName > k__BackingField = value;
            }
        }

        public static string GetAllowedUserAccountPassword
        {
            [CompilerGenerated]
            get
            {
                return ServiceInstaller.< GetAllowedUserAccountPassword > k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                ServiceInstaller.< GetAllowedUserAccountPassword > k__BackingField = value;
            }
        }

        public static string GetAllowedDomainUserAccount
        {
            [CompilerGenerated]
            get
            {
                return ServiceInstaller.< GetAllowedDomainUserAccount > k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                ServiceInstaller.< GetAllowedDomainUserAccount > k__BackingField = value;
            }
        }

        [DllImport("advapi32.dll", CharSet = 3, EntryPoint = "OpenSCManagerW", ExactSpelling = true, SetLastError = true)]
        private static extern System.IntPtr OpenSCManager(string machineName, string databaseName, ScmAccessRights dwDesiredAccess);

        [DllImport("advapi32.dll", CharSet = 4, SetLastError = true)]
        private static extern System.IntPtr OpenService(System.IntPtr hSCManager, string lpServiceName, ServiceAccessRights dwDesiredAccess);

        [DllImport("advapi32.dll", CharSet = 4, SetLastError = true)]
        private static extern System.IntPtr CreateService(System.IntPtr hSCManager, string lpServiceName, string lpDisplayName, ServiceAccessRights dwDesiredAccess, int dwServiceType, ServiceBootFlag dwStartType, ServiceError dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, System.IntPtr lpdwTagId, string lpDependencies, string lp, string lpPassword);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(2)]
        private static extern bool CloseServiceHandle(System.IntPtr hSCObject);

        [DllImport("advapi32.dll")]
        private static extern int QueryServiceStatus(System.IntPtr hService, SERVICE_STATUS lpServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(2)]
        private static extern bool DeleteService(System.IntPtr hService);

        [DllImport("advapi32.dll")]
        private static extern int ControlService(System.IntPtr hService, ServiceControl dwControl, SERVICE_STATUS lpServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int StartService(System.IntPtr hService, int dwNumServiceArgs, int lpServiceArgVectors);

        [DllImport("advapi32.dll", CharSet = 3, SetLastError = true)]
        [return: MarshalAs(2)]
        private static extern bool ChangeServiceConfig2(System.IntPtr hService, int dwInfoLevel, System.IntPtr lpInfo);

        [DllImport("advapi32.dll", CharSet = 3, SetLastError = true)]
        private static extern int QueryServiceConfig2(System.IntPtr hService, int dwInfoLevel, System.IntPtr lpBuffer, uint cbBufSize, out uint pcbBytesNeeded);

        public static void Uninstall(string serviceName)
        {
            System.IntPtr intPtr = ServiceInstaller.OpenSCManager(ScmAccessRights.AllAccess);
            try
            {
                System.IntPtr intPtr2 = ServiceInstaller.OpenService(intPtr, serviceName, ServiceAccessRights.AllAccess);
                if (intPtr2 == System.IntPtr.Zero)
                {
                    throw new ApplicationException("Service not installed.");
                }
                try
                {
                    StopService(intPtr2);
                    if (!DeleteService(intPtr2))
                    {
                        int lastWin32Error = Marshal.GetLastWin32Error();
                        if (lastWin32Error != 0x430)
                        {
                            throw new ApplicationException("Could not delete service " + lastWin32Error.ToString());
                        }
                    }
                }
                finally
                {
                    CloseServiceHandle(intPtr2);
                }
            }
            finally
            {
                CloseServiceHandle(intPtr);
            }
        }

        public static bool ServiceIsInstalled(string serviceName)
        {
            System.IntPtr intPtr = ServiceInstaller.OpenSCManager(ScmAccessRights.Connect);
            bool result;
            try
            {
                System.IntPtr intPtr2 = ServiceInstaller.OpenService(intPtr, serviceName, ServiceAccessRights.QueryStatus);
                if (intPtr2 == System.IntPtr.Zero)
                {
                    result = false;
                }
                else
                {
                    CloseServiceHandle(intPtr2);
                    result = true;
                }
            }
            finally
            {
                CloseServiceHandle(intPtr);
            }
            return result;
        }

        public static void Install(string serviceName, string displayName, string fileName)
        {
            System.IntPtr intPtr = ServiceInstaller.OpenSCManager(ScmAccessRights.AllAccess);
            try
            {
                System.IntPtr intPtr2 = System.IntPtr.Zero;
                try
                {
                    intPtr2 = ServiceInstaller.CreateService(intPtr, serviceName, displayName, ServiceAccessRights.AllAccess, 0x10, ServiceBootFlag.AutoStart, ServiceError.Normal, fileName, null, System.IntPtr.Zero, null, null, null);
                    if (intPtr2 == System.IntPtr.Zero)
                    {
                        int lastWin32Error = Marshal.GetLastWin32Error();
                        if (lastWin32Error == 0x430)
                        {
                            throw new InvalidOperationException("The service cannot be installed as it is in an uninstalling state after being recently uninstalled. Please wait and retry again in a few minutes. If the issue persists, restart the machine before attempting again.");
                        }
                        throw new ApplicationException("Failed to install service with error: " + lastWin32Error.ToString());
                    }
                }
                finally
                {
                    if (intPtr2 != System.IntPtr.Zero)
                    {
                        CloseServiceHandle(intPtr2);
                    }
                }
            }
            finally
            {
                CloseServiceHandle(intPtr);
            }
        }

        public static void StartService(string serviceName)
        {
            System.IntPtr intPtr = ServiceInstaller.OpenSCManager(ScmAccessRights.Connect);
            try
            {
                System.IntPtr intPtr2 = ServiceInstaller.OpenService(intPtr, serviceName, ServiceAccessRights.QueryStatus | ServiceAccessRights.Start);
                if (intPtr2 == IntPtr.Zero)
                {
                    throw new ApplicationException("Could not open service.");
                }
                try
                {
                    SetServiceRecoveryOptions(serviceName);
                }
                catch (Exception)
                {
                }
                try
                {
                    StartService(intPtr2);
                }
                finally
                {
                    CloseServiceHandle(intPtr2);
                }
            }
            finally
            {
                CloseServiceHandle(intPtr);
            }
        }

        public static void StopService(string serviceName)
        {
            System.IntPtr intPtr = ServiceInstaller.OpenSCManager(ScmAccessRights.Connect);
            try
            {
                System.IntPtr intPtr2 = ServiceInstaller.OpenService(intPtr, serviceName, ServiceAccessRights.QueryStatus | ServiceAccessRights.Stop);
                if (intPtr2 == IntPtr.Zero)
                {
                    throw new ApplicationException("Could not open service.");
                }
                try
                {
                    StopService(intPtr2);
                }
                finally
                {
                    CloseServiceHandle(intPtr2);
                }
            }
            finally
            {
                CloseServiceHandle(intPtr);
            }
        }

        private static void StartService(System.IntPtr service)
        {
            new SERVICE_STATUS();
            StartService(service, 0, 0);
            if (!ServiceInstaller.WaitForServiceStatus(service, ServiceState.StartPending, ServiceState.Running))
            {
                throw new ApplicationException("Unable to start service");
            }
        }

        private static void StopService(System.IntPtr service)
        {
            SERVICE_STATUS lpServiceStatus = new SERVICE_STATUS();
            ServiceInstaller.ControlService(service, ServiceControl.Stop, lpServiceStatus);
            if (!ServiceInstaller.WaitForServiceStatus(service, ServiceState.StopPending, ServiceState.Stopped))
            {
                throw new ApplicationException("Unable to stop service");
            }
        }

        public static ServiceState GetServiceStatus(string serviceName)
        {
            System.IntPtr intPtr = ServiceInstaller.OpenSCManager(ScmAccessRights.Connect);
            ServiceState result;
            try
            {
                System.IntPtr intPtr2 = ServiceInstaller.OpenService(intPtr, serviceName, ServiceAccessRights.QueryStatus);
                if (intPtr2 == System.IntPtr.Zero)
                {
                    result = ServiceState.NotFound;
                }
                else
                {
                    try
                    {
                        result = GetServiceStatus(intPtr2);
                    }
                    finally
                    {
                        CloseServiceHandle(intPtr2);
                    }
                }
            }
            finally
            {
                CloseServiceHandle(intPtr);
            }
            return result;
        }

        private static ServiceState GetServiceStatus(System.IntPtr service)
        {
            SERVICE_STATUS service_STATUS = new SERVICE_STATUS();
            if (QueryServiceStatus(service, service_STATUS) == 0)
            {
                throw new ApplicationException("Failed to query service status.");
            }
            return service_STATUS.dwCurrentState;
        }

        private static bool WaitForServiceStatus(System.IntPtr service, ServiceState waitStatus, ServiceState desiredStatus)
        {
            SERVICE_STATUS service_STATUS = new SERVICE_STATUS();
            QueryServiceStatus(service, service_STATUS);
            if (service_STATUS.dwCurrentState == desiredStatus)
            {
                return true;
            }
            int tickCount = Environment.TickCount;
            int dwCheckPoint = service_STATUS.dwCheckPoint;
            for (int i = 0; i < 0xF; i++)
            {
                Thread.Sleep(0x3E8);
                QueryServiceStatus(service, service_STATUS);
                if (service_STATUS.dwCurrentState == desiredStatus)
                {
                    break;
                }
            }
            return service_STATUS.dwCurrentState == desiredStatus;
        }

        private static System.IntPtr OpenSCManager(ScmAccessRights rights)
        {
            System.IntPtr intPtr = OpenSCManager(null, null, rights);
            if (intPtr == System.IntPtr.Zero)
            {
                throw new ApplicationException("Could not connect to service control manager.");
            }
            return intPtr;
        }

        public static void SetServiceRecoveryOptions(string serviceName)
        {
            ServiceRecoveryOptions serviceRecoveryOptions = GetServiceRecoveryOptions(serviceName);
            if (serviceRecoveryOptions.FirstFailureAction != ServiceRecoveryAction.RestartTheComputer && serviceRecoveryOptions.SecondFailureAction != ServiceRecoveryAction.RestartTheComputer)
            {
                bool flag = serviceRecoveryOptions.SubsequentFailureActions == ServiceRecoveryAction.RestartTheComputer;
            }
            uint restartServiceAfter = (uint)TimeSpan.FromMinutes((double)serviceRecoveryOptions.MinutesToRestartService).TotalMilliseconds;
            System.IntPtr intPtr = System.IntPtr.Zero;
            System.IntPtr intPtr2 = System.IntPtr.Zero;
            System.ServiceProcess.ServiceController serviceController = null;
            try
            {
                serviceController = new System.ServiceProcess.ServiceController(serviceName);
                SERVICE_FAILURE_ACTIONS service_FAILURE_ACTIONS = new SERVICE_FAILURE_ACTIONS
                {
                    dwResetPeriod = (int)TimeSpan.FromDays((double)serviceRecoveryOptions.DaysToResetFailAcount).TotalSeconds,
                    cActions = 3,
                    lpRebootMsg = serviceRecoveryOptions.RebootMessage
                };
                intPtr2 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SC_ACTION)) * 3);
                ServiceRecoveryAction[] array = new ServiceRecoveryAction[]
               {
                    serviceRecoveryOptions.FirstFailureAction,
                    serviceRecoveryOptions.SecondFailureAction,
                    serviceRecoveryOptions.SubsequentFailureActions
               };
                for (int i = 0; i < array.Length; i++)
                {
                    Marshal.StructureToPtr(GetScAction(array[i], restartServiceAfter), (System.IntPtr)((long)intPtr2 + (long)(Marshal.SizeOf(typeof(SC_ACTION)) * i)), false);
                }
                service_FAILURE_ACTIONS.lpsaActions = intPtr2;
                string commandToLaunchOnFailure = serviceRecoveryOptions.CommandToLaunchOnFailure;
                if (commandToLaunchOnFailure != null)
                {
                    service_FAILURE_ACTIONS.lpCommand = commandToLaunchOnFailure;
                }
                intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SERVICE_FAILURE_ACTIONS)));
                Marshal.StructureToPtr(service_FAILURE_ACTIONS, intPtr, false);
                if (!ServiceInstaller.ChangeServiceConfig2(serviceController.ServiceHandle.DangerousGetHandle(), 2, intPtr))
                {
                    throw new Exception(Marshal.GetLastWin32Error().ToString() + "Unable to change the Service configuration.");
                }
            }
            finally
            {
                if (intPtr != System.IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(intPtr);
                }
                if (intPtr2 != System.IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(intPtr2);
                }
                if (serviceController != null)
                {
                    serviceController.Close();
                }
            }
        }

        public static ServiceRecoveryOptions GetServiceRecoveryOptions(string serviceName)
        {
            System.IntPtr intPtr = System.IntPtr.Zero;
            System.ServiceProcess.ServiceController serviceController = null;
            ServiceRecoveryOptions serviceRecoveryOptions;
            try
            {
                serviceController = new System.ServiceProcess.ServiceController(serviceName);
                intPtr = Marshal.AllocHGlobal(0x2000);
                uint num;
                if (ServiceInstaller.QueryServiceConfig2(serviceController.ServiceHandle.DangerousGetHandle(), 2, intPtr, 0x2000U, out num) == 0)
                {
                    throw new Exception(Marshal.GetLastWin32Error().ToString() + "Unable to query the Service configuration.");
                }
                SERVICE_FAILURE_ACTIONS service_FAILURE_ACTIONS = (SERVICE_FAILURE_ACTIONS)Marshal.PtrToStructure(intPtr, typeof(SERVICE_FAILURE_ACTIONS));
                serviceRecoveryOptions = new ServiceRecoveryOptions
                {
                    DaysToResetFailAcount = (int)TimeSpan.FromSeconds((double)service_FAILURE_ACTIONS.dwResetPeriod).TotalDays,
                    RebootMessage = service_FAILURE_ACTIONS.lpRebootMsg,
                    CommandToLaunchOnFailure = service_FAILURE_ACTIONS.lpCommand
                };
                int num2 = 3;
                if (num2 != 0)
                {
                    uint num3 = 0xEA60U;
                    SC_ACTION[] array = new SC_ACTION[num2];
                    for (int i = 0; i < service_FAILURE_ACTIONS.cActions; i++)
                    {
                        SC_ACTION sc_ACTION = (SC_ACTION)Marshal.PtrToStructure((System.IntPtr)(service_FAILURE_ACTIONS.lpsaActions.ToInt32() + Marshal.SizeOf(typeof(SC_ACTION)) * i), typeof(SC_ACTION));
                        array[i] = sc_ACTION;
                    }
                    serviceRecoveryOptions.FirstFailureAction = GetServiceRecoveryAction(default(SC_ACTION?));
                    serviceRecoveryOptions.SecondFailureAction = GetServiceRecoveryAction(default(SC_ACTION?));
                    serviceRecoveryOptions.SubsequentFailureActions = GetServiceRecoveryAction(default(SC_ACTION?));
                    serviceRecoveryOptions.MinutesToRestartService = (int)TimeSpan.FromMilliseconds(num3).TotalMinutes;
                }
            }
            finally
            {
                if (intPtr != System.IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(intPtr);
                }
                if (serviceController != null)
                {
                    serviceController.Close();
                }
            }
            return serviceRecoveryOptions;
        }

        private static SC_ACTION GetScAction(ServiceRecoveryAction action, uint restartServiceAfter)
        {
            SC_ACTION result = default(SC_ACTION);
            SC_ACTION_TYPE type = SC_ACTION_TYPE.None;
            switch (action)
            {
                case ServiceRecoveryAction.TakeNoAction:
                    type = SC_ACTION_TYPE.None;
                    break;
                case ServiceRecoveryAction.RestartTheService:
                    type = SC_ACTION_TYPE.RestartService;
                    break;
                case ServiceRecoveryAction.RunAProgram:
                    type = SC_ACTION_TYPE.RunCommand;
                    break;
                case ServiceRecoveryAction.RestartTheComputer:
                    type = SC_ACTION_TYPE.RebootComputer;
                    break;
            }
            result.Type = type;
            result.Delay = restartServiceAfter;
            return result;
        }

        private static ServiceRecoveryAction GetServiceRecoveryAction(SC_ACTION? action)
        {
            if (action == null)
            {
                action = new ServiceInstaller.SC_ACTION?(new SC_ACTION
                {
                    Type = SC_ACTION_TYPE.RestartService,
                    Delay = 1U
                });
            }
            ServiceRecoveryAction result = ServiceRecoveryAction.TakeNoAction;
            SC_ACTION_TYPE? sc_ACTION_TYPE = (action != null) ? new ServiceInstaller.SC_ACTION_TYPE?(action.GetValueOrDefault().Type) : default(SC_ACTION_TYPE?);
            if (sc_ACTION_TYPE != null)
            {
                switch (sc_ACTION_TYPE.GetValueOrDefault())
                {
                    case SC_ACTION_TYPE.None:
                        result = ServiceRecoveryAction.TakeNoAction;
                        break;
                    case SC_ACTION_TYPE.RestartService:
                        result = ServiceRecoveryAction.RestartTheService;
                        break;
                    case SC_ACTION_TYPE.RebootComputer:
                        result = ServiceRecoveryAction.RestartTheComputer;
                        break;
                    case SC_ACTION_TYPE.RunCommand:
                        result = ServiceRecoveryAction.RunAProgram;
                        break;
                }
            }
            return result;
        }

        private const int STANDARD_RIGHTS_REQUIRED = 0xF0000;

        private const int SERVICE_WIN32_OWN_PROCESS = 0x10;

        private const int SERVICE_CONFIG_FAILURE_ACTIONS = 2;

        private const uint SERVICE_RESTART_RECOVERY_DURATION = 0xEA60U;

        [CompilerGenerated]
        private static string <GetAllowedUserAccountUserName>k__BackingField;

		[CompilerGenerated]
        private static string <GetAllowedUserAccountPassword>k__BackingField;

		[CompilerGenerated]
        private static string <GetAllowedDomainUserAccount>k__BackingField;

		private enum SC_ACTION_TYPE
        {
            None,
            RestartService,
            RebootComputer,
            RunCommand
        }

        private struct SC_ACTION
        {
            public SC_ACTION_TYPE Type;

            public uint Delay;
        }

        [StructLayout(0, CharSet = 4)]
        private struct SERVICE_FAILURE_ACTIONS
        {
            public int dwResetPeriod;

            [MarshalAs(0x15)]
            public string lpRebootMsg;

            [MarshalAs(0x15)]
            public string lpCommand;

            public int cActions;

            public System.IntPtr lpsaActions;
        }

        [StructLayout(0)]
        private class SERVICE_STATUS
        {
            public SERVICE_STATUS()
            {
            }

            public int dwServiceType;

            public ServiceState dwCurrentState;

            public int dwControlsAccepted;

            public int dwWin32ExitCode;

            public int dwServiceSpecificExitCode;

            public int dwCheckPoint;

            public int dwWaitHint;
        }
    }
}
