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
                //foreach (var client in serv.Clients)
                //    listView.Items.Add(client.Client.Client.RemoteEndPoint.ToString());

                for (int i = 0; i < serv.Clients.Count; i++)
                {
                    if (serv.Clients[i].Client.Connected)
                    {
                        listView.Items.Add(serv.Clients[i].Client.Client.RemoteEndPoint.ToString() + " {" + serv.Clients[i].DeclaredThreads + "}");
                        if (serv.Clients[i].DeclaredThreads != 0)
                            listView.Items[listView.Items.Count - 1].BackColor = System.Drawing.Color.Green;
                    }
                }
            }
            catch (Exception)
            { }
        }
    }
}
