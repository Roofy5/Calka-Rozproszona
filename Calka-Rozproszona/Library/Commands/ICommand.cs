using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public interface ICommand
    {
        void SendCommand(CommandType type, params object[] parameters);
        void ReceiveCommand(object client);
    }
}
