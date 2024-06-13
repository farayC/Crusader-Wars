using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Crusader_Wars.twbattle;
using System.Xml.Linq;
using System.Drawing;
using Crusader_Wars.unit_mapper;
using Crusader_Wars.client;
using System.Xml;
using System.Windows;
using System.Diagnostics;
using Crusader_Wars.sieges;
using static Crusader_Wars.CK3LogData;

namespace Crusader_Wars.data.save_file
{


    public static class ArmiesReader
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
            ReadArmiesUnits();
            ReadArmyRegiments();

            // Clear Empty Regiments
            for (int i = 0; i < attacker_armies.Count; i++)
            {
                attacker_armies[i].ClearNullRegiments();
            }
            for (int i = 0; i < defender_armies.Count; i++)
            {
                defender_armies[i].ClearNullRegiments();
            }
            ReadRegiments();
            LandedTitles.ReadProvinces(attacker_armies, defender_armies);
            ReadCountiesManager();
            ReadMercenaries();
            BattleFile.SetArmiesSides(attacker_armies, defender_armies);
            CreateKnights();
            ReadCharacters();
            ReadCultureManager();

            // Clear Empty Regiments
            for(int i = 0; i < attacker_armies.Count;i++)
            {
                attacker_armies[i].ClearEmptyRegimnts();
            }
            for (int i = 0; i < defender_armies.Count; i++)
            {
                defender_armies[i].ClearEmptyRegimnts();
            }

            // Organize Units
            CreateUnits();



            // Print Armies
            //Print.PrintArmiesData(attacker_armies, defender_armies);

            return (attacker_armies, defender_armies);
        }


        static void ReadCharacters()
        {
            bool searchStarted = false;
            bool isAttacker = false, isDefender = false;
            Army attacker_army = null;
            Army defender_army = null;
            Knight attacker_knight = null;
            Knight defender_knight = null;
            using (StreamReader sr = new StreamReader(Writter.DataFilesPaths.Living_Path()))
            {
                while(!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line == null) break;
                    

                    if(Regex.IsMatch(line, @"\d+={") && !searchStarted)
                    {
                        string line_id = Regex.Match(line, @"(\d)+={").Groups[1].Value;
                        foreach (var army in attacker_armies)
                        {
                            if(army.Knights.GetKnightsList() != null)
                            {
                                foreach (var knight in army.Knights.GetKnightsList())
                                {
                                    if (line == knight.GetID() + "={")
                                    {
                                        searchStarted = true;
                                        attacker_knight = knight;
                                        attacker_army = army;
                                        isAttacker = true;
                                        break;
                                    }
                                }
                            }

                        }
                        foreach (var army in defender_armies)
                        {
                            if (army.Knights.GetKnightsList() != null)
                            {
                                foreach (var knight in army.Knights.GetKnightsList())
                                {
                                    if (line == knight.GetID() + "={")
                                    {
                                        searchStarted = true;
                                        defender_knight = knight;
                                        defender_army = army;
                                        isDefender = true;
                                        break;
                                    }
                                }
                            }

                        }
                    }
                    else if (searchStarted && line.Contains("\ttraits={")) //# TRAITS
                    {

                    }
                    else if(searchStarted && line.Contains("\tculture=")) //# CULTURE
                    {
                        string culture_id = Regex.Match(line,@"\d+").Value;
                        if(isAttacker)
                        {
                            attacker_army.Knights.GetKnightsList().Find(x => x == attacker_knight).ChangeCulture(new Culture(culture_id));
                            attacker_army.Knights.SetMajorCulture();
                        }
                        else if (isDefender)
                        {
                            defender_army.Knights.GetKnightsList().Find(x => x == defender_knight).ChangeCulture(new Culture(culture_id));
                            defender_army.Knights.SetMajorCulture();
                        }

                    }
                    else if(searchStarted && line == "}")
                    {
                        searchStarted = false;
                        isAttacker = false;
                        isDefender = false;
                        attacker_knight = null;
                        defender_knight = null;
                        attacker_army = null;
                        defender_army = null;
                    }
                }


            }
        }


        static void RemoveCommandersAsKnights()
        {
            for (int i = 0; i < attacker_armies.Count; i++)
            {
                var army = attacker_armies[i];
                for (int j = 0; j < attacker_armies[i].ArmyRegiments.Count; j++)
                {
                    var regiment = attacker_armies[i].ArmyRegiments[j];
                    if (regiment.Type == RegimentType.Knight && regiment.ID == army.CommanderID)
                    {
                        attacker_armies[i].ArmyRegiments.Remove(regiment);
                    }
                }
            }
            for (int i = 0; i < defender_armies.Count; i++)
            {
                var army = defender_armies[i];
                for (int j = 0; j < defender_armies[i].ArmyRegiments.Count; j++)
                {
                    var regiment = defender_armies[i].ArmyRegiments[j];
                    if (regiment.Type == RegimentType.Knight && regiment.ID == army.CommanderID)
                    {
                        defender_armies[i].ArmyRegiments.Remove(regiment);
                    }
                }
            }
        }
        
        static List<Army> GetSideArmies(string side)
        {
            List<Army> left_side = null, right_side = null;
            foreach (var army in attacker_armies)
            {
                if (army.IsPlayer())
                {
                    left_side = attacker_armies;
                    break;
                }
                else if (army.IsEnemy())
                {
                    right_side = attacker_armies;
                    break;
                }
            }
            foreach (var army in defender_armies)
            {
                if (army.IsPlayer())
                {
                    left_side = defender_armies;
                    break;
                }
                else if (army.IsEnemy())
                {
                    right_side = defender_armies;
                    break;
                }
            }

            if (side == "left")
                return left_side;
            else
                return right_side;
        }
        public static void CreateKnights()
        {
            RemoveCommandersAsKnights();

            var left_side_armies = GetSideArmies("left");
            var right_side_armies = GetSideArmies("right");

            


            var KnightsList = new List<Knight>();
            for (int x = 0; x < left_side_armies.Count; x++)
            {
                var army = left_side_armies[x];
                for (int y = 0; y< army.ArmyRegiments.Count;y++)
                {
                    var regiment = army.ArmyRegiments[y];
                    if(regiment.Type == RegimentType.Knight)
                    {
                        for (int i = 0; i < CK3LogData.LeftSide.GetKnights().Count; i++)
                        {
                            string id = CK3LogData.LeftSide.GetKnights()[i].id;
                            if (id == regiment.MAA_Name)
                            {
                                int prowess = Int32.Parse(CK3LogData.LeftSide.GetKnights()[i].prowess);
                                string name = CK3LogData.LeftSide.GetKnights()[i].name;

                                KnightsList.Add(new Knight(name, regiment.MAA_Name, null, prowess, 4));
                            }
                        }
                    }

                }

                int leftEffectivenss = 0;
                if (CK3LogData.LeftSide.GetKnights() is null || CK3LogData.LeftSide.GetKnights().Count == 0)
                    leftEffectivenss = 0;
                else
                    leftEffectivenss = CK3LogData.LeftSide.GetKnights()[0].effectiveness;
                if (CK3LogData.LeftSide.GetKnights().Count > 0)
                {
                    leftEffectivenss = CK3LogData.LeftSide.GetKnights()[0].effectiveness;
                }
                
                KnightSystem leftSide = new KnightSystem(KnightsList, leftEffectivenss);
                if(left_side_armies == attacker_armies)
                {
                    attacker_armies[x].SetKnights(leftSide);
                }
                else if(left_side_armies == defender_armies)
                {
                    defender_armies[x].SetKnights(leftSide);
                }
                KnightsList = new List<Knight>();
                
            }


            KnightsList = new List<Knight>();
            for (int x = 0; x < right_side_armies.Count; x++)
            {
                var army = right_side_armies[x];
                for (int y = 0; y < army.ArmyRegiments.Count; y++)
                {
                    var regiment = army.ArmyRegiments[y];
                    if(regiment.Type == RegimentType.Knight)
                    {
                        for (int i = 0; i < CK3LogData.RightSide.GetKnights().Count; i++)
                        {
                            string id = CK3LogData.RightSide.GetKnights()[i].id;
                            if (id == regiment.MAA_Name)
                            {
                                int prowess = Int32.Parse(CK3LogData.RightSide.GetKnights()[i].prowess);
                                string name = CK3LogData.RightSide.GetKnights()[i].name;

                                KnightsList.Add(new Knight(name, regiment.MAA_Name, null, prowess, 4));
                            }
                        }
                    }

                }

                int rightEffectivenss = 0;
                if (CK3LogData.RightSide.GetKnights() is null || CK3LogData.RightSide.GetKnights().Count == 0)
                    rightEffectivenss = 0;
                else
                    rightEffectivenss = CK3LogData.RightSide.GetKnights()[0].effectiveness;

                KnightSystem rightSide = new KnightSystem(KnightsList, rightEffectivenss);

                if (right_side_armies == attacker_armies)
                {
                    attacker_armies[x].SetKnights(rightSide);
                }
                else if (right_side_armies == defender_armies)
                {
                    defender_armies[x].SetKnights(rightSide);
                }
                KnightsList = new List<Knight>();
            }
        }


        private static string GetCharacterCultureID(string character_id)
        {
            bool isSearchStarted = false;
            string culture_id = "";
            using (StreamReader stringReader = new StreamReader(Writter.DataFilesPaths.Living_Path()))
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


        private static void CreateUnits()
        {
            Armies_Functions.CreateUnits(attacker_armies);
            Armies_Functions.CreateUnits(defender_armies);
        }

        private static void ReadMercenaries()
        {
            bool isSearchStarted = false;
            using (StreamReader sr = new StreamReader(Writter.DataFilesPaths.Mercenaries_Path()))
            {
                string culture_id = "";
                List<string> regiments_ids = new List<string>();

                while(true)
                {
                    string line = sr.ReadLine();
                    if (line == null) break;

                    //Mercenary Company ID
                    if(Regex.IsMatch(line, @"\t\t\d+={") && !isSearchStarted)
                    {
                        isSearchStarted = true;
                        continue;
                    }
                    else if(line == "\t\t}")
                    {
                        var attacker_mercenaries_regiments = attacker_armies.SelectMany(army => army.ArmyRegiments.SelectMany(armyRegiment => armyRegiment.Regiments))
                                                            .Where(regiment => regiment.isMercenary())
                                                            .ToList();

                        
                        var defender_mercenaries_regiments = defender_armies.SelectMany(army => army.ArmyRegiments.SelectMany(armyRegiment => armyRegiment.Regiments))
                                                            .Where(regiment => regiment.isMercenary())
                                                            .ToList();

                        for (int i = 0; i < attacker_armies.Count; i++)
                        {
                            //Army Regiments
                            for (int x = 0; x < attacker_armies[i].ArmyRegiments.Count; x++)
                            {
                                //Regiments
                                if(attacker_armies[i].ArmyRegiments[x].Regiments != null)
                                {
                                    for (int y = 0; y < attacker_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                    {
                                        var regiment = attacker_armies[i].ArmyRegiments[x].Regiments[y];

                                        foreach (var t in regiments_ids)
                                        {

                                            if (t == regiment.ID && (regiment.isMercenary() || regiment.Culture is null))
                                            {
                                                attacker_armies[i].ArmyRegiments[x].Regiments[y].SetCulture(culture_id);
                                                break;
                                            }
                                        }

                                    }
                                }

                            }
                        }

                        for (int i = 0; i < defender_armies.Count; i++)
                        {
                            //Army Regiments
                            for (int x = 0; x < defender_armies[i].ArmyRegiments.Count; x++)
                            {
                                //Regiments
                                if(defender_armies[i].ArmyRegiments[x].Regiments != null)
                                {
                                    for (int y = 0; y < defender_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                    {
                                        var regiment = defender_armies[i].ArmyRegiments[x].Regiments[y];
                                        foreach (var t in regiments_ids)
                                        {
                                            if (t == regiment.ID && (regiment.isMercenary() || regiment.Culture is null))
                                            {
                                                defender_armies[i].ArmyRegiments[x].Regiments[y].SetCulture(culture_id);
                                                break;
                                            }
                                        }
                                    }
                                }

                            }
                        }

                    }
                    else if (isSearchStarted)
                    {
                        if (line.Contains("\t\tculture="))
                        {
                            culture_id = Regex.Match(line, @"\d+").Value;
                        }
                        else if (line.Contains("\t\tregiments={ "))
                        {
                            regiments_ids = Regex.Matches(line, @"\d+").Cast<Match>().Select(match => match.Value).ToList();
                        }
                    }

                }
            }
        }

        private static void ReadCultureManager()
        {
            bool isSearchStared = false;
            string culture_id = "";
            string culture_name = "";
            string heritage_name = "";
            List<(string culture_id, string culture_name, string heritage_name)> found_cultures = new List<(string culture_id, string culture_name, string heritage_name)> ();

            using (StreamReader sr = new StreamReader(Writter.DataFilesPaths.Cultures_Path()))
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

                            //Knights
                            if(attacker_armies[i].Knights.GetKnightsList() != null)
                            {
                                foreach (var knight in attacker_armies[i].Knights.GetKnightsList())
                                {
                                    string knight_culture_id = "";
                                    if (knight.GetCultureObj() == null)
                                    {
                                        var new_culture = attacker_armies[i].Knights.GetKnightsList()[0].GetCultureObj();
                                        knight.ChangeCulture(new_culture);
                                        knight_culture_id = knight.GetCultureObj().ID;
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
                            for (int x = 0; x < attacker_armies[i].ArmyRegiments.Count; x++)
                            {
                                if (isSearchStared) break;
                                if (attacker_armies[i].ArmyRegiments[x].Regiments is null) continue;
                                //Regiments
                                for (int y = 0; y < attacker_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                {


                                    //if culture is null, skip
                                    if (attacker_armies[i].ArmyRegiments[x].Regiments[y].Culture is null)
                                    {
                                        continue;
                                    }
                                    
                                    //If is player character
                                    if (string.IsNullOrEmpty(attacker_armies[i].ArmyRegiments[x].Regiments[y].Culture.ID))
                                    {
                                        attacker_armies[i].ArmyRegiments[x].Regiments[y].Culture.SetHeritage(DataSearch.Player_Character.GetHeritage());
                                        attacker_armies[i].ArmyRegiments[x].Regiments[y].Culture.SetName(DataSearch.Player_Character.GetCulture());
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

                                //Knights
                                if (defender_armies[i].Knights.GetKnightsList() != null)
                                {
                                    foreach (var knight in defender_armies[i].Knights.GetKnightsList())
                                    {
                                        string knight_culture_id = "";
                                        if (knight.GetCultureObj() == null)
                                        {
                                            var new_culture = defender_armies[i].Knights.GetKnightsList()[0].GetCultureObj();
                                            knight.ChangeCulture(new_culture);
                                            knight_culture_id = knight.GetCultureObj().ID;
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
                                for (int x = 0; x < defender_armies[i].ArmyRegiments.Count; x++)
                                {
                                    if (isSearchStared) break;
                                    //Regiments
                                    if (defender_armies[i].ArmyRegiments[x].Regiments is null) continue;
                                    for (int y = 0; y < defender_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                    {


                                        //if culture is null, skip
                                        if (defender_armies[i].ArmyRegiments[x].Regiments[y].Culture is null)
                                        {
                                            continue;
                                        }
                                        //If is player character
                                        else if (string.IsNullOrEmpty(defender_armies[i].ArmyRegiments[x].Regiments[y].Culture.ID))
                                        {
                                            defender_armies[i].ArmyRegiments[x].Regiments[y].Culture.SetHeritage(DataSearch.Player_Character.GetHeritage());
                                            defender_armies[i].ArmyRegiments[x].Regiments[y].Culture.SetName(DataSearch.Player_Character.GetCulture());
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
                        //culture_name = FirstCharToUpper(culture_name);
                        
                    }
                    //Heritage Name
                    else if(isSearchStared && line.Contains("\t\t\theritage="))
                    {
                        heritage_name = Regex.Match(line, @"""(\w+)""").Groups[1].Value;
                        //heritage_name = heritage_name.Replace("heritage_", "");
                        //heritage_name = heritage_name.Replace("_", " ");
                        //heritage_name = FirstCharToUpper(heritage_name);

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

                        //Knights
                        if(attacker_armies[i].Knights.GetKnightsList() != null)
                        {
                            foreach (var knight in attacker_armies[i].Knights.GetKnightsList())
                            {
                                string knight_culture_id = knight.GetCultureObj().ID;

                                if (knight_culture_id == culture.culture_id)
                                {
                                    isSearchStared = true;
                                    knight.GetCultureObj().SetName(culture.culture_name);
                                    knight.GetCultureObj().SetHeritage(culture.heritage_name);
                                }
                            }
                        }


                        //Army Regiments
                        for (int x = 0; x < attacker_armies[i].ArmyRegiments.Count; x++)
                        {
                            //Regiments
                            if (attacker_armies[i].ArmyRegiments[x].Regiments is null) continue;
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
                        //Knights
                        if (defender_armies[i].Knights.GetKnightsList() != null)
                        {
                            foreach (var knight in defender_armies[i].Knights.GetKnightsList())
                            {
                                string knight_culture_id = knight.GetCultureObj().ID;

                                if (knight_culture_id == culture.culture_id)
                                {
                                    isSearchStared = true;
                                    knight.GetCultureObj().SetName(culture.culture_name);
                                    knight.GetCultureObj().SetHeritage(culture.heritage_name);
                                }
                            }
                        }


                        //Army Regiments
                        for (int x = 0; x < defender_armies[i].ArmyRegiments.Count; x++)
                        {
                            //Regiments
                            if (defender_armies[i].ArmyRegiments[x].Regiments is null) continue;
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

        private static void ReadCountiesManager()
        {

            List<(string county_key, string culture_id)> FoundCounties = new List<(string county_key, string culture_id)>();

            bool isSearchStared = false;
            string county_key = "";
            using (StreamReader sr = new StreamReader(Writter.DataFilesPaths.Counties_Path()))
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
                                if (attacker_armies[i].ArmyRegiments[x].Regiments != null)
                                {
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
                                    if(defender_armies[i].ArmyRegiments[x].Regiments != null)
                                    {
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
                            if(attacker_armies[i].ArmyRegiments[x].Regiments != null)
                            {
                                for (int y = 0; y < attacker_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                {
                                    string regiment_county_key = attacker_armies[i].ArmyRegiments[x].Regiments[y].GetCountyKey();
                                    string owner = attacker_armies[i].ArmyRegiments[x].Regiments[y].Owner;
                                    if (!string.IsNullOrEmpty(owner))
                                    {
                                        if (temp_characters_cultures.Exists(t => t.char_id == owner))
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

                                    if (county.county_key == regiment_county_key)
                                    {
                                        attacker_armies[i].ArmyRegiments[x].Regiments[y].SetCulture(county.culture_id);
                                    }
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
                            if(defender_armies[i].ArmyRegiments[x].Regiments != null)
                            {
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
        }
        
  

        private static void ReadRegiments()
        {
            bool isSearchStarted = false;
            bool isDefender = false, isAttacker = false;
            int army_index = 0, army_regiment_index = 0, regiment_index = 0;
            int index = -1;
            int reg_chunk_index = 0;
            using (StreamReader sr = new StreamReader(Writter.DataFilesPaths.Regiments_Path()))
            {
                while (true)
                {
                    string line = sr.ReadLine();
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
                                if(attacker_armies[i].ArmyRegiments[x].Regiments != null)
                                {
                                    for (int y = 0; y < attacker_armies[i].ArmyRegiments[x].Regiments.Count; y++)
                                    {
                                        if (y == attacker_armies[i].ArmyRegiments[x].Regiments.Count) break;
                                        string id = attacker_armies[i].ArmyRegiments[x].Regiments[y].ID;
                                        if (id == regiment_id)
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
                        }

                        if (!isSearchStarted)
                        {
                            for (int i = 0; i < defender_armies.Count; i++)
                            {
                                if (isSearchStarted) break;
                                for (int x = 0; x < defender_armies[i].ArmyRegiments.Count; x++)
                                {
                                    if (isSearchStarted) break;

                                    if(defender_armies[i].ArmyRegiments[x].Regiments != null)
                                    {
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


        private static void ReadArmiesUnits()
        {
            bool isSearchStarted = false;
            bool isAttacker = false, isDefender = false;
            Army attacker = null, defender = null;

            using (StreamReader SR = new StreamReader(Writter.DataFilesPaths.Units_Path()))
            {
                while(!SR.EndOfStream)
                {
                    string line  = SR.ReadLine();
                    if (line == null) break;

                    if (Regex.IsMatch(line, @"\t\d+={") && !isSearchStarted)
                    {
                        string id = Regex.Match(line, @"\t(\d+)={").Groups[1].Value;
                        for(int i = 0; i < attacker_armies.Count;i++)
                        {
                            if(id == attacker_armies[i].ArmyUnitID)
                            {
                                isSearchStarted = true;
                                attacker = attacker_armies[i];
                                isAttacker=true;
                                break;
                            }

                        }
                        for (int i = 0; i < defender_armies.Count; i++)
                        {
                            if (id == defender_armies[i].ArmyUnitID)
                            {
                                isSearchStarted = true;
                                defender = defender_armies[i];
                                isDefender = true;
                                break;
                            }

                        }
                    }
                    else if(isSearchStarted && line.Contains("\t\towner="))
                    {
                        string owner = Regex.Match(line, @"\d+").Value;
                        if (isAttacker)
                        {
                            attacker_armies.FirstOrDefault(x => x == attacker).Owner = owner;
                        }
                        else if(isDefender)
                        {
                            defender_armies.FirstOrDefault(x => x == defender).Owner = owner;
                        }
                        
                    }
                    else if (isSearchStarted && line == "\t}")
                    {
                        isSearchStarted = false;
                        isAttacker = false;
                        isDefender = false;
                        attacker = null;
                        defender = null;
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
            string army_regiment_id = "";

            string regiment_id = "";
            string index = "";

            bool isNameSet = false;

            using (StreamReader SR = new StreamReader(Writter.DataFilesPaths.ArmyRegiments_Path()))
            {
                while (true)
                {
                    string line = SR.ReadLine();

                    if (line == null) break;

                    // Army Regiment ID Line
                    if (Regex.IsMatch(line, @"\t\t\d+={") && !isSearchStarted)
                    {
                        army_regiment_id = Regex.Match(line, @"\t\t(\d+)={").Groups[1].Value;
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
                        //Debug purposes, remove later...
                        if(found_regiments != null)
                        {
                            if (isAttacker)
                            {
                                attacker_armies[army_index].ArmyRegiments[army_regiment_index].SetRegiments(found_regiments);
                            }
                            else if (isDefender)
                            {
                                defender_armies[army_index].ArmyRegiments[army_regiment_index].SetRegiments(found_regiments);
                            }
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
            using (StreamReader SR = new StreamReader(Writter.DataFilesPaths.Armies_Path()))
            {
                while(true)
                {
                    string line = SR.ReadLine();
                    if (line == null) break;

                    // Army ID Line
                    if(Regex.IsMatch(line, @"\t\t\d+={") && !isSearchStarted)
                    {
                        // Check if it's a battle army

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
                    else if (isSearchStarted && line.Contains("\t\t\tunit="))
                    {
                        string armyUnitId = Regex.Match(line, @"\d+").Value;
                        if(isAttacker)
                        {
                            attacker_armies[index].ArmyUnitID = armyUnitId;
                        }
                        else if(isDefender)
                        {
                            defender_armies[index].ArmyUnitID = armyUnitId;
                        }
                    }



                    // Army End Line
                    if (isSearchStarted && line == "\t\t}")
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
                            if(i == 0) //<-------------------------------------------------------------------[FIX THIS] !!!
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
                            if (i == 0)//<-------------------------------------------------------------------[FIX THIS] !!!
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
