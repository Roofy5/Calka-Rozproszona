using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Library;
using System.Windows.Forms;

namespace CalkaRozproszona.Classes
{
    class ConcreteResultObservator : IObserver
    {
        private TextBox resultBox;

        public ConcreteResultObservator(TextBox resultBox)
        {
            this.resultBox = resultBox;
        }

        public void Update(object ob)
        {
            MathematicalCalculations math = ob as MathematicalCalculations;
            resultBox.Text = math.Result.ToString();
        }
    }
}
