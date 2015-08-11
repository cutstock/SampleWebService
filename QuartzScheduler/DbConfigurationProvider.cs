using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using JobRunner.Core;
using Common.Entities;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Spi;

namespace QuartzScheduler
{
    internal class DbConfigurationProvider : IConfigurationProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DbConfigurationProvider));
        //private readonly ISessionFactory _sessionFactory;

        //public DbConfigurationProvider(ISessionFactory sessionFactory)
        //{
        //    _sessionFactory = sessionFactory;
        //}

        public IDictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> GetJobs(string schedulerName)
        {
            var res = new Dictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>>();
            //using (var session = _sessionFactory.OpenSession())
            //{
            //    var jobs = GetJobs(session, schedulerName);
            //    if (jobs.Length == 0)
            //        return res;

            //    foreach (var job in jobs)
            //    {
            //        var jobDetail = GetJobDetail(job);
            //        var triggers = GetTriggers(job);
            //        if (triggers.Any())
            //            res.Add(jobDetail, triggers);
            //    }
            //}
            return res;
        }

        public void GetChangedJobs(string schedulerName, DateTime lastCheckDate, out JobKey[] deletedJobs, out IDictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> changedJobs)
        {
            deletedJobs = null;
            changedJobs = null;
            var changedQJobs = new Dictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>>();

            var delJobsList = new List<JobKey>();

            //Action<SchJob, DateTime> createJob = (qjob, uDate) =>
            //{
            //    if (qjob.Scheduler.Code != schedulerName)
            //        return;
            //    var jobDetail = GetJobDetail(qjob);
            //    var triggers = GetTriggers(qjob, t => t.StartTimeUtc <= uDate && (!t.EndTimeUtc.HasValue || uDate < t.EndTimeUtc) &&
            //        t.Disabled != true);
            //    if (triggers.Any())
            //        changedQJobs.Add(jobDetail, triggers);
            //};

            //using (var session = _sessionFactory.OpenSession())
            //{
            //    var chgJobIds = GetChangedJobs(session, lastCheckDate);
            //    if (!chgJobIds.Any())
            //        return;

            //    var chgJobIdsdb = chgJobIds.Where(p => p.Value == null || p.Value.JobKey == null).Select(p => p.Key).ToArray();
            //    var skipDb = chgJobIdsdb.Length == 0;

            //    var jobs = skipDb ? new Dictionary<Guid, SchJob>()
            //        : session.Query<SchJob>()
            //            .Where(p => chgJobIdsdb.Contains(p.ID))
            //            .ToDictionary(k => k.ID);

            //    var utcDate = DateTime.UtcNow;
            //    foreach (var id in chgJobIds.Keys)
            //    {
            //        //Обрабатываем удаленные SchJob'ы
            //        if (!jobs.ContainsKey(id))
            //        {
            //            var hjobinfo = chgJobIds[id];
            //            if (hjobinfo != null && hjobinfo.IsDeleted && hjobinfo.JobKey != null)
            //                delJobsList.Add(hjobinfo.JobKey);
            //            continue;
            //        }

            //        var job = jobs[id];
            //        if (chgJobIds[id] != null && chgJobIds[id].IsDeleted)
            //        {
            //            delJobsList.Add(new JobKey(job.Code, job.JobGroup));
            //            createJob(job, utcDate);
            //        }
            //        else
            //        {
            //            createJob(job, utcDate);
            //        }
            //    }
            //}

            deletedJobs = delJobsList.ToArray();
            changedJobs = changedQJobs;
        }

        //Проверяем изменение истории сущностей: HSchJob, HSchTrigger, HSchJobParam. При удалении в HSchTrigger, HSchJobParam удаляем и запускаем соответствующие job'ы.
        //private static Dictionary<Guid, HJobInfo> GetChangedJobs(ISession session, DateTime lastCheckDate)
        //{
        //    const string operationDelete = "delete";
        //    //const string operationCreate = "create";

        //    //Deleted - true
        //    var result = new Dictionary<Guid, HJobInfo>();

        //    //1. HSchJob
        //    var hjobchItems = GetHistoryChanged<HSchJob>(session, lastCheckDate);
        //    if (hjobchItems.Length > 0)
        //    {
        //        foreach (var h in hjobchItems)
        //        {
        //            var key = h.ID;
        //            if (h.Operation.ToLower() == operationDelete)
        //            {
        //                //TODO: Проблема. Если были изменены значения свойств Code, JobGroup, то job не удалится при удалении.
        //                result[key] = new HJobInfo
        //                {
        //                    IsDeleted = true,
        //                    JobKey = new JobKey(h.Code, h.JobGroup)
        //                };
        //            }
        //            else
        //            {
        //                result[key] = new HJobInfo();
        //            }
        //        }
        //    }

        //    //2. HSchTrigger
        //    var hschtriggers = GetHistoryChanged<HSchTrigger>(session, lastCheckDate);
        //    if (hschtriggers.Length > 0)
        //    {
        //        var utcDate = DateTime.UtcNow;
        //        foreach (var h in hschtriggers)
        //        {
        //            var key = h.JobId;
        //            if (h.Operation.ToLower() == operationDelete || h.Disabled == true || (h.EndTimeUtc.HasValue && utcDate >= h.EndTimeUtc))
        //            {
        //                result[key] = new HJobInfo { IsDeleted = true };
        //            }
        //            else
        //            {
        //                if (!result.ContainsKey(key))
        //                    result[key] = new HJobInfo();
        //            }
        //        }
        //    }

        //    //3. HSchJobParam
        //    var hschjobparams = GetHistoryChanged<HSchJobParam>(session, lastCheckDate);
        //    if (hschjobparams.Length > 0)
        //    {
        //        foreach (var h in hschtriggers)
        //        {
        //            var key = h.JobId;
        //            if (!result.ContainsKey(key))
        //                result[key] = new HJobInfo();
        //        }
        //    }

        //    return result;
        //}

        //private static T[] GetHistoryChanged<T>(ISession session, DateTime lastCheckDate) where T : IHistoryEntity
        //{
        //    var history = session.Query<T>()
        //       .Where(p => p.HDateTill > DateTime.Now && p.HDateFrom >= lastCheckDate).ToArray();
        //    return history;
        //}

        //private static SchJob[] GetJobs(ISession session, string schedulerName)
        //{
        //    var utcDate = DateTime.UtcNow;
        //    var jobs = session.Query<SchTrigger>()
        //        .Where(i => i.StartTimeUtc <= utcDate && (!i.EndTimeUtc.HasValue || utcDate < i.EndTimeUtc) &&
        //                    i.Disabled != true &&
        //                    i.Scheduler.Code == schedulerName)
        //        .Select(i => i.Job)
        //        .Distinct()
        //        .ToArray();
        //    return jobs;
        //}

        //private static Quartz.Collection.ISet<ITrigger> GetTriggers(SchJob job, Func<SchTrigger, bool> validateHandler = null)
        //{
        //    var res = new Quartz.Collection.HashSet<ITrigger>();
        //    foreach (var trigger in job.Job_Trigger_List)
        //    {
        //        if (validateHandler != null && !validateHandler(trigger))
        //            continue;

        //        var qtrigger = GetTrigger(trigger);
        //        if (qtrigger == null)
        //            continue;

        //        res.Add(qtrigger);
        //    }
        //    return res;
        //}

        //private static ITrigger GetTrigger(SchTrigger trigger)
        //{
        //    IMutableTrigger qtrigger;
        //    var cronTrigger = trigger as SchCronTrigger;
        //    if (cronTrigger != null)
        //    {
        //        qtrigger = new CronTriggerImpl(trigger.Code, trigger.TriggerGroup)
        //        {
        //            Description = trigger.Description,
        //            StartTimeUtc = trigger.StartTimeUtc,
        //            EndTimeUtc = trigger.EndTimeUtc,
        //            Priority = trigger.Priority,

        //            CronExpressionString = cronTrigger.CronExpression
        //        };

        //        if (!ValidateMisfireInstruction(qtrigger, cronTrigger.MisfireInstruction, MisfireInstruction.CronTrigger.DoNothing, trigger.ID))
        //            return null;
        //        return qtrigger;
        //    }

        //    var simpleTrigger = trigger as SchSimpleTrigger;
        //    if (simpleTrigger != null)
        //    {
        //        qtrigger = new SimpleTriggerImpl(trigger.Code, trigger.TriggerGroup)
        //        {
        //            Description = trigger.Description,
        //            StartTimeUtc = trigger.StartTimeUtc,
        //            EndTimeUtc = trigger.EndTimeUtc,
        //            Priority = trigger.Priority,

        //            RepeatCount = simpleTrigger.RepeatCount,
        //            RepeatInterval = TimeSpan.FromMilliseconds(simpleTrigger.RepeatIntervalInMs)
        //        };

        //        if (!ValidateMisfireInstruction(qtrigger, simpleTrigger.MisfireInstruction, 0, trigger.ID))
        //            return null;
        //        return qtrigger;
        //    }

        //    throw new Exception(string.Format("Unknown trigger type '{0}'.", trigger.TriggerType));
        //}

        //private static IJobDetail GetJobDetail(SchJob job)
        //{
        //    var jobType = Type.GetType(job.JobType.ClassName);
        //    return new JobDetailImpl(job.Code, job.JobGroup, jobType)
        //    {
        //        Description = job.Description,
        //        JobDataMap = GetJobDataMap(job)
        //    };
        //}

        //private static JobDataMap GetJobDataMap(SchJob job)
        //{
        //    var res = new JobDataMap();
        //    foreach (var p in job.Job_JobParam_List)
        //        res.Add(p.Name, p.Value);
        //    return res;
        //}

        //private static bool ValidateMisfireInstruction(IMutableTrigger qTrigger, int? misfireInstruction, int nvlValue, object id)
        //{
        //    if (qTrigger == null)
        //        return false;

        //    try
        //    {
        //        qTrigger.MisfireInstruction = misfireInstruction ?? nvlValue;
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(string.Format("The misfire instruction code '{0}' is invalid for this type of trigger '{1}' (id: '{2}').", misfireInstruction, qTrigger.GetType(), id), ex);
        //        return false;
        //    }
        //}

        //private class HJobInfo
        //{
        //    public bool IsDeleted { get; set; }
        //    public JobKey JobKey { get; set; }
        //}
    }
}
