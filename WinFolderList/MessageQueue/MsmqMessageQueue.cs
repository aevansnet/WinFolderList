using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WinFolderList
{

    /// <summary>
    ///  An Msmq message queue which will enqueue and dequeue items of type T
    /// </summary>
    /// <typeparam name="T">Type of object to send through the queue</typeparam>
    public abstract class MsmqMessageQueue<T> : IMessageQueue<T>
    {

        private MessageQueue _messageQueue;
        private XmlMessageFormatter _messageFormatter;
        protected abstract string QueuePath { get; }


        public MsmqMessageQueue()
        {
            if (!CheckForService())
            {
                throw new Exception("MSMQ does not appear to be running on this machine. Is it installed and started?");
            }

            if (MessageQueue.Exists(QueuePath))
            {
                _messageQueue = new MessageQueue(QueuePath);
                _messageQueue.Label = "File List Queue";
            }
            else
            {
                // Create the Queue
                MessageQueue.Create(QueuePath);
                _messageQueue = new MessageQueue(QueuePath);
                _messageQueue.Label = "File List Queue";
            }

            _messageFormatter = new XmlMessageFormatter(new[] { typeof(T) });
            _messageQueue.Formatter = _messageFormatter;

        }

        public void Enqueue(T t)
        {
            _messageQueue.Send(t);
        }


        public T Dequeue()
        {
            while (true)
            {
                var message = _messageQueue.Receive();
                if (message != null)
                {
                    message.Formatter = _messageFormatter;
                    return (T)message.Body;
                }
            }
        }

        private bool CheckForService()
        {
            // crudely check if msmq is installed and running..

            var msmqService = ServiceController.GetServices()
                .FirstOrDefault(s => s.ServiceName == "MSMQ");

            return msmqService != null && msmqService.Status == ServiceControllerStatus.Running;
        }

    }
}
