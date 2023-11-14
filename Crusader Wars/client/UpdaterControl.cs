using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crusader_Wars
{
    public partial class UpdaterControl : Form
    {
        public UpdaterControl()
        {
            InitializeComponent();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdaterControl_Load(object sender, EventArgs e)
        {
            
        }
    }
}
