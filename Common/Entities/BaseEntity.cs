using System;

namespace Common.Entities
{
    [Serializable]
    public class BaseEntity
    {
        public virtual string EntityName
        {
            get { return GetType().Name; }
            set { }
        }
    }
}
