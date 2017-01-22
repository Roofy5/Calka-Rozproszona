using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalkaRozproszona
{
    public partial class SelectMenu : Form
    {
        public SelectMenu()
        {
            InitializeComponent();
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            ServerApplication form = new ServerApplication();
            this.Hide();
            form.ShowDialog();
            this.Close();
        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            ClientApplication form = new ClientApplication();
            this.Hide();
            form.ShowDialog();
            this.Close();
        }
    }
}
