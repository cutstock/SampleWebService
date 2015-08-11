using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using log4net;
using log4net.Config;
using Common.Services;

namespace QuartzScheduler
{
    public class Program
    {
        #region .  Fields  .

        private const string ServiceName = "QuartzScheduler";
        private const string ParamService = "service";
        private const string ParamServiceNo = "noservice";
        private const string Re = @"(?:-{1,2}|/)(?<name>\w+)(?:[=:]?|\s+)(?<value>[^-\s""][^""]*?|""[^""]*"")?(?=\s+[-/]|$)";
        private static Dictionary<string, string> _parameters = new Dictionary<string, string>();
        private static ILog _log;
        private static string _appName;
        private static string _displayName;
        private const string Description = "QuartzScheduler";

        #endregion

        private static void Main()
        {
            XmlConfigurator.Configure();
            _log = LogManager.GetLogger(ServiceName);
            _log.InfoFormat("Starting service ...");

            _appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.Replace(".vshost", string.Empty);
            _displayName = _appName;
            _parameters = ParseCommandLine(Environment.CommandLine);

            //определяем комманду на удаление сервиса
            var isNeedRemoveService = _parameters.ContainsKey(ParamServiceNo);
            if (isNeedRemoveService)
            {
                RemoveService();
                return;
            }

            //определяем какой вид запуска (сервис или консоль)
            var runAsService = _parameters.ContainsKey(ParamService);

            //запуск приложения как сервиса
            if (runAsService)
            {
                // если сервиса нет - устанавливаем его и запускаем
                var sysService = GetInstalledService();
                if (sysService == null)
                {
                    sysService = CreateService();
                    sysService.Start();
                    return;
                }

                // если сервис есть - запускаем логику
                var svc = new SchedulerHost(new ServiceContext(_parameters));
                ServiceBase.Run(svc);
            }
            //запуск в режиме консольного приложения
            else
            {
                var app = new SchedulerHost(new ServiceContext(_parameters));
                try
                {
                    app.Start(null);

                    _log.Info("Press escape to exit");
                    ConsoleKeyInfo keyInfo;
                    do keyInfo = Console.ReadKey(); while (keyInfo.Key != ConsoleKey.Escape);
                }
                catch (Exception ex)
                {
                    _log.ErrorFormat("Fatal error: {0}", ex);
                    Console.WriteLine("Press a key to exit");
                    Console.ReadKey();
                }
                finally
                {
                    app.Stop();
                }
            }
        }

        private static ServiceController GetInstalledService()
        {
            return ServiceController.GetServices().FirstOrDefault(con => con.ServiceName == ServiceName);
        }

        private static ServiceController CreateService()
        {
            var service = ServiceController.GetServices().FirstOrDefault(con => con.ServiceName == ServiceName);
            if (service != null)
                throw new Exception(string.Format("Service '{0}' already installed", ServiceName));

            _log.DebugFormat("Try to install service '{0}'", ServiceName);

            var parameters = string.Join(" ", _parameters.Select(i => "-" + i.Key + (i.Value != null ? "=" + i.Value : string.Empty)));
            var appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var svcPath = string.Format("\"{0}\\{1}.exe\" {2}", appPath, _appName, parameters);
            if (!DirectServiceInstaller.InstallService(svcPath, ServiceName, _displayName, Description, true, false))
                throw new Exception("Can't install service. Check user rights for this operation.");

            // проверяем, что сервис установился
            service = ServiceController.GetServices().FirstOrDefault(con => con.ServiceName == ServiceName);
            if (service == null)
                throw new Exception(string.Format("Can't find just installed service '{0}'", ServiceName));

            _log.DebugFormat("Service '{0}' successfully installed", ServiceName);
            return service;
        }

        private static void RemoveService()
        {
            var service = ServiceController.GetServices().FirstOrDefault(con => con.ServiceName == ServiceName);
            if (service == null)
            {
                _log.WarnFormat("Can't stop service '{0}'. Service not found", ServiceName);
                return;
            }

            // если работает - останавливаем
            if (service.Status == ServiceControllerStatus.Running)
                service.Stop();

            // удаляем
            _log.InfoFormat(
                DirectServiceInstaller.UnInstallService(ServiceName)
                    ? "Service '{0}' successfully removed"
                    : "Can't remove '{0}' service. Unknown error in ServiceInstaller", ServiceName);
        }

        private static Dictionary<string, string> ParseCommandLine(string commandLine)
        {
            var ms = Regex.Matches(commandLine, Re);
            var matches = ms.Cast<Match>()
                .Select(m => m.Groups[1].Value + (m.Groups[2].Success ? "=" + Regex.Replace(
                    m.Groups[2].Value, @"""", string.Empty) : string.Empty)).ToArray();

            return matches.ToDictionary(i => i.Split('=')[0], i => i.Split('=').Length > 1 ? i.Split('=')[1] : null);
        }
    }
}
