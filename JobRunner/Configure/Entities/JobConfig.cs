using System.Collections.Generic;

namespace JobRunner.Configure.Entities
{
    public class JobConfig : BaseQuartzConfigEntity
    {
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }

        public virtual string Group { get; set; }
        public virtual JobTypeConfig JobType { get; set; }
        public virtual SchedulerConfig Scheduler { get; set; }

        public virtual ISet<TriggerConfig> Triggers { get; set; }
        public virtual ISet<JobParamConfig> Parameters { get; set; }
    }
}
