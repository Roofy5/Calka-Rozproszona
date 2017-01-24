using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Library;

namespace CalkaRozproszona.Classes
{
    abstract class ListObservator : Library.IObserver
    {
        protected string lastMessage;
        protected ListView listView;
        protected abstract void ListUpdate(object server);

        public ListObservator(ListView list)
        {
            listView = list;
            lastMessage = "";
        }

        public void Update(object ob)
        {
            var received = ob as AMessage;
            //string message = server.Message;
            /*listView.Invoke(new Action(
                delegate ()
                {
                //listView.Items.Add(message);
                    ListViewItem item = new ListViewItem(message.Split('@'));
                    listView.Items.Add(item);
                    listView.EnsureVisible(listView.Items.Count - 1);
                }));*/
            Action<AMessage> function = ListUpdate;
            listView.Invoke(function, new AMessage[]{ received });
        }
    }
}
