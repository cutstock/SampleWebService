using System.ServiceModel;
using System.ServiceModel.Web;
using Common;

namespace WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IQueueService" in both code and config file together.
    [ServiceContract(Namespace = NamespaceHelper.V1Namespace)]
    public interface IQueueService
    {
        [OperationContract(IsOneWay = true)]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        void Push(QueueMessage message);

        [OperationContract(IsOneWay = false)]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        QueueMessage Get(QueueMessage message);
    }
}
