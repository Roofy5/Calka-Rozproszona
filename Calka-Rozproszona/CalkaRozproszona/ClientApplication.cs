﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Library;
using System.Net;
using CalkaRozproszona.Classes;

namespace CalkaRozproszona
{
    public partial class ClientApplication : Form
    {
        Client client;
        IObserver listObservator;
        IObserver mathObservator;
        IObserver resultObservator;
        int numberOfSharedThreads = 2;

        public ClientApplication()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
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

            try
            {
                numberOfSharedThreads = (int)numericThreads.Value;
            }
            catch (Exception)
            {
                AddInformation("Podaj poprawną ilość wątków.");
                return;
            }

            PoolOfThreads.Instance.MaxThreads = numberOfSharedThreads+1;

            client = new Client(address, port);
            
            listObservator = new ConcreteListServerInformations(listOfInformations);
            mathObservator = new ConcreteMathematicObservator(comboFunction, txtFrom, txtTo, txtAccuracy);
            resultObservator = new ConcreteResultObservator(txtResult);
            
            client.AddObserver(listObservator);
            client.Mathematic.AddObserver(mathObservator);
            client.Mathematic.AddObserver(resultObservator);

            client.Connect();

            if (client.Server.Connected)
            {
                //btnConnect.Enabled = false;
                groupBox1.Enabled = false;
                button1.Enabled = true;
            }
        }

        private void AddInformation(string message)
        {
            string currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
            ListViewItem item = new ListViewItem(new[] { currentThread, DateTime.Now.ToString("HH:mm:ss.ffff"), message });
            listOfInformations.Items.Add(item);
            listOfInformations.EnsureVisible(listOfInformations.Items.Count - 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.DeclaredThreads = numberOfSharedThreads;
            client.SendCommand(null, Library.CommandType.READY, numberOfSharedThreads);
            button1.Enabled = false;
        }

        private void ClientApplication_FormClosing(object sender, FormClosingEventArgs e)
        {
            //client.Server.Close();
            if (client != null)
                client.Finish();
        }
    }
}
