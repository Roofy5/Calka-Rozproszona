using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Library
{
    public class Client : AMessage, IObservable
    {
        private TcpClient client;
        private IPAddress serverAddress;
        private int serverPort;
        private List<IObserver> observers;
        bool working;

        public Client(IPAddress address, int port)
        {
            serverAddress = address;
            serverPort = port;
            observers = new List<IObserver>();
            working = true;
        }

        public void AddObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        public void RemoveObserver(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void UpdateObservers()
        {
            foreach (var observer in observers)
                observer.Update(this.message);
        }

        public void Connect()
        {
            try
            {
                client = new TcpClient();
                client.Connect(serverAddress, serverPort);
                SetMessage("Połączyłem się z serwerem");
                PoolOfThreads.Instance.NewThread(Listen).Start();
            }
            catch (Exception exc)
            {
                SetMessage("!!! Nie moge sie połączyć lub brak wolych wątków: " + exc.Message);
            }
        }

        protected override void SetMessage(string messag)
        {
            base.SetMessage(messag);
            UpdateObservers();
        }

        private void Listen(object ob)
        {
            try
            {
                SetMessage("Nasłuchuję...");
                StreamReader reader = new StreamReader(client.GetStream());
                string serverMessage = "";
                while (!serverMessage.Equals("EXIT") && working)
                {
                    string newServerMessage = reader.ReadLine();
                    if (!newServerMessage.Equals(serverMessage))
                        SetMessage(newServerMessage);
                    serverMessage = newServerMessage;
                }
            }
            catch (Exception exc)
            {
                SetMessage("!!! Błąd przy nasłuchu: " + exc.Message);
            }
        }
    }
}
