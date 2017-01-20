using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Library
{
    public class Server : IObservable
    {
        private object locker = new object();
        private IPAddress host;
        private int port;
        private List<TcpClient> clients;
        private TcpListener server;
        private bool working;
        private List<IObserver> observers;
        private string message;

        public IPAddress Host
        {
            get { return host; }
        }
        public int Port
        {
            get { return port; }
        }
        public List<TcpClient> Clients
        {
            get { return clients; }
        }
        public TcpListener ThisServer
        {
            get { return server; }
        }
        public string Message
        {
            get { return message; }
        }


        public Server(IPAddress host, int port)
        {
            this.host = host;
            this.port = port;
            clients = new List<TcpClient>();
            working = true;
            observers = new List<IObserver>();
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
                observer.Update(this);
        }

        /// <exception cref="Exception"></exception>
        public void StartServer()
        {
            try
            {
                working = true;
                Thread serverThread = PoolOfThreads.Instance.NewThread(Start);
                serverThread.Start();
            }
            catch (Exception exc)
            {
                SetMessage("!!! Brak dostepnych watkow dla serwera: " + exc.Message);
                //throw;
            }
        }

        public void StopServer()
        {
            try
            {
                working = false;
                server.Stop();
                // TODO zwolnic watki
            }
            catch (Exception exc)
            {
                SetMessage("!!! Nie moge zatrzymac serwera: " + exc.Message);
            }
        }

        private void Start(object ob)
        {
            try
            {
                server = new TcpListener(host, port);
                server.Start();
                SetMessage("Server uruchomiony");
                PoolOfThreads.Instance.NewThread(Listen).Start();
            }
            catch (Exception exc)
            {
                SetMessage("!!! Nie moge uruchomic serwera: " + exc.Message);
                //throw;
            }
        }

        private void Listen(object ob)
        {
            try
            {
                while (working)
                {
                    SetMessage("Czekam na nowego klienta...");
                    TcpClient client = server.AcceptTcpClient();
                    SetMessage("Nowy klient probuje sie polaczyc...");
                    PoolOfThreads.Instance.NewThread(ClientService).Start(client);
                    clients.Add(client);
                }
            }
            catch (Exception exc)
            {
                SetMessage("!!! Nie moge utworzyc watku klienta: " + exc.Message);
                //throw;
            }
        }

        private void ClientService(object _client)
        {
            TcpClient client = (TcpClient)_client;
            SetMessage("Dodano klienta: " + client.Client.RemoteEndPoint.ToString());

            while (true) { }
            // Operacje na kliencie
        }

        private void SetMessage(string messag)
        {
            lock (locker)
            {
                //string newMessage = "[" + Thread.CurrentThread.ManagedThreadId + "] " + messag;
                //message = newMessage;

                string newMessage = Thread.CurrentThread.ManagedThreadId + "@" + DateTime.Now.ToString("HH:mm:ss.ffff") + "@" + messag;
                message = newMessage;

                UpdateObservers();
            }
        }
    }
}
