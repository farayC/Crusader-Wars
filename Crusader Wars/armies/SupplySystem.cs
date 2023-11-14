using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crusader_Wars.armies
{

    //What needs to do to be finished:
    /*
     * Read supplies data from save file.
     * 
     * Write loop for reduzing the ammo and fatigue for all units
     */
    public class Supplys
    {
        int Quantity { get; set; }
        int XP { get; set; }

        public int GetXP()
        {
            return XP;
        }

        void tt (ICharacter Side)
        {
            /*175-200 - 
             * plus 50% deployables
             * plus 50% ammunition
             */
            /*150-175 - 
             * plus 35% deployables
             * plus 35% ammunition
             */

            /*125-150 -
             * plus 25% deployables
             * plus 25% ammunition
             */

            /*100-125 -
             * plus 10% deployables
             * plus 10% ammunition 
             */

            /*75 - 100
             * ammo cut by 25%
             * deployables cut by 25%
             */

            /*50 - 75
             * ammo cut by 50%
             * deployables cut by 50%
             * army xp reduced by 1
             */

            /*25 - 50
             * ammo cut by 70%
             * no deployables
             * army xp reduced by 1
             */

            /*0 - 25
             * ammo cut by 90%
             * no deployables
             * army xp reduced by 2
             */
            int total = Side.Defences.GetTotalDeployments();
            //Vast surplus
            if (Quantity > 175) 
            {
                int buff = (int)Math.Round(total * 0.5);
                Side.Defences.IncreaseDeployables (total + buff);
            }
            //Huge surplus
            else if (Quantity > 150 &&  Quantity <= 175)
            {
                int buff = (int)Math.Round(total * 0.4);
                Side.Defences.IncreaseDeployables(total + buff);
            }
            //Good surplus
            else if (Quantity > 125 && Quantity <= 150)
            {
                int buff = (int)Math.Round(total * 0.25);
                Side.Defences.IncreaseDeployables(total + buff);
            }
            //Small surplus
            else if (Quantity > 100 && Quantity <= 125)
            {
                int buff = (int)Math.Round(total * 0.1);
                Side.Defences.IncreaseDeployables(total + buff);
            }
            //Normal Supplies
            else if (Quantity > 75 && Quantity <= 100)
            {
                //nothing happens...
            }
            //Below Average Supplies
            else if (Quantity > 50 && Quantity <= 75)
            {
                int debuff = (int)Math.Round(total * 0.50);
                Side.Defences.DecreaseDeployables(debuff);
            }
            //Low Supplies
            else if (Quantity > 25 && Quantity <= 50)
            {
                int debuff = (int)Math.Round(total * 0.25);
                Side.Defences.DecreaseDeployables(debuff);
                XP = -1;
            }
            //Extremely Low Supplies
            else if (Quantity > 0 && Quantity <= 25)
            {
                Side.Defences.DecreaseDeployables(total);
                XP = -1;
            }
            //No Supplies
            else if (Quantity == 0 )
            {
                Side.Defences.DecreaseDeployables(total);
                XP = -2;
            }


        }
    }
}
