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
    public class Client : AMessage, IObservable, ICommand
    {
        private TcpClient client;
        private IPAddress serverAddress;
        private int serverPort;
        private List<IObserver> observers;
        private bool working;
        private int declaredThreads = 1;
        NetworkStream stream;

        public TcpClient Server
        {
            get { return client; }
        }

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
                observer.Update(this);
        }

        public void SendCommand(CommandType type, params object[] parameters)
        {
            string parametersToSend = "";
            foreach (var parameter in parameters)
                parametersToSend += parameter.ToString() + Configuration.PACKETS_DATA_SEPARATOR;

            parametersToSend = parametersToSend.Substring(0, parametersToSend.Length - 1);
            string message = (int)type + Configuration.COMMAND_SEPARATOR + parametersToSend;

            try
            {
                byte[] data = new byte[Configuration.NUMBER_OF_BYTES];
                System.Text.Encoding.ASCII.GetBytes(message).CopyTo(data, 0);

                //using (NetworkStream stream = client.GetStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
                
            }
            catch (Exception exc)
            {
                SetMessage("!!! Błąd wysyłania: " + exc.Message);
                return;
            }
        }

        public void ReceiveCommand(object client)
        {
            TcpClient _client = client as TcpClient;
            try
            {
                //using (NetworkStream stream = _client.GetStream())
                {
                    if (!stream.DataAvailable)
                        return;
                    byte[] data = new byte[Configuration.NUMBER_OF_BYTES];
                    stream.Read(data, 0, data.Length);

                    string receivedMessage = System.Text.Encoding.ASCII.GetString(data, 0, data.Length);

                    char separator = char.Parse(Configuration.COMMAND_SEPARATOR);
                    string[] dane = receivedMessage.Split(separator);
                    CommandType type = (CommandType)int.Parse(dane[0]);

                    ExecuteOperation(type, receivedMessage, _client);
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
                        break;
                    }
            }
        }

        public void Connect()
        {
            try
            {
                client = new TcpClient();
                client.Connect(serverAddress, serverPort);
                if (!client.Connected)
                    throw new Exception("Nie ma dla mnie miejsca");
                SetMessage("Połączyłem się z serwerem");
                stream = client.GetStream();
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
                while (working)
                {
                    //ReceiveCommand(client);
                    //TODO
                }
                /*StreamReader reader = new StreamReader(client.GetStream());
                string serverMessage = "";
                while (!serverMessage.Equals("EXIT") && working)
                {
                    string newServerMessage = reader.ReadLine();
                    if (!newServerMessage.Equals(serverMessage))
                        SetMessage(newServerMessage);
                    serverMessage = newServerMessage;
                }
                SetMessage("Serwer zakończył transmisję lub klient zakończył nasłuchiwanie.");*/
            }
            catch (Exception exc)
            {
                SetMessage("!!! Błąd przy nasłuchu: " + exc.Message);
            }
        }

    }
}
