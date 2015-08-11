using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using JobRunner.Core;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Spi;

namespace JobRunner.Plugins.Db
{
    /// <summary>
    /// <remarks>Для plugin-ов не предусмотрена ни какая фабрика. Потому получаем доступ к IoC через соответсвующий scheduler</remarks>
    /// </summary>
    public class DbConfiguratorPlugin : ISchedulerPlugin, IDbConfiguratorPlugin
    {
        #region .  Fields & Consts  .
        private const string DefaultSchedulerConfigName = "DEFAULT";
        internal const string ContextPluginNameJobDataMap = "DB_CONFIGURATOR_PLAGIN_NAME";

        private CustomScheduler _scheduler;
        private IConfigurationProvider _provider;
        private string _name;
        private DateTime? _lastCheckDate;

        private static readonly ILog Log = LogManager.GetLogger(typeof(DbConfiguratorPlugin));
        #endregion

        #region .  Properties  .
        [TimeSpanParseRule(TimeSpanParseRule.Seconds)]
        public TimeSpan ScanInterval { get; set; }

        private string PluginName { get { return GetType().Name; } }

        private string SchedulerConfigName
        {
            get { return _scheduler.SchedulerName ?? DefaultSchedulerConfigName; }
        }
        #endregion

        public void Initialize(string pluginName, IScheduler sched)
        {
            _name = pluginName;
            _scheduler = sched as CustomScheduler;
            if (_scheduler != null)
                _provider = _scheduler.Resolve<IConfigurationProvider>();
        }

        public void Start()
        {
            AddRefreshJob();
            ScheduleJobs();
        }

        public void Shutdown()
        {
        }

        private void AddRefreshJob()
        {
            //XMLSchedulingDataProcessorPlugin
            //XMLSchedulingDataProcessor

            const string triggerName = "WatchingJob";
            var name = string.Format("{0}_{1}", PluginName, _name);

            try
            {
                if (ScanInterval > TimeSpan.Zero)
                {
                    _scheduler.Context.Put(name, this);

                    var triggerKey = new TriggerKey(triggerName, PluginName);
                    _scheduler.UnscheduleJob(triggerKey);

                    var trigger = new SimpleTriggerImpl
                    {
                        Name = triggerName,
                        Group = GetType().Name,
                        StartTimeUtc = SystemTime.UtcNow(),
                        EndTimeUtc = null,
                        RepeatCount = -1,
                        RepeatInterval = ScanInterval
                    };

                    var jobDetail = new JobDetailImpl(triggerName, PluginName, typeof(DbConfiguratorJob));
                    jobDetail.JobDataMap.Put(ContextPluginNameJobDataMap, string.Format("{0}_{1}", PluginName, _name));

                    _scheduler.ScheduleJob(jobDetail, trigger);

                    Log.DebugFormat("Scheduled file scan job for db data, at interval: {0}", ScanInterval);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error starting background-task for watching jobs.", ex);
            }
        }

        private void ScheduleJobs()
        {
            try
            {
                var jobs = _provider.GetJobs(SchedulerConfigName);
                if (jobs.Any())
                    _scheduler.ScheduleJobs(jobs, true);
            }
            catch (Exception ex)
            {
                Log.Error("Error starting scheduler jobs.", ex);
            }
        }

        void IDbConfiguratorPlugin.JobProcessing()
        {
            //var jobkeys = _scheduler.GetJobKeys(Quartz.Impl.Matchers.GroupMatcher<JobKey>.AnyGroup());
            //var triggerkeys = _scheduler.GetTriggerKeys(Quartz.Impl.Matchers.GroupMatcher<TriggerKey>.AnyGroup());
            //if (triggerkeys != null && triggerkeys.Any())
            //{
            //    var trList = triggerkeys.Select(p => _scheduler.GetTrigger(p)).ToList();
            //}

            if (_lastCheckDate.HasValue)
            {
                JobKey[] deletedJobs;
                IDictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> changedJobs;
                _provider.GetChangedJobs(SchedulerConfigName, _lastCheckDate.Value, out deletedJobs, out changedJobs);

                if (deletedJobs != null && deletedJobs.Length > 0)
                {
                    _scheduler.DeleteJobs(deletedJobs);
                    Log.Debug(string.Format("Deleted jobs: {0}.", string.Join(", ", deletedJobs.Select(p => string.Format("'{0}'", p)))));
                }

                if (changedJobs != null && changedJobs.Any())
                {
                    _scheduler.ScheduleJobs(changedJobs, true);
                    Log.Debug(string.Format("Scheduled new jobs: {0}.", string.Join(", ", changedJobs.Select(p => string.Format("'{0}'", p.Key.Key)))));
                }

                _lastCheckDate = DateTime.Now;
            }
            else
            {
                _lastCheckDate = DateTime.Now;
            }

            Log.Debug(string.Format("Now processing {0} jobs.", _scheduler.GetJobKeys(Quartz.Impl.Matchers.GroupMatcher<JobKey>.AnyGroup()).Count));
        }
    }
}
