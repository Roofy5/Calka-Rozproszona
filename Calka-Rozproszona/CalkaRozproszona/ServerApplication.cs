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
        ListObservator connectedClientsObservator;

        double lowerBound = 0;
        double upperBound = 0;
        double accuracy = 0.001;

        public ServerApplication()
        {
            InitializeComponent();
            SetUpMathematicalFunctions();
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

            int maxKlientow = 10;
            try
            {
                maxKlientow = (int)numericClients.Value;
            }
            catch (Exception)
            {
                AddInformation("Podaj poprawna ilosc klientow.");
                return;
            }

            PoolOfThreads.Instance.MaxThreads = maxKlientow + 1;

            server = new Server(address, port);

            SetUpObservers();
           
            server.StartServer();
        }

        private void AddInformation(string message)
        {
            string currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
            ListViewItem item = new ListViewItem(new[] {currentThread, DateTime.Now.ToString("HH:mm:ss.ffff"), message });
            listOfInformations.Items.Add(item);
            listOfInformations.EnsureVisible(listOfInformations.Items.Count - 1);
        }

        private void SetUpObservers()
        {
            listObservator = new ConcreteListServerInformations(listOfInformations);
            connectedClientsObservator = new ConcreteListConnectedClients(listConnectedClients);

            server.AddObserver(listObservator);
            server.AddObserver(connectedClientsObservator);
        }

        private void SetUpMathematicalFunctions()
        {
            comboFunction.Items.Add(new SinFunction());
            comboFunction.Items.Add(new CosFunction());
        }

        private void btnStartCalculations_Click(object sender, EventArgs e)
        {
            try
            {
                upperBound = double.Parse(txtTo.Text, System.Globalization.CultureInfo.InvariantCulture);
                lowerBound = double.Parse(txtFrom.Text, System.Globalization.CultureInfo.InvariantCulture);
                accuracy = double.Parse(txtAccuracy.Text, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception exc)
            {
                AddInformation(exc.Message);
                return;
            }
        }

        private void ServerApplication_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*if (server.Clients != null && server.Clients.Count != 0)
            {
                foreach (var client in server.Clients)
                {
                    client.Client.Close();
                }
            }

            PoolOfThreads.Instance.ExitThreads();*/
        }
    }
}
