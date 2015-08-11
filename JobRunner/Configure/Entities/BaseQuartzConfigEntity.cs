using System;

namespace JobRunner.Configure.Entities
{
    public abstract class BaseQuartzConfigEntity
    {
        public virtual Guid Id { get; set; }
    }
}
