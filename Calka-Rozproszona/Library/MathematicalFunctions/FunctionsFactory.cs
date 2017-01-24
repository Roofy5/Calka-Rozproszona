using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public abstract class FunctionsFactory
    {
        public static IFunction GetFunction(string function)
        {
            List<IFunction> functions = new List<IFunction>()
            {
                new SinFunction(),
                new CosFunction()
            };

            return functions.Where(fun => function.Split('\0')[0].Equals(fun.ToString())).ToArray()[0]; 
        }
    }
}
