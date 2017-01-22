using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalkaRozproszona.Classes
{
    class ConcreteListConnectedClients : ListObservator
    {
        public ConcreteListConnectedClients(ListView list) : base(list)
        {
        }

        protected override void ListUpdate(string message)
        {
            // TODO naprawic
            if (message.StartsWith("#"))
            {
                listView.Items.Add(message);
                listView.EnsureVisible(listView.Items.Count - 1);
            }
        }
    }
}
