using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class MathematicalCalculations : IObservable
    {
        public double LowerBound { get; set; }
        public double UpperBound { get; set; }
        public double Accuracy { get; set; }
        public double Result { get; set; }
        public int NumberOfThreads { get; set; }
        public IFunction Function { get; set; }
        private List<IObserver> observers;

        public MathematicalCalculations()
        {
            LowerBound = 0;
            UpperBound = 0;
            Accuracy = 0.001;
            Result = 0;
            NumberOfThreads = 1;
            Function = null;
            observers = new List<IObserver>();
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

            Result = wyniki.Sum();
            return Result;
        }

        private double Trapez(double a, double b, double h)
        {
            return (a + b) * h / 2.0;
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
            foreach (var ob in observers)
                ob.Update(this);
        }
    }
}
