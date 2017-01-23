using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalkaRozproszona.Classes
{
    class CosFunction : IFunction
    {
        public double function(double x)
        {
            return Math.Cos(x);
        }

        public override string ToString()
        {
            return "Cos(x)";
        }
    }
}
