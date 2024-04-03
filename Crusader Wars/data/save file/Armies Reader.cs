using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Crusader_Wars.data.save_file
{
    public class Unit
    {
        string Name { get; set; }
        RegimentType Type { get; set; }
        Culture UnitCulture { get; set; }
        int Soldiers {  get; set; }
        string AttilaKey {  get; set; }
        string AttilaFaction {  get; set; }
        int Max {  get; set; }
        
        public Unit(string regiment_name, int soldiers ,Culture culture_obj, RegimentType type) 
        {
            Name = regiment_name;
            UnitCulture = culture_obj;
            Soldiers = soldiers;
            Type = type;
        }


        public void SetAttilaFaction(string a) { AttilaFaction = a; }
        public void SetUnitKey(string unit_key) { AttilaKey = unit_key; }
        public void ChangeName(string y) {  Name = y; }
        public void SetMax(int i ) {  Max = i; }


        public int GetMax() { return Max; }
        public string GetAttilaFaction() { return AttilaFaction; }
        public string GetAttilaUnitKey() { return AttilaKey; }
        public string GetName() { return Name; }
        public Culture GetObjCulture() { return UnitCulture; }
        public string GetCulture() { if (UnitCulture is null) return "Mercenary Placeholder"; return UnitCulture.GetCultureName(); }
        public string GetHeritage() { if (UnitCulture is null) return "Mercenary Placeholder"; return UnitCulture.GetHeritageName(); }
        public int GetSoldiers() { return Soldiers; }
        public RegimentType GetRegimentType() { return Type; }


    }
    public enum RegimentType
    {
        MenAtArms,
        Levy,
        Knight
    };

    public class ArmyRegiment
    {
        public string ID {  get; private set; }
        public RegimentType Type {  get; private set; }
        public string MAA_Name { get; private set; }
        public List<Regiment> Regiments { get; private set; }


        public ArmyRegiment(string id)
        {
            ID = id;
        }

        public void SetType(RegimentType type)
        {
            Type = type;
        }
        public void SetType(RegimentType type, string maa_name)
        {
            MAA_Name = maa_name;
            Type = type;
        }

        public void SetRegiments (List<Regiment> regiments)
        {
            Regiments = regiments;
            
        }

        


    }

    public class Regiment
    {
        public string ID { get; private set; }
        public string Index {  get; private set; }
        public string Origin {  get; private set; }
        public string Owner { get; private set; }
        public string StartingNum { get;private set; }
        public string CurrentNum {  get; private set; }
        public Culture Culture { get; private set; }
        string county_key { get; set; }
        bool IsMercenary { get; set; }

        public Regiment(string id, string index) 
        {
            ID = id;
            Index = index;
        }

        //Getters
        public string GetCountyKey() { return county_key; }
        public bool isMercenary() { return IsMercenary; }

        //Setters
        public void SetOwner(string o) {  Owner = o; }
        public void SetStartingSoldiers(string o) { StartingNum = o; }
        public void SetCulture(string id) { Culture = new Culture(id); }
        public void isMercenary(bool t) { IsMercenary = t; }
        public void SetOrigin(string origin) {  Origin = origin; }
        public void SetSoldiers(string  soldiers) { CurrentNum = soldiers; }
        public void StoreCountyKey(string key) {  county_key = key; }
    }

    public class Culture
    {
        public string ID { get; private set; }
        string CultureName { get; set; }
        string HeritageName { get; set; }

        public Culture(string id)
        {
            ID = id;
        }

        public string GetCultureName() {
            if (CultureName is null)
                return "";
            else
                return CultureName; 
        }
        public string GetHeritageName() { return HeritageName; }


        public void SetName(string t) { CultureName = t; }
        public void SetHeritage(string t) { HeritageName = t; }

    }

    static class ArmiesReader
    {

        // V1.0 Beta
        static List<Army> attacker_armies;
        static List<Army> defender_armies;

        public static void ReadCombats(string g)
        {
            ReadCombatArmies(g);
        }
        public static (List<Army> attacker, List<Army> defender) ReadBattleArmies()
        {
            ReadArmiesData();
            ReadArmyRegiments();
            ReadRegiments();
            ReadProvincesFiles();
            ReadCountiesManager();
            ReadCultureManager();

            for(int i = 0; i < attacker_armies.Count;i++)
            {
                attacker_armies[i].ClearEmptyRegimnts();
            }
            for (int i = 0; i < defender_armies.Count; i++)
            {
                defender_armies[i].ClearEmptyRegimnts();
            }

            OrganizeIntoUnits();

            PrintArmiesData();

            return (attacker_armies, defender_armies);
        }


        private static List<Unit> ReadUnitMapper(List<Unit> units)
        {
            //Get Unit Mapper Faction
            foreach(var unit in units)
            {
               unit.SetAttilaFaction(UnitMapper.GetAttilaFaction(unit.GetCulture(), unit.GetHeritage()));                
            }

            //Read Unit Limit
            foreach(var unit in units)
            {
                unit.SetMax(UnitMapper.GetMax(unit));
            }

            //Set Unit Keys
            foreach(var unit in units)
            {
                string key = UnitMapper.GetUnitKey(unit);
                if (key == "not_found")
                    unit.SetUnitKey("cha_spa_royal_cav");
                else 
                    unit.SetUnitKey(key);
            }

            return units;
        }

        private static List<Unit> OrganizeLeviesUnits(List<Unit> units)
        {
            // Filter Levy units
            var levyUnits = units.Where(u => u.GetName() == "Levy").OrderByDescending(u => u.GetSoldiers()).ToList();

            // Calculate percentages
            int totalLevyUnits = units.Sum(u => u.GetSoldiers());
            int firstPercentage = (int)(totalLevyUnits * 0.65);
            int secondPercentage = (int)(totalLevyUnits * 0.25);

            // Distribute Levy units into three lists
            var firstList = new List<Unit>();
            var secondList = new List<Unit>();
            var thirdList = new List<Unit>();

            int currentSoldiers = 0;
            if(levyUnits.Count == 1)
            {
                int levy_spearmen = (int)(totalLevyUnits * 0.65);
                int levy_infantry = (int)(totalLevyUnits * 0.25);
                int levy_ranged = (int)(totalLevyUnits * 0.10);

                levyUnits = new List<Unit>{ new Unit("Levy Spearmen", levy_spearmen, levyUnits[0].GetObjCulture(), RegimentType.Levy),
                                            new Unit("Levy Infantry", levy_infantry, levyUnits[0].GetObjCulture(), RegimentType.Levy),
                                            new Unit("Levy Ranged", levy_ranged, levyUnits[0].GetObjCulture(), RegimentType.Levy) };


                return levyUnits;
            }
            else
            {
                foreach (var unit in levyUnits)
                {

                    if (currentSoldiers < firstPercentage)
                    {
                        unit.ChangeName("Levy Spearmen");
                    }
                    else if (currentSoldiers < firstPercentage + secondPercentage)
                    {
                        unit.ChangeName("Levy Infantry");
                    }
                    else
                    {
                        unit.ChangeName("Levy Ranged");
                    }

                    currentSoldiers += unit.GetSoldiers();
                }

                return levyUnits;
            }

        }

        private static string GetCharacterCultureID(string character_id)
        {
            bool isSearchStarted = false;
            string culture_id = "";
            using (StringReader stringReader = new StringReader(Data.String_Living))
            {
                while(true)
                {
                    string line = stringReader.ReadLine();
                    if (line == null) break;

                    if(line == $"{character_id}={{" && !isSearchStarted)
                    {
                        isSearchStarted = true;
                    }
                    
                    if(isSearchStarted && Regex.IsMatch(line, @"\tculture=\d+"))
                    {
                        culture_id = Regex.Match(line, @"\tculture=(\d+)").Groups[1].Value;
                        return culture_id;
                    }

                    if(isSearchStarted && line == "}")
                    {
                        return "";
                    }
                }

                return culture_id;
            }

        }

        public static List<Unit> OrganizeUnitsIntoCultures(List<Unit> units)
        {
            var organizedUnits = new List<Unit>();

            // Group units by Name and Culture
            var groupedUnits = units.GroupBy(u => new { Name = u.GetName(), Culture = u.GetCulture() });

            // Merge units with the same Name and Culture
            foreach (var group in groupedUnits)
            {
                int totalSoldiers = group.Sum(u => u.GetSoldiers());

                // Create a new Unit with the merged NumberOfSoldiers
                Unit mergedUnit = new Unit(group.Key.Name, totalSoldiers, group.ElementAt(0).GetObjCulture(), group.ElementAt(0).GetRegimentType());

                organizedUnits.Add(mergedUnit);
            }

            return organizedUnits;
        }



        private static void OrganizeIntoUnits()
        {
            foreach(var army in attacker_armies)
            {
                List<(Regiment regiment, RegimentType type, string maa_name)> list = new List<(Regiment regiment, RegimentType type, string maa_name)>(); 
                foreach(var army_regiment in army.ArmyRegiments)
                {

                    foreach(var regiment in  army_regiment.Regiments)
                    {
                        list.Add((regiment, army_regiment.Type, army_regiment.MAA_Name));   
                    }
                }            
                
                List<Unit> units = new List<Unit>();
                foreach (var regiment in list)
                {
                    // if no soldiers, skip
                    if (regiment.regiment.CurrentNum is null) continue;
                    if (Int32.Parse(regiment.regiment.CurrentNum) == 0) continue;

                    Unit unit;
                    if(regiment.type == RegimentType.Levy)  
                        unit = new Unit("Levy", Int32.Parse(regiment.regiment.CurrentNum), regiment.regiment.Culture, regiment.type);
                    else if (regiment.type == RegimentType.MenAtArms)
                        unit = new Unit(regiment.maa_name, Int32.Parse(regiment.regiment.CurrentNum), regiment.regiment.Culture, regiment.type);
                    else
                        continue;

                    if(unit != null)
                        units.Add(unit);

                    
                }

                units = OrganizeUnitsIntoCultures(units);
                var levies = OrganizeLeviesUnits(units);
                units.RemoveAll(u => u.GetName() == "Levy");
                units.InsertRange(0, levies);
                units = ReadUnitMapper(units);

                army.SetUnits(units);

                army.PrintUnits();
            }

            foreach (var army in defender_armies)
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
                    if (regiment.regiment.CurrentNum is null) continue;
                    if (Int32.Parse(regiment.regiment.CurrentNum) == 0) continue;

                    Unit unit;
                    if (regiment.type == RegimentType.Levy)
                        unit = new Unit("Levy", Int32.Parse(regiment.regiment.CurrentNum), regiment.regiment.Culture, regiment.type);
                    else if (regiment.type == RegimentType.MenAtArms)
                        unit = new Unit(regiment.maa_name, Int32.Parse(regiment.regiment.CurrentNum), regiment.regiment.Culture, regiment.type);
                    else
                        continue;

                    if (unit != null)
                        units.Add(unit);


                }

                units = OrganizeUnitsIntoCultures(units);
                var levies = OrganizeLeviesUnits(units);
                units.RemoveAll(u => u.GetName() == "Levy");
                units.InsertRange(0, levies);
                units = ReadUnitMapper(units);

                army.SetUnits(units);
                army.PrintUnits();
            }
        }

        private static void PrintArmiesData()
        {
            foreach (var i in attacker_armies)
            {
                Console.WriteLine($"#Army - {i.ID} | {i.CombatSide} | Commander {i.CommanderID}");
                Console.WriteLine("------------------------------------------------------------");
                foreach (var x in i.ArmyRegiments)
                {
                    if (x.Type == RegimentType.Knight)
                    Console.WriteLine($"##Army Regiment - {x.ID} |{x.Type} | Character ID {x.MAA_Name}");
                    else
                    Console.WriteLine($"##Army Regiment - {x.ID} |{x.Type} | {x.MAA_Name}");
                    foreach (var t in x.Regiments)
                    {
                        if(!t.isMercenary())
                            if(t.Culture is null)
                                Console.WriteLine($"## ## Chunk Regiment: {t.ID} | Owner: {t.Owner} |Index: {t.Index} | Origin: {t.Origin} | Soldiers: {t.CurrentNum} | County Key: {t.GetCountyKey()} | Culture ID: null");
                            else
                                Console.WriteLine($"## ## Chunk Regiment: {t.ID} | Owner: {t.Owner} | Index: {t.Index} | Origin: {t.Origin} | Soldiers: {t.CurrentNum} | County Key: {t.GetCountyKey()} | Culture: {t.Culture.GetCultureName()} | Heritage: {t.Culture.GetHeritageName()}");


                        else
                            if(t.Culture is null)
                            Console.WriteLine($"## ## Mercenary Chunk Regiment: {t.ID} | Owner: {t.Owner} | Index: {t.Index} | Origin: {t.Origin} | Soldiers: {t.CurrentNum} | County Key: {t.GetCountyKey()} | Culture ID: null");
                            else
                            Console.WriteLine($"## ## Mercenary Chunk Regiment: {t.ID} | Owner: {t.Owner} | Index: {t.Index} | Origin: {t.Origin} | Soldiers: {t.CurrentNum} | County Key: {t.GetCountyKey()} | Culture: {t.Culture.GetCultureName()} | Heritage: {t.Culture.GetHeritageName()}");


                    }

                }
                Console.WriteLine("\n");
            }

            Console.WriteLine("\n");
            Console.WriteLine("\n");

            foreach (var i in defender_armies)
            {
                Console.WriteLine($"#Army - {i.ID} | {i.CombatSide} | Commander {i.CommanderID}");
                Console.WriteLine("------------------------------------------------------------");
                foreach (var x in i.ArmyRegiments)
                {
                    if (x.Type == RegimentType.Knight)
                        Console.WriteLine($"##Army Regiment - {x.ID} |{x.Type} | Character ID {x.MAA_Name}");
                    else
                        Console.WriteLine($"##Army Regiment - {x.ID} |{x.Type} | {x.MAA_Name}");
                    foreach (var t in x.Regiments)
                    {
                        if (!t.isMercenary())
                            if (t.Culture is null)
                                Console.WriteLine($"## ## Chunk Regiment: {t.ID} | Owner: {t.Owner} | Index: {t.Index} | Origin: {t.Origin} | Soldiers: {t.CurrentNum} | County Key: {t.GetCountyKey()} | Culture ID: null");
                            else
                                Console.WriteLine($"## ## Chunk Regiment: {t.ID} | Owner: {t.Owner} | Index: {t.Index} | Origin: {t.Origin} | Soldiers: {t.CurrentNum} | County Key: {t.GetCountyKey()} | Culture: {t.Culture.GetCultureName()} | Heritage: {t.Culture.GetHeritageName()}");
                        else
                            if(t.Culture is null)
                                Console.WriteLine($"## ## Mercenary Chunk Regiment: {t.ID} | Owner: {t.Owner} | Index: {t.Index} | Origin: {t.Origin} | Soldiers: {t.CurrentNum} | County Key: {t.GetCountyKey()} | Culture ID: null");
                            else
                                Console.WriteLine($"## ## Mercenary Chunk Regiment: {t.ID} | Owner: {t.Owner} | Index: {t.Index} | Origin: {t.Origin} | Soldiers: {t.CurrentNum} | County Key: {t.GetCountyKey()} | Culture: {t.Culture.GetCultureName()} | Heritage: {t.Culture.GetHeritageName()}");
                    }
                }
                Console.WriteLine("\n");
            }
        }

        private static void ReadMercenaries()
        {
            
        }

        private static void ReadCultureManager()
        {
            bool isSearchStared = false;
            string culture_id = "";
            string culture_name = "";
            string heritage_name = "";
            List<(string culture_id, string culture_name, string heritage_name)> found_cultures = new List<(string culture_id, string culture_name, string heritage_name)> ();

            using (StringReader sr = new StringReader(Data.String_Cultures))
            {
                while(true)
                {
                    string line = sr.ReadLine();
                    if (line == null) break;

                    //Culture Line
                    if(Regex.IsMatch(line, @"\t\t\d+={") && !isSearchStared)
                    {
                        culture_id = Regex.Match(line, @"\t\t(\d+)={").Groups[1].Value;
                        
                        //
                        // ATTACKER REGIMENTS

                        //Armies
                        for (int i = 0; i < attacker_armies.Count; i++)
                        {
                            if (isSearchStared) break;
                            //Army Regiments
                            for (int x = 0; x < attacker_armies[i].ArmyRegiments.Count; x++)
                            {
                                if (isSearchStared) break;
                                //Regiments
                                for (int y = 0; y < attacker_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                {

                                    //if culture is null, skip
                                    if (attacker_armies[i].ArmyRegiments[x].Regiments[y].Culture is null)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        string regiment_culture_id = attacker_armies[i].ArmyRegiments[x].Regiments[y].Culture.ID;
                                       
                                        if (regiment_culture_id == culture_id)
                                        {
                                            isSearchStared = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (!isSearchStared)
                        {
                            //
                            // DEFENDER REGIMENTS

                            //Armies
                            for (int i = 0; i < defender_armies.Count; i++)
                            {
                                if (isSearchStared) break;
                                //Army Regiments
                                for (int x = 0; x < defender_armies[i].ArmyRegiments.Count; x++)
                                {
                                    if (isSearchStared) break;
                                    //Regiments
                                    for (int y = 0; y < defender_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                    {
                                        //if culture is null, skip
                                        if (defender_armies[i].ArmyRegiments[x].Regiments[y].Culture is null)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            string regiment_culture_id = defender_armies[i].ArmyRegiments[x].Regiments[y].Culture.ID;

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
                    }


                    //Culture Name
                    if(isSearchStared && line.Contains("\t\t\tname="))
                    {
                        culture_name = Regex.Match(line, @"""(\w+)""").Groups[1].Value;
                        culture_name = FirstCharToUpper(culture_name);
                        
                    }
                    //Heritage Name
                    else if(isSearchStared && line.Contains("\t\t\theritage="))
                    {
                        heritage_name = Regex.Match(line, @"""(\w+)""").Groups[1].Value;
                        heritage_name = heritage_name.Replace("heritage_", "");
                        heritage_name = heritage_name.Replace("_", " ");
                        heritage_name = FirstCharToUpper(heritage_name);

                    }

                    if(isSearchStared && line == "\t\t}")
                    {
                        found_cultures.Add((culture_id, culture_name, heritage_name));
                        isSearchStared = false;
                    }
                }

                foreach(var culture in found_cultures)
                {
                    //
                    //  ATTACKER REGIMENTS

                    //Armies
                    for (int i = 0; i < attacker_armies.Count; i++)
                    {
                        //Army Regiments
                        for (int x = 0; x < attacker_armies[i].ArmyRegiments.Count; x++)
                        {
                            //Regiments
                            for (int y = 0; y < attacker_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                            {
                                if (attacker_armies[i].ArmyRegiments[x].Regiments[y].Culture is null) { continue; }

                                string regiment_culture_id = attacker_armies[i].ArmyRegiments[x].Regiments[y].Culture.ID;
                                if (culture.culture_id == regiment_culture_id)
                                {
                                    attacker_armies[i].ArmyRegiments[x].Regiments[y].Culture.SetName(culture.culture_name);
                                    attacker_armies[i].ArmyRegiments[x].Regiments[y].Culture.SetHeritage(culture.heritage_name);
                                }
                            }
                        }
                    }

                    //
                    //  DEFENDER REGIMENTS
                    //Armies
                    for (int i = 0; i < defender_armies.Count; i++)
                    {
                        //Army Regiments
                        for (int x = 0; x < defender_armies[i].ArmyRegiments.Count; x++)
                        {
                            //Regiments
                            for (int y = 0; y < defender_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                            {
                                if (defender_armies[i].ArmyRegiments[x].Regiments[y].Culture is null) { continue; }

                                string regiment_culture_id = defender_armies[i].ArmyRegiments[x].Regiments[y].Culture.ID;
                                if (culture.culture_id == regiment_culture_id)
                                {
                                    defender_armies[i].ArmyRegiments[x].Regiments[y].Culture.SetName(culture.culture_name);
                                    defender_armies[i].ArmyRegiments[x].Regiments[y].Culture.SetHeritage(culture.heritage_name);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input[0].ToString().ToUpper() + input.Substring(1);
            }
        }
        private static void ReadCountiesManager()
        {

            List<(string county_key, string culture_id)> FoundCounties = new List<(string county_key, string culture_id)>();

            bool isSearchStared = false;
            string county_key = "";
            using (StringReader sr = new StringReader(Data.String_Counties))
            {
                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null) break;

                    //County Line
                    if(Regex.IsMatch(line,@"\t\t\w+={") && !isSearchStared)
                    {
                        county_key = Regex.Match(line, @"\t\t(\w+)={").Groups[1].Value;

                        //
                        // ATTACKER REGIMENTS

                        //Armies
                        for (int i = 0; i < attacker_armies.Count; i++)
                        {
                            if (isSearchStared) break;
                            //Army Regiments
                            for (int x = 0; x < attacker_armies[i].ArmyRegiments.Count; x++)
                            {
                                if (isSearchStared) break;
                                //Regiments
                                for (int y = 0; y < attacker_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                {
                                    
                                    //if county key is empty, skip
                                    if (string.IsNullOrEmpty(attacker_armies[i].ArmyRegiments[x].Regiments[y].GetCountyKey()))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        string regiment_county_key = attacker_armies[i].ArmyRegiments[x].Regiments[y].GetCountyKey();
                                        if (regiment_county_key == county_key)
                                        {
                                            isSearchStared = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (!isSearchStared)
                        {
                            //
                            // DEFENDER REGIMENTS

                            //Armies
                            for (int i = 0; i < defender_armies.Count; i++)
                            {
                                if (isSearchStared) break;
                                //Army Regiments
                                for (int x = 0; x < defender_armies[i].ArmyRegiments.Count; x++)
                                {
                                    if (isSearchStared) break;
                                    //Regiments
                                    for (int y = 0; y < defender_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                    {
                                        //if county key is empty, skip
                                        if (string.IsNullOrEmpty(defender_armies[i].ArmyRegiments[x].Regiments[y].GetCountyKey()))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            string regiment_county_key = defender_armies[i].ArmyRegiments[x].Regiments[y].GetCountyKey();
                                            if (regiment_county_key == county_key)
                                            {
                                                isSearchStared = true;
                                                break;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }


                    //Culture ID
                    if(isSearchStared && line.Contains("\t\t\tculture=")) 
                    {
                        string culture_id = Regex.Match(line, @"\t\t\tculture=(\d+)").Groups[1].Value;
                        FoundCounties.Add((county_key, culture_id));                        
                    }

                    // County End Line
                    if(isSearchStared && line == "\t\t}")
                    {
                        isSearchStared = false;
                    }


                }

                List<(string char_id, string culture_id)> temp_characters_cultures = new List<(string char_id, string culture_id)>();
                //Populate regiments with culture id's
                foreach(var county in FoundCounties)
                {
                    //
                    //  ATTACKER REGIMENTS

                    //Armies
                    for (int i = 0; i < attacker_armies.Count; i++)
                    {
                        //Army Regiments
                        for (int x = 0; x < attacker_armies[i].ArmyRegiments.Count; x++)
                        {
                            //Regiments
                            for (int y = 0; y < attacker_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                            {
                                string regiment_county_key = attacker_armies[i].ArmyRegiments[x].Regiments[y].GetCountyKey();
                                string owner = attacker_armies[i].ArmyRegiments[x].Regiments[y].Owner;
                                if (!string.IsNullOrEmpty(owner))
                                {
                                    if(temp_characters_cultures.Exists(t => t.char_id == owner))
                                    {
                                        string culture_id = temp_characters_cultures.FirstOrDefault(p => p.char_id == owner).culture_id;
                                        attacker_armies[i].ArmyRegiments[x].Regiments[y].SetCulture(culture_id);
                                        continue;
                                    }
                                    else
                                    {
                                        string culture_id = GetCharacterCultureID(owner);
                                        temp_characters_cultures.Add((owner, culture_id));
                                        attacker_armies[i].ArmyRegiments[x].Regiments[y].SetCulture(culture_id);
                                        continue;
                                    }

                                }
                                
                                if (county.county_key == regiment_county_key )
                                {
                                    attacker_armies[i].ArmyRegiments[x].Regiments[y].SetCulture(county.culture_id);
                                }
                            }
                        }
                    }

                    //
                    //  DEFENDER REGIMENTS

                    //Armies
                    for (int i = 0; i < defender_armies.Count; i++)
                    {
                        //Army Regiments
                        for (int x = 0; x < defender_armies[i].ArmyRegiments.Count; x++)
                        {
                            //Regiments
                            for (int y = 0; y < defender_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                            {
                                string regiment_county_key = defender_armies[i].ArmyRegiments[x].Regiments[y].GetCountyKey();
                                string owner = defender_armies[i].ArmyRegiments[x].Regiments[y].Owner;
                                if (!string.IsNullOrEmpty(owner))
                                {
                                    if (temp_characters_cultures.Exists(t => t.char_id == owner))
                                    {
                                        string culture_id = temp_characters_cultures.FirstOrDefault(p => p.char_id == owner).culture_id;
                                        defender_armies[i].ArmyRegiments[x].Regiments[y].SetCulture(culture_id);
                                        continue;
                                    }
                                    else
                                    {
                                        string culture_id = GetCharacterCultureID(owner);
                                        temp_characters_cultures.Add((owner, culture_id));
                                        defender_armies[i].ArmyRegiments[x].Regiments[y].SetCulture(culture_id);
                                        continue;
                                    }
                                }
                                if (county.county_key == regiment_county_key)
                                {
                                    defender_armies[i].ArmyRegiments[x].Regiments[y].SetCulture(county.culture_id);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string SearchForCounty(string barony_name)
        {

            string county_search_name = barony_name.ToLower();
            county_search_name = "c_" + county_search_name;
            using(StringReader  sr = new StringReader(Data.String_Counties))
            {
                while(true)
                {
                    string line = sr.ReadLine();
                    if (line == null) break;

                    if(line == $"\t\t{county_search_name}={{")
                    {
                        return county_search_name;
                    }
                }

                return "";
            }
        }

        private static void ReadProvincesFiles()
        {
            //TODO: MODS

            //DEFAULT CK3

            string ck3_exe_path = Properties.Settings.Default.VAR_ck3_path;
            string default_provinces_path = Regex.Replace(ck3_exe_path, @"binaries\\ck3.exe", @"game\\history\\provinces");

            var dir_files = Directory.GetFiles(default_provinces_path);

            string county_key = "";
            foreach(var file_path in dir_files)
            {
                using(StreamReader reader = new StreamReader(file_path))
                {
                    string line = reader.ReadLine();
                    while(!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        if (line == null) break;
                        

                        //County Key
                        if(Regex.IsMatch(line, @"###\w+")) // FILE TYPE 1
                        {
                            county_key = Regex.Match(line, @"###(\w+)").Groups[1].Value;
                        }
                        else if(Regex.IsMatch(line, @"# \d+ - \w+")) // FILE TYPE 2
                        {
                            string barony_name = Regex.Match(line, @"# \d+ - (\w+)").Groups[1].Value;
                            county_key = SearchForCounty(barony_name);
                        }



                        //Province Line
                        if(Regex.IsMatch(line, @"\d+ = {"))
                        {
                            //
                            // ATTACKER REGIMENTS

                            //Armies
                            for (int i = 0; i < attacker_armies.Count; i++)
                            {
                                //Army Regiments
                                for (int x = 0; x < attacker_armies[i].ArmyRegiments.Count; x++)
                                {
                                    //Regiments
                                    for (int y = 0; y < attacker_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                    {

                                        //if county key is set, skip
                                        if(!string.IsNullOrEmpty(attacker_armies[i].ArmyRegiments[x].Regiments[y].GetCountyKey())) {
                                            continue;
                                        }
                                        string origin = attacker_armies[i].ArmyRegiments[x].Regiments[y].Origin;

                                        //Regiment Province Origin
                                        if (line.Contains($@"{origin} = {{") && !attacker_armies[i].ArmyRegiments[x].Regiments[y].isMercenary())
                                        {
                                            attacker_armies[i].ArmyRegiments[x].Regiments[y].StoreCountyKey(county_key);
                                        }
                                    }
                                }
                            }

                            //
                            // DEFENDER REGIMENTS

                            //Armies
                            for (int i = 0; i < defender_armies.Count; i++)
                            {
                                //Army Regiments
                                for (int x = 0; x < defender_armies[i].ArmyRegiments.Count; x++)
                                {
                                    //Regiments
                                    for (int y = 0; y < defender_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                    {
                                        //if county key is set, skip
                                        if (!string.IsNullOrEmpty(defender_armies[i].ArmyRegiments[x].Regiments[y].GetCountyKey()))
                                        {
                                            continue;
                                        }
                                        string origin = defender_armies[i].ArmyRegiments[x].Regiments[y].Origin;

                                        //Regiment Province Origin
                                        if (line.Contains($@"{origin} = {{") && !defender_armies[i].ArmyRegiments[x].Regiments[y].isMercenary())
                                        {
                                            defender_armies[i].ArmyRegiments[x].Regiments[y].StoreCountyKey(county_key);
                                        }
                                    }
                                }
                            }

                        }



                    }


                    reader.Close();
                }
            }

        }

        private static void ReadRegiments()
        {
            bool isSearchStarted = false;
            bool isDefender = false, isAttacker = false;
            int army_index = 0, army_regiment_index = 0, regiment_index = 0;
            int index = -1;
            int reg_chunk_index = 0;
            using (StringReader SR = new StringReader(Data.String_Regiments))
            {
                while (true)
                {
                    string line = SR.ReadLine();
                    if (line == null) break;


                    // Regiment ID Line
                    if (Regex.IsMatch(line, @"\t\t\d+={") && !isSearchStarted)
                    {
                        string regiment_id = Regex.Match(line, @"\t\t(\d+)={").Groups[1].Value;
                        for (int i = 0; i < attacker_armies.Count; i++)
                        {
                            if (isSearchStarted) break;
                            for (int x = 0; x < attacker_armies[i].ArmyRegiments.Count; x++)
                            {
                                if (isSearchStarted) break;
                                for (int y = 0; y < attacker_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                {
                                    if (y == attacker_armies[i].ArmyRegiments[x].Regiments.Count) break;
                                    string id = attacker_armies[i].ArmyRegiments[x].Regiments[y].ID;
                                    if(id == regiment_id)
                                    {
                                        army_index = i;
                                        army_regiment_index = x;
                                        regiment_index = y;
                                        isAttacker = true;
                                        isDefender = false;
                                        isSearchStarted = true;
                                        break;
                                    }
                                }

                            }
                        }

                        if (!isSearchStarted)
                        {
                            for (int i = 0; i < defender_armies.Count; i++)
                            {
                                if (isSearchStarted) break;
                                for (int x = 0; x < defender_armies[i].ArmyRegiments.Count; x++)
                                {
                                    if (isSearchStarted) break;
                                    for (int y = 0; y < defender_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                    {
                                        string id = defender_armies[i].ArmyRegiments[x].Regiments[y].ID;
                                        if (id == regiment_id)
                                        {
                                            army_index = i;
                                            army_regiment_index = x;
                                            regiment_index = y;
                                            isDefender = true;
                                            isAttacker = false;
                                            isSearchStarted = true;
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                    }


                    // Index Counter
                    if(line == "\t\t\t\t{" && isSearchStarted)
                    {
                        if (isAttacker)
                        {
                            string str_index = attacker_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].Index;
                            if (!string.IsNullOrEmpty(str_index))
                            {
                                reg_chunk_index = Int32.Parse(str_index);
                                index++;
                            }
                            else
                            {
                                reg_chunk_index = 0;
                                index = 0;
                            }
                            
                        }
                        else if (isDefender)
                        {
                            string str_index = defender_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].Index;
                            if (!string.IsNullOrEmpty(str_index))
                            {
                                reg_chunk_index = Int32.Parse(str_index);
                                index++;
                            }
                            else
                            {
                                reg_chunk_index = 0;
                                index = 0;
                            }
                        }
                    }

                    // isMercenary 
                    else if (isSearchStarted && line.Contains("\t\t\tsource=hired"))
                    {
                        if (isAttacker)
                        {
                            attacker_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].isMercenary(true);
                        }
                        else if (isDefender)
                        {
                            defender_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].isMercenary(true);
                        }

                    }
                    // Origin 
                    else if (isSearchStarted && line.Contains("\t\t\torigin="))
                    {
                        string origin = Regex.Match(line, @"\d+").Value;

                        if (isAttacker)
                        {
                            attacker_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].SetOrigin(origin);
                        }
                        else if (isDefender)
                        {
                            defender_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].SetOrigin(origin);
                        }

                    }
                    // Owner 
                    else if (isSearchStarted && line.Contains("\t\t\towner="))
                    {
                        string owner = Regex.Match(line, @"\d+").Value;

                        if (isAttacker)
                        {
                            attacker_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].SetOwner(owner);
                        }
                        else if (isDefender)
                        {
                            defender_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].SetOwner(owner);
                        }

                    }

                    // Soldiers
                    else if (isSearchStarted && line.Contains("\t\t\t\t\tcurrent="))
                    {
                        string current = Regex.Match(line, @"\d+").Value;

                        if (isAttacker && index == reg_chunk_index)
                        {
                            attacker_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].SetSoldiers(current);
                        }
                        else if (isDefender && index == reg_chunk_index)
                        {
                            defender_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].SetSoldiers(current);
                        }

                    }
                    // Max "StartingNum"
                    else if (isSearchStarted && line.Contains("\t\t\t\t\tmax="))
                    {
                        string max = Regex.Match(line, @"\d+").Value;

                        if (isAttacker && index == reg_chunk_index)
                        {
                            attacker_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].SetStartingSoldiers(max);
                        }
                        else if (isDefender && index == reg_chunk_index)
                        {
                            defender_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].SetStartingSoldiers(max);
                        }

                    }

                    //Regiment End Line
                    else if (isSearchStarted && line == "\t\t}")
                    {
                        isSearchStarted = false;
                        isAttacker = false;
                        isDefender = false;
                        index = -1;
                        army_index=0;
                        army_regiment_index=0;
                        regiment_index = 0;

                    }

                }
            }
        }

        
        private static void ReadArmyRegiments()
        {
            List<Regiment> found_regiments = new List<Regiment>();

            bool isSearchStarted = false;
            bool isDefender = false, isAttacker = false;
            int army_index = 0;
            int army_regiment_index = 0;

            string regiment_id = "";
            string index = "";

            bool isNameSet = false;


            using (StringReader SR = new StringReader(Data.String_ArmyRegiments))
            {
                while (true)
                {
                    string line = SR.ReadLine();

                    if (line == null) break;

                    // Army Regiment ID Line
                    if (Regex.IsMatch(line, @"\t\t\d+={") && !isSearchStarted)
                    {
                        string army_regiment_id = Regex.Match(line, @"\t\t(\d+)={").Groups[1].Value;
                        for (int i = 0; i < attacker_armies.Count; i++)
                        {
                            for(int x = 0; x < attacker_armies[i].ArmyRegiments.Count; x++)
                            {
                                string id = attacker_armies[i].ArmyRegiments[x].ID;
                                if (id == army_regiment_id)
                                {
                                    army_index = i;
                                    army_regiment_index = x;
                                    isAttacker = true;
                                    isSearchStarted = true;
                                    break;
                                }
                            }
                        }

                        if (!isSearchStarted)
                        {
                            for (int i = 0; i < defender_armies.Count; i++)
                            {
                                for (int x = 0; x < defender_armies[i].ArmyRegiments.Count; x++)
                                {
                                    string id = defender_armies[i].ArmyRegiments[x].ID;
                                    if (id == army_regiment_id)
                                    {
                                        army_index = i;
                                        army_regiment_index = x;
                                        isDefender = true;
                                        isSearchStarted = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    //Regiment ID
                    if(isSearchStarted && line.Contains("\t\t\t\t\tregiment="))
                    {
                        if(isNameSet == false)
                        {
                            if (isAttacker)
                            {
                                attacker_armies[army_index].ArmyRegiments[army_regiment_index].SetType(RegimentType.Levy);
                            }
                            else if (isDefender)
                            {
                                defender_armies[army_index].ArmyRegiments[army_regiment_index].SetType(RegimentType.Levy);
                            }
                        }

                        regiment_id = Regex.Match(line, @"(\d+)").Groups[1].Value;                    

                    }

                    //Regiment Index
                    else if (isSearchStarted && line.Contains("\t\t\t\t\tindex="))
                    {
                        index = Regex.Match(line, @"(\d+)").Groups[1].Value;
                    }

                    //Add Found Regiment
                    else if (isSearchStarted && line == "\t\t\t\t}")
                    {
                        Regiment regiment = new Regiment(regiment_id, index);
                        found_regiments.Add(regiment);
                    }

                    //Men At Arms
                    else if (isSearchStarted && line.Contains("\t\t\ttype="))
                    {
                        string type = Regex.Match(line, "\"(.+)\"").Groups[1].Value;
                        if (isAttacker)
                        {
                            attacker_armies[army_index].ArmyRegiments[army_regiment_index].SetType(RegimentType.MenAtArms, type);
                        }
                        else if (isDefender)
                        {
                            defender_armies[army_index].ArmyRegiments[army_regiment_index].SetType(RegimentType.MenAtArms, type);
                        }
                        isNameSet = true;
                    }

                    //Knight
                    else if (isSearchStarted && line.Contains("\t\t\tknight="))
                    {
                        string character_id = Regex.Match(line, @"knight=(\d+)").Groups[1].Value;
                        if (isAttacker)
                        {
                            attacker_armies[army_index].ArmyRegiments[army_regiment_index].SetType(RegimentType.Knight, character_id);
                        }
                        else if (isDefender)
                        {
                            defender_armies[army_index].ArmyRegiments[army_regiment_index].SetType(RegimentType.Knight, character_id);
                        }
                        isNameSet = true;
                    }

                    
                    //Levies
                    else if (isSearchStarted && line == "\t\t\t\tlevies={")
                    {
                        if (isAttacker)
                        {
                            attacker_armies[army_index].ArmyRegiments[army_regiment_index].SetType(RegimentType.Levy);
                        }
                        else if (isDefender)
                        {
                            defender_armies[army_index].ArmyRegiments[army_regiment_index].SetType(RegimentType.Levy);
                        }
                        isNameSet = true;
                    }

                    // Army Regiment End Line
                    else if (line == "\t\t}" && isSearchStarted)
                    {

                        if (isAttacker)
                        {
                            attacker_armies[army_index].ArmyRegiments[army_regiment_index].SetRegiments(found_regiments);
                        }
                        else if (isDefender)
                        {
                            defender_armies[army_index].ArmyRegiments[army_regiment_index].SetRegiments(found_regiments);
                        }

                        found_regiments = new List<Regiment>();
                        isAttacker = false;
                        isDefender = false;
                        army_index = 0;
                        army_regiment_index = 0;
                        regiment_id = "";
                        index = "";
                        isSearchStarted = false;
                        isNameSet= false;


                    }

                }
            }
        }


        private static void ReadArmiesData()
        {
            bool isSearchStarted = false;
            bool isDefender = false, isAttacker = false;
            int index = 0;
            using(StringReader SR = new StringReader(Data.String_Armies))
            {
                while(true)
                {
                    string line = SR.ReadLine();
                    if (line == null) break;

                    // Army ID Line
                    if(Regex.IsMatch(line, @"\t\t\d+={") && !isSearchStarted)
                    {
                        // Check if it's an battle army

                        string army_id = Regex.Match(line, @"\t\t(\d+)={").Groups[1].Value;
                        for (int i = 0; i < attacker_armies.Count; i++)
                        {
                            if (attacker_armies[i].ID == army_id)
                            {
                                index = i;
                                isAttacker = true;
                                isDefender = false;
                                isSearchStarted = true;
                                break;
                            }

                        }
                        if(!isSearchStarted)
                        {
                            for (int i = 0; i < defender_armies.Count; i++)
                            {
                                if (defender_armies[i].ID == army_id)
                                {
                                    index = i;
                                    isDefender = true;
                                    isAttacker = false;
                                    isSearchStarted = true;
                                    break;
                                }
                            }
                        }

                    }

                    // Regiments ID's Line
                    if (isSearchStarted && line.Contains("\t\t\tregiments={"))
                    {
                        MatchCollection regiments_ids = Regex.Matches(line, @"(\d+) ");
                        List<ArmyRegiment> army_regiments = new List<ArmyRegiment>();
                        foreach(Match match in regiments_ids)
                        {
                            string id_ = match.Groups[1].Value;
                            ArmyRegiment army_regiment = new ArmyRegiment(id_);
                            army_regiments.Add(army_regiment);
                        }

                        if(isAttacker)
                        {
                            attacker_armies[index].SetArmyRegiments(army_regiments);
                        }
                        else if(isDefender)
                        {
                            defender_armies[index].SetArmyRegiments(army_regiments);
                        }

                    }
                    else if(isSearchStarted && line.Contains("\t\t\tcommander="))
                    {
                        string id = Regex.Match(line, @"commander=(\d+)").Groups[1].Value;
                        if (isAttacker)
                        {
                            attacker_armies[index].CommanderID = id;
                        }
                        else if (isDefender)
                        {
                            defender_armies[index].CommanderID = id;
                        }
                    }


                    // Army End Line
                    if(isSearchStarted && line == "\t\t}")
                    {
                        index = 0;
                        isAttacker = false;
                        isDefender = false;
                        isSearchStarted = false;
                    }

                }
            }
        }

        private static void ReadCombatArmies(string g)
        {
            bool isAttacker = false, isDefender = false;

            using (StringReader SR = new StringReader(g))//Player_Combat
            {
                while (true)
                {
                    string line = SR.ReadLine();
                    if (line == null) break;

                    if (line == "\t\t\tattacker={")
                    {
                        isAttacker = true;
                        isDefender = false;
                    }
                    else if (line == "\t\t\tdefender={")
                    {
                        isAttacker = false;
                        isDefender = true;
                    }
                    else if (line == "\t\t\t}")
                    {
                        isDefender = false;
                        isAttacker = false;
                    }

                    if (isAttacker && line.Contains("\t\t\t\tarmies={"))
                    {
                        MatchCollection found_armies = Regex.Matches(line, @"(\d+) ");
                        attacker_armies = new List<Army>();

                        for(int i = 0; i < found_armies.Count; i++)
                        {
                            //Create new Army with combat sides on the constructor
                            //Army army
                            string id = found_armies[i].Groups[1].Value;
                            string combat_side = "attacker";

                            // main army
                            if(i == 0)
                            {
                                Army army = new Army(id, combat_side, true);
                                attacker_armies.Add(army);
                            }
                            // ally army
                            else
                            {
                               Army army = new Army(id, combat_side, false);
                               attacker_armies.Add(army);
                            }
                        }
  
                    }
                    else if (isDefender && line.Contains("\t\t\t\tarmies={"))
                    {
                        MatchCollection found_armies = Regex.Matches(line, @"(\d+) ");
                        defender_armies = new List<Army>();

                        for (int i = 0; i < found_armies.Count; i++)
                        {
                            //Create new Army with combat sides on the constructor
                            //Army army
                            string id = found_armies[i].Groups[1].Value;
                            string combat_side = "defender";

                            // main army
                            if (i == 0)
                            {
                                Army army = new Army(id, combat_side, true);
                                defender_armies.Add(army);
                            }
                            // ally army
                            else
                            {
                                Army army = new Army(id, combat_side, false);
                                defender_armies.Add(army);
                            }
                        }
                    }

                }
            }
        }


    }
}
