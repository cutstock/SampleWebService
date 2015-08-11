using System;
using System.Collections.Generic;

namespace JobRunner.Model
{
    public interface ISchScheduler
    {
        String Code { get; set; }
        String Description { get; set; }
    }

    public interface ISchTrigger
    {
        object GetKey();

        ISchJob Job { get; set; }
        String TriggerType { get; set; }
        ISchScheduler Scheduler { get; set; }
        String Code { get; set; }
        String Description { get; set; }
        String TriggerGroup { get; set; }
        Int32 Priority { get; set; }
        DateTime StartTimeUtc { get; set; }
        DateTime? EndTimeUtc { get; set; }
        Boolean? Disabled { get; set; }
        Guid? ID { get; set; }
    }

    public interface ISchCronTrigger : ISchTrigger
    {
        String CronExpression { get; set; }
        Int32? MisfireInstruction { get; set; }
    }

    public interface ISchSimpleTrigger : ISchTrigger
    {
        Int32 RepeatCount { get; set; }
        Int32 RepeatIntervalInMs { get; set; }
        Int32? MisfireInstruction { get; set; }
    }

    public interface ISchJob
    {
        ISchScheduler Scheduler { get; set; }
        ISchJobType JobType { get; set; }
        String JobGroup { get; set; }
        String Code { get; set; }
        String Description { get; set; }

        IEnumerable<ISchTrigger> Job_Trigger_List { get; set; }
        IEnumerable<ISchJobParam> Job_JobParam_List { get; set; }
    }

    public interface ISchJobParam
    {
        ISchJob Job { get; set; }
        ISchJobTypeParam JobTypeParam { get; set; }
        String Name { get; set; }
        String Value { get; set; }
    }

    public interface ISchJobTypeParam
    {
        ISchJobType JobType { get; set; }
        String Code { get; set; }
        String Description { get; set; }
        String DefaultValue { get; set; }
        Boolean IsRequired { get; set; }
    }


    public interface ISchJobType
    {
        ISchScheduler Scheduler { get; set; }
        String Code { get; set; }
        String Description { get; set; }
        String ClassName { get; set; }
    }
}
