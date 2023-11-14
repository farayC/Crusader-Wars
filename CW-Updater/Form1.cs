using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace CW_Updater
{
    public partial class AutoUpdater : Form
    {
        private static string AppVersion { get; set; }
        private static string ModVersion { get; set; }

        public AutoUpdater()
        {
            InitializeComponent();

            System.Threading.Thread.Sleep(1000);
            GetVersions();

            if(AppVersion == ModVersion) 
            {
                Process.Start(@".\Crusader Wars.exe");
                this.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                UpdaterData.UpdateApp(this, ModVersion);
                btnUpdate.Enabled = false;
                btnUpdate.Text = "Updating..";
            }
            catch 
            {
                Process.Start(@".\Crusader Wars.exe");
                this.Close();
            }

        }

        private void GetVersions()
        {
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length >= 3) // Check if at least two arguments are present
            {
                string currentVersion = args[1];
                string newVersion = args[2];

                //here
                AppVersion = currentVersion;
                ModVersion = newVersion;


                labelCurrentVersion.Text = currentVersion;
                labelNewVersion.Text = newVersion;

            }
        }

        //UI Client Movement
        Point mouseOffset;
        private void AutoUpdater_MouseDown(object sender, MouseEventArgs e)
        {
            mouseOffset = new Point(-e.X, -e.Y);

        }

        private void AutoUpdater_MouseMove(object sender, MouseEventArgs e)
        {
            // Move the form when the left mouse button is down
            if (e.Button == MouseButtons.Left)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start("www.crusaderwars.com");
        }
    }
}
