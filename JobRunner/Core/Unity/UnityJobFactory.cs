using System;
using System.Diagnostics.Contracts;
using Common.Logging;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Spi;

namespace JobRunner.Core.Unity
{
    public class UnityJobFactory : IJobFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UnityJobFactory));
        private readonly IUnityContainer _container;

        static UnityJobFactory()
        {
        }

        public UnityJobFactory(IUnityContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            var jobType = jobDetail.JobType;

            try
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug(string.Format("Producing instance of Job '{0}', class={1}",
                        new object[] { jobDetail.Key, jobType.FullName }));
                }

                var disallowConcurent =
                    jobType.GetCustomAttributes(typeof(DisallowConcurrentExecutionAttribute), true).Length == 1;

                return typeof(IInterruptableJob).IsAssignableFrom(jobType)
                    ? disallowConcurent
                        ? new InterruptableDisallowConcurrentExecutionJobWrapper(bundle, _container)
                        : new InterruptableJobWrapper(bundle, _container)
                    : disallowConcurent
                        ? new DisallowConcurrentExecutionJobWrapper(bundle, _container)
                        : new JobWrapper(bundle, _container);
            }
            catch (Exception ex)
            {
                throw new SchedulerException(
                    string.Format("Problem instantiating class '{0}'", new object[] { jobDetail.JobType.FullName }), ex);
            }
        }

        public void ReturnJob(IJob job)
        {
            // Nothing here. Unity does not maintain a handle to container created instances.
        }

        #region .  Job Wrappers  .

        /// <summary>
        ///     Job execution wrapper.
        /// </summary>
        /// <remarks>
        ///     Creates nested lifetime scope per job execution and resolves Job from Autofac.
        /// </remarks>
        internal class JobWrapper : IJob
        {
            #region .  Fields  .

            protected readonly TriggerFiredBundle Bundle;
            private readonly IUnityContainer _unityContainer;

            #endregion

            /// <summary>
            ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
            /// </summary>
            public JobWrapper(TriggerFiredBundle bundle, IUnityContainer unityContainer)
            {
                Contract.Requires(bundle != null);
                Contract.Requires(unityContainer != null);

                Bundle = bundle;
                _unityContainer = unityContainer;
            }

            protected IJob RunningJob { get; private set; }

            /// <summary>
            ///     Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
            ///     fires that is associated with the <see cref="T:Quartz.IJob" />.
            /// </summary>
            /// <remarks>
            ///     The implementation may wish to set a  result object on the
            ///     JobExecutionContext before this method exits.  The result itself
            ///     is meaningless to Quartz, but may be informative to
            ///     <see cref="T:Quartz.IJobListener" />s or
            ///     <see cref="T:Quartz.ITriggerListener" />s that are watching the job's
            ///     execution.
            /// </remarks>
            /// <param name="context">The execution context.</param>
            /// <exception cref="SchedulerConfigException">Job cannot be instantiated.</exception>
            public void Execute(IJobExecutionContext context)
            {
                var childContainer = _unityContainer.CreateChildContainer();

                try
                {
                    try
                    {
                        RunningJob = (IJob)childContainer.Resolve(Bundle.JobDetail.JobType);
                    }
                    catch (Exception ex)
                    {
                        throw new JobExecutionException(string.Format("Failed to instantiate Job '{0}' of type '{1}'",
                            Bundle.JobDetail.Key, Bundle.JobDetail.JobType), ex);
                    }

                    RunningJob.Execute(context);
                }
                catch (JobExecutionException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new JobExecutionException(
                        string.Format("Unhandled exception in job '{0}'. {1}", Bundle.JobDetail.Key, ex.Message), ex);
                }
                finally
                {
                    ClearJob();
                    childContainer.Dispose();
                }
            }

            private void ClearJob()
            {
                var disposableJob = RunningJob as IDisposable;
                if (disposableJob != null)
                    disposableJob.Dispose();
                RunningJob = null;
            }
        }

        internal class InterruptableJobWrapper : JobWrapper, IInterruptableJob
        {
            public InterruptableJobWrapper(TriggerFiredBundle bundle, IUnityContainer unityContainer)
                : base(bundle, unityContainer)
            {
            }

            public void Interrupt()
            {
                var interruptableJob = RunningJob as IInterruptableJob;
                if (interruptableJob == null)
                    throw new UnableToInterruptJobException(
                        string.Format("Job '{0}' of type '{1}' is not interruptable.",
                            Bundle.JobDetail.Key, Bundle.JobDetail.JobType));

                interruptableJob.Interrupt();
            }
        }

        [DisallowConcurrentExecution]
        internal sealed class DisallowConcurrentExecutionJobWrapper : JobWrapper
        {
            public DisallowConcurrentExecutionJobWrapper(TriggerFiredBundle bundle, IUnityContainer unityContainer)
                : base(bundle, unityContainer)
            {
            }
        }

        [DisallowConcurrentExecution]
        internal sealed class InterruptableDisallowConcurrentExecutionJobWrapper : InterruptableJobWrapper
        {
            public InterruptableDisallowConcurrentExecutionJobWrapper(TriggerFiredBundle bundle,
                IUnityContainer unityContainer)
                : base(bundle, unityContainer)
            {
            }
        }

        #endregion
    }
}
