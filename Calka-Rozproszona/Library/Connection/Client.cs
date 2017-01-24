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
        private NetworkStream stream;
        private SenderReceiverAdapter senderReceiver;

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
                string message = senderReceiver.SendCommand(stream, type, parameters);
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

                //ExecuteOperation(type, receivedMessage, _client); 
                SetMessage("Odczytalem dane: " + receivedData);
                return receivedData;
            }
            catch (Exception exc)
            {
                SetMessage("!!! Błąd przy odczycie: " + exc.Message);
                throw new Exception("Prawdopodobnie serewer zakończył działanie");
            }
        }

        private void ExecuteOperation(CommandType type, string [] receivedValues, TcpClient _client)
        {
            switch (type)
            {
                case CommandType.READY:
                    {
                        break;
                    }
                case CommandType.STOP:
                    {
                        Finish();
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

        public void Finish()
        {
            SetMessage("Kończę pracę");
            observers.Clear();
            working = false;
            PoolOfThreads.Instance.ExitThreads();
            client.Close();
            stream.Close();
            //PoolOfThreads.Instance.ExitThreads();
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
                    string data = ReceiveCommand(stream);
                    if (data == null)
                        continue;

                    CommandType command;
                    string[] values;
                    senderReceiver.DecomposeData(data, out command, out values);

                    ExecuteOperation(command, values, client);
                }
            }
            catch (Exception exc)
            {
                SetMessage("!!! Błąd przy nasłuchu: " + exc.Message);
            }
        }

    }
}
