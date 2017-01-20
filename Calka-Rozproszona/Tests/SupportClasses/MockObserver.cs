using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Library;

namespace Tests.SupportClasses
{
    class MockObserver : IObserver
    {
        private string message;
        public string Message
        {
            get { return message; }
        }

        public void Update(object ob)
        {
            Server server = (Server)ob;
            message = server.Message;
        }
    }
}
