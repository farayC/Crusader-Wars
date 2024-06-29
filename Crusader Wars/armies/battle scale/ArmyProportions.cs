using Crusader_Wars.client;
using System;

namespace Crusader_Wars
{
    public static class ArmyProportions
    {
        private static double Ratio { get; set; } = ModOptions.GetBattleScale();
        public static bool UnitAutoSizeState { get; set; } = ModOptions.GetAutoScale();

        public static void SetRatio(int a)
        {
            Ratio = a;
        }


        static int no_auto_levy;
        static int no_auto_infantry;
        static int no_auto_ranged;
        static int no_auto_cavalry;
        public static void AutoSizeUnits(int total)
        {

            //Before auto size unit sizes
            no_auto_levy = ModOptions.GetLevyMax();
            no_auto_infantry = ModOptions.GetInfantryMax();
            no_auto_ranged = ModOptions.GetRangedMax();
            no_auto_cavalry = ModOptions.GetCavalryMax();

            if (UnitAutoSizeState)
            {
                if (total >= 20000 && total < 30000)
                {
                    ModOptions.SetLevyMax(400);
                    ModOptions.SetInfantryMax(400);
                    ModOptions.SetRangedMax(400);
                    ModOptions.SetCavalryMax(150);

                }
                else if (total >= 30000 && total < 40000)
                {
                    ModOptions.SetLevyMax(450);
                    ModOptions.SetInfantryMax(450);
                    ModOptions.SetRangedMax(450);
                    ModOptions.SetCavalryMax(300);

                }
                else if (total >= 40000)
                {
                    ModOptions.SetLevyMax(550);
                    ModOptions.SetInfantryMax(550);
                    ModOptions.SetRangedMax(550);
                    ModOptions.SetCavalryMax(350);

                }
               
            }

        }

        public static void ResetUnitSizes()
        {
            ModOptions.SetLevyMax(no_auto_levy);
            ModOptions.SetInfantryMax(no_auto_infantry);
            ModOptions.SetRangedMax(no_auto_ranged);
            ModOptions.SetCavalryMax(no_auto_cavalry);

            no_auto_levy = 0;
            no_auto_infantry = 0;
            no_auto_ranged = 0;
            no_auto_cavalry = 0;
        }


        public static int SetResultsRatio(int num_soldiers)
        {
            if (Ratio > 0)
            {
                double porcentage = (double)Ratio / 100;
                double num_ratio = num_soldiers / porcentage;
                num_ratio = Math.Round(num_ratio);
                return (int)num_ratio;
            }

            return num_soldiers;
            
        }
    }
}
