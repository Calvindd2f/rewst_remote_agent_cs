using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

namespace Rewst.RemoteAgent.Calvindd2f
{
	public static class ServiceInstaller
	{
		public static string GetAllowedUserAccountUserName
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.<GetAllowedUserAccountUserName>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.<GetAllowedUserAccountUserName>k__BackingField = value;
			}
		}

		public static string GetAllowedUserAccountPassword
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.<GetAllowedUserAccountPassword>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.<GetAllowedUserAccountPassword>k__BackingField = value;
			}
		}

		public static string GetAllowedDomainUserAccount
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.<GetAllowedDomainUserAccount>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.<GetAllowedDomainUserAccount>k__BackingField = value;
			}
		}

		[ System.Runtime.InteropServices.DllImport("advapi32.dll", CharSet = 3, EntryPoint = "OpenSCManagerW", ExactSpelling = true, SetLastError = true)]
		private static extern  System.IntPtr OpenSCManager(string machineName, string databaseName,  Rewst.RemoteAgent.Calvindd2f.ScmAccessRights dwDesiredAccess);

		[ System.Runtime.InteropServices.DllImport("advapi32.dll", CharSet = 4, SetLastError = true)]
		private static extern  System.IntPtr OpenService( System.IntPtr hSCManager, string lpServiceName,  Rewst.RemoteAgent.Calvindd2f.ServiceAccessRights dwDesiredAccess);

		[ System.Runtime.InteropServices.DllImport("advapi32.dll", CharSet = 4, SetLastError = true)]
		private static extern  System.IntPtr CreateService( System.IntPtr hSCManager, string lpServiceName, string lpDisplayName,  Rewst.RemoteAgent.Calvindd2f.ServiceAccessRights dwDesiredAccess, int dwServiceType,  Rewst.RemoteAgent.Calvindd2f.ServiceBootFlag dwStartType,  Rewst.RemoteAgent.Calvindd2f.ServiceError dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup,  System.IntPtr lpdwTagId, string lpDependencies, string lp, string lpPassword);

		[ System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
		[return:  System.Runtime.InteropServices.MarshalAs(2)]
		private static extern bool CloseServiceHandle( System.IntPtr hSCObject);

		[ System.Runtime.InteropServices.DllImport("advapi32.dll")]
		private static extern int QueryServiceStatus( System.IntPtr hService,  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_STATUS lpServiceStatus);

		[ System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
		[return:  System.Runtime.InteropServices.MarshalAs(2)]
		private static extern bool DeleteService( System.IntPtr hService);

		[ System.Runtime.InteropServices.DllImport("advapi32.dll")]
		private static extern int ControlService( System.IntPtr hService,  Rewst.RemoteAgent.Calvindd2f.ServiceControl dwControl,  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_STATUS lpServiceStatus);

		[ System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
		private static extern int StartService( System.IntPtr hService, int dwNumServiceArgs, int lpServiceArgVectors);

		[ System.Runtime.InteropServices.DllImport("advapi32.dll", CharSet = 3, SetLastError = true)]
		[return:  System.Runtime.InteropServices.MarshalAs(2)]
		private static extern bool ChangeServiceConfig2( System.IntPtr hService, int dwInfoLevel,  System.IntPtr lpInfo);

		[ System.Runtime.InteropServices.DllImport("advapi32.dll", CharSet = 3, SetLastError = true)]
		private static extern int QueryServiceConfig2( System.IntPtr hService, int dwInfoLevel,  System.IntPtr lpBuffer, uint cbBufSize, out uint pcbBytesNeeded);

		public static void Uninstall(string serviceName)
		{
			 System.IntPtr intPtr =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.OpenSCManager( Rewst.RemoteAgent.Calvindd2f.ScmAccessRights.AllAccess);
			try
			{
				 System.IntPtr intPtr2 =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.OpenService(intPtr, serviceName,  Rewst.RemoteAgent.Calvindd2f.ServiceAccessRights.AllAccess);
				if (intPtr2 ==  System.IntPtr.Zero)
				{
					throw new  System.ApplicationException("Service not installed.");
				}
				try
				{
					 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.StopService(intPtr2);
					if (! Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.DeleteService(intPtr2))
					{
						int lastWin32Error =  System.Runtime.InteropServices.Marshal.GetLastWin32Error();
						if (lastWin32Error != 0x430)
						{
							throw new  System.ApplicationException("Could not delete service " + lastWin32Error.ToString());
						}
					}
				}
				finally
				{
					 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.CloseServiceHandle(intPtr2);
				}
			}
			finally
			{
				 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.CloseServiceHandle(intPtr);
			}
		}

		public static bool ServiceIsInstalled(string serviceName)
		{
			 System.IntPtr intPtr =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.OpenSCManager( Rewst.RemoteAgent.Calvindd2f.ScmAccessRights.Connect);
			bool result;
			try
			{
				 System.IntPtr intPtr2 =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.OpenService(intPtr, serviceName,  Rewst.RemoteAgent.Calvindd2f.ServiceAccessRights.QueryStatus);
				if (intPtr2 ==  System.IntPtr.Zero)
				{
					result = false;
				}
				else
				{
					 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.CloseServiceHandle(intPtr2);
					result = true;
				}
			}
			finally
			{
				 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.CloseServiceHandle(intPtr);
			}
			return result;
		}

		public static void Install(string serviceName, string displayName, string fileName)
		{
			 System.IntPtr intPtr =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.OpenSCManager( Rewst.RemoteAgent.Calvindd2f.ScmAccessRights.AllAccess);
			try
			{
				 System.IntPtr intPtr2 =  System.IntPtr.Zero;
				try
				{
					intPtr2 =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.CreateService(intPtr, serviceName, displayName,  Rewst.RemoteAgent.Calvindd2f.ServiceAccessRights.AllAccess, 0x10,  Rewst.RemoteAgent.Calvindd2f.ServiceBootFlag.AutoStart,  Rewst.RemoteAgent.Calvindd2f.ServiceError.Normal, fileName, null,  System.IntPtr.Zero, null, null, null);
					if (intPtr2 ==  System.IntPtr.Zero)
					{
						int lastWin32Error =  System.Runtime.InteropServices.Marshal.GetLastWin32Error();
						if (lastWin32Error == 0x430)
						{
							throw new  System.InvalidOperationException("The service cannot be installed as it is in an uninstalling state after being recently uninstalled. Please wait and retry again in a few minutes. If the issue persists, restart the machine before attempting again.");
						}
						throw new  System.ApplicationException("Failed to install service with error: " + lastWin32Error.ToString());
					}
				}
				finally
				{
					if (intPtr2 !=  System.IntPtr.Zero)
					{
						 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.CloseServiceHandle(intPtr2);
					}
				}
			}
			finally
			{
				 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.CloseServiceHandle(intPtr);
			}
		}

		public static void StartService(string serviceName)
		{
			 System.IntPtr intPtr =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.OpenSCManager( Rewst.RemoteAgent.Calvindd2f.ScmAccessRights.Connect);
			try
			{
				 System.IntPtr intPtr2 =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.OpenService(intPtr, serviceName,  Rewst.RemoteAgent.Calvindd2f.ServiceAccessRights.QueryStatus |  Rewst.RemoteAgent.Calvindd2f.ServiceAccessRights.Start);
				if (intPtr2 ==  System.IntPtr.Zero)
				{
					throw new  System.ApplicationException("Could not open service.");
				}
				try
				{
					 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SetServiceRecoveryOptions(serviceName);
				}
				catch ( System.Exception)
				{
				}
				try
				{
					 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.StartService(intPtr2);
				}
				finally
				{
					 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.CloseServiceHandle(intPtr2);
				}
			}
			finally
			{
				 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.CloseServiceHandle(intPtr);
			}
		}

		public static void StopService(string serviceName)
		{
			 System.IntPtr intPtr =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.OpenSCManager( Rewst.RemoteAgent.Calvindd2f.ScmAccessRights.Connect);
			try
			{
				 System.IntPtr intPtr2 =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.OpenService(intPtr, serviceName,  Rewst.RemoteAgent.Calvindd2f.ServiceAccessRights.QueryStatus |  Rewst.RemoteAgent.Calvindd2f.ServiceAccessRights.Stop);
				if (intPtr2 ==  System.IntPtr.Zero)
				{
					throw new  System.ApplicationException("Could not open service.");
				}
				try
				{
					 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.StopService(intPtr2);
				}
				finally
				{
					 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.CloseServiceHandle(intPtr2);
				}
			}
			finally
			{
				 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.CloseServiceHandle(intPtr);
			}
		}

		private static void StartService( System.IntPtr service)
		{
			new  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_STATUS();
			 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.StartService(service, 0, 0);
			if (! Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.WaitForServiceStatus(service,  Rewst.RemoteAgent.Calvindd2f.ServiceState.StartPending,  Rewst.RemoteAgent.Calvindd2f.ServiceState.Running))
			{
				throw new  System.ApplicationException("Unable to start service");
			}
		}

		private static void StopService( System.IntPtr service)
		{
			 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_STATUS lpServiceStatus = new  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_STATUS();
			 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.ControlService(service,  Rewst.RemoteAgent.Calvindd2f.ServiceControl.Stop, lpServiceStatus);
			if (! Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.WaitForServiceStatus(service,  Rewst.RemoteAgent.Calvindd2f.ServiceState.StopPending,  Rewst.RemoteAgent.Calvindd2f.ServiceState.Stopped))
			{
				throw new  System.ApplicationException("Unable to stop service");
			}
		}

		public static  Rewst.RemoteAgent.Calvindd2f.ServiceState GetServiceStatus(string serviceName)
		{
			 System.IntPtr intPtr =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.OpenSCManager( Rewst.RemoteAgent.Calvindd2f.ScmAccessRights.Connect);
			 Rewst.RemoteAgent.Calvindd2f.ServiceState result;
			try
			{
				 System.IntPtr intPtr2 =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.OpenService(intPtr, serviceName,  Rewst.RemoteAgent.Calvindd2f.ServiceAccessRights.QueryStatus);
				if (intPtr2 ==  System.IntPtr.Zero)
				{
					result =  Rewst.RemoteAgent.Calvindd2f.ServiceState.NotFound;
				}
				else
				{
					try
					{
						result =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.GetServiceStatus(intPtr2);
					}
					finally
					{
						 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.CloseServiceHandle(intPtr2);
					}
				}
			}
			finally
			{
				 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.CloseServiceHandle(intPtr);
			}
			return result;
		}

		private static  Rewst.RemoteAgent.Calvindd2f.ServiceState GetServiceStatus( System.IntPtr service)
		{
			 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_STATUS service_STATUS = new  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_STATUS();
			if ( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.QueryServiceStatus(service, service_STATUS) == 0)
			{
				throw new  System.ApplicationException("Failed to query service status.");
			}
			return service_STATUS.dwCurrentState;
		}

		private static bool WaitForServiceStatus( System.IntPtr service,  Rewst.RemoteAgent.Calvindd2f.ServiceState waitStatus,  Rewst.RemoteAgent.Calvindd2f.ServiceState desiredStatus)
		{
			 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_STATUS service_STATUS = new  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_STATUS();
			 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.QueryServiceStatus(service, service_STATUS);
			if (service_STATUS.dwCurrentState == desiredStatus)
			{
				return true;
			}
			int tickCount =  System.Environment.TickCount;
			int dwCheckPoint = service_STATUS.dwCheckPoint;
			for (int i = 0; i < 0xF; i++)
			{
				 System.Threading.Thread.Sleep(0x3E8);
				 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.QueryServiceStatus(service, service_STATUS);
				if (service_STATUS.dwCurrentState == desiredStatus)
				{
					break;
				}
			}
			return service_STATUS.dwCurrentState == desiredStatus;
		}

		private static  System.IntPtr OpenSCManager( Rewst.RemoteAgent.Calvindd2f.ScmAccessRights rights)
		{
			 System.IntPtr intPtr =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.OpenSCManager(null, null, rights);
			if (intPtr ==  System.IntPtr.Zero)
			{
				throw new  System.ApplicationException("Could not connect to service control manager.");
			}
			return intPtr;
		}

		public static void SetServiceRecoveryOptions(string serviceName)
		{
			 Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryOptions serviceRecoveryOptions =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.GetServiceRecoveryOptions(serviceName);
			if (serviceRecoveryOptions.FirstFailureAction !=  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.RestartTheComputer && serviceRecoveryOptions.SecondFailureAction !=  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.RestartTheComputer)
			{
				bool flag = serviceRecoveryOptions.SubsequentFailureActions ==  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.RestartTheComputer;
			}
			uint restartServiceAfter = (uint) System.TimeSpan.FromMinutes((double)serviceRecoveryOptions.MinutesToRestartService).TotalMilliseconds;
			 System.IntPtr intPtr =  System.IntPtr.Zero;
			 System.IntPtr intPtr2 =  System.IntPtr.Zero;
			 System.ServiceProcess.ServiceController serviceController = null;
			try
			{
				serviceController = new  System.ServiceProcess.ServiceController(serviceName);
				 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_FAILURE_ACTIONS service_FAILURE_ACTIONS = new  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_FAILURE_ACTIONS
				{
					dwResetPeriod = (int) System.TimeSpan.FromDays((double)serviceRecoveryOptions.DaysToResetFailAcount).TotalSeconds,
					cActions = 3,
					lpRebootMsg = serviceRecoveryOptions.RebootMessage
				};
				intPtr2 =  System.Runtime.InteropServices.Marshal.AllocHGlobal( System.Runtime.InteropServices.Marshal.SizeOf(typeof( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION)) * 3);
				 Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction[] array = new  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction[]
				{
					serviceRecoveryOptions.FirstFailureAction,
					serviceRecoveryOptions.SecondFailureAction,
					serviceRecoveryOptions.SubsequentFailureActions
				};
				for (int i = 0; i < array.Length; i++)
				{
					 System.Runtime.InteropServices.Marshal.StructureToPtr< Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION>( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.GetScAction(array[i], restartServiceAfter), ( System.IntPtr)((long)intPtr2 + (long)( System.Runtime.InteropServices.Marshal.SizeOf(typeof( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION)) * i)), false);
				}
				service_FAILURE_ACTIONS.lpsaActions = intPtr2;
				string commandToLaunchOnFailure = serviceRecoveryOptions.CommandToLaunchOnFailure;
				if (commandToLaunchOnFailure != null)
				{
					service_FAILURE_ACTIONS.lpCommand = commandToLaunchOnFailure;
				}
				intPtr =  System.Runtime.InteropServices.Marshal.AllocHGlobal( System.Runtime.InteropServices.Marshal.SizeOf(typeof( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_FAILURE_ACTIONS)));
				 System.Runtime.InteropServices.Marshal.StructureToPtr< Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_FAILURE_ACTIONS>(service_FAILURE_ACTIONS, intPtr, false);
				if (! Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.ChangeServiceConfig2(serviceController.ServiceHandle.DangerousGetHandle(), 2, intPtr))
				{
					throw new  System.Exception( System.Runtime.InteropServices.Marshal.GetLastWin32Error().ToString() + "Unable to change the Service configuration.");
				}
			}
			finally
			{
				if (intPtr !=  System.IntPtr.Zero)
				{
					 System.Runtime.InteropServices.Marshal.FreeHGlobal(intPtr);
				}
				if (intPtr2 !=  System.IntPtr.Zero)
				{
					 System.Runtime.InteropServices.Marshal.FreeHGlobal(intPtr2);
				}
				if (serviceController != null)
				{
					serviceController.Close();
				}
			}
		}

		public static  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryOptions GetServiceRecoveryOptions(string serviceName)
		{
			 System.IntPtr intPtr =  System.IntPtr.Zero;
			 System.ServiceProcess.ServiceController serviceController = null;
			 Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryOptions serviceRecoveryOptions;
			try
			{
				serviceController = new  System.ServiceProcess.ServiceController(serviceName);
				intPtr =  System.Runtime.InteropServices.Marshal.AllocHGlobal(0x2000);
				uint num;
				if ( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.QueryServiceConfig2(serviceController.ServiceHandle.DangerousGetHandle(), 2, intPtr, 0x2000U, out num) == 0)
				{
					throw new  System.Exception( System.Runtime.InteropServices.Marshal.GetLastWin32Error().ToString() + "Unable to query the Service configuration.");
				}
				 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_FAILURE_ACTIONS service_FAILURE_ACTIONS = ( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_FAILURE_ACTIONS) System.Runtime.InteropServices.Marshal.PtrToStructure(intPtr, typeof( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SERVICE_FAILURE_ACTIONS));
				serviceRecoveryOptions = new  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryOptions
				{
					DaysToResetFailAcount = (int) System.TimeSpan.FromSeconds((double)service_FAILURE_ACTIONS.dwResetPeriod).TotalDays,
					RebootMessage = service_FAILURE_ACTIONS.lpRebootMsg,
					CommandToLaunchOnFailure = service_FAILURE_ACTIONS.lpCommand
				};
				int num2 = 3;
				if (num2 != 0)
				{
					uint num3 = 0xEA60U;
					 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION[] array = new  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION[num2];
					for (int i = 0; i < service_FAILURE_ACTIONS.cActions; i++)
					{
						 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION sc_ACTION = ( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION) System.Runtime.InteropServices.Marshal.PtrToStructure(( System.IntPtr)(service_FAILURE_ACTIONS.lpsaActions.ToInt32() +  System.Runtime.InteropServices.Marshal.SizeOf(typeof( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION)) * i), typeof( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION));
						array[i] = sc_ACTION;
					}
					serviceRecoveryOptions.FirstFailureAction =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.GetServiceRecoveryAction(default( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION?));
					serviceRecoveryOptions.SecondFailureAction =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.GetServiceRecoveryAction(default( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION?));
					serviceRecoveryOptions.SubsequentFailureActions =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.GetServiceRecoveryAction(default( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION?));
					serviceRecoveryOptions.MinutesToRestartService = (int) System.TimeSpan.FromMilliseconds(num3).TotalMinutes;
				}
			}
			finally
			{
				if (intPtr !=  System.IntPtr.Zero)
				{
					 System.Runtime.InteropServices.Marshal.FreeHGlobal(intPtr);
				}
				if (serviceController != null)
				{
					serviceController.Close();
				}
			}
			return serviceRecoveryOptions;
		}

		private static  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION GetScAction( Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction action, uint restartServiceAfter)
		{
			 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION result = default( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION);
			 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE type =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE.None;
			switch (action)
			{
			case  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.TakeNoAction:
				type =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE.None;
				break;
			case  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.RestartTheService:
				type =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE.RestartService;
				break;
			case  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.RunAProgram:
				type =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE.RunCommand;
				break;
			case  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.RestartTheComputer:
				type =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE.RebootComputer;
				break;
			}
			result.Type = type;
			result.Delay = restartServiceAfter;
			return result;
		}

		private static  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction GetServiceRecoveryAction( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION? action)
		{
			if (action == null)
			{
				action = new  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION?(new  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION
				{
					Type =  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE.RestartService,
					Delay = 1U
				});
			}
			 Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction result =  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.TakeNoAction;
			 Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE? sc_ACTION_TYPE = (action != null) ? new  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE?(action.GetValueOrDefault().Type) : default( Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE?);
			if (sc_ACTION_TYPE != null)
			{
				switch (sc_ACTION_TYPE.GetValueOrDefault())
				{
				case  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE.None:
					result =  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.TakeNoAction;
					break;
				case  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE.RestartService:
					result =  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.RestartTheService;
					break;
				case  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE.RebootComputer:
					result =  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.RestartTheComputer;
					break;
				case  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE.RunCommand:
					result =  Rewst.RemoteAgent.Calvindd2f.ServiceRecoveryAction.RunAProgram;
					break;
				}
			}
			return result;
		}

		private const int STANDARD_RIGHTS_REQUIRED = 0xF0000;

		private const int SERVICE_WIN32_OWN_PROCESS = 0x10;

		private const int SERVICE_CONFIG_FAILURE_ACTIONS = 2;

		private const uint SERVICE_RESTART_RECOVERY_DURATION = 0xEA60U;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private static string <GetAllowedUserAccountUserName>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private static string <GetAllowedUserAccountPassword>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
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
			public  Rewst.RemoteAgent.Calvindd2f.ServiceInstaller.SC_ACTION_TYPE Type;

			public uint Delay;
		}

		[ System.Runtime.InteropServices.StructLayout(0, CharSet = 4)]
		private struct SERVICE_FAILURE_ACTIONS
		{
			public int dwResetPeriod;

			[ System.Runtime.InteropServices.MarshalAs(0x15)]
			public string lpRebootMsg;

			[ System.Runtime.InteropServices.MarshalAs(0x15)]
			public string lpCommand;

			public int cActions;

			public  System.IntPtr lpsaActions;
		}

		[ System.Runtime.InteropServices.StructLayout(0)]
		private class SERVICE_STATUS
		{
			public SERVICE_STATUS()
			{
			}

			public int dwServiceType;

			public  Rewst.RemoteAgent.Calvindd2f.ServiceState dwCurrentState;

			public int dwControlsAccepted;

			public int dwWin32ExitCode;

			public int dwServiceSpecificExitCode;

			public int dwCheckPoint;

			public int dwWaitHint;
		}
	}
}
