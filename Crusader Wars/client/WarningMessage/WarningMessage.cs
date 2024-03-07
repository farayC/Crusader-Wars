using System;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crusader_Wars.client.WarningMessage
{
    public partial class WarningMessage : Form
    {
        public WarningMessage()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.logo;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void SetMessage(string message)
        {
            Text_Warning.Text = message;
        }

        public static void ShowWarningMessage(string message)
        {
            var warning_message = new WarningMessage();
            warning_message.SetMessage(message);
            SystemSounds.Exclamation.Play();
            warning_message.ShowDialog();

        }


    }
}
