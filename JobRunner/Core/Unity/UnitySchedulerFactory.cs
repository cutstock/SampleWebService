using Quartz;
using Quartz.Core;
using Quartz.Impl;

namespace JobRunner.Core.Unity
{
    public class UnitySchedulerFactory : StdSchedulerFactory
    {
        private readonly UnityJobFactory _unityJobFactory;

        public UnitySchedulerFactory(UnityJobFactory unityJobFactory)
        {
            _unityJobFactory = unityJobFactory;
        }

        protected override IScheduler Instantiate(QuartzSchedulerResources rsrcs, QuartzScheduler qs)
        {
            SetFactory(qs);
            return base.Instantiate(rsrcs, qs);
        }

        protected void SetFactory(QuartzScheduler qs)
        {
            qs.JobFactory = _unityJobFactory;
        }
    }
}
