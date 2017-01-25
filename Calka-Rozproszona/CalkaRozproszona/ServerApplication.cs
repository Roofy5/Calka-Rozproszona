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
        IObserver listObservator;
        IObserver connectedClientsObservator;
        IObserver resultObservator;

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

            btnStart.Enabled = false;

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
            resultObservator = new ConcreteListCheckIfFinished(listResult);

            server.AddObserver(listObservator);
            server.AddObserver(connectedClientsObservator);
            server.AddObserver(resultObservator);
        }

        private void SetUpMathematicalFunctions()
        {
            comboFunction.Items.Add(new SinFunction());
            comboFunction.Items.Add(new CosFunction());
            comboFunction.SelectedItem = comboFunction.Items[0];
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

            SendSettingsToClients();
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

            if(server!=null)
                server.StopServer();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            server.StopServer();
            server = null;
        }

        private void SendSettingsToClients()
        {
            // TODO
            //System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            //timer.Start();
            RemoveNotConnectedClients();

            int totalNumberOfThreads = server.TotalNumberOfThreads();

            double section = (upperBound - lowerBound) / totalNumberOfThreads;

            txtNumberOfThreads.Text = totalNumberOfThreads.ToString();
            txtSection.Text = section.ToString();

            double clientLowerBound = lowerBound;
            double clientUpperBound = lowerBound;

            foreach (var client in server.Clients)
            {
                clientUpperBound += section * client.DeclaredThreads;
                server.SendCommand(client.Stream, Library.CommandType.FUNCTION, comboFunction.SelectedItem.ToString()); // new SinFunction()
                server.SendCommand(client.Stream, Library.CommandType.SECTION, clientLowerBound, clientUpperBound, accuracy);
                clientLowerBound = clientUpperBound;
            }
        }

        private void RemoveNotConnectedClients()
        {
            server.Clients.RemoveAll(client => client.Client.Connected == false || client.DeclaredThreads == 0);
        }
    }
}
