using Common.Logging;
using Quartz;

namespace JobRunner.Core.Listeners
{
    /// <summary>
    /// This class is used to track job events
    /// </summary>
    public class JobListener : IJobListener
    {
        private static readonly ILog Log = LogManager.GetLogger<JobListener>();

        /// <summary>
        /// Initializes a new instance of the JobListener class 
        /// </summary>
        /// <param name="name">Job listener name.</param>
        public JobListener(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the name of the job listener. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Called by the IScheduler when a IJobDetail is about to be executed (an associated ITrigger has occurred). 
        /// This method will not be invoked if the execution of the Job was vetoed by a ITriggerListener. 
        /// </summary>
        /// <param name="context">The IJobExecutionContext that was passed to the IJob'sExecute method.</param>
        public void JobToBeExecuted(IJobExecutionContext context)
        {
            Log.DebugFormat("The job '{0}' is about to be executed.", context.JobDetail.Key);
        }

        /// <summary>
        /// Called by the IScheduler when a IJobDetail was about to be executed (an associated ITrigger has occurred), 
        /// but a ITriggerListener vetoed it's execution. 
        /// </summary>
        /// <param name="context">The IJobExecutionContext that was passed to the IJob'sExecute method.</param>
        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            Log.DebugFormat("The job '{0}' execution was vetoed.", context.JobDetail.Key);
        }

        /// <summary>
        /// Called by the IScheduler after a IJobDetail has been executed, 
        /// and be for the associated IOperableTrigger's Triggered method has been called. 
        /// </summary>
        /// <param name="context">The IJobExecutionContext that was passed to the IJob'sExecute method.</param>
        /// <param name="jobException">Job execution exception, if any.</param>
        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (jobException != null)
                Log.Error(string.Format("The job '{0}' thow exception. {1}", context.JobDetail.Key, jobException.Message), jobException);
            else
                Log.DebugFormat("The job '{0}' was executed: Result = '{1}'.", context.JobDetail.Key, context.Result ?? "None");
        }
    }
}
