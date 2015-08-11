using Common.Logging;
using Quartz;

namespace JobRunner.Core.Listeners
{
    /// <summary>
    /// This class is used to track trigger events
    /// </summary>
    public class TriggerListener : ITriggerListener
    {
        private static readonly ILog Log = LogManager.GetLogger<TriggerListener>();

        /// <summary>
        /// Initializes a new instance of the TriggerListener class 
        /// </summary>
        /// <param name="name">Trigger listener name.</param>
        public TriggerListener(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the name of the trigger listener. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Called by the IScheduler when a ITrigger has fired, and it's associated IJobDetail is about to be executed. 
        /// It is called before the VetoJobExecution method of this interface.
        /// </summary>
        /// <param name="trigger">The ITrigger that was fired.</param>
        /// <param name="context">The IJobExecutionContext that was passed to the IJob'sExecute method.</param>
        public void TriggerFired(ITrigger trigger, IJobExecutionContext context)
        {
            Log.DebugFormat("The trigger '{0}' was fired on job '{1}'.", trigger.Key, context.JobDetail.Key);
        }

        /// <summary>
        /// Called by the IScheduler when a ITrigger has fired, and it's associated IJobDetail is about to be executed. 
        /// It is called after the TriggerFired method of this interface. If the implementation vetos the execution (via returning true), 
        /// the job's execute method will not be called
        /// </summary>
        /// <param name="trigger">The ITrigger that was fired.</param>
        /// <param name="context">The IJobExecutionContext that was passed to the IJob'sExecute method.</param>
        /// <returns>Returns true if job execution should be vetoed, false otherwise.</returns>
        public bool VetoJobExecution(ITrigger trigger, IJobExecutionContext context)
        {
            return false;
        }

        /// <summary>
        /// Called by the IScheduler when a ITrigger has misfired. 
        /// Consideration should be given to how much time is spent in this method, 
        /// as it will affect all triggers that are misfiring. If you have lots of triggers misfiring at once, 
        /// it could be an issue it this method does a lot. 
        /// </summary>
        /// <param name="trigger">The ITrigger that was fired.</param>
        public void TriggerMisfired(ITrigger trigger)
        {
            Log.DebugFormat("The trigger '{0}' was misfired. Misfire instruction '{1}'.", trigger.Key, trigger.MisfireInstruction);
        }

        /// <summary>
        /// Called by the IScheduler when a ITrigger has fired, 
        /// it's associated IJobDetail has been executed, and it's Triggered method has been called. 
        /// </summary>
        /// <param name="trigger">The ITrigger that was fired.</param>
        /// <param name="context">The IJobExecutionContext that was passed to the IJob'sExecute method.</param>
        /// <param name="triggerInstructionCode">The result of the call on the ITrigger'sTriggered method.</param>
        public void TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode)
        {
            Log.DebugFormat("The trigger '{0}' on job '{1}' is complete: Instruction Code = '{2}'.", trigger.Key, context.JobDetail.Key, triggerInstructionCode);
        }
    }
}
