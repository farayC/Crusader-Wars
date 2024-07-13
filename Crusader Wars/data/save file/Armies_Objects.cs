using Crusader_Wars.armies.commander_traits;
using Crusader_Wars.armies;
using Crusader_Wars.client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crusader_Wars.data.save_file
{
    public static class Print
    {
        public static void PrintArmiesData(List<Army> attacker_armies, List<Army> defender_armies)
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
                        if (!t.isMercenary())
                            if (t.Culture is null)
                                Console.WriteLine($"## ## Chunk Regiment: {t.ID} | Owner: {t.Owner} |Index: {t.Index} | Origin: {t.Origin} | Soldiers: {ModOptions.FullArmies(t)} | County Key: {t.GetCountyKey()} | Culture ID: null");
                            else
                                Console.WriteLine($"## ## Chunk Regiment: {t.ID} | Owner: {t.Owner} | Index: {t.Index} | Origin: {t.Origin} | Soldiers: {ModOptions.FullArmies(t)} | County Key: {t.GetCountyKey()} | Culture: {t.Culture.GetCultureName()} | Heritage: {t.Culture.GetHeritageName()}");
                        else
                            ///Console.WriteLine($"## ## Mercenary Chunk Regiment: {t.ID} | Owner: {t.Owner} | Index: {t.Index} | Origin: {t.Origin} | Soldiers: {t.CurrentNum} | County Key: {t.GetCountyKey()} | Culture ID: null");
                            Console.WriteLine($"## ## Mercenary Chunk Regiment: {t.ID} | Owner: {t.Owner} | Index: {t.Index} | Origin: {t.Origin} | Soldiers: {ModOptions.FullArmies(t)} | County Key: {t.GetCountyKey()} | Culture: {t.Culture.GetCultureName()} | Heritage: {t.Culture.GetHeritageName()}");


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
                                Console.WriteLine($"## ## Chunk Regiment: {t.ID} | Owner: {t.Owner} | Index: {t.Index} | Origin: {t.Origin} | Soldiers: {ModOptions.FullArmies(t)} | County Key: {t.GetCountyKey()} | Culture ID: null");
                            else
                                Console.WriteLine($"## ## Chunk Regiment: {t.ID} | Owner: {t.Owner} | Index: {t.Index} | Origin: {t.Origin} | Soldiers: {ModOptions.FullArmies(t)} | County Key: {t.GetCountyKey()} | Culture: {t.Culture.GetCultureName()} | Heritage: {t.Culture.GetHeritageName()}");
                        else
                            Console.WriteLine($"## ## Mercenary Chunk Regiment: {t.ID} | Owner: {t.Owner} | Index: {t.Index} | Origin: {t.Origin} | Soldiers: {ModOptions.FullArmies(t)} | County Key: {t.GetCountyKey()} | Culture: {t.Culture.GetCultureName()} | Heritage: {t.Culture.GetHeritageName()}");
                    }
                }
                Console.WriteLine("\n");
            }
        }
    }
    public class Unit
    {
        string Name { get; set; }
        RegimentType Type { get; set; }
        Culture UnitCulture { get; set; }
        bool IsMercenaryBool { get; set; }
        int Soldiers { get; set; }
        string AttilaKey { get; set; }
        string AttilaFaction { get; set; }
        int Max { get; set; }

        public void PrintData()
        {
            Console.WriteLine("Name:" + Name);
            Console.WriteLine("Type:" +  Type.ToString());
            Console.WriteLine("Culture:" + UnitCulture.GetCultureName());
            Console.WriteLine("Merc:" + IsMercenaryBool);
            Console.WriteLine("Soldiers:" + Soldiers);
            Console.WriteLine("Key:" + AttilaKey);
            Console.WriteLine("Faction:" + AttilaFaction);
            Console.WriteLine("Max:" + Max);
            Console.WriteLine();
            Console.WriteLine();

        }

        public Unit(string regiment_name, int soldiers, Culture culture_obj, RegimentType type)
        {
            Name = regiment_name;
            UnitCulture = culture_obj;
            Soldiers = soldiers;
            Type = type;
        }
        public Unit(string regiment_name, int soldiers, Culture culture_obj, RegimentType type, bool is_merc)
        {
            Name = regiment_name;
            UnitCulture = culture_obj;
            Soldiers = soldiers;
            Type = type;
            IsMercenaryBool = is_merc;
        }
        public Unit(string regiment_name, int soldiers, Culture culture_obj, RegimentType type, bool is_merc, string attilaFaction)
        {
            Name = regiment_name;
            UnitCulture = culture_obj;
            Soldiers = soldiers;
            Type = type;
            IsMercenaryBool = is_merc;
            AttilaFaction = attilaFaction;
        }

        public Unit(string regiment_name, int soldiers, Culture culture_obj, RegimentType type, bool is_merc, string attilaFaction, int max)
        {
            Name = regiment_name;
            UnitCulture = culture_obj;
            Soldiers = soldiers;
            Type = type;
            IsMercenaryBool = is_merc;
            AttilaFaction = attilaFaction;
            Max = max;
        }


        public void SetAttilaFaction(string a) { AttilaFaction = a; }
        public void SetUnitKey(string unit_key) { AttilaKey = unit_key; }
        public void ChangeName(string y) { Name = y; }
        public void ChangeSoldiers(int y) { Soldiers = y; }
        public void SetMax(int i) { Max = i; }


        public int GetMax() { return Max; }
        public string GetAttilaFaction() { return AttilaFaction; }
        public string GetAttilaUnitKey() { return AttilaKey; }
        public string GetName() { return Name; }
        public Culture GetObjCulture() { return UnitCulture; }
        public string GetCulture() { if (UnitCulture is null) return "NOT FOUND"; return UnitCulture.GetCultureName(); }
        public string GetHeritage() { if (UnitCulture is null) return "NOT FOUND"; return UnitCulture.GetHeritageName(); }
        public int GetSoldiers() { return Soldiers; }
        public bool IsMerc() { return IsMercenaryBool; }
        public RegimentType GetRegimentType() { return Type; }


    }
    public enum RegimentType
    {
        Commander,
        MenAtArms,
        Levy,
        Knight
    };

    public class ArmyRegiment
    {
        public string ID { get; private set; }
        public RegimentType Type { get; private set; }
        public string MAA_Name { get; private set; }
        public List<Regiment> Regiments { get; private set; }
        public int CurrentNum { get; private set; }
        public int StartingNum { get; private set; }
        public int Max { get; private set; }


        public ArmyRegiment(string id)
        {
            ID = id;
        }

        public void SetType(RegimentType type)
        {
            Type = type;
            if (type == RegimentType.Levy) MAA_Name = "Levy";
        }
        public void SetType(RegimentType type, string maa_name)
        {
            MAA_Name = maa_name;
            Type = type;
        }

        public void SetRegiments(List<Regiment> regiments)
        {
            Regiments = regiments;

        }
        public void SetCurrentNum (string x)
        {
            CurrentNum = Int32.Parse(x);
        }
        public void SetMax(string x)
        {
            Max = Int32.Parse(x);
        }
        public void SetStartingNum(string x)
        {
            StartingNum = Int32.Parse(x);
        }



    }

    public class Regiment
    {
        public string ID { get; private set; }
        public string Index { get; private set; }
        public string Origin { get; private set; }
        public string Owner { get; private set; }
        public string Max {  get; private set; }
        public string CurrentNum { get; private set; }
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
        public void SetOwner(string o) { Owner = o; }
        public void SetCulture(string id) { Culture = new Culture(id); }
        public void isMercenary(bool t) { IsMercenary = t; }
        public void SetOrigin(string origin) { Origin = origin; }
        public void SetMax(string max) { Max = max; }
        public void SetSoldiers(string soldiers) { CurrentNum = soldiers; }
        public void StoreCountyKey(string key) { county_key = key; }
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
        public Culture(string id, string name, string heritage)
        {
            ID = id;
            CultureName = name;
            HeritageName = heritage;
        }

        public string GetCultureName()
        {
            if (CultureName is null)
                return "";
            else
                return CultureName;
        }
        public string GetHeritageName() { return HeritageName; }


        public void SetName(string t) { CultureName = t; }
        public void SetHeritage(string t) { HeritageName = t; }

    }
}
