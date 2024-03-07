using System;
using System.Collections.Generic;
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

            // Set form properties
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            // Set PictureBox properties
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            // Load background image
            pictureBox1.Image = Properties.Resources.loadingscreen1;

        }


        public void ChangeMessage(string message)
        {
            Label_Message.Text = message;
        }

    }
}

