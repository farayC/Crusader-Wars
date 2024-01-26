using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crusader_Wars.client.RequiredMods
{
    public partial class RequiredModsMessage : Form
    {
        public RequiredModsMessage()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.logo;
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }

            base.WndProc(ref m);
        }

        private void SetTitleAndMods(string mapperName, string requiredMods)
        {
            label_mapper_title.Text = mapperName;
            Text_Mods.Text = requiredMods;
        }

        public static void ShowRequiredMods(string mapperName,string requiredMods)
        {
            if (mapperName.StartsWith("OfficialCW_"))
            {
                mapperName = mapperName.Remove(0, "OfficialCW_".Length);
            }
            else if (mapperName.StartsWith("xCW_"))
            {
                mapperName = mapperName.Remove(0, "xCW_".Length);
            }

            RequiredModsMessage window = new RequiredModsMessage();
            window.SetTitleAndMods(mapperName,requiredMods);
            SystemSounds.Exclamation.Play();
            window.Show();
        }

        public static void CloseAllWindows()
        {
            // Iterate through all open forms
            foreach (Form form in Application.OpenForms)
            {
                // Check if the form is of type "RequiredModsMessage"
                if (form is RequiredModsMessage)
                {
                    // Close the form
                    form.Close();
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
