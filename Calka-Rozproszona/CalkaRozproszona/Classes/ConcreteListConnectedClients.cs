using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Library;

namespace CalkaRozproszona.Classes
{
    class ConcreteListConnectedClients : ListObservator
    {
        public ConcreteListConnectedClients(ListView list) : base(list)
        {
        }

        protected override void ListUpdate(object server)
        {
            try
            {
                Server serv = server as Server;
                listView.Items.Clear();
                foreach (var client in serv.Clients)
                    listView.Items.Add(client.Client.Client.RemoteEndPoint.ToString());
            }
            catch (Exception)
            { }
        }
    }
}
