using System;
using System.Runtime.Serialization;

namespace Common
{
    [DataContract(Namespace = NamespaceHelper.V1Namespace, Name = "QueueMessage")]
    public class QueueMessage
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public String Type { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public String Container { get; set; }
    }
}
