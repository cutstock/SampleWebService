
namespace JobRunner.Configure.Entities
{
    public class JobTypeParamConfig : BaseQuartzConfigEntity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual JobTypeConfig JobType { get; set; }
        public virtual object DefaultValue { get; set; }
        public virtual bool IsRequared { get; set; }
    }
}
