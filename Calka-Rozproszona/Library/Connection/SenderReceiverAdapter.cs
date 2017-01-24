using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Library
{
    class SenderReceiverAdapter : ICommand
    {
        /// <exception cref="Exception"></exception>
        public string ReceiveCommand(object client)
        {
            NetworkStream clientStream = client as NetworkStream;
            try
            {
                if (!clientStream.CanRead)
                    return null;

                byte[] data = new byte[Configuration.NUMBER_OF_BYTES];
                clientStream.Read(data, 0, data.Length);

                return System.Text.Encoding.ASCII.GetString(data, 0, data.Length);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <exception cref="Exception"></exception>
        public string SendCommand(object streamToSend, CommandType type, params object[] parameters)
        {
            NetworkStream stream = streamToSend as NetworkStream;

            string parametersToSend = "";
            foreach (var parameter in parameters)
                parametersToSend += parameter.ToString() + Configuration.PACKETS_DATA_SEPARATOR;

            parametersToSend = parametersToSend.Substring(0, parametersToSend.Length - 1);
            string message = (int)type + Configuration.COMMAND_SEPARATOR + parametersToSend;

            try
            {
                byte[] data = new byte[Configuration.NUMBER_OF_BYTES];
                System.Text.Encoding.ASCII.GetBytes(message).CopyTo(data, 0);

                while (!stream.CanWrite)
                { }

                stream.Write(data, 0, data.Length);
                stream.Flush();
                return message;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DecomposeData(string receivedData, out CommandType type, out string[] data)
        {
            char separator = char.Parse(Configuration.COMMAND_SEPARATOR);
            string[] allData = receivedData.Split(separator);
            CommandType commandType = (CommandType)int.Parse(allData[0]);

            string[] onlyData = new string[allData.Length - 1];
            for (int i = 1; i < allData.Length; i++)
                onlyData[i-1] = allData[i];

            data = onlyData;
            type = commandType;
        }
    }
}
