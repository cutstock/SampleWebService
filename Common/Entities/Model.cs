using System;

namespace Common.Entities
{
    [Serializable]
    public partial class SchScheduler : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual String Code { get; set; }
        public virtual String Description { get; set; }
        public virtual Int32 Transact { get; set; }
        public virtual String UserIns { get; set; }
        public virtual DateTime DateIns { get; set; }
        public virtual String UserUpd { get; set; }
        public virtual DateTime? DateUpd { get; set; }
    }

    [Serializable]
    public partial class SchJobType : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual SchScheduler Scheduler { get; set; }
        public virtual String Code { get; set; }
        public virtual String Description { get; set; }
        public virtual String ClassName { get; set; }
        public virtual Int32 Transact { get; set; }
        public virtual String UserIns { get; set; }
        public virtual DateTime DateIns { get; set; }
        public virtual String UserUpd { get; set; }
        public virtual DateTime? DateUpd { get; set; }
    }

    [Serializable]
    public partial class SchJobTypeParam : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual SchJobType JobType { get; set; }
        public virtual String Code { get; set; }
        public virtual String Description { get; set; }
        public virtual String DefaultValue { get; set; }
        public virtual Boolean IsRequired { get; set; }
        public virtual Int32 Transact { get; set; }
        public virtual String UserIns { get; set; }
        public virtual DateTime DateIns { get; set; }
        public virtual String UserUpd { get; set; }
        public virtual DateTime? DateUpd { get; set; }
    }

    [Serializable]
    public partial class SchJob : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual SchScheduler Scheduler { get; set; }
        public virtual SchJobType JobType { get; set; }
        public virtual String JobGroup { get; set; }
        public virtual String Code { get; set; }
        public virtual String Description { get; set; }
        public virtual Int32 Transact { get; set; }
        public virtual String UserIns { get; set; }
        public virtual DateTime DateIns { get; set; }
        public virtual String UserUpd { get; set; }
        public virtual DateTime? DateUpd { get; set; }
    }

    [Serializable]
    public partial class SchJobParam : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual SchJob Job { get; set; }
        public virtual SchJobTypeParam JobTypeParam { get; set; }
        public virtual String Name { get; set; }
        public virtual String Value { get; set; }
        public virtual Int32 Transact { get; set; }
        public virtual String UserIns { get; set; }
        public virtual DateTime DateIns { get; set; }
        public virtual String UserUpd { get; set; }
        public virtual DateTime? DateUpd { get; set; }
    }

    [Serializable]
    public partial class SchTrigger : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual SchJob Job { get; set; }
        public virtual String TriggerType { get; set; }
        public virtual SchScheduler Scheduler { get; set; }
        public virtual String Code { get; set; }
        public virtual String Description { get; set; }
        public virtual String TriggerGroup { get; set; }
        public virtual Int32 Priority { get; set; }
        public virtual DateTime StartTimeUtc { get; set; }
        public virtual DateTime? EndTimeUtc { get; set; }
        public virtual Boolean? Disabled { get; set; }
        public virtual Int32 Transact { get; set; }
        public virtual String UserIns { get; set; }
        public virtual DateTime DateIns { get; set; }
        public virtual String UserUpd { get; set; }
        public virtual DateTime? DateUpd { get; set; }
    }

    [Serializable]
    public partial class SchCronTrigger : SchTrigger
    {
        public virtual String CronExpression { get; set; }
        public virtual Int32? MisfireInstruction { get; set; }
    }
}
