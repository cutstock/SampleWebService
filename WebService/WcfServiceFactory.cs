using System;
using System.ServiceModel.Activation;
using log4net;
using log4net.Config;
using Microsoft.Practices.Unity;
using Unity.Wcf;
using System.ServiceModel;

namespace WebService
{
    public class WcfServiceFactory : ServiceHostFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WcfServiceFactory));
        private static IUnityContainer _rootContainer;

	    public WcfServiceFactory()
	    {
            XmlConfigurator.Configure();
	    }

        public static void SetRootContainer(IUnityContainer container)
        {
            if (_rootContainer != null)
                throw new NotSupportedException("Root container already set. You can't update it");
            _rootContainer = container;
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var sh = new UnityServiceHost(_rootContainer, serviceType, baseAddresses);
            sh.Faulted += (s, e) =>
            {
                Log.DebugFormat("Faulted connection: {0}", e);
            };
            sh.UnknownMessageReceived += (s, e) =>
            {
                Log.ErrorFormat("Unknown message received: {0}", e.Message);
            };
            return sh;
        }
    }    
}