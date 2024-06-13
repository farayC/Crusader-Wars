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
using Crusader_Wars.unit_mapper;
using Crusader_Wars.data.save_file;
using Crusader_Wars.terrain;
using System.Windows;

namespace Crusader_Wars
{
    public static class UnitsFile
    {
        public static int MAX_LEVIES_UNIT_NUMBER = ModOptions.GetLevyMax();
        public static int MAX_CAVALRY_UNIT_NUMBER = ModOptions.GetCavalryMax();
        public static int MAX_INFANTRY_UNIT_NUMBER = ModOptions.GetInfantryMax();
        public static int MAX_RANGED_UNIT_NUMBER = ModOptions.GetRangedMax();

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

        static void BETA_LevyComposition(Unit unit, Army army,List<(int porcentage, string unit_key, string name)> faction_levy_porcentages)
        {

            var Levies_Data = RetriveCalculatedUnits(unit.GetSoldiers(), unit.GetMax());
            
            int total_soldiers = unit.GetSoldiers();
            string culture = unit.GetCulture();


            //  SINGULAR UNIT
            //  select random levy type
            if (unit.GetSoldiers() <= unit.GetMax())
            {
                Random r = new Random();
                var random = faction_levy_porcentages[r.Next(faction_levy_porcentages.Count-1)];
                BattleFile.AddUnit(random.unit_key, Levies_Data.UnitSoldiers, 1, Levies_Data.SoldiersRest, $"{army.CombatSide}__army{army.ID}__Levy{random.porcentage}__{culture}__", "0", Deployments.beta_GeDirection(army.CombatSide));
                return;
            }


            //  MULTIPLE UNITS
            //  fulfill every levy type
            int levySoldiers = unit.GetSoldiers();

            int compareNum = 0;
            
            foreach (var porcentageData in faction_levy_porcentages)
            {
                

                double t = (double)porcentageData.porcentage / 100;
                //int result = (int)Math.Round(Levies_Data.UnitNum * t);
                int result = (int)Math.Round(levySoldiers * t);
                var levy_type_data = RetriveCalculatedUnits(result, unit.GetMax());
                compareNum += (levy_type_data.UnitSoldiers * levy_type_data.UnitNum);
                //if (Levies_Data.UnitNum * t >= 0.5 && Levies_Data.UnitNum * t < 1) result = 1;
                BattleFile.AddUnit(porcentageData.unit_key, levy_type_data.UnitSoldiers, levy_type_data.UnitNum, Levies_Data.SoldiersRest, $"{army.CombatSide}__army{army.ID}__Levy{t}__{culture}__", "0", Deployments.beta_GeDirection(army.CombatSide));
            }

            Console.WriteLine("OG-"+levySoldiers+"\t"+"Attila Num-"+compareNum);
            Console.WriteLine("^Difference:" + (levySoldiers - compareNum));

        }

        public static void BETA_ConvertandAddArmyUnits(Army army)
        {
            /*

                //Army XP
                int commander_army_xp = Side.Commander.GetUnitsExperience();
                //int modifiers_xp = army.Modifiers.GetXP();
                //int supplies_xp = Side.Supplys.GetXP();
                //if (TerrainGenerator.isStrait && army.CombatSide == "attacker") { modifiers_xp -= 2; }
                //int army_xp = commander_army_xp + modifiers_xp;
                //Knights & Commanders XP
                //int commander_xp = Side.Commander.GetCommanderExperience();
                //int knights_xp = Side.Knights.SetExperience();
                //Modifiers only decrease knights xp
                //int knights_xp = 0;
                //if (modifiers_xp < 0) knights_xp += modifiers_xp;

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
             */

            army.RemoveNullUnits();
            if(army.Commander != null)
            {
                int commander_army_xp = army.Commander.GetUnitsExperience();
                int commander_xp = army.Commander.GetCommanderExperience();
                Unit commanderUnit = new Unit("General", army.Commander.GetUnitSoldiers(), UnitMappers_BETA.geta)
            }
            

            //Knights
            Unit knights_unit = new Unit("Knight", army.Knights.GetKnightsSoldiers(), army.Knights.GetMajorCulture(), RegimentType.Knight);
            knights_unit.SetAttilaFaction(UnitMappers_BETA.GetAttilaFaction(knights_unit.GetCulture(), knights_unit.GetHeritage()));
            BattleFile.AddKnightUnit(army.Knights, UnitMappers_BETA.GetUnitKey(knights_unit), $"{army.CombatSide}__army{army.ID}__KNIGHTS__{army.Knights.GetMajorCulture().GetCultureName()}__", army.Knights.SetExperience(), Deployments.beta_GeDirection(army.CombatSide));

            int levy_max = ModOptions.GetLevyMax();
            //Levies
            var levies_units = army.Units.Where(item => item.GetRegimentType() == data.save_file.RegimentType.Levy);
            if (levies_units.Count() > 0)
            {
                foreach (var levy_culture in levies_units)
                {
                    var levy_porcentages = UnitMappers_BETA.GetFactionLevies(levy_culture.GetAttilaFaction());
                    BETA_LevyComposition(levy_culture,army, levy_porcentages);                    
                }
            }


            //MenAtArms
            foreach (var unit in army.Units)
            {
                string unitName = unit.GetName();
                //Skip if its not a Men at Arms Unit
                if (unitName == "General" || unit.GetRegimentType() == RegimentType.Knight || unit.GetRegimentType() == RegimentType.Levy) continue;

                var MAA_Data = RetriveCalculatedUnits(unit.GetSoldiers(), unit.GetMax());

                //If is retinue maa, increase 2xp.
                if (unitName.Contains("accolade")) BattleFile.AddUnit(unit.GetAttilaUnitKey(), MAA_Data.UnitSoldiers, MAA_Data.UnitNum, MAA_Data.SoldiersRest, $"{army.CombatSide}__army{army.ID}__{unit.GetName()}__{unit.GetCulture()}__", "2", Deployments.beta_GeDirection(army.CombatSide));
                //If is normal maa
                else BattleFile.AddUnit(unit.GetAttilaUnitKey(), MAA_Data.UnitSoldiers, MAA_Data.UnitNum, MAA_Data.SoldiersRest, $"{army.CombatSide}__army{army.ID}__{unit.GetName()}__{unit.GetCulture()}__", "0", Deployments.beta_GeDirection(army.CombatSide));

            }

            army.PrintUnits();
        }

    }
}
