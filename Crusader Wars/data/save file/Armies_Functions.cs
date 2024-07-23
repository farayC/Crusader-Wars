using Crusader_Wars.client;
using Crusader_Wars.unit_mapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Crusader_Wars.data.save_file
{
    internal static class Armies_Functions
    {

        /*##############################################
         *####               CULTURES               #### 
         *####--------------------------------------####
         *####   Reader for the culture manager     ####
         *##############################################
         */

        public static void ReadArmiesCultures(List<Army> armies)
        {
            bool isSearchStared = false;
            string culture_id = "";
            string culture_name = "";
            string heritage_name = "";
            var found_cultures = new List<(string culture_id, string culture_name, string heritage_name)>();

            using (StreamReader sr = new StreamReader(Writter.DataFilesPaths.Cultures_Path()))
            {
                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null) break;

                    //Culture Line
                    if (Regex.IsMatch(line, @"\t\t\d+={") && !isSearchStared)
                    {
                        culture_id = Regex.Match(line, @"\t\t(\d+)={").Groups[1].Value;

                        //Armies
                        for (int i = 0; i < armies.Count; i++)
                        {
                            if (isSearchStared) break;

                            //Owner
                            if (armies[i].OwnerCulture != null && armies[i].OwnerCulture.ID == culture_id)
                            {
                                isSearchStared = true;
                                break;
                            }

                            //Commanders
                            if (armies[i].Commander != null)
                            {
                                if (armies[i].Commander.GetCultureObj() == null)
                                {
                                    if (armies[i].IsPlayer())
                                    {
                                        armies[i].Commander.ChangeCulture(new Culture(CK3LogData.LeftSide.GetCommander().culture_id));
                                    }
                                    else if (armies[i].IsEnemy())
                                    {
                                        armies[i].Commander.ChangeCulture(new Culture(CK3LogData.RightSide.GetCommander().culture_id));
                                    }
                                    else
                                    {
                                        armies[i].Commander.ChangeCulture(armies[i].OwnerCulture);
                                    }
                                    
                                    continue;
                                }
                                if (armies[i].Commander.GetCultureObj().ID == culture_id)
                                {
                                    isSearchStared = true;
                                    break;
                                }
                            }

                            //Knights
                            if (armies[i].Knights.GetKnightsList() != null)
                            {
                                foreach (var knight in armies[i].Knights.GetKnightsList())
                                {
                                    string knight_culture_id = "";
                                    if (knight.GetCultureObj() == null)
                                    {
                                        Culture mainParticipantCulture = null;
                                        if (armies[i].IsPlayer())
                                        {
                                            string id = CK3LogData.LeftSide.GetMainParticipant().culture_id;
                                            mainParticipantCulture = new Culture(id);
                                        }
                                        else if(armies[i].IsEnemy())
                                        {
                                            string id = CK3LogData.RightSide.GetMainParticipant().culture_id;
                                            mainParticipantCulture = new Culture(id);
                                        }
                                            
                                        Culture new_culture = armies[i].Knights.GetKnightsList()?.Find(x => x.GetCultureObj() != null)?.GetCultureObj() ?? mainParticipantCulture;
                                        knight.ChangeCulture(new_culture);
                                        knight_culture_id = knight.GetCultureObj().ID;
                                        armies[i].Knights.SetMajorCulture();
                                    }
                                    else
                                    {
                                        knight_culture_id = knight.GetCultureObj().ID;
                                    }

                                    if (knight_culture_id == culture_id)
                                    {
                                        isSearchStared = true;
                                        break;
                                    }
                                }
                            }


                            //Army Regiments 
                            for (int x = 0; x < armies[i].ArmyRegiments.Count; x++)
                            {
                                if (isSearchStared) break;
                                if (armies[i].ArmyRegiments[x].Regiments is null) continue;
                                //Regiments
                                for (int y = 0; y < armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                {


                                    //if culture is null, skip
                                    if (armies[i].ArmyRegiments[x].Regiments[y].Culture is null)
                                    {
                                        continue;
                                    }

                                    //If is player character
                                    if (string.IsNullOrEmpty(armies[i].ArmyRegiments[x].Regiments[y].Culture.ID))
                                    {
                                        //armies[i].ArmyRegiments[x].Regiments[y].Culture.SetHeritage(DataSearch.Player_Character.GetHeritage());
                                        //armies[i].ArmyRegiments[x].Regiments[y].Culture.SetName(DataSearch.Player_Character.GetCulture());
                                        continue;
                                    }
                                    else
                                    {
                                        string regiment_culture_id = armies[i].ArmyRegiments[x].Regiments[y].Culture.ID;

                                        if (regiment_culture_id == culture_id)
                                        {
                                            isSearchStared = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //Culture Name
                    if (isSearchStared && line.Contains("\t\t\tname="))
                    {
                        culture_name = Regex.Match(line, @"""(\w+)""").Groups[1].Value;
                    }
                    //Heritage Name
                    else if (isSearchStared && line.Contains("\t\t\theritage="))
                    {
                        heritage_name = Regex.Match(line, @"""(\w+)""").Groups[1].Value;
                    }

                    //End Line
                    if (isSearchStared && line == "\t\t}")
                    {
                        found_cultures.Add((culture_id, culture_name, heritage_name));
                        isSearchStared = false;
                    }
                }
            }

            SetCulturesToAll(armies, found_cultures);
        }

        internal static void SetCulturesToAll(List<Army> armies, List<(string culture_id, string culture_name, string heritage_name)> foundCultures)
        {
            foreach (var culture in foundCultures)
            {
                //Armies
                for (int i = 0; i < armies.Count; i++)
                {
                    //Owner
                    if (armies[i].OwnerCulture != null && armies[i].OwnerCulture.ID == culture.culture_id)
                    {
                        armies[i].OwnerCulture.SetName(culture.culture_name);
                        armies[i].OwnerCulture.SetHeritage(culture.culture_name);
                    }

                    //Commanders
                    if (armies[i].Commander != null)
                    {
                        if (armies[i].Commander.GetCultureObj().ID == culture.culture_id)
                        {
                            armies[i].Commander.GetCultureObj().SetName(culture.culture_name);
                            armies[i].Commander.GetCultureObj().SetHeritage(culture.culture_name);
                        }
                    }

                    //Knights
                    if (armies[i].Knights.GetKnightsList() != null)
                    {
                        foreach (var knight in armies[i].Knights.GetKnightsList())
                        {
                            string knight_culture_id = knight.GetCultureObj().ID;

                            if (knight_culture_id == culture.culture_id)
                            {
                                knight.GetCultureObj().SetName(culture.culture_name);
                                knight.GetCultureObj().SetHeritage(culture.heritage_name);
                            }
                        }
                    }


                    //Army Regiments
                    for (int x = 0; x < armies[i].ArmyRegiments.Count; x++)
                    {
                        //Regiments
                        if (armies[i].ArmyRegiments[x].Regiments is null) continue;

                        for (int y = 0; y < armies[i].ArmyRegiments[x].Regiments.Count; y++)
                        {
                            if (armies[i].ArmyRegiments[x].Regiments[y].Culture is null) { continue; }

                            string regiment_culture_id = armies[i].ArmyRegiments[x].Regiments[y].Culture.ID;
                            if (culture.culture_id == regiment_culture_id)
                            {
                                armies[i].ArmyRegiments[x].Regiments[y].Culture.SetName(culture.culture_name);
                                armies[i].ArmyRegiments[x].Regiments[y].Culture.SetHeritage(culture.heritage_name);
                            }
                        }
                    }
                }
            }
        }

        /*##############################################
         *####              CHARACTERS              #### 
         *####--------------------------------------####
         *####      Reader for the living file      ####
         *##############################################
         */

        public static (bool searchStarted, Army searchingArmy, bool isCommander, bool isMainCommander, bool isKnight, Knight knight) SearchCharacters(string id, List<Army> armies)
        {
            foreach (Army army in armies)
            {
                //Main Commanders
                if (army.Commander != null && id == army.Commander.ID)
                {
                    return (true, army, false, true,false, null);
                }

                //Commanders
                if (id == army.CommanderID)
                {
                    return (true, army, true, false, false, null);
                }

                // KNIGHTS
                else if (army.Knights.GetKnightsList() != null)
                {
                    foreach (var knight in army.Knights.GetKnightsList())
                    {
                        if (id == knight.GetID())
                        {
                            return (true, army, false, false,true, knight);
                        }
                    }
                }
                //ARMY OWNER
                else if (id == army.Owner)
                {
                    return (true, army, false, false,false, null);
                }
            }

            return (false, null, false, false,false, null);
        }

        /*##############################################
         *####                UNITS                 #### 
         *####--------------------------------------####
         *####   Conversion of regiments to units   ####
         *##############################################
         */

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
