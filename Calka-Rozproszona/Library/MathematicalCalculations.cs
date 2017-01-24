using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class MathematicalCalculations
    {
        public double LowerBound { get; set; }
        public double UpperBound { get; set; }
        public double Accuracy { get; set; }
        public int NumberOfThreads { get; set; }
        public IFunction Function { get; set; }

        public MathematicalCalculations()
        {
            LowerBound = 0;
            UpperBound = 0;
            Accuracy = 0.001;
            NumberOfThreads = 1;
            Function = null;
        }

        public double Calculate()
        {
            double[] wyniki = new double[NumberOfThreads];
            double przedzial = (UpperBound - LowerBound) / NumberOfThreads;

            Parallel.For(0, NumberOfThreads, watek =>
            {
                for (double x = LowerBound + watek * przedzial; x < LowerBound + (watek + 1) * przedzial; x += Accuracy)
                    wyniki[watek] += Trapez(Function.function(x), Function.function(x + Accuracy), Accuracy);
            });

            return wyniki.Sum();
        }

        private double Trapez(double a, double b, double h)
        {
            return (a + b) * h / 2.0;
        }
    }
}
