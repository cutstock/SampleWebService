
namespace JobRunner.Configure.Entities
{
    public class JobParamConfig : BaseQuartzConfigEntity
    {
        public virtual string Description { get; set; }

        public virtual JobConfig Job { get; set; }
        public virtual JobTypeParamConfig JobTypeParam { get; set; }
        public virtual string Name { get; set; }
        public virtual object Value { get; set; }
    }
}
