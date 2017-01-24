using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public interface ICommand
    {
        string SendCommand(object streamToSend, CommandType type, params object[] parameters);
        string ReceiveCommand(object client);
    }
}
