using System.Web.Hosting;
using log4net;
using log4net.Config;
using Microsoft.Practices.Unity;
using Bootstrap;

namespace WebService
{
    public class Initializer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Initializer));
        private static bool _isStarting;

        public static void AppInitialize()
        {
            if (_isStarting || HostingEnvironment.InClientBuildManager)
                return;

            _isStarting = true;
            XmlConfigurator.Configure();

            UnityConfig.RegisterComponents(Configure);

            Log.Debug("Queue service was configured");
        }

        private static void Configure(IUnityContainer container)
        {
            // регистрируем сервисы
            container.RegisterType<IQueueService, v1.QueueService>();
            //container.RegisterType<IQueueService, v1.QueueService>(NamespaceHelper.V1Namespace);
            //container.RegisterType<IQueueService, v2.QueueService>(NamespaceHelper.V2Namespace);

            // выставляем контейнер для фабрики сервисов
            WcfServiceFactory.SetRootContainer(container);

            // инициализируем Environment
            //container.RegisterType<IOmsEnvironmentInfoProvider, SvcOmsEnvironmentInfoProvider>(new ContainerControlledLifetimeManager());
            //OmsEnvironment.Init(container.Resolve<IOmsEnvironmentInfoProvider>());
        }
    }
}