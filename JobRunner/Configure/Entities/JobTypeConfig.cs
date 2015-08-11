
namespace JobRunner.Configure.Entities
{
    public class JobTypeConfig : BaseQuartzConfigEntity
    {
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }

        public virtual SchedulerConfig Scheduler { get; set; }
        public virtual string ClassName { get; set; }
    }
}
