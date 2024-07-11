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

        static int i;
        public static void BETA_ConvertandAddArmyUnits(Army army)
        {
            BETA_AddArmyUnits(army);
            if(army.MergedArmies != null)
            {
                foreach(Army merged_army in army.MergedArmies)
                {
                    BETA_AddArmyUnits(merged_army);
                }
            }
        }

        static void BETA_AddArmyUnits(Army army)
        {
            army.RemoveNullUnits();

            i = 0;
            int modifiers_xp = 0;
            int army_xp = 0;

            if (TerrainGenerator.isStrait || TerrainGenerator.isRiver && army.CombatSide == "attacker") { modifiers_xp -= 2; }

            //##################
            //                 #
            //    COMMANDER    #
            //                 #
            //##################
            int commander_army_xp = 0;
            if (army.Commander != null)
            {
                commander_army_xp = army.Commander.GetUnitsExperience();
                int commander_xp = army.Commander.GetCommanderExperience();
                int commander_soldiers = army.Commander.GetUnitSoldiers();
                Unit commander_unit = new Unit("General", commander_soldiers, army.Commander.GetCultureObj(), RegimentType.Commander);
                commander_unit.SetAttilaFaction(UnitMappers_BETA.GetAttilaFaction(army.Commander.GetCultureName(), army.Commander.GetHeritageName()));

                string general_script_name = $"{i}_{army.CombatSide}_army{army.ID}_COMMANDER{army.Commander.ID}_{army.Commander.GetCultureName()}_";
                BattleFile.AddGeneralUnit(army.Commander, UnitMappers_BETA.GetUnitKey(commander_unit), general_script_name, commander_xp, Deployments.beta_GeDirection(army.CombatSide));
                i++;
            }


            //##################
            //                 #
            //     KNIGHTS     #
            //                 #
            //##################
            if (army.Knights != null)
            {
                Unit knights_unit;
                if (army.Knights.GetMajorCulture() != null)
                    knights_unit = new Unit("Knight", army.Knights.GetKnightsSoldiers(), army.Knights.GetMajorCulture(), RegimentType.Knight);
                else
                    knights_unit = new Unit("Knight", army.Knights.GetKnightsSoldiers(), army.OwnerCulture, RegimentType.Knight);

                knights_unit.SetAttilaFaction(UnitMappers_BETA.GetAttilaFaction(knights_unit.GetCulture(), knights_unit.GetHeritage()));

                string knights_script_name;
                if (army.Knights.GetMajorCulture() != null)
                    knights_script_name = $"{i}_{army.CombatSide}_army{army.ID}_KNIGHTS_{army.Knights.GetMajorCulture().GetCultureName()}_";
                else
                    knights_script_name = $"{i}_{army.CombatSide}_army{army.ID}_KNIGHTS_{army.OwnerCulture.GetCultureName()}_";


                BattleFile.AddKnightUnit(army.Knights, UnitMappers_BETA.GetUnitKey(knights_unit), knights_script_name, army.Knights.SetExperience(), Deployments.beta_GeDirection(army.CombatSide));
                i++;
            }


            //##################
            //     ARMY XP     #
            //##################
            army_xp += commander_army_xp;
            army_xp += modifiers_xp;
            if (army_xp < 0) { army_xp = 0; }
            if (army_xp > 9) { army_xp = 9; }

            //##################
            //                 #
            //      LEVIES     #
            //                 #
            //##################
            int levy_max = ModOptions.GetLevyMax();
            var levies_units = army.Units.Where(item => item.GetRegimentType() == data.save_file.RegimentType.Levy);
            if (levies_units.Count() > 0)
            {
                foreach (var levy_culture in levies_units)
                {
                    var levy_porcentages = UnitMappers_BETA.GetFactionLevies(levy_culture.GetAttilaFaction());
                    BETA_LevyComposition(levy_culture, army, levy_porcentages, army_xp);
                }
            }

            //##################
            //                 #
            //   MEN-AT-ARMS   #
            //                 #
            //##################
            foreach (var unit in army.Units)
            {
                string unitName = unit.GetName();
                //Skip if its not a Men at Arms Unit
                if (unitName == "General" || unit.GetRegimentType() == RegimentType.Knight || unit.GetRegimentType() == RegimentType.Levy) continue;

                var MAA_Data = RetriveCalculatedUnits(unit.GetSoldiers(), unit.GetMax());

                //If is retinue maa, increase 2xp.
                if (unitName.Contains("accolade"))
                {
                    string unit_script_name = $"{i}_{army.CombatSide}_army{army.ID}_{unit.GetName()}_{unit.GetCulture()}_";
                    BattleFile.AddUnit(unit.GetAttilaUnitKey(), MAA_Data.UnitSoldiers, MAA_Data.UnitNum, MAA_Data.SoldiersRest, unit_script_name, army_xp.ToString(), Deployments.beta_GeDirection(army.CombatSide));
                }
                //If is normal maa
                else
                {
                    string unit_script_name = $"{i}_{army.CombatSide}_army{army.ID}_{unit.GetName()}_{unit.GetCulture()}_";
                    BattleFile.AddUnit(unit.GetAttilaUnitKey(), MAA_Data.UnitSoldiers, MAA_Data.UnitNum, MAA_Data.SoldiersRest, unit_script_name, army_xp.ToString(), Deployments.beta_GeDirection(army.CombatSide));
                }
                i++;


            }

            army.PrintUnits();
        }

        static void BETA_LevyComposition(Unit unit, Army army, List<(int porcentage, string unit_key, string name)> faction_levy_porcentages, int army_xp)
        {

            var Levies_Data = RetriveCalculatedUnits(unit.GetSoldiers(), unit.GetMax());

            int total_soldiers = unit.GetSoldiers();
            string culture = unit.GetCulture();


            //  SINGULAR UNIT
            //  select random levy type
            if (unit.GetSoldiers() <= unit.GetMax())
            {
                Random r = new Random();
                var random = faction_levy_porcentages[r.Next(faction_levy_porcentages.Count - 1)];
                string script_name = $"{i}_{army.CombatSide}_army{army.ID}_Levy{random.porcentage}_{culture}_";
                BattleFile.AddUnit(random.unit_key, Levies_Data.UnitSoldiers, 1, Levies_Data.SoldiersRest, script_name, army_xp.ToString(), Deployments.beta_GeDirection(army.CombatSide));
                i++;
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
                string script_name = $"{i}_{army.CombatSide}_army{army.ID}_Levy{porcentageData.porcentage}_{culture}_";
                BattleFile.AddUnit(porcentageData.unit_key, levy_type_data.UnitSoldiers, levy_type_data.UnitNum, Levies_Data.SoldiersRest, script_name, army_xp.ToString(), Deployments.beta_GeDirection(army.CombatSide));
                i++;
            }

        }

    }
}
