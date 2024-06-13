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
    public partial class UC_GeneralOptions : UserControl
    {
        public UC_GeneralOptions()
        {
            InitializeComponent();
        }

        private void OptionSelection_CulturalPreciseness_ValueChanged(object sender, EventArgs e)
        {
            
            if(OptionSelection_CulturalPreciseness.Value <= -10 && OptionSelection_CulturalPreciseness.Value > -50)
            {
            }
            else if (OptionSelection_CulturalPreciseness.Value <= -50 && OptionSelection_CulturalPreciseness.Value > -90)
            {
            }
            else if (OptionSelection_CulturalPreciseness.Value <= -90 && OptionSelection_CulturalPreciseness.Value > -130)
            {
            }
            else if (OptionSelection_CulturalPreciseness.Value <= -130 && OptionSelection_CulturalPreciseness.Value > -170)
            {
            }
            else if (OptionSelection_CulturalPreciseness.Value <= -170 && OptionSelection_CulturalPreciseness.Value > -200)
            {
            }
        }
    }
}
