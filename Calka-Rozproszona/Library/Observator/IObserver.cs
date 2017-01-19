using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Observator
{
    public interface IObserver
    {
        void Update(object ob);
    }
}
