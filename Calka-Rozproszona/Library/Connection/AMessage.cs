using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Library
{
    public abstract class AMessage
    {
        protected object locker = new object();
        protected string message;

        public string Message
        {
            get { return message; }
        }

        protected virtual void SetMessage(string messag)
        {
            lock (locker)
            {
                string newMessage = Thread.CurrentThread.ManagedThreadId + "@" + DateTime.Now.ToString("HH:mm:ss.ffff") + "@" + messag;
                message = newMessage;
            }
        }
    }
}
