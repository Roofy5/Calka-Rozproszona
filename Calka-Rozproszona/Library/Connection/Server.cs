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
    public class Server : AMessage, IObservable, ICommand
    {
        private IPAddress host;
        private int port;
        private List<ConnectedClient> clients;
        private TcpListener server;
        private bool working;
        private List<IObserver> observers;

        public IPAddress Host
        {
            get { return host; }
        }
        public int Port
        {
            get { return port; }
        }
        public List<ConnectedClient> Clients
        {
            get { return clients; }
        }
        public TcpListener ThisServer
        {
            get { return server; }
        }


        public Server(IPAddress host, int port)
        {
            this.host = host;
            this.port = port;
            clients = new List<ConnectedClient>();
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

        public void SendCommand(CommandType type, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void ReceiveCommand(object client)
        {
            TcpClient _client = client as TcpClient;
            try
            {
                using (NetworkStream stream = _client.GetStream())
                {
                    while (!stream.DataAvailable)
                        continue;
                    byte[] data = new byte[Configuration.NUMBER_OF_BYTES];
                    stream.Read(data, 0, data.Length);

                    string receivedMessage = System.Text.Encoding.ASCII.GetString(data, 0, data.Length);

                    char separator = char.Parse(Configuration.COMMAND_SEPARATOR);
                    string[] dane = receivedMessage.Split(separator);
                    CommandType type = (CommandType)int.Parse(dane[0]);

                    ExecuteOperation(type, receivedMessage, _client);

                    SetMessage("Odczytalem dane: " + type.ToString());
                }
            }
            catch (Exception exc)
            {
                SetMessage("!!! Błąd przy odczycie: " + exc.Message);
            }
        }

        private void ExecuteOperation(CommandType type, string receivedMessage, TcpClient _client)
        {
            switch (type)
            {
                case CommandType.READY:
                    {
                        int declaredThreads = int.Parse(receivedMessage.Split(char.Parse(Configuration.COMMAND_SEPARATOR))[1]);
                        clients.Select(c => c).Where(c => c.Client == _client).ToArray()[0].DeclaredThreads = declaredThreads;
                        break;
                    }
            }
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
            TcpClient client = null;
            try
            {
                while (working)
                {
                    /*if (PoolOfThreads.Instance.AvailableThreads() <= 0)
                    {
                        working = false;
                        break;
                    }*/
                    SetMessage("Czekam na nowego klienta...");
                    client = server.AcceptTcpClient();
                    SetMessage("Nowy klient probuje sie polaczyc...");
                    PoolOfThreads.Instance.NewThread(ClientService).Start(client);
                    clients.Add(new ConnectedClient(client));
                }
            }
            catch (Exception exc)
            {
                SetMessage("!!! Nie moge utworzyc watku klienta: " + exc.Message);
                if (client != null)
                    client.Close();
                //throw;
            }
            finally
            {
                server.Server.Close();
                SetMessage("Osiągnięto max ilość klientów");
            }
        }

        private void ClientService(object _client)
        {
            TcpClient client = (TcpClient)_client;
            SetMessage("Dodano klienta: " + client.Client.RemoteEndPoint.ToString());


            while (true) { }
           // ReceiveCommand(_client);
            // Operacje na kliencie
            //TODO
        }

        protected override void SetMessage(string messag)
        {
            base.SetMessage(messag);
            UpdateObservers();
        }
    }
}
