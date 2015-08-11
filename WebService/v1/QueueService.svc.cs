using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Common;
using System.Diagnostics.Contracts;

namespace WebService.v1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "QueueService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select QueueService.svc or QueueService.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)] 
    public class QueueService : IQueueService
    {
        private IQueueManager _queueManager;
        public QueueService(IQueueManager queueManager)
        {
            Contract.Requires(queueManager != null);
            _queueManager = queueManager;
        }

        public void Push(QueueMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            _queueManager.Send(message);
        }

        public QueueMessage Get(QueueMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            var res = _queueManager.Read("Test");
            return new QueueMessage { Id = Guid.NewGuid(), Date = DateTime.Now, Type = "Response", Container = res.Count().ToString() };
        }
    }
}
