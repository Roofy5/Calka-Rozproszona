using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Library
{
    public class ConnectedClient
    {
        public TcpClient Client
        {
            get; set;
        }
        public int DeclaredThreads
        {
            get; set;
        }
        public double ReturnedResult
        {
            get; set;
        }

        public ConnectedClient(TcpClient client, int declaredThreads = 0, double returnedResult = 0)
        {
            Client = client;
            DeclaredThreads = declaredThreads;
            ReturnedResult = returnedResult;
        }
    }
}
