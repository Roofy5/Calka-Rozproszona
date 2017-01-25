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
        private SenderReceiverAdapter senderReceiver;

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
            senderReceiver = new SenderReceiverAdapter();
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

        public string SendCommand(object streamToSend, CommandType type, params object[] parameters)
        {
            try
            {
                string message = senderReceiver.SendCommand(streamToSend, type, parameters);
                SetMessage("Wysałełm dane: " + message);
            }
            catch (Exception exc)
            {
                SetMessage("!!! Błąd wysyłania: " + exc.Message);
            }

            return "";
        }

        public string ReceiveCommand(object client)
        {
            try
            {
                string receivedData = senderReceiver.ReceiveCommand(client);

                if (receivedData == null)
                    return null;

                SetMessage("Odczytalem dane: " + receivedData);
                return receivedData;
            }
            catch (Exception exc)
            {
                SetMessage("!!! Błąd przy odczycie: " + exc.Message);
                throw new Exception("Prawdopodobnie klient zakończył działanie");
            }
        }

        private void ExecuteOperation(CommandType type, string [] receivedValues, TcpClient _client)
        {
            switch (type)
            {
                case CommandType.READY:
                    {
                        int declaredThreads = int.Parse(receivedValues[0]);
                        clients.Select(c => c).Where(c => c.Client == _client).ToArray()[0].DeclaredThreads = declaredThreads;
                        break;
                    }
                case CommandType.RESULT:
                    {
                        double result = double.Parse(receivedValues[0]);
                        clients.Select(c => c).Where(c => c.Client == _client).ToArray()[0].ReturnedResult = result;
                        clients.Select(c => c).Where(c => c.Client == _client).ToArray()[0].Finished = true;
                        break;
                    }
            }
            UpdateObservers(); // <<<<<<<<========
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
                SetMessage("Koncze prace");
                observers.Clear();
                working = false;

                foreach (var client in clients)
                {
                    if (client.Client.Connected)
                    {
                        senderReceiver.SendCommand(client.Stream, CommandType.STOP, "Koniec pracy");
                        client.Stream.Close();
                        client.Client.Close();
                    }
                }

                server.Stop();

                //server.Stop();
                // TODO zwolnic watki

                PoolOfThreads.Instance.ExitThreads();
                
                //FreeClients();
            }
            catch (Exception exc)
            {
                SetMessage("!!! Nie moge zatrzymac serwera: " + exc.Message);
            }
        }

        public int TotalNumberOfThreads()
        {
            int sum = 0;
            foreach (var client in clients)
                sum += client.DeclaredThreads;
            return sum;
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
                    ConnectedClient newClient = new ConnectedClient(client);
                    clients.Add(newClient);
                    PoolOfThreads.Instance.NewThread(ClientService).Start(newClient);

                    //PoolOfThreads.Instance.NewThread(ClientService).Start(client);
                    //clients.Add(new ConnectedClient(client));
                }
            }
            catch (Exception exc)
            {
                SetMessage("!!! Nie moge utworzyc watku klienta: " + exc.Message);
                if (client != null)
                    client.Close();
            }
            finally
            {
                //server.Server.Close();
                SetMessage("Osiągnięto max ilość klientów");
                //FreeClients();
            }
        }

        private void ClientService(object _client)
        {
            ConnectedClient connectedClient = (ConnectedClient)_client;
            TcpClient client = connectedClient.Client;
            connectedClient.Stream = connectedClient.Client.GetStream();

            SetMessage("Dodano klienta: " + client.Client.RemoteEndPoint.ToString());

            try
            {
                //using (NetworkStream stream = client.GetStream())
                //using (NetworkStream stream = clients.Where(c => c.Client == client).ToArray()[0].Stream = client.GetStream())
                {
                    while (working)
                    {
                        string data = ReceiveCommand(connectedClient.Stream);
                        if (data == null)
                            continue;

                        CommandType command;
                        string[] values;
                        senderReceiver.DecomposeData(data, out command, out values);

                        ExecuteOperation(command, values, client);
                    }
                }
            }
            catch (Exception exc)
            {
                SetMessage("!!! Błąd przy obsłudze klienta: " + exc.Message);
            }
            finally
            {
                client.Close();
            }
            // Operacje na kliencie
        }

        protected override void SetMessage(string messag)
        {
            base.SetMessage(messag);
            UpdateObservers();
        }
    }
}
