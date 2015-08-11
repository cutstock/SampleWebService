using Microsoft.Practices.Unity;
using JobRunner.Core.Listeners;
using JobRunner.Core.Unity;
using Quartz;
using Quartz.Core;

namespace JobRunner.Core
{
    public class CustomSchedulerFactory : UnitySchedulerFactory
    {
        private readonly IUnityContainer _container;

        public CustomSchedulerFactory(UnityJobFactory unityJobFactory,
            IUnityContainer container)
            : base(unityJobFactory)
        {
            _container = container;
        }

        protected override IScheduler Instantiate(QuartzSchedulerResources rsrcs, QuartzScheduler qs)
        {
            SetFactory(qs);
            var res = new CustomScheduler(qs, _container);
            AddListeners(res);
            return res;
        }

        private static void AddListeners(IScheduler res)
        {
            res.ListenerManager.AddJobListener(new JobListener(res.SchedulerName));
            res.ListenerManager.AddTriggerListener(new TriggerListener(res.SchedulerName));
            res.ListenerManager.AddSchedulerListener(new SchedulerListener());
        }
    }
}
