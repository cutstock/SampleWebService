using System.Runtime.Serialization;
using Common;

namespace WebService
{
    [DataContract(Namespace = NamespaceHelper.V1Namespace, Name = "MessageType")]
    public enum MessageType
    {
        Register,
        Bid
    }
}