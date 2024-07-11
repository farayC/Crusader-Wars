using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Crusader_Wars
{
    public partial class LoadingScreen : Form
    {
        public LoadingScreen()
        {
            InitializeComponent();

            //Icon
            this.Icon = Properties.Resources.logo;
        }

        public void ChangeMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(ChangeMessage), new object[] { message });
            }
            else
            {
                Label_Message.Text = message;
            }
        }

        private void LoadingScreen_Shown(object sender, EventArgs e)
        {
            this.TopMost = false;
        }

    }
}

