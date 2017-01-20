using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using Library;
using System.Net;
using CalkaRozproszona.Classes;

namespace CalkaRozproszona
{
    public partial class ServerApplication : Form
    {
        Server server;
        ListObservator listObservator;

        public ServerApplication()
        {
            InitializeComponent();
            PoolOfThreads.Instance.MaxThreads = 10;
           
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            IPAddress address;
            try
            {
                address = IPAddress.Parse(txtAddress.Text);
            }
            catch (Exception)
            {
                AddInformation("Podaj poprawny adres.");
                return;
            }

            int port = 0;
            try
            {
                port = (int)numericPort.Value;
            }
            catch (Exception)
            {
                AddInformation("Podaj poprawny port.");
                return;
            }

            server = new Server(address, port);

            listObservator = new ListObservator(listOfInformations);
            server.AddObserver(listObservator);

            server.StartServer();
        }

        private void AddInformation(string message)
        {
            string currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
            ListViewItem item = new ListViewItem(new[] {currentThread, DateTime.Now.ToString("HH:mm:ss.ffff"), message });
            listOfInformations.Items.Add(item);
            listOfInformations.EnsureVisible(listOfInformations.Items.Count - 1);
        }
    }
}
