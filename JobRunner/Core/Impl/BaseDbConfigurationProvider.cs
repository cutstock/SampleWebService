using System;
using System.Collections.Generic;
using System.Linq;
using Common.Entities;
using Common.Logging;
using JobRunner.Model;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Spi;

namespace JobRunner.Core.Impl
{
    public abstract class BaseDbConfigurationProvider : IConfigurationProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaseDbConfigurationProvider));

        public IDictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> GetJobs(string schedulerName)
        {
            var res = new Dictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>>();
            var jobs = GetJobsInternal(schedulerName);
            if (jobs.Length == 0)
                return res;

            foreach (var job in jobs)
            {
                var jobDetail = GetJobDetail(job);
                var triggers = GetTriggers(job);
                if (triggers.Any())
                    res.Add(jobDetail, triggers);
            }
            return res;
        }

        public void GetChangedJobs(string schedulerName, DateTime lastCheckDate, out JobKey[] deletedJobs, out IDictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> changedJobs)
        {
            deletedJobs = null;
            changedJobs = null;
            var changedQJobs = new Dictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>>();

            var delJobsList = new List<JobKey>();

            Action<ISchJob, DateTime> createJob = (qjob, uDate) =>
            {
                if (qjob.Scheduler.Code != schedulerName)
                    return;
                var jobDetail = GetJobDetail(qjob);
                var triggers = GetTriggers(qjob, t => t.StartTimeUtc <= uDate && (!t.EndTimeUtc.HasValue || uDate < t.EndTimeUtc) && t.Disabled != true);
                if (triggers.Any())
                    changedQJobs.Add(jobDetail, triggers);
            };

            var chgJobIds = GetChangedJobsInternal(lastCheckDate);
            if (!chgJobIds.Any())
                return;

            var chgJobIdsdb = chgJobIds.Where(p => p.Value == null || p.Value.JobKey == null).Select(p => p.Key).ToArray();
            var skipDb = chgJobIdsdb.Length == 0;

            var jobs = skipDb
                ? new Dictionary<Guid, ISchJob>()
                :
                //: session.Query<SchJob>()
                //    .Where(p => chgJobIdsdb.Contains(p.ID))
                //    .ToDictionary(k => k.ID);
                //STUB
                new Dictionary<Guid, ISchJob>() {};

            var utcDate = DateTime.UtcNow;
            foreach (var id in chgJobIds.Keys)
            {
                //Обрабатываем удаленные SchJob'ы
                if (!jobs.ContainsKey(id))
                {
                    var hjobinfo = chgJobIds[id];
                    if (hjobinfo != null && hjobinfo.IsDeleted && hjobinfo.JobKey != null)
                        delJobsList.Add(hjobinfo.JobKey);
                    continue;
                }

                var job = jobs[id];
                if (chgJobIds[id] != null && chgJobIds[id].IsDeleted)
                {
                    delJobsList.Add(new JobKey(job.Code, job.JobGroup));
                    createJob(job, utcDate);
                }
                else
                {
                    createJob(job, utcDate);
                }
            }

            deletedJobs = delJobsList.ToArray();
            changedJobs = changedQJobs;
        }

        //Проверяем изменение истории сущностей: HSchJob, HSchTrigger, HSchJobParam. При удалении в HSchTrigger, HSchJobParam удаляем и запускаем соответствующие job'ы.
        protected abstract Dictionary<Guid, HJobInfo> GetChangedJobsInternal(DateTime lastCheckDate);

        protected abstract ISchJob[] GetJobsInternal(string schedulerName);

        private static Quartz.Collection.ISet<ITrigger> GetTriggers(ISchJob job, Func<ISchTrigger, bool> validateHandler = null)
        {
            var res = new Quartz.Collection.HashSet<ITrigger>();
            foreach (var trigger in job.Job_Trigger_List)
            {
                if (validateHandler != null && !validateHandler(trigger))
                    continue;

                var qtrigger = GetTrigger(trigger);
                if (qtrigger == null)
                    continue;

                res.Add(qtrigger);
            }
            return res;
        }

        private static ITrigger GetTrigger(ISchTrigger trigger)
        {
            IMutableTrigger qtrigger;
            var cronTrigger = trigger as ISchCronTrigger;
            if (cronTrigger != null)
            {
                qtrigger = new CronTriggerImpl(trigger.Code, trigger.TriggerGroup)
                {
                    Description = trigger.Description,
                    StartTimeUtc = trigger.StartTimeUtc,
                    EndTimeUtc = trigger.EndTimeUtc,
                    Priority = trigger.Priority,

                    CronExpressionString = cronTrigger.CronExpression
                };

                if (!ValidateMisfireInstruction(qtrigger, cronTrigger.MisfireInstruction, MisfireInstruction.CronTrigger.DoNothing, trigger.ID))
                    return null;
                return qtrigger;
            }

            var simpleTrigger = trigger as ISchSimpleTrigger;
            if (simpleTrigger != null)
            {
                qtrigger = new SimpleTriggerImpl(trigger.Code, trigger.TriggerGroup)
                {
                    Description = trigger.Description,
                    StartTimeUtc = trigger.StartTimeUtc,
                    EndTimeUtc = trigger.EndTimeUtc,
                    Priority = trigger.Priority,

                    RepeatCount = simpleTrigger.RepeatCount,
                    RepeatInterval = TimeSpan.FromMilliseconds(simpleTrigger.RepeatIntervalInMs)
                };

                if (!ValidateMisfireInstruction(qtrigger, simpleTrigger.MisfireInstruction, 0, trigger.GetKey()))
                    return null;
                return qtrigger;
            }

            throw new Exception(string.Format("Unknown trigger type '{0}'.", trigger.TriggerType));
        }

        private static IJobDetail GetJobDetail(ISchJob job)
        {
            var jobType = Type.GetType(job.JobType.ClassName);
            return new JobDetailImpl(job.Code, job.JobGroup, jobType)
            {
                Description = job.Description,
                JobDataMap = GetJobDataMap(job)
            };
        }

        private static JobDataMap GetJobDataMap(ISchJob job)
        {
            var res = new JobDataMap();
            foreach (var p in job.Job_JobParam_List)
                res.Add(p.Name, p.Value);
            return res;
        }

        private static bool ValidateMisfireInstruction(IMutableTrigger qTrigger, int? misfireInstruction, int nvlValue, object id)
        {
            if (qTrigger == null)
                return false;

            try
            {
                qTrigger.MisfireInstruction = misfireInstruction ?? nvlValue;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("The misfire instruction code '{0}' is invalid for this type of trigger '{1}' (id: '{2}').", misfireInstruction, qTrigger.GetType(), id), ex);
                return false;
            }
        }

        public class HJobInfo
        {
            public bool IsDeleted { get; set; }
            public ISchJob Job { get; set; }
            public JobKey JobKey { get; set; }
        }
    }
}
