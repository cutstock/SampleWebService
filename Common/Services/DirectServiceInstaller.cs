using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Common.Services
{
    /// <summary>
    /// Summary description for ServiceInstaller.
    /// </summary>
    public class DirectServiceInstaller
    {
        #region DLLImport

        [DllImport("advapi32.dll")]
        public static extern IntPtr OpenSCManager(string lpMachineName, string lpScdb, int scParameter);
        [DllImport("Advapi32.dll")]
        public static extern IntPtr CreateService(IntPtr scHandle, string lpSvcName, string lpDisplayName,
            int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl, string lpPathName,
            string lpLoadOrderGroup, int lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);
        [DllImport("advapi32.dll")]
        public static extern void CloseServiceHandle(IntPtr schandle);
        [DllImport("advapi32.dll")]
        public static extern int StartService(IntPtr svhandle, int dwNumServiceArgs, string lpServiceArgVectors);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr schandle, string lpSvcName, int dwNumServiceArgs);
        [DllImport("advapi32.dll")]
        public static extern int DeleteService(IntPtr svhandle);
        [DllImport("Advapi32.dll", SetLastError = true, EntryPoint = "ChangeServiceConfig2")]
        public static extern int ChangeServiceConfig2(
            IntPtr hService,
            int dwInfoLevel,
            [MarshalAs(UnmanagedType.Struct)] ref ServiceDescription lpInfo);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceDescription
        {
            public string lpDescription;
        }

        #endregion DLLImport

        /// <summary>
        /// This method installs and runs the service in the service conrol manager.
        /// </summary>
        /// <param name="svcPath">The complete path of the service.</param>
        /// <param name="svcName">Name of the service.</param>
        /// <param name="svcDispName">Display name of the service.</param>
        /// <param name="svcDescription">Description of the service.</param>
        /// <param name="autoStart"></param>
        /// <param name="startNow"></param>
        /// <returns>True if the process went thro successfully. False if there was any error.</returns>
        public static bool InstallService(string svcPath, string svcName, string svcDispName, string svcDescription,
                                          bool autoStart, bool startNow)
        {
            #region Constants declaration.

            const int scManagerCreateService = 0x0002;
            const int serviceWin32OwnProcess = 0x00000010;
            //int SERVICE_DEMAND_START = 0x00000003;
            const int serviceErrorNormal = 0x00000001;

            const int standardRightsRequired = 0xF0000;
            const int serviceQueryConfig = 0x0001;
            const int serviceChangeConfig = 0x0002;
            const int serviceDemandStart = 0x00000003;
            const int serviceQueryStatus = 0x0004;
            const int serviceEnumerateDependents = 0x0008;
            const int serviceStart = 0x0010;
            const int serviceStop = 0x0020;
            const int servicePauseContinue = 0x0040;
            const int serviceInterrogate = 0x0080;
            const int serviceUserDefinedControl = 0x0100;

            const int serviceAllAccess = (standardRightsRequired |
                                          serviceQueryConfig |
                                          serviceChangeConfig |
                                          serviceQueryStatus |
                                          serviceEnumerateDependents |
                                          serviceStart |
                                          serviceStop |
                                          servicePauseContinue |
                                          serviceInterrogate |
                                          serviceUserDefinedControl);
            const int serviceAutoStart = 0x00000002;

            const int serviceConfigDescription = 1;
            //const int serviceConfigFailureActions = 2;
            //const int serviceConfigDelayedAutoStartInfo = 3;
            //const int serviceConfigFailureActionsFlag = 4;
            //const int serviceConfigServiceSidInfo = 5;
            //const int serviceConfigRequiredPrivilegesInfo = 6;
            //const int serviceConfigPreshutdownInfo = 7;

            #endregion Constants declaration.


            var dwStartType = serviceAutoStart;
            if (autoStart == false) dwStartType = serviceDemandStart;

            var scHandle = OpenSCManager(null, null, scManagerCreateService);
            if (scHandle == IntPtr.Zero)
                return false;

            var svHandle = CreateService(scHandle, svcName, svcDispName, serviceAllAccess,
                                         serviceWin32OwnProcess, dwStartType, serviceErrorNormal, svcPath, null,
                                         0, null, null, null);
            if (svHandle == IntPtr.Zero)
            {
                CloseServiceHandle(scHandle);
                return false;
            }

            var desc = new ServiceDescription { lpDescription = svcDescription };
            ChangeServiceConfig2(svHandle, serviceConfigDescription, ref desc);

            if (startNow)
            {
                //now trying to start the service
                var i = StartService(svHandle, 0, null);
                // If the value i is zero, then there was an error starting the service.
                // note: error may arise if the service is already running or some other problem.
                if (i == 0)
                {
                    //Console.WriteLine("Couldnt start service");
                    return false;
                }
                CloseServiceHandle(scHandle);
                CloseServiceHandle(svHandle);
                return true;
            }
            CloseServiceHandle(scHandle);
            CloseServiceHandle(svHandle);
            return true;
        }


        /// <summary>
        /// This method uninstalls the service from the service conrol manager.
        /// </summary>
        /// <param name="svcName">Name of the service to uninstall.</param>
        public static bool UnInstallService(string svcName)
        {
            const int genericWrite = 0x40000000;
            var scHndl = OpenSCManager(null, null, genericWrite);
            if (scHndl == IntPtr.Zero)
                return false;

            const int delete = 0x10000;
            var svcHndl = OpenService(scHndl, svcName, delete);
            if (svcHndl == IntPtr.Zero)
                return false;

            var i = DeleteService(svcHndl);
            if (i != 0)
            {
                CloseServiceHandle(scHndl);
                CloseServiceHandle(svcHndl);
                return true;
            }
            CloseServiceHandle(scHndl);
            CloseServiceHandle(svcHndl);
            return false;
        }
    }
}
