using System;

namespace JobRunner.Configure.Entities
{
    public class TriggerConfig : BaseQuartzConfigEntity
    {
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }

        public virtual JobConfig Job { get; set; }
        public virtual SchedulerConfig Scheduler { get; set; }
        public virtual TriggerType TriggerType { get; set; }
        public virtual string Expression { get; set; }
        public virtual string Group { get; set; }

        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public int Priority { get; set; }
    }
}
