using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Library;

namespace CalkaRozproszona.Classes
{
    class ConcreteListServerInformations : ListObservator
    {
        public ConcreteListServerInformations(ListView list) : base(list)
        {}

        protected override void ListUpdate(object message)
        {
            try
            {
                string _message = (message as AMessage).Message;
                ListViewItem item = new ListViewItem(_message.Split('@'));
                listView.Items.Add(item);
                listView.EnsureVisible(listView.Items.Count - 1);
            }
            catch (Exception)
            { }
        }
    }
}
