﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using System.IO.Pipes;

namespace Crusader_Wars
{
    public static class BattleResult
    {

        public static void LoadSaveFile(string savePath)
        {
            try
            {
                Reader.ReadFile(savePath);
            }
            catch
            {
                Console.WriteLine("Error loading the save file");
            }

        }


        //Combats
        static List<string> CombatsList = new List<string>();
        public static void GetAllCombats()
        {
            try
            {
                MatchCollection Match_AllCombats = Regex.Matches(Data.String_Combats, @"(?s)(\d+).*?(?=\d+=|\z)");

                //Get all ID's and Combats
                foreach (Match ID in Match_AllCombats)
                {
                    CombatsList.Add(ID.Value);
                }

                Console.WriteLine("All combats were read successfully");
            }
            catch
            {
                Console.WriteLine("Error reading all combats!");
            }
       


        }

        //Player Combat
        static string Player_Combat;
        public static void FindPlayerBattle(string PlayerID)
        {
            try
            {
                Player_Combat = CombatsList.FirstOrDefault(stringToCheck => stringToCheck.Contains($"commander={PlayerID}"));
                Console.WriteLine("Found player combat");
            }
            catch
            {
                Console.WriteLine("Error finding player combat");
            }
           
        }

        //Attacker Regiments
        static string Attacker;
        static public List<(string ID, string StartingNum, string CurrentNum)> Attacker_Regiments = new List<(string ID, string StartingNum, string CurrentNum)>();

        public static void GetAttackerRegiments()
        {

            try
            {
                //Get Attacker String
                string pattern = @"(attacker={[\s\S]*?)(defender={[\s\S]*?)phase";
                Attacker = Regex.Match(Player_Combat, pattern).Groups[1].Value;

                MatchCollection Regiments = Regex.Matches(Attacker, @"regiment=(?<ID>\d+)\s+starting=(?<StartingNum>[\d.]+)\s+current=(?<CurrentNum>[\d.]+)");
                (string ID, string StartingNum, string CurrentNum) data;
                foreach (Match regiment in Regiments)
                {
                    data.ID = regiment.Groups["ID"].Value;
                    data.StartingNum = regiment.Groups["StartingNum"].Value;
                    data.CurrentNum = regiment.Groups["CurrentNum"].Value;

                    Attacker_Regiments.Add(data);
                }
                Console.WriteLine("Regiments from attacker side read sucessfully");
            }
            catch
            {
                Console.WriteLine("Error reading attacker side regiments");
            }

        }

        static string Defender;
        static List<(string ID, string StartingNum, string CurrentNum)> Defender_Regiments = new List<(string ID, string StartingNum, string CurrentNum)>();
        public static void GetDefenderRegiments()
        {
            try
            {
                //Get Defender String
                string pattern = @"(attacker={[\s\S]*?)(defender={[\s\S]*?)phase";
                Defender = Regex.Match(Player_Combat, pattern).Groups[2].Value;

                MatchCollection Regiments = Regex.Matches(Defender, @"regiment=(?<ID>\d+)\s+starting=(?<StartingNum>[\d.]+)\s+current=(?<CurrentNum>[\d.]+)");
                (string ID, string StartingNum, string CurrentNum) data;
                foreach (Match regiment in Regiments)
                {
                    data.ID = regiment.Groups["ID"].Value;
                    data.StartingNum = regiment.Groups["StartingNum"].Value;
                    data.CurrentNum = regiment.Groups["CurrentNum"].Value;

                    Defender_Regiments.Add(data);
                }

                Console.WriteLine("Regiments from defender side read sucessfully");
            }
            catch
            {

                Console.WriteLine("Error reading defender side regiments");
            }

        }

        public static void GetCombatSides(Player Player, Enemy Enemy)
        {
            if(Attacker != null) 
            {
                string player_commander = "commander=" + Player.ID;
                if (Attacker.Contains(player_commander))
                {
                    Player.CombatSide = "attacker";
                    Enemy.CombatSide = "defender";
                }
                else
                {
                    Player.CombatSide = "defender";
                    Enemy.CombatSide = "attacker";
                }
            }
            else 
            {
                throw new NotImplementedException();
            }


        }

        public static void SetAttackerGUIRegiments()
        {
            try
            {

                MatchCollection Regiments = Regex.Matches(Attacker, @"regiment=(?<ID>\d+)\s+starting=(?<StartingNum>[\d.]+)\s+current=(?<CurrentNum>[\d.]+)");
                string Edited_Attacker = Attacker;

                double totalSoldiers = 0;
                for (int i = 0; i < Regiments.Count; i++)
                {

                    string remaining_num = Attacker_Regiments[i].CurrentNum;
                    remaining_num = string.Format("{0:0.00}", remaining_num);
                    remaining_num = remaining_num.Replace(',', '.');

                    double starting_num = FileData.ConvertDouble(Attacker_Regiments[i].StartingNum);

                    //If is not a knight
                    if (starting_num > 1)
                    {
                        totalSoldiers += FileData.ConvertDouble(remaining_num);

                        MatchCollection GG_Regiments = Regex.Matches(Edited_Attacker, @"current=(?<CurrentNum>[\d.|,]+)");
                        int index = GG_Regiments[i].Groups["CurrentNum"].Index;
                        int length = GG_Regiments[i].Groups["CurrentNum"].Length;

                        Edited_Attacker = Edited_Attacker.Remove(index, length)
                                                         .Insert(index, remaining_num);

                    }
                }
                Edited_Attacker = Regex.Replace(Edited_Attacker, @"total_fighting_men=\d+", "total_fighting_men=" + totalSoldiers.ToString());
                Attacker = Edited_Attacker;

                //Add edited GUI to Player Combat
                Player_Combat = Regex.Replace(Player_Combat, @"(attacker={[\s\S]*?)(defender={)", Attacker + "\n\t\t$2");

                Console.WriteLine("GUI from attacker side set sucessfully");

            }
            catch
            {
                Console.WriteLine("Error setting GUI from attacker side!");
            }
  
        }

        public static void SetDefenderGUIRegiments()
        {
            try
            {

                MatchCollection Regiments = Regex.Matches(Defender, @"regiment=(?<ID>\d+)\s+starting=(?<StartingNum>[\d.]+)\s+current=(?<CurrentNum>[\d.]+)");
                string Edited_Defender = Defender;

                double totalSoldiers = 0;
                for (int i = 0; i < Regiments.Count; i++)
                {
                    string remaining_num = Defender_Regiments[i].CurrentNum;
                    remaining_num = string.Format("{0:0.00}", remaining_num);
                    remaining_num = remaining_num.Replace(',', '.');

                    double starting_num = FileData.ConvertDouble(Defender_Regiments[i].StartingNum);

                    //If is not a knight
                    if (starting_num > 1)
                    {
                        totalSoldiers += FileData.ConvertDouble(remaining_num);
                        MatchCollection GG_Regiments = Regex.Matches(Edited_Defender, @"current=(?<CurrentNum>[\d.|,]+)");
                        int index = GG_Regiments[i].Groups["CurrentNum"].Index;
                        int length = GG_Regiments[i].Groups["CurrentNum"].Length;

                        Edited_Defender = Edited_Defender.Remove(index, length)
                                                         .Insert(index, remaining_num);

                    }
                }


                Edited_Defender = Regex.Replace(Edited_Defender, @"total_fighting_men=[\d.]+", "total_fighting_men=" + totalSoldiers.ToString());
                Defender = Edited_Defender;


                //Add edited GUI to Player Combat
                Player_Combat = Regex.Replace(Player_Combat, @"(defender={[\s\S]*?)(phase)", Defender + "\n\t\t\t$2");

                Console.WriteLine("GUI from defender side set sucessfully");

            }
            catch
            {
                Console.WriteLine("Error setting GUI from defender side!");

            }


        }

        public static void SetWinner(string PlayerID, string winner)
        {
            try
            {

                //Set pursuit phase
                Player_Combat = Regex.Replace(Player_Combat, @"(phase=)\w+", "$1" + "pursuit");

                //Set last day of phase
                Player_Combat = Regex.Replace(Player_Combat, @"(days=\d+)", "days=3");

                //Set winner
                Player_Combat = Regex.Replace(Player_Combat, @"(base_combat_width=\d+)", "$1\n\t\t\twinning_side=" + winner);

                




                int index = CombatsList.FindIndex(stringToCheck => stringToCheck.Contains($"commander={PlayerID}"));
                CombatsList[index] = Player_Combat;

                StringBuilder sb = new StringBuilder();
                foreach (var combat in CombatsList)
                {
                    sb.Append(combat);
                }
                string Edited_CombatList = sb.ToString();
                //AllCombats = Regex.Replace(AllCombats, @"\d+={[\s\S]*?\z", Edited_CombatList);
                Data.String_Combats = Regex.Replace(Data.String_Combats, @"\d+={[\s\S]*?\z", Edited_CombatList);
                Console.WriteLine("Winner of battld set sucessfully");
            }
            catch
            {
                Console.WriteLine("Erroe setting winner of battle!");
            }
      
        }

        static List<(string ID, string Type ,string[] ChunksIDs, string Full)> ArmyRegimentsList = new List<(string ID, string Type, string[] ChunksID, string Full)>();
        public static void GetAllArmyRegiments()
        {
            try
            {
                MatchCollection allRegiments = Regex.Matches(Data.String_ArmyRegiments, @"(?s)\s+(?<ID>\d+)={.*?(?=\t\t\d+={|\z)");

                (string ID, string Type, string[] ChunksIDs, string Full) data;
                List<string> chunks_id_list = new List<string>();
                string[] ChunkIDs;
                foreach (Match regiment in allRegiments)
                {
                    data.ID = regiment.Groups["ID"].Value;
                    data.Full = regiment.Value;

                    Match type_match = Regex.Match(regiment.Value, "type=(?<Type>\"\\w+\")");
                    if (type_match.Success)
                    {
                        data.Type = type_match.Groups["Type"].Value;
                    }
                    else
                    {
                        Match knight_match = Regex.Match(regiment.Value, @"knight");
                        if (knight_match.Success)
                        {
                            data.Type = "\"knight\"";
                        }
                        else
                        {
                            data.Type = "\"levy\"";
                        }

                    }

                    //Chunks
                    string Chunks = Regex.Match(regiment.Value, @"(?<Chunks>chunks={[\w\W]*?)\t\t\tcached={").Groups["Chunks"].Value;
                    MatchCollection chunks_matches = Regex.Matches(Chunks, @"regiment=(?<ChunkID>\d+)");
                    foreach (Match chunk in chunks_matches)
                    {
                        chunks_id_list.Add(chunk.Groups["ChunkID"].Value);

                    }
                    ChunkIDs = chunks_id_list.ToArray();

                    data.ChunksIDs = ChunkIDs;

                    ArmyRegimentsList.Add(data);
                    chunks_id_list.Clear();
                }

                Console.WriteLine("All army regiments read sucessfully");
            }
            catch
            {
                Console.WriteLine("Erroe reading all army regiments !");
            }
         
        }

        static List<(string ID, string Type, string Max, string Chunks, string Full)> RegimentsList = new List<(string ID, string Type, string Max, string Chunks, string Full)>();
        public static void GetAllRegiments()
        {
            try
            {
                MatchCollection allRegiments = Regex.Matches(Data.String_Regiments, "(?s)(?<ID>\\d+)={.*?(?=\\d+={|\\z)");
                (string ID, string Type, string Max, string Chunks, string Full) data;
                string type, max = String.Empty;
                foreach (Match regiment in allRegiments)
                {
                    data.ID = regiment.Groups["ID"].Value;
                    data.Full = regiment.Value;
                    data.Chunks = Regex.Match(regiment.Value, @"\Wchunks={[\s\S]*?\z").Value;

                    //--Type--
                    Match type_match = Regex.Match(regiment.Value, @"type=(?<MAA_Type>\W\w+\W)");
                    if (type_match.Success)
                    {
                        type = type_match.Groups["MAA_Type"].Value;
                    }
                    else
                    {
                        type = "levies";
                    }

                    data.Type = type;

                    //--Max--
                    if (data.Type == "levies")
                    {
                        MatchCollection size_match = Regex.Matches(regiment.Value, @"{\W+size=(?<Size>\d+)");
                        foreach (Match size_found in size_match)
                        {
                            double size_num = FileData.ConvertDouble(size_found.Groups["Size"].Value);
                            if (size_num != 0)
                            {
                                string Size = size_num.ToString();
                                max = Size;
                                break;
                            }
                        }

                        MatchCollection max_match = Regex.Matches(regiment.Value, @"{\W+max=(?<Size>\d+)");
                        foreach (Match max_found in max_match)
                        {
                            double max_num = FileData.ConvertDouble(max_found.Groups["Size"].Value);
                            if (max_num != 0)
                            {
                                string Size = max_num.ToString();
                                max = Size;
                                break;
                            }
                        }

                    }
                    else //Men at Arms
                    {
                        Match size_match = Regex.Match(regiment.Value, @"size=(?<Size>\d+)");
                        switch (size_match.Success)
                        {
                            case true:
                                string Size = size_match.Groups["Size"].Value;
                                max = Size;
                                break;

                            case false:
                                Match max_match = Regex.Match(regiment.Value, @"max=(?<Max>\d+)");
                                string Max = max_match.Groups["Max"].Value;
                                max = Max;
                                break;

                        }
                    }

                    data.Max = max;

                    RegimentsList.Add(data);
                }

                Console.WriteLine("All regiments read sucessfully");
            }
            catch
            {
                Console.WriteLine("Error reading all regiments!");
            }

        }

        public static void SetAttackerDATA()
        {
            try
            {
                long startMemory = GC.GetTotalMemory(false);

                FileData.SetChunks(Data.String_ArmyRegiments, ref Data.String_Regiments, Attacker_Regiments, ArmyRegimentsList, RegimentsList);
                Console.WriteLine("Attacker Chunks data set sucessfully");

                FileData.SetCache(ref Data.String_ArmyRegiments, Attacker_Regiments, ArmyRegimentsList);
                Console.WriteLine("Attacker Cache data set sucessfully");

                long endMemory = GC.GetTotalMemory(false);
                long memoryUsage = endMemory - startMemory;

                Console.WriteLine($"----\nSetting attacker casualities of battle...\nMemory Usage: {memoryUsage / 1048576} mb\n----");
            }
            catch 
            {
                Console.WriteLine("Error setting Attacker DATA");
            }

        }

        public static void SetDefenderDATA()
        {
            try
            {
                long startMemory = GC.GetTotalMemory(false);

                FileData.SetChunks(Data.String_ArmyRegiments, ref Data.String_Regiments, Defender_Regiments, ArmyRegimentsList, RegimentsList);
                Console.WriteLine("Defender Chunks data set sucessfully");

                FileData.SetCache(ref Data.String_ArmyRegiments, Defender_Regiments, ArmyRegimentsList);
                Console.WriteLine("Defender Cache data set sucessfully");

                long endMemory = GC.GetTotalMemory(false);
                long memoryUsage = endMemory - startMemory;

                Console.WriteLine($"----\nSetting defender casualities of battle...\nMemory Usage: {memoryUsage / 1048576} mb\n----");
            }
            catch
            {
                Console.WriteLine("Error setting Defender DATA");
            }

        }

        public static void SendToSaveFile(string filePath)
        {
            try
            {
                Reader.SendDataToFile(filePath);
            }
            catch
            {
                Console.WriteLine("Error replacing data in save file!!");
            }


            Data.Reset();

            Player_Combat = "";

            Attacker = "";
            Defender= "";

            Attacker_Regiments = new List<(string ID, string StartingNum, string CurrentNum)>();
            Defender_Regiments = new List<(string ID, string StartingNum, string CurrentNum)>();

            CombatsList = new List<string>();

            ArmyRegimentsList = new List<(string ID, string Type, string[] ChunksIDs, string Full)>();
            RegimentsList = new List<(string ID, string Type, string Max, string Chunks, string Full)>();

            GC.Collect();
        }

        public static void SetRemainingAttacker(List<(string Name, string Remaining)> Attila_RemainingSoldiers)
        {
            try
            {
                FileData.SetRemainingSoldiers(Attila_RemainingSoldiers, ref Attacker_Regiments, ArmyRegimentsList);
                Console.WriteLine("Attacker Remaining Soldiers are set!");
            }
            catch 
            {
                Console.WriteLine("Error setting attacker side remaining soldiers!");
            }
            
        }

        public static void SetRemainingDefender(List<(string Name, string Remaining)> Attila_RemainingSoldiers)
        {
            try
            {
                FileData.SetRemainingSoldiers(Attila_RemainingSoldiers, ref Defender_Regiments, ArmyRegimentsList);
                Console.WriteLine("Defender Remaining Soldiers are set!");
            }
            catch
            {
                Console.WriteLine("Error setting defender side remaining soldiers!");
            }
            
        }




        //---------------------------------//
        //----------Functions--------------//
        //---------------------------------//


        // Get attila remaining soldiers
        public static List<(string Name, string Remaining)> GetRemainingSoldiersData(string path_attila_log)
        {
            try
            {
                List<(string Name, string Remaining)> RemainingSoldiers = new List<(string Name, string Remaining)>();

                using (FileStream logFile = File.Open(path_attila_log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(logFile))
                {
                    string line;
                    bool isFirst = false;
                    while ((line = reader.ReadLine()) != null)
                    {
                        //if its a copy of the results stop.
                        if (line == "-----REMAINING SOLDIERS-----!!" && isFirst) break;

                        if (line == "-----REMAINING SOLDIERS-----!!") isFirst = true;



                        if(line.StartsWith("player_") || line.StartsWith("enemy_"))
                        {
                            Match pattern = Regex.Match(line, "(?<Unit>.+)-(?<Remaining>.+)");
                            if (pattern.Success)
                            {
                                string unit_name = pattern.Groups["Unit"].Value;
                                string remaining = pattern.Groups["Remaining"].Value;

                                RemainingSoldiers.Add((unit_name, remaining));
                            }

                        }
                    }

                   
                    reader.Close();
                    logFile.Close();

                    Console.WriteLine("Attila remaining soldiers read sucessfully!");

                    return RemainingSoldiers;
                }

                
            }
            catch
            {
                Console.WriteLine("Error reading Attila remaining soldiers!");
                return null;
            }
       
        }

        //Get winner from Attila
        public static string GetAttilaWinner(string path_attila_log, string playerCombatSide, string enemyCombatSide)
        {
            string winner = "";
            using (FileStream logFile = File.Open(path_attila_log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(logFile))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("Victory")) {winner = playerCombatSide; break; }
                    else if (line.Contains("Defeat")) { winner = enemyCombatSide; break; }
                    else winner = enemyCombatSide;
                }

                reader.Close();
                logFile.Close();

                return winner;
            }


        }

        public static bool HasBattleEnded(string path_attila_log)
        {

            using (FileStream logFile = File.Open(path_attila_log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(logFile))
            {
                string str = reader.ReadToEnd();


                if (str.Contains("Battle has finished"))
                {
                    reader.Close();
                    logFile.Close();
                    return true;
                }
                else { return false; }

            }
        }


        public static void ClearAttilaLog()
        {
            string Attila_Path = Properties.Settings.Default.VAR_attila_path;
            Properties.Settings.Default.VAR_log_attila = Attila_Path.Substring(0, Attila_Path.IndexOf("Attila.exe")) + "data\\BattleResults_log.txt";
            Properties.Settings.Default.Save();
            string path_attila_log = Properties.Settings.Default.VAR_log_attila;

            bool isCreated = false;
            if (isCreated == false)
            {
                using (FileStream logFile = File.Open(path_attila_log, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    isCreated = true;
                    logFile.Close();
                }
            }
        }

    }
}
