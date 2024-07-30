using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crusader_Wars.client
{
    public partial class UC_Toggle : UserControl
    {
        public bool State {  get; set; }
        public UC_Toggle()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangeState();
        }

        void ChangeState()
        {
            if (State == true)
            {
                State = false;
                button1.BackgroundImage = Properties.Resources.toggle_no;
            }
            else
            {
                State = true;
                button1.BackgroundImage = Properties.Resources.toggle_yes;
            }
        }

        public void SetState(bool state)
        {
            State = state;
            if (State == true)
            {
                button1.BackgroundImage = Properties.Resources.toggle_yes;
            }
            else
            {
                button1.BackgroundImage = Properties.Resources.toggle_no;
            }
        }
    }
}
