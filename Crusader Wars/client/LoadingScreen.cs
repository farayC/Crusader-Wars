using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crusader_Wars
{
    public partial class LoadingScreen : Form
    {
        public LoadingScreen()
        {
            InitializeComponent();

            //Icon
            this.Icon = Properties.Resources.logo;

            //Top-Must


        }

        public void ChangeMessage(string message)
        {
            Label_Message.Text = message;
        }

        private void LoadingScreen_Shown(object sender, EventArgs e)
        {
            this.TopLevel = true;
        }

    }
}

