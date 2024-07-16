﻿using System;
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
        static List<(string name, int index)> save_file_traits;
        public static void ReadCombats(string g)
        {
            ReadCombatArmies(g);
        }
        public static (List<Army> attacker, List<Army> defender) ReadBattleArmies()
        {
            ReadArmiesData();
            ReadArmiesUnits();
            ReadArmyRegiments();
            ReadCombatSoldiersNum(BattleResult.Player_Combat);
            ReadRegiments();

            LandedTitles.ReadProvinces(attacker_armies, defender_armies);
            ReadCountiesManager();
            ReadMercenaries();
            BattleFile.SetArmiesSides(attacker_armies, defender_armies);
            ReadSaveFileTraits();
            CreateKnights();
            CreateCommanders();
            ReadCharacters();
            ReadCultureManager();



            // Organize Units
            CreateUnits();

            // Print Armies
            //Print.PrintArmiesData(attacker_armies, defender_armies);

            return (attacker_armies, defender_armies);
        }

        static void ClearEmptyRegiments()
        {
            // Clear Empty Regiments
            for (int i = 0; i < attacker_armies.Count; i++)
            {
                attacker_armies[i].ClearEmptyRegiments();
            }
            for (int i = 0; i < defender_armies.Count; i++)
            {
                defender_armies[i].ClearEmptyRegiments();
            }
        }

        static void ClearNullArmyRegiments()
        {
            // Clear Empty Regiments
            for (int i = 0; i < attacker_armies.Count; i++)
            {
                attacker_armies[i].ClearNullArmyRegiments();
            }
            for (int i = 0; i < defender_armies.Count; i++)
            {
                defender_armies[i].ClearNullArmyRegiments();
            }
        }

        static void ReadSaveFileTraits()
        {
            MatchCollection allTraits = Regex.Matches(File.ReadAllText(Writter.DataFilesPaths.Traits_Path()), @" (\w+)");
            var save_file_traits = new List<(string name, int index)>();

            for (int i = 0; i < allTraits.Count; i++)
            {
                //save_file_traits[i] = (allTraits[i].Groups[1].Value, i);
                save_file_traits.Add((allTraits[i].Groups[1].Value, i));
            }
        }

        public static int GetTraitIndex(string trait_name)
        {
            int index;
            index = save_file_traits.FirstOrDefault(x => x.name == trait_name).index;
            return index;

        }

        public static string GetTraitKey(int trait_index)
        {
            string key;
            key = save_file_traits.FirstOrDefault(x => x.index == trait_index).name;
            return key;

        }


        static void ReadCharacters()
        {
            bool searchStarted = false;
            bool isAttacker = false, isDefender = false;
            bool isKnight = false, isCommander = false;
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
                            //ARMY OWNER
                            if(line == army.Owner + "={")
                            {
                                searchStarted = true;
                                attacker_army = army;
                                isAttacker = true;
                                break;
                            }

                            //COMMANDERS
                            else if (army.Commander != null && line == army.Commander.ID + "={")
                            {
                                searchStarted = true;
                                attacker_army = army;
                                isAttacker = true;
                                isCommander = true;
                                break;
                            }

                            // KNIGHTS
                            else if (army.Knights.GetKnightsList() != null)
                            {
                                foreach (var knight in army.Knights.GetKnightsList())
                                {
                                    if (line == knight.GetID() + "={")
                                    {
                                        searchStarted = true;
                                        attacker_knight = knight;
                                        attacker_army = army;
                                        isAttacker = true;
                                        isKnight = true;
                                        break;
                                    }
                                }
                                if (searchStarted) break;
                            }

                            
                        }

                        if (searchStarted) continue;
                        
                        foreach (var army in defender_armies)
                        {
                            //ARMY OWNER
                            if (line == army.Owner + "={")
                            {
                                searchStarted = true;
                                defender_army = army;
                                isDefender = true;
                                break;
                            }

                            //COMMANDERS
                            if (army.Commander != null && line == army.Commander.ID + "={")
                            {
                                searchStarted = true;
                                defender_army = army;
                                isDefender = true;
                                isCommander = true;
                                break;
                            }

                            // KNIGHTS
                            else if (army.Knights.GetKnightsList() != null)
                            {
                                foreach (var knight in army.Knights.GetKnightsList())
                                {
                                    if (line == knight.GetID() + "={")
                                    {
                                        searchStarted = true;
                                        defender_knight = knight;
                                        defender_army = army;
                                        isDefender = true;
                                        isKnight = true;
                                        break;
                                    }
                                }
                                if (searchStarted) break;
                            }
                        }
                    }
                    else if (searchStarted && isCommander && line.Contains("\ttraits={")) //# TRAITS
                    {
                        MatchCollection found_traits = Regex.Matches(line, @"\d+");
                        foreach(var trait in found_traits)
                        {

                        }
                    }
                    else if(searchStarted && line.Contains("\tculture=")) //# CULTURE
                    {
                        string culture_id = Regex.Match(line,@"\d+").Value;
                        if(isAttacker && isKnight)
                        {
                            attacker_army.Knights.GetKnightsList().Find(x => x == attacker_knight).ChangeCulture(new Culture(culture_id));
                            attacker_army.Knights.SetMajorCulture();
                        }
                        else if (isDefender && isKnight)
                        {
                            defender_army.Knights.GetKnightsList().Find(x => x == defender_knight).ChangeCulture(new Culture(culture_id));
                            defender_army.Knights.SetMajorCulture();
                        }
                        else if (isAttacker && isCommander)
                        {
                            attacker_army.Commander.ChangeCulture(new Culture(culture_id));
                        }
                        else if (isDefender && isCommander)
                        {
                            defender_army.Commander.ChangeCulture(new Culture(culture_id));
                        }
                        else if(isAttacker)
                        {
                            foreach(var army in attacker_armies)
                            {
                                army.OwnerCulture = new Culture(culture_id);
                            }
                        }
                        else if (isDefender)
                        {
                            foreach (var army in defender_armies)
                            {
                                army.OwnerCulture = new Culture(culture_id);
                            }
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
                        isCommander = false;
                        isKnight = false;
                    }
                }


            }
        }

        //----- FIX THIS FUNCTION BELOW
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
        
        public static List<Army> GetSideArmies(string side)
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

        static void CreateCommanders()
        {
            var left_side_armies = GetSideArmies("left");
            var right_side_armies = GetSideArmies("right");
            for (int x = 0; x < left_side_armies.Count; x++)
            {
                var army = left_side_armies[x];
                if(army.isMainArmy)
                {
                    var main_commander_data = CK3LogData.LeftSide.GetCommander();
                    army.SetCommander(new CommanderSystem(main_commander_data.name, main_commander_data.id, main_commander_data.prowess, main_commander_data.martial, main_commander_data.rank, true));
                }
            }

            for (int x = 0; x < right_side_armies.Count; x++)
            {
                var army = right_side_armies[x];
                if (army.isMainArmy)
                {
                    var main_commander_data = CK3LogData.RightSide.GetCommander();
                    army.SetCommander(new CommanderSystem(main_commander_data.name, main_commander_data.id, main_commander_data.prowess, main_commander_data.martial, main_commander_data.rank, true));
                }
            }

        }
        static void CreateKnights()
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
            Armies_Functions.ReadArmiesCultures(attacker_armies);
            Armies_Functions.ReadArmiesCultures(defender_armies);
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

                    else if (isSearchStarted && line.Contains("\t\t\t\t\tmax="))
                    {
                        string max = Regex.Match(line, @"\d+").Value;

                        if (isAttacker)
                        {
                            attacker_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].SetMax(max);
                        }
                        else if (isDefender)
                        {
                            defender_armies[army_index].ArmyRegiments[army_regiment_index].Regiments[regiment_index].SetMax(max);
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

            ClearEmptyRegiments();
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

                    //Current Number
                    else if(isSearchStarted && line.Contains("\t\t\t\tcurrent="))
                    {
                        string currentNum = Regex.Match(line, @"\d+").Value;
                        if(isAttacker) 
                            attacker_armies[army_index].ArmyRegiments[army_regiment_index].SetCurrentNum(currentNum);
                        else if(isDefender)
                            defender_armies[army_index].ArmyRegiments[army_regiment_index].SetCurrentNum(currentNum);
                    }

                    //Max
                    else if (isSearchStarted && line.Contains("\t\t\t\tmax="))
                    {
                        string max = Regex.Match(line, @"\d+").Value;
                        if (isAttacker)
                            attacker_armies[army_index].ArmyRegiments[army_regiment_index].SetMax(max);
                        else if (isDefender)
                            defender_armies[army_index].ArmyRegiments[army_regiment_index].SetMax(max);
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

            ClearNullArmyRegiments();
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

        static void ReadCombatSoldiersNum(string combat_string)
        {
            bool isAttacker = false, isDefender = false;
            string searchingArmyRegiment = null;
            using (StringReader SR = new StringReader(combat_string))//Player_Combat
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

                    else if (isAttacker && line.Contains("\t\t\t\t\t\tregiment="))
                    {
                        searchingArmyRegiment = Regex.Match(line, @"\d+").Value;
                    }
                    else if (isDefender && line.Contains("\t\t\t\t\t\tregiment="))
                    {
                        searchingArmyRegiment = Regex.Match(line, @"\d+").Value;
                    }

                    else if(isAttacker && line.Contains("\t\t\t\t\t\tstarting="))
                    {
                        string startingNum = Regex.Match(line,@"\d+").Value;

                        foreach(var army in attacker_armies)
                        {
                            army.ArmyRegiments.FirstOrDefault(x => x.ID == searchingArmyRegiment)?.SetStartingNum(startingNum);
                        }

                    }
                    else if(isDefender && line.Contains("\t\t\t\t\t\tstarting="))
                    {
                        string startingNum = Regex.Match(line, @"\d+").Value;
                        foreach (var army in defender_armies)
                        {
                            army.ArmyRegiments.FirstOrDefault(x => x.ID == searchingArmyRegiment)?.SetStartingNum(startingNum);
                        }
                    }

                    else if((isAttacker || isDefender) && line == "\t\t\t}")
                    {
                        isAttacker = false;
                        isDefender = false;
                        searchingArmyRegiment = null;
                    }

                    //end line
                    else if(line == "\t\t}")
                    {
                        break;
                    }
 

                }
            }
        }


    }
}