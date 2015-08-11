using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Common;
using MsmqManager;

namespace Bootstrap
{
    public static class UnityConfig
    {
        public static void RegisterComponents(Action<IUnityContainer> configure = null)
        {
            var container = new UnityContainer();

            container.RegisterInstance(container);
            container.RegisterType<IQueueManager, QueueManager>();

            if (configure != null)
                configure(container);
        }
    }
}
