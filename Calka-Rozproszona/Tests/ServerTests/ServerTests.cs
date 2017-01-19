using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Library.Connection;
using System.Net;

using Tests.SupportClasses;
using Library;

namespace Tests.ServerTests
{
    [TestClass]
    public class ServerTests
    {
        [TestMethod]
        public void CreateServer()
        {
            PoolOfThreads.Instance.MaxThreads = 5;

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 12500;

            Server server = new Server(ip, port);

            MockObserver observer = new MockObserver();
            server.AddObserver(observer);

            server.StartServer();
            //server.StopServer();

            string message = "";
            while (true)
            {
                string newMessage = observer.Message;
                if (string.Compare(newMessage, message) != 0)
                {
                    message = newMessage;
                }
            }
        }
    }
}
