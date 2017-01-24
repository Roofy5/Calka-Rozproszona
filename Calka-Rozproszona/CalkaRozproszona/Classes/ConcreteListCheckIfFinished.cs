using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Library;

namespace CalkaRozproszona.Classes
{
    class ConcreteListCheckIfFinished : ListObservator
    {
        public ConcreteListCheckIfFinished(ListView list) : base(list)
        {
        }

        protected override void ListUpdate(object server)
        {
            try
            {
                Server serv = server as Server;
                int numberOfFinished = 0;
                double result = 0;

                foreach (var client in serv.Clients)
                {
                    if (client.Finished)
                        numberOfFinished++;
                    else
                        continue;
                    result += client.ReturnedResult;
                }

                if (numberOfFinished == serv.Clients.Count)
                {
                    listView.Items.Clear();
                    listView.Items.Add(result.ToString());
                }

            }
            catch (Exception)
            { }
        }
    }
}
