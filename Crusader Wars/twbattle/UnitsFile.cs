using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections;
using Crusader_Wars.client;

namespace Crusader_Wars
{
    public static class UnitsFile
    {
        public static int MAX_LEVIES_UNIT_NUMBER = ModOptions.GetLevyMax();
        public static int MAX_CAVALRY_UNIT_NUMBER = ModOptions.GetCavalryMax();
        public static int MAX_INFANTRY_UNIT_NUMBER = ModOptions.GetInfantryMax();
        public static int MAX_RANGED_UNIT_NUMBER = ModOptions.GetRangedMax();

        public static void ConvertandAddArmyUnits(ICharacter Side)
        {
            try
            {
                //Army XP
                int commander_army_xp = Side.Commander.GetUnitsExperience();
                int modifiers_xp = Side.Modifiers.GetXP();
                //int supplies_xp = Side.Supplys.GetXP();
                if (TerrainGenerator.isStrait && Side.CombatSide == "attacker") { modifiers_xp -= 2; }
                int army_xp = commander_army_xp + modifiers_xp;
                //Knights & Commanders XP
                int commander_xp = Side.Commander.GetCommanderExperience();
                int knights_xp = Side.Knights.SetExperience();
                //Modifiers only decrease knights xp
                if (modifiers_xp < 0) knights_xp += modifiers_xp;


                //XP Limiters
                if (army_xp < 0) army_xp = 0;
                if (army_xp > 9) army_xp = 9;

                if (commander_xp < 0) commander_xp = 0;
                if (commander_xp > 9) commander_xp = 9;

                if (knights_xp < 0) knights_xp = 0;
                if (knights_xp > 9) knights_xp = 9;

                //General
                var knights = Side.Army.FirstOrDefault(item => item.Type == "Knights");
                var general = Side.Army.FirstOrDefault(item => item.Type == "General");

                if (Side.Commander.Rank == 1 || Side.Commander.Rank == 2)
                {
                    BattleFile.AddGeneralUnit(Side.Commander, knights.Key, general.Script, commander_xp);
                }
                else
                {
                    BattleFile.AddGeneralUnit(Side.Commander, general.Key, general.Script, commander_xp);
                }

                //Knights
                BattleFile.AddKnightUnit(Side.Knights, knights.Key, knights.Script, knights_xp);

                //Levies
                var levies_units = Side.Army.Where(item => item.Type.Contains("Levy") || item.Type.Contains("Levies"));
                if (levies_units.Count() > 0)
                {
                    int levies_total_number = levies_units.ElementAt(0).SoldiersNum;
                    LevyDiversity(levies_units, levies_total_number, army_xp.ToString());
                }


                //MenAtArms
                foreach (var troop in Side.Army)
                {
                    //Skip if its not a Men at Arms Unit
                    if (troop.Type == "General" || troop.Type == "Knights" || troop.Type.Contains("Levy")) continue;

                    var MAA_Data = RetriveCalculatedUnits(troop.SoldiersNum, troop.Max);

                    //If is retinue maa, increase 2xp.
                    if (troop.Script.Contains("accolade")) BattleFile.AddUnit(troop.Key, MAA_Data.UnitSoldiers, MAA_Data.UnitNum, MAA_Data.SoldiersRest, troop.Script, (army_xp + 2).ToString());
                    //If is normal maa
                    else BattleFile.AddUnit(troop.Key, MAA_Data.UnitSoldiers, MAA_Data.UnitNum, MAA_Data.SoldiersRest, troop.Script, army_xp.ToString());

                }

                BattleFile.ResetPositions();
            }
            catch
            {
                MessageBox.Show("Error on converting units\nThis can happen if the wrong mapper is loaded", "Data Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Application.Exit();
            }
        }









        private static void LevyDiversity(IEnumerable<(string Type, string Key, int Max, string Script, int SoldiersNum)> Units, int TroopTypeNumber, string unit_experience)
        {

            //Units
            var LevySpearmen = Units.ElementAt(0);
            var LevyInfantry = Units.ElementAt(1);
            var LevyRanged = Units.ElementAt(2);

            //Spearmen
            int spearmen_total = GivePorcentageNumber(TroopTypeNumber, 0.65); //65% porcent
            int levy_spearmen_soldiers_per_unit = RetriveCalculatedUnits(spearmen_total, MAX_LEVIES_UNIT_NUMBER).UnitSoldiers;
            int levy_spearmen_unitNumber = RetriveCalculatedUnits(spearmen_total, MAX_LEVIES_UNIT_NUMBER).UnitNum;
            int levy_spearmen_leftoverNumber = RetriveCalculatedUnits(spearmen_total, MAX_LEVIES_UNIT_NUMBER).SoldiersRest;
            BattleFile.AddUnit(LevySpearmen.Key, levy_spearmen_soldiers_per_unit, levy_spearmen_unitNumber, levy_spearmen_leftoverNumber, LevySpearmen.Script, unit_experience);

            //Infantry
            int infantry_total = GivePorcentageNumber(TroopTypeNumber, 0.25); //25% porcent
            int levy_infantry_soldiers_per_unit = RetriveCalculatedUnits(infantry_total, MAX_LEVIES_UNIT_NUMBER).UnitSoldiers;
            int levy_infantry_unitNumber = RetriveCalculatedUnits(infantry_total, MAX_LEVIES_UNIT_NUMBER).UnitNum;
            int levy_iinfantry_leftoverNumber = RetriveCalculatedUnits(infantry_total, MAX_LEVIES_UNIT_NUMBER).SoldiersRest;
            BattleFile.AddUnit(LevyInfantry.Key, levy_infantry_soldiers_per_unit, levy_infantry_unitNumber, levy_iinfantry_leftoverNumber, LevyInfantry.Script, unit_experience);

            //Ranged
            int ranged_total = GivePorcentageNumber(TroopTypeNumber, 0.10); //10% porcent
            int levy_ranged_soldiers_per_unit = RetriveCalculatedUnits(ranged_total, MAX_LEVIES_UNIT_NUMBER).UnitSoldiers;
            int levy_ranged_unitNumber = RetriveCalculatedUnits(ranged_total, MAX_LEVIES_UNIT_NUMBER).UnitNum;
            int levy_ranged_leftoverNumber = RetriveCalculatedUnits(ranged_total, MAX_LEVIES_UNIT_NUMBER).SoldiersRest;
            BattleFile.AddUnit(LevyRanged.Key, levy_ranged_soldiers_per_unit, levy_ranged_unitNumber, levy_ranged_leftoverNumber, LevyRanged.Script, unit_experience);
        }



        private static int GivePorcentageNumber(int total_number, double porcentage)
        {
            double value = total_number * porcentage;
            value = Math.Round(value);
            return (int)value;
        }

        public static (int UnitSoldiers, int UnitNum, int SoldiersRest) RetriveCalculatedUnits(int soldiers, int unit_limit)
        {
            //if it's a special unit like siege equipement, monsters, etc...
            if (unit_limit == 1111)
            {
                int special_num = soldiers / 10; //10, because siege equipement is 10 persons one equipement
                return (special_num, 1, 0);
            }

            int unit_num;
            for (int i = 1; i <= soldiers; i++)
            {
                int result = soldiers / i;

                if (result <= unit_limit)
                {
                    unit_num = i;
                    int rest = soldiers % i;

                    return (result, unit_num, rest);
                }

            }

            return (0, 0, 0);
        }


        public static void BETA_ConvertandAddArmyUnits(Army army)
        {


            int levy_max = ModOptions.GetLevyMax();
            //Levies
            var levies_units = army.Units.Where(item => item.GetName().Contains("Levy"));
            if (levies_units.Count() > 0)
            {
                foreach (var levy_culture in levies_units)
                {
                    var Levies_Data = RetriveCalculatedUnits(levy_culture.GetSoldiers(), levy_max);
                    BattleFile.AddUnit(levy_culture.GetAttilaUnitKey(), Levies_Data.UnitSoldiers, Levies_Data.UnitNum, Levies_Data.SoldiersRest, $"{army.CombatSide}_{army.ID}_{levy_culture.GetName()}_", "0");
                }
            }


            //MenAtArms
            foreach (var unit in army.Units)
            {
                string unitName = unit.GetName();
                //Skip if its not a Men at Arms Unit
                if (unitName == "General" || unitName == "Knights" || unitName.Contains("Levy")) continue;

                var MAA_Data = RetriveCalculatedUnits(unit.GetSoldiers(), unit.GetMax());

                //If is retinue maa, increase 2xp.
                if (unitName.Contains("accolade")) BattleFile.AddUnit(unit.GetAttilaUnitKey(), MAA_Data.UnitSoldiers, MAA_Data.UnitNum, MAA_Data.SoldiersRest, $"{army.CombatSide}_{army.ID}_{unit.GetName()}_", "2");
                //If is normal maa
                else BattleFile.AddUnit(unit.GetAttilaUnitKey(), MAA_Data.UnitSoldiers, MAA_Data.UnitNum, MAA_Data.SoldiersRest, $"{army.CombatSide}_{army.ID}_{unit.GetName()}_", "0");

            }


        }

    }
}
