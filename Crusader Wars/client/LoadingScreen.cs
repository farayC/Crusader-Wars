using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Crusader_Wars
{
    public partial class LoadingScreen : Form
    {

        public LoadingScreen()
        {
            InitializeComponent();
        }

        private void Btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

