using System;

namespace Crusader_Wars
{
    public static class ArmyProportions
    {
        private static double Ratio { get; set; } = 100;

        private static int BattleLimit { get; set; } = Properties.Settings.Default.OPTIONS_LIMIT;
        public static bool UnitAutoSizeState { get; set; } = Properties.Settings.Default.OPTIONS_AUTOSCALE;

        public static void SetRatio(int a)
        {
            Ratio = a;
        }

        public static void SetLimit(int a)
        {
            Properties.Settings.Default.OPTIONS_LIMIT= a;
            Properties.Settings.Default.Save(); 
        }

        static int no_auto_levy;
        static int no_auto_infantry;
        static int no_auto_ranged;
        static int no_auto_cavalry;
        public static void AutoSizeUnits(int player_total, int enemy_total)
        {
            int total = player_total + enemy_total;

            //Before auto size unit sizes
            no_auto_levy = Properties.Settings.Default.LEVY_LIMIT;
            no_auto_infantry = Properties.Settings.Default.INFANTRY_LIMIT;
            no_auto_ranged = Properties.Settings.Default.RANGED_LIMIT;
            no_auto_cavalry = Properties.Settings.Default.CAVALVRY_LIMIT;

            if (UnitAutoSizeState)
            {
                if (total >= 20000 && total < 30000)
                {
                    Properties.Settings.Default.LEVY_LIMIT = 400;
                    Properties.Settings.Default.INFANTRY_LIMIT = 400;
                    Properties.Settings.Default.RANGED_LIMIT = 350;
                    Properties.Settings.Default.CAVALVRY_LIMIT = 150;
                    Properties.Settings.Default.Save();
                }
                else if (total >= 30000 && total < 40000)
                {
                    Properties.Settings.Default.LEVY_LIMIT = 450;
                    Properties.Settings.Default.INFANTRY_LIMIT = 400;
                    Properties.Settings.Default.RANGED_LIMIT = 350;
                    Properties.Settings.Default.CAVALVRY_LIMIT = 200;
                    Properties.Settings.Default.Save();
                }
                else if (total >= 40000)
                {
                    Properties.Settings.Default.LEVY_LIMIT = 550;
                    Properties.Settings.Default.INFANTRY_LIMIT = 550;
                    Properties.Settings.Default.RANGED_LIMIT = 500;
                    Properties.Settings.Default.CAVALVRY_LIMIT = 250;
                    Properties.Settings.Default.Save();
                }
               
            }

        }

        public static void ResetUnitSizes()
        {
            Properties.Settings.Default.LEVY_LIMIT = no_auto_levy;
            Properties.Settings.Default.INFANTRY_LIMIT = no_auto_infantry;
            Properties.Settings.Default.RANGED_LIMIT = no_auto_ranged;
            Properties.Settings.Default.CAVALVRY_LIMIT = no_auto_cavalry;
            Properties.Settings.Default.Save();

            no_auto_levy = 0;
            no_auto_infantry = 0;
            no_auto_ranged = 0;
            no_auto_cavalry = 0;
        }

        public static void isBiggerThanLimit(int player_total, int enemy_total)
        {
            int total = player_total + enemy_total;
            if(total > BattleLimit && UnitAutoSizeState == true)
            {
                Ratio = BattleLimit / total;
              //  return true;
            }

            //return false;
        }


        public static int SetSoldiersRatio(int num_soldiers)
        {
            if(Ratio > 0)
            {
                double porcentage = (double)Ratio / 100;
                double num_ratio = num_soldiers * porcentage;
                num_ratio = Math.Round(num_ratio);

                return (int)num_ratio;
            }

            return num_soldiers;
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
