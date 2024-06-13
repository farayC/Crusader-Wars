using Crusader_Wars.client;
using Crusader_Wars.unit_mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crusader_Wars.data.save_file
{
    internal static class Armies_Functions
    {
        internal static List<Unit> GetAllUnits_UnitKeys(List<Unit> units)
        {

            //Set Unit Keys
            foreach (var unit in units)
            {
                if (unit.GetRegimentType() == RegimentType.Levy) continue;

                string key = UnitMappers_BETA.GetUnitKey(unit);
                if (key == "not_found")
                    unit.SetUnitKey("cha_spa_royal_cav");
                else
                    unit.SetUnitKey(key);
            }

            return units;
        }

        internal static List<Unit> GetAllUnits_AttilaFaction(List<Unit> units)
        {
            //Get Unit Mapper Faction
            foreach (var unit in units)
            {
                unit.SetAttilaFaction(UnitMappers_BETA.GetAttilaFaction(unit.GetCulture(), unit.GetHeritage()));
            }

            return units;
        }
        internal static List<Unit> GetAllUnits_Max(List<Unit> units)
        {
            //Read Unit Limit
            foreach (var unit in units)
            {
                unit.SetMax(UnitMappers_BETA.GetMax(unit));
            }

            return units;
        }

        // Function to get the top N units by soldiers count
        internal static List<Unit> GetTopUnits(List<Unit> units, int topN)
        {
            if (topN <= 0)
            {
                throw new ArgumentException("topN must be greater than 0");
            }

            // Sort units by soldiers count in descending order
            var sortedUnits = units.OrderByDescending(u => u.GetSoldiers()).ToList();

            // Take the top N units
            return sortedUnits.Take(topN).ToList();
        }

        internal static void CreateUnits(List<Army> armies)
        {
            foreach (var army in armies)
            {
                List<(Regiment regiment, RegimentType type, string maa_name)> list = new List<(Regiment regiment, RegimentType type, string maa_name)>();
                foreach (var army_regiment in army.ArmyRegiments)
                {

                    foreach (var regiment in army_regiment.Regiments)
                    {
                        list.Add((regiment, army_regiment.Type, army_regiment.MAA_Name));
                    }
                }

                List<Unit> units = new List<Unit>();
                foreach (var regiment in list)
                {
                    // if no soldiers, skip
                    if (ModOptions.FullArmies(regiment.regiment) is null) continue;
                    if (Int32.Parse(ModOptions.FullArmies(regiment.regiment)) == 0) continue;

                    Unit unit;
                    if (regiment.type == RegimentType.Levy)
                        if (regiment.regiment.isMercenary())
                            unit = new Unit("Levy", Int32.Parse(ModOptions.FullArmies(regiment.regiment)), regiment.regiment.Culture, regiment.type, true);
                        else
                            unit = new Unit("Levy", Int32.Parse(ModOptions.FullArmies(regiment.regiment)), regiment.regiment.Culture, regiment.type);
                    else if (regiment.type == RegimentType.MenAtArms)
                        if (regiment.regiment.isMercenary())
                            unit = new Unit(regiment.maa_name, Int32.Parse(ModOptions.FullArmies(regiment.regiment)), regiment.regiment.Culture, regiment.type, true);
                        else
                            unit = new Unit(regiment.maa_name, Int32.Parse(ModOptions.FullArmies(regiment.regiment)), regiment.regiment.Culture, regiment.type);
                    else
                        continue;

                    if (unit != null)
                        units.Add(unit);


                }

                units = OrganizeUnitsIntoCultures(units);
                units = OrganizeLeviesUnits(units);
                units = Armies_Functions.GetAllUnits_AttilaFaction(units);
                units = Armies_Functions.GetAllUnits_Max(units);
                //units = OrganizeLevyComposition(units);
                units = Armies_Functions.GetAllUnits_UnitKeys(units);
                army.SetUnits(units);
                //army.PrintUnits();
            }
        }

        static List<Unit> OrganizeUnitsIntoCultures(List<Unit> units)
        {
            var organizedUnits = new List<Unit>();

            // Group units by Name and Culture
            var groupedUnits = units.GroupBy(u => new { Name = u.GetName(), Culture = u.GetCulture(), Type = u.GetType(), IsMerc = u.IsMerc() });

            // Merge units with the same Name and Culture
            foreach (var group in groupedUnits)
            {
                int totalSoldiers = group.Sum(u => u.GetSoldiers());

                // Create a new Unit with the merged NumberOfSoldiers
                Unit mergedUnit = new Unit(group.Key.Name, totalSoldiers, group.ElementAt(0).GetObjCulture(), group.ElementAt(0).GetRegimentType(), group.ElementAt(0).IsMerc());

                organizedUnits.Add(mergedUnit);
            }

            return organizedUnits;
        }

        static List<Unit> OrganizeLeviesUnits(List<Unit> units)
        {
            var unitsBelowThreshold = units.Where(u => u.GetSoldiers() <= ModOptions.CulturalPreciseness() && u.GetName() == "Levy").ToList();
            if (unitsBelowThreshold.Count == 0) return units;

            int total = 0;
            Unit biggest = null;
            int lastRegistered = 0;
            foreach (var u in unitsBelowThreshold)
            {
                total += u.GetSoldiers();

                if (u.GetSoldiers() > lastRegistered)
                {
                    lastRegistered = u.GetSoldiers();
                    biggest = u;
                }

            }

            var unit = new Unit("Levy", total, biggest.GetObjCulture(), RegimentType.Levy);
            var unit_data = UnitsFile.RetriveCalculatedUnits(unit.GetSoldiers(), ModOptions.GetLevyMax());
            var levies_top_cultures = GetTopUnits(unitsBelowThreshold, unit_data.UnitNum);

            int null_soldiers = 0;

            var null_cultures_levies = units.Where(x => x.GetObjCulture() == null).ToList();
            if (null_cultures_levies.Count > 0)
            {
                foreach (var null_levie in null_cultures_levies)
                {
                    null_soldiers += null_levie.GetSoldiers();
                    units.Remove(null_levie);
                }
            }


            int limit = ModOptions.CulturalPreciseness();
            units.RemoveAll(x => x.GetName() == "Levy" && x.GetSoldiers() <= limit);
            for (int i = 0; i < unit_data.UnitNum; i++)
            {
                if (i == 0)
                    units.Add(new Unit("Levy", unit_data.UnitSoldiers + null_soldiers, levies_top_cultures[i].GetObjCulture(), RegimentType.Levy));
                else
                    units.Add(new Unit("Levy", unit_data.UnitSoldiers, levies_top_cultures[i].GetObjCulture(), RegimentType.Levy));
            }





            return units;
        }
    }
}
