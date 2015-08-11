using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Helpers;
using System.Messaging;

namespace MsmqManager
{
    public class QueueManager : IQueueManager
    {
        //private MessageQueue _messageQueue;

        private MessageQueue GetQueue()
        {
            //if(_messageQueue != null)
                //return _messageQueue;
            MessageQueue _messageQueue;
            var queuePath = AppConfigHelper.GetSettings<string>("QueuePath", true);
            if (MessageQueue.Exists(queuePath))
                _messageQueue = new MessageQueue(queuePath);
            else
                _messageQueue = MessageQueue.Create(queuePath);
            ((XmlMessageFormatter)_messageQueue.Formatter).TargetTypes = new Type[] { typeof(QueueMessage) };
            return _messageQueue;
        }

        public void Send(QueueMessage message)
        {
            using (var mq = GetQueue())
            {
                Message m = new Message(message) { Label = message.Type, Recoverable = true };
                mq.Send(m);
            }
        }

        public IEnumerable<QueueMessage> Read(string type)
        {
            var result = new List<QueueMessage>();
            using (var mq = GetQueue())
            {
                using (var enumerator = mq.GetMessageEnumerator2())
                {
                    while (enumerator.MoveNext(new TimeSpan(1)))
                    {
                        if (enumerator.Current.Label != type)
                            continue;
                        var body = (QueueMessage)enumerator.Current.Body;
                        if (body != null)
                            result.Add(body);
                    }
                }
            }
            return result;
        }

        public QueueMessage Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            var mq = GetQueue();
            var m = mq.PeekById(id, new TimeSpan(1));
            return (QueueMessage)m.Body;
        }

        public QueueMessage Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            var mq = GetQueue();
            var m = mq.ReceiveById(id, new TimeSpan(1));
            if (m == null)
                return null;
            return (QueueMessage)m.Body;
        }
    }
}
