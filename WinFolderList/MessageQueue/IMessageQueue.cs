using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFolderList
{
    /// <summary>
    /// Interface describing a basic message queue
    /// </summary>
    /// <typeparam name="T">The Type of messages this queue is required to handle</typeparam>
    public interface IMessageQueue<T>
    {
        /// <summary>
        /// Push message onto the message queue
        /// </summary>
        /// <param name="t">The message</param>
        void Enqueue(T t);

        /// <summary>
        /// Pull a message off of the message queue. This method will block until there is a message available
        /// </summary>
        /// <returns>The message</returns>
        T Dequeue();
    }
}
