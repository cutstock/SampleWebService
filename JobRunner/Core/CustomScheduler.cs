using Microsoft.Practices.Unity;
using Quartz.Core;
using Quartz.Impl;

namespace JobRunner.Core
{
    public class CustomScheduler : StdScheduler
    {
        private readonly IUnityContainer _container;

        public CustomScheduler(QuartzScheduler sched, IUnityContainer container)
            : base(sched)
        {
            _container = container;
        }

        internal T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }

}
