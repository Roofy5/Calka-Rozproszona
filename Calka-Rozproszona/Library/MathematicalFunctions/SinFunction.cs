using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class SinFunction : IFunction
    {
        public double function(double x)
        {
            return Math.Sin(x);
        }

        public override string ToString()
        {
            return "Sin(x)";
        }
    }
}
