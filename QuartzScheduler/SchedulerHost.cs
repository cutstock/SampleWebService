using Common.Logging;
using Quartz;
using System.ServiceProcess;
using Common.Helpers;
using Common.Services;
using Bootstrap;
using Microsoft.Practices.Unity;
using System.Security.Authentication;
using Common;
using Common.Environment;
using Common.Environment.Impl;
using JobRunner.Core;
using log4net.Config;

namespace QuartzScheduler
{
    public partial class SchedulerHost : ServiceBase
    {
        #region .  Fields  .

        private readonly ILog _log = LogManager.GetLogger(typeof(SchedulerHost));
        private IScheduler _scheduler;

        #endregion

        #region .  ctors  .

        public SchedulerHost(ServiceContext context)
        {
            XmlConfigurator.Configure();
            // конфигурируем приложение
            UnityConfig.RegisterComponents(Configure);
        }

        #endregion

        #region .  ServiceBase  .

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            // запускаем scheduler-а
            _scheduler.Start();
        }

        protected override void OnPause()
        {
            _scheduler.PauseAll();
            base.OnPause();
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            _scheduler.ResumeAll();
        }

        protected override void OnShutdown()
        {
            if (_scheduler.IsStarted)
                _scheduler.Shutdown(true);
            base.OnShutdown();
        }

        protected override void OnStop()
        {
            if (_scheduler != null && _scheduler.IsStarted)
                _scheduler.Shutdown(true);
            base.OnStop();
        }

        #endregion

        #region .  Methods  .

        public void Start(string[] args)
        {
            OnStart(args);
        }

        private void Configure(IUnityContainer container)
        {
            // инициализируем Environment
            container.RegisterType<IEnvironmentInfoProvider, SvcEnvironmentInfoProvider>(new ContainerControlledLifetimeManager());
            SvcEnvironment.Init(container.Resolve<IEnvironmentInfoProvider>());

            AuthenticateService(container);

            // инициализируем job-ы
            container.RegisterType<ISchedulerFactory, CustomSchedulerFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<IScheduler>(new InjectionFactory(c => c.Resolve<ISchedulerFactory>().GetScheduler()));
            container.RegisterType<IConfigurationProvider, DbConfigurationProvider>();

            _scheduler = container.Resolve<IScheduler>();
        }

        /// <summary>
        /// Аутентификация себя (для подписки сессий от имени сервиса)
        /// </summary>
        /// <param name="container"></param>
        private void AuthenticateService(IUnityContainer container)
        {
            var login = AppConfigHelper.GetSettings<string>("SvcLogin", true);
            if (string.IsNullOrEmpty(login))
                throw new ConfigurationException(string.Format("Can't find auth settings '{0}'.", "SvcLogin"));

            var pass = AppConfigHelper.GetSettings<string>("SvcPassword");
            var authService = container.Resolve<IAuthService>();

            if (!authService.Authenticate(login, pass))
                throw new AuthenticationException(string.Format("Service is not authenticated with login '{0}'.", login));
        }

        #endregion

        private void InitializeComponent()
        {
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
        }
    }
}
