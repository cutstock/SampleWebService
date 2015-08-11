using Common.Logging;
using Quartz;

namespace JobRunner.Core.Listeners
{
    /// <summary>
    /// This class is used to track trigger events
    /// </summary>
    public class SchedulerListener : ISchedulerListener
    {
        private static readonly ILog Log = LogManager.GetLogger<SchedulerListener>();

        /// <summary>
        /// Called by the IScheduler when a IJobDetail has been added. 
        /// </summary>
        /// <param name="jobDetail">The job details.</param>
        public void JobAdded(IJobDetail jobDetail)
        {
            Log.DebugFormat("The job '{0}' was added to the scheduler.", jobDetail.Key);
        }

        /// <summary>
        /// Called by the IScheduler when a IJobDetail has been deleted. 
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        public void JobDeleted(JobKey jobKey)
        {
            Log.DebugFormat("The job '{0}' was deleted from the scheduler.", jobKey);
        }

        /// <summary>
        /// Called by the IScheduler when a IJobDetail is scheduled.
        /// </summary>
        /// <param name="trigger">The trigger that was fired.</param>
        public void JobScheduled(ITrigger trigger)
        {
            Log.DebugFormat("The trigger '{0}' on job '{1}' was scheduled.", trigger.Key, trigger.JobKey);
        }

        /// <summary>
        /// Called by the IScheduler when a IJobDetail is unscheduled. 
        /// </summary>
        /// <param name="triggerKey">The trigger key.</param>
        public void JobUnscheduled(TriggerKey triggerKey)
        {
            Log.DebugFormat("The trigger '{0}' was unscheduled.", triggerKey);
        }

        /// <summary>
        /// Called by the IScheduler when a group of ITriggers has been un-paused. 
        /// </summary>
        /// <param name="triggerGroup">The trigger group.</param>
        public void TriggersResumed(string triggerGroup)
        {
            Log.DebugFormat("The triggers in the group '{0}' were resumed.", triggerGroup);
        }

        /// <summary>
        /// Called by the IScheduler a group of ITriggers has been paused. 
        /// </summary>
        /// <param name="triggerGroup">The trigger group.</param>
        public void TriggersPaused(string triggerGroup)
        {
            Log.DebugFormat("The triggers in the group '{0}' were paused.", triggerGroup);
        }

        /// <summary>
        /// Called by the IScheduler when a ITrigger has reached the condition in which it will never fire again. 
        /// </summary>
        /// <param name="trigger">The trigger that was fired.</param>
        public void TriggerFinalized(ITrigger trigger)
        {
            Log.DebugFormat("The trigger '{0}' on job '{1}' was finalized.", trigger.Key, trigger.JobKey);
        }

        /// <summary>
        /// Called by the IScheduler a ITriggers has been paused. 
        /// </summary>
        /// <param name="triggerKey">The trigger key.</param>
        public void TriggerPaused(TriggerKey triggerKey)
        {
            Log.DebugFormat("The trigger '{0}' was paused.", triggerKey);
        }

        /// <summary>
        /// Called by the IScheduler when a ITrigger has been un-paused. 
        /// </summary>
        /// <param name="triggerKey">The trigger key.</param>
        public void TriggerResumed(TriggerKey triggerKey)
        {
            Log.DebugFormat("The trigger '{0}' was resumed.", triggerKey);
        }

        /// <summary>
        /// Called by the IScheduler when a group of IJobDetails has been paused. 
        /// If all groups were paused, then the parameter will be null. 
        /// If all jobs were paused, then both parameters will be null. 
        /// </summary>
        /// <param name="jobGroup">The job group.</param>
        public void JobsPaused(string jobGroup)
        {
            Log.DebugFormat("The jobs in the group '{0}' were paused.", jobGroup);
        }

        /// <summary>
        /// Called by the IScheduler when a IJobDetail has been un-paused. 
        /// </summary>
        /// <param name="jobGroup">The job group.</param>
        public void JobsResumed(string jobGroup)
        {
            Log.DebugFormat("The jobs in the group '{0}' were resumed.", jobGroup);
        }

        /// <summary>
        /// Called by the IScheduler when a IJobDetail has been paused. 
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        public void JobPaused(JobKey jobKey)
        {
            Log.DebugFormat("The job '{0}' was paused.", jobKey);
        }

        /// <summary>
        /// Called by the IScheduler when a IJobDetail has been un-paused. 
        /// </summary>
        /// <param name="jobKey">The job key</param>
        public void JobResumed(JobKey jobKey)
        {
            Log.DebugFormat("The job '{0}' was resumed.", jobKey);
        }

        /// <summary>
        /// Called by the IScheduler to inform the listener that all jobs, triggers and calendars were deleted. 
        /// </summary>
        public void SchedulingDataCleared()
        {
            Log.DebugFormat("Scheduling data was cleared.");
        }

        /// <summary>
        /// Called by the IScheduler when a serious error has occurred within the scheduler, 
        /// such as repeated failures in the IJobStore, or the inability to instantiate a IJob 
        /// instance when its ITrigger has fired. 
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="cause">The scheduler exception.</param>
        public void SchedulerError(string message, SchedulerException cause)
        {
            Log.DebugFormat("An error occurred in the scheduler: message = '{0}' cause = '{1}'.", message, cause);
        }

        /// <summary>
        /// Called by the IScheduler to inform the listener that it is starting. 
        /// </summary>
        public void SchedulerStarting()
        {
            Log.DebugFormat("The scheduler is starting.");
        }

        /// <summary>
        /// Called by the IScheduler to inform the listener that it has started. 
        /// </summary>
        public void SchedulerStarted()
        {
            Log.DebugFormat("The scheduler has started.");
        }

        /// <summary>
        /// Called by the IScheduler to inform the listener that it has move to standby mode. 
        /// </summary>
        public void SchedulerInStandbyMode()
        {
            Log.DebugFormat("The scheduler is in stand-by mode.");
        }

        /// <summary>
        /// Called by the IScheduler to inform the listener that it has shut down. 
        /// </summary>
        public void SchedulerShutdown()
        {
            Log.DebugFormat("The scheduler has shut down.");
        }

        /// <summary>
        /// Called by the IScheduler to inform the listener that it is shutting down. 
        /// </summary>
        public void SchedulerShuttingdown()
        {
            Log.DebugFormat("The scheduler is shutting down.");
        }
    }
}
