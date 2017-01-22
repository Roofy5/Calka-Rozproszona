using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalkaRozproszona.Classes
{
    class ConcreteListServerInformations : ListObservator
    {
        public ConcreteListServerInformations(ListView list) : base(list)
        {}

        protected override void ListUpdate(string message)
        {
            ListViewItem item = new ListViewItem(message.Split('@'));
            listView.Items.Add(item);
            listView.EnsureVisible(listView.Items.Count - 1);
        }
    }
}
