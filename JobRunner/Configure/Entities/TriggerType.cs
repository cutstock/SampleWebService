
namespace JobRunner.Configure.Entities
{
    public class TriggerType : BaseQuartzConfigEntity
    {
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }
    }
}
