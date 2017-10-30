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
    /// <typeparam name="T"></typeparam>
    public interface IMessageQueue<T>
    {
        void Enqueue(T t);

        T Dequeue();
    }
}
