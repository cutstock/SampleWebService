using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Common;
using Common.Logging;
using Quartz;

namespace BussinesJobs
{
    public class QueueJob : IJob
    {
        #region .  Fields  .
        private static readonly ILog Log = LogManager.GetLogger(typeof(QueueJob));
        private IQueueManager _queueManager;
        #endregion

        public QueueJob(IQueueManager queueManager)
        {
            Contract.Requires(queueManager != null);
            _queueManager = queueManager;
        }
        public void Execute(IJobExecutionContext context)
        {
            var messageType = JobHelper.GetRequiredParameter<string>(context, "MessageType");
            var watch = new Stopwatch();
            Log.Debug("QueueJob start");
            watch.Start();
            var result = _queueManager.Read(messageType);
            watch.Stop();
            Log.DebugFormat("Read message time {0}", watch.Elapsed);
            Log.DebugFormat("MSMQ has {0} messages typeof {1}", result.Count(), messageType);
        }
    }
}
