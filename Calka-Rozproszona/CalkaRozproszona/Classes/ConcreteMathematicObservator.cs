using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using Library;

namespace CalkaRozproszona.Classes
{
    class ConcreteMathematicObservator : IObserver
    {
        ComboBox function;
        TextBox lowerBound;
        TextBox upperBound;
        TextBox accuracy;

        public ConcreteMathematicObservator(ComboBox function, TextBox lowerBound, TextBox upperBound, TextBox accuracy)
        {
            this.function = function;
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
            this.accuracy = accuracy;
        }

        public void Update(object ob)
        {
            MathematicalCalculations math = ob as MathematicalCalculations;
            function.Text = math.Function.ToString();
            lowerBound.Text = math.LowerBound.ToString();
            upperBound.Text = math.UpperBound.ToString();
            accuracy.Text = math.Accuracy.ToString();
        }
    }
}
