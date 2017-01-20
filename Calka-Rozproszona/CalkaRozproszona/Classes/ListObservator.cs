using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalkaRozproszona.Classes
{
    class ListObservator : Library.IObserver
    {
        private ListView listView;

        public void Update(object ob)
        {
            string message = (ob as Library.Server).Message;
            listView.Invoke(new Action(
                delegate ()
                {
                //listView.Items.Add(message);
                    ListViewItem item = new ListViewItem(message.Split('@'));
                    listView.Items.Add(item);
                    listView.EnsureVisible(listView.Items.Count - 1);
                }));
        }

        public ListObservator(ListView list)
        {
            listView = list;
        }
    }
}
