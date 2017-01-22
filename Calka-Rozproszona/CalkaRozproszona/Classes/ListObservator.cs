using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalkaRozproszona.Classes
{
    abstract class ListObservator : Library.IObserver
    {
        protected ListView listView;
        protected abstract void ListUpdate(string message);

        public ListObservator(ListView list)
        {
            listView = list;
        }

        public void Update(object ob)
        {
            string message = ob as string;
            /*listView.Invoke(new Action(
                delegate ()
                {
                //listView.Items.Add(message);
                    ListViewItem item = new ListViewItem(message.Split('@'));
                    listView.Items.Add(item);
                    listView.EnsureVisible(listView.Items.Count - 1);
                }));*/
            Action<string> function = ListUpdate;
            listView.Invoke(function, new string[]{ message });
        }
    }
}
