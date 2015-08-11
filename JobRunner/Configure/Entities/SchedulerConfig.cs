using System.Collections.Generic;

namespace JobRunner.Configure.Entities
{
    public class SchedulerConfig : BaseQuartzConfigEntity
    {
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }

        public virtual List<JobConfig> Jobs { get; set; }
        public virtual List<TriggerConfig> Triggers { get; set; }
    }
}
