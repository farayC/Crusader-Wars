using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using System.IO.Pipes;
using Crusader_Wars.data;
using System.Diagnostics.Eventing.Reader;
using Crusader_Wars.data.battle_results;
using System.CodeDom;
using Crusader_Wars.data.save_file;

namespace Crusader_Wars
{
    public static class BattleResult
    {

        public static string CombatID { get; set; }
        public static string ResultID { get; set; }
        public static string ProvinceID { get; set; }

        public static void LoadSaveFile(string savePath)
        {
            Reader.ReadFile(savePath);

        }


        //Combats
        static string Player_Combat;
        public static void ReadPlayerCombat(string playerID)
        {
            try
            {
                bool isSearchStarted = false;
                string battleID = "";
                StringBuilder sb = new StringBuilder();
                using(StreamReader sr = new StreamReader(@".\data\save_file_data\Combats.txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line == null) break;
                        if (Regex.IsMatch(line, @"\t\t\d+={"))
                        {
                            battleID = Regex.Match(line, @"\t\t(\d+)={").Groups[1].Value;
                        }
                        else if (line == $"\t\t\t\tcommander={playerID}")
                        {
                            break;
                        }
                    }

                    sr.BaseStream.Position = 0;
                    sr.DiscardBufferedData();

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if(line == null) break;

                        //Battle ID
                        if (!isSearchStarted && line == $"\t\t{battleID}={{")
                        {
                            sb.AppendLine(line);
                            isSearchStarted = true;
                        }
                        //Battle end line
                        else if (isSearchStarted && line == "\t\t}")
                        {
                            sb.AppendLine(line);
                            isSearchStarted = false;
                            break;
                        }
                        else if (isSearchStarted)
                        {
                            sb.AppendLine(line);
                        }
                    }
                }
                BattleResult.CombatID = battleID;
                Player_Combat = sb.ToString();
                File.WriteAllText(@".\data\save_file_data\temp\Combats.txt", Player_Combat);
                File.WriteAllText(@".\data\save_file_data\Combats.txt", "");
                Console.WriteLine("All combats were read successfully");
            }
            catch
            {
                Console.WriteLine("Error reading all combats!");
            }
       


        }

        public static void GetPlayerCombatResult()
        {
            try
            {
                string battle_id="";
                StringBuilder f = new StringBuilder();
                using(StreamReader sr = new StreamReader(@".\data\save_file_data\BattleResults.txt"))
                {
                    while(!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line == null) break;
                        if (Regex.IsMatch(line, @"\t\t\d+={"))
                        {
                            battle_id = Regex.Match(line, @"\t\t(\d+)={").Groups[1].Value;
                        }
                        else if (line == $"\t\t\tlocation={ProvinceID}")
                        {
                            break;
                        }
                    }

                    sr.BaseStream.Position = 0;
                    sr.DiscardBufferedData();

                    bool isSearchStarted = false;
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line == null) break;
                        if (line == $"\t\t{battle_id}={{")
                        {
                            f.AppendLine(line);
                            isSearchStarted = true;
                        }
                        else if(isSearchStarted && line == "\t\t}")
                        {
                            f.AppendLine(line);
                            isSearchStarted = false;
                            break;
                        }
                        else if (isSearchStarted)
                        {
                            f.AppendLine(line);
                        }
                    }
                }

                BattleResult.ResultID = battle_id;
                File.WriteAllText(@".\data\save_file_data\BattleResults.txt", f.ToString());
                Console.WriteLine("All combat results were read successfully");
            }
            catch
            {
                Console.WriteLine("Error reading all combat results!");
            }
        }

        public static void EditCombatResults(ICharacter player,ICharacter enemy, bool side)
        {
            string MAAtype = "";

            var regiment_type = RegimentType.None; 
            using (StreamReader sr = new StreamReader(@".\data\save_file_data\BattleResults.txt"))
            using (StreamWriter sw = new StreamWriter(@".\data\save_file_data\temp\BattleResults.txt"))
            {
                bool isSearchingAttacker = false;
                bool isSearchingDefender = false;

                string line = "";
                while (line != null)
                {
                    line = sr.ReadLine();
                    if (line == null) break;
                    if (line.Contains("\t\t\tattacker={"))
                    {
                        isSearchingAttacker = true;
                        isSearchingDefender = false;
                    }
                    else if (line.Contains("\t\t\tdefender={"))
                    {
                        isSearchingAttacker = false;
                        isSearchingDefender = true;
                    }
                    else if (line.Contains("\t\t\twinning_side="))
                    {
                        isSearchingAttacker = false;
                        isSearchingDefender = false;
                    }



                    //
                    //Levies & Knights Search
                    if (line.Contains("\tknight=") && regiment_type != RegimentType.MenAtArms)
                    {
                        string id = Regex.Match(line, @"\d+").Value;
                        int id_num = 0;

                        //isKnight
                        if (int.TryParse(id, out id_num))
                        {
                            regiment_type = RegimentType.Knight;
                        }
                        else // isLevies
                        {
                            regiment_type = RegimentType.Levies;
                        }
                    }

                    //
                    //MenAtArm Search
                    if (line.Contains("\ttype=") && regiment_type == RegimentType.None)
                    {
                        regiment_type = RegimentType.MenAtArms;
                        MAAtype = Regex.Match(line, @"""(.+)""").Groups[1].Value;

                        //skip if they are siege maa
                        if (MAAtype == "mangonel" || MAAtype == "onager" || MAAtype == "bombard" || MAAtype == "trebuchet") { 
                            sw.WriteLine(line); 
                            continue; 
                        }
                    }
                    



                    //
                    //Data Editing
                    if (side is false)
                    {
                        if(isSearchingAttacker == true)
                        {
                            line = SearchRegiment(line, player, MAAtype, regiment_type);
                        }
                        else if (isSearchingDefender == true) 
                        {
                            line = SearchRegiment(line, enemy, MAAtype, regiment_type);
                        }
                    }
                    else
                    {
                        if (isSearchingAttacker == true)
                        {
                            line = SearchRegiment(line, enemy, MAAtype, regiment_type);
                        }
                        else if (isSearchingDefender == true)
                        {
                            line = SearchRegiment(line, player, MAAtype, regiment_type);
                        }
                    }

                    if (line.Contains("\tfinal_count=") && regiment_type != RegimentType.None)
                        regiment_type = RegimentType.None;

                    sw.WriteLine(line);
                }
            }
            File.Delete(@".\data\save_file_data\BattleResults.txt");
            File.Copy(@".\data\save_file_data\temp\BattleResults.txt", @".\data\save_file_data\BattleResults.txt");
        }

        enum RegimentType
        {
            Levies,
            MenAtArms,
            Knight,
            None
        }

        private static string SearchRegiment(string line, ICharacter Side, string MAAtype, RegimentType type)
        {
            //
            //  MEN AT ARMS
            //
            if (type is RegimentType.MenAtArms)
            {
                return GUI_BattleResultTab.MenAtArms(line, Side, MAAtype);
            }

            //
            //  LEVIES
            //                  
            if (type is RegimentType.Levies)
            {
                return GUI_BattleResultTab.Levies(line, Side);
            }

            return line;
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

        public static void SetWinner(string winner)
        {
            try
            {
                //Set pursuit phase
                Player_Combat = Regex.Replace(Player_Combat, @"(phase=)\w+", "$1" + "pursuit");

                //Set last day of phase
                Player_Combat = Regex.Replace(Player_Combat, @"(days=\d+)", "days=3");

                //Set winner
                Player_Combat = Regex.Replace(Player_Combat, @"(base_combat_width=\d+)", "$1\n\t\t\twinning_side=" + winner);

                File.WriteAllText(@".\data\save_file_data\Combats.txt", Player_Combat);

                Console.WriteLine("Winner of battle set sucessfully");
            }
            catch
            {
                Console.WriteLine("Error setting winner of battle!");
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
            Writter.SendDataToFile(filePath);


            Data.Reset();

            Attacker = "";
            Defender= "";
            Player_Combat = "";


            Attacker_Regiments = new List<(string ID, string StartingNum, string CurrentNum)>();
            Defender_Regiments = new List<(string ID, string StartingNum, string CurrentNum)>();

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
        public static void GetUnitsData(ICharacter Side, string path_attila_log)
        {
            Units units = new Units();
            try
            {
                List<(string Name, string Remaining)> Alive_MainPhase = new List<(string Name, string Remaining)>();
                List<(string Name, string Remaining)> Alive_PursuitPhase = new List<(string Name, string Remaining)>();
                List<(string Name, string Remaining)> Kills_MainPhase = new List<(string Name, string Kills)>();
                List<(string Name, string Remaining)> Kills_PursuitPhase = new List<(string Name, string Kills)>();

                using (FileStream logFile = File.Open(path_attila_log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(logFile))
                {
                    string alltext = reader.ReadToEnd();

                    MatchCollection all_alive_texts = Regex.Matches(alltext, "-----REMAINING SOLDIERS-----!!\\r\\n([\\s\\S]*?)([\\s\\S]*?)-----NUMBERS OF KILLS-----!!");
                    MatchCollection all_kills_texts = Regex.Matches(alltext, "-----NUMBERS OF KILLS-----!!\\r\\n([\\s\\S]*?)(-----REMAINING SOLDIERS-----!!|Battle has finished)");
                    string alive_text = "";
                    string kills_text = "";

                    int remaining_soldiers_ocurrences = Regex.Matches(alltext, "-----REMAINING SOLDIERS-----!!").Count;
                    int kills_ocurrences = Regex.Matches(alltext, "-----NUMBERS OF KILLS-----!!").Count;


                    if(remaining_soldiers_ocurrences == 1 && kills_ocurrences == 1)
                    {
                        alive_text = all_alive_texts[0].Groups[2].Value;
                        kills_text = all_kills_texts[0].Groups[1].Value;

                        Alive_MainPhase = ReturnList(Side, alive_text, DataType.Alive);
                        units.SetAliveMainPhase(Alive_MainPhase);
                        Kills_MainPhase = ReturnList(Side, kills_text, DataType.Kills);
                        units.SetKillsMainPhase(Kills_MainPhase);

                    }
                    else if(remaining_soldiers_ocurrences > 1 && kills_ocurrences > 1) 
                    {
                        string mainphase_alive_text = all_alive_texts[0].Groups[2].Value;
                        Alive_MainPhase = ReturnList(Side, mainphase_alive_text, DataType.Alive);
                        units.SetAliveMainPhase(Alive_MainPhase);

                        string pursuitphase_alive_text = all_alive_texts[1].Groups[2].Value;
                        Alive_PursuitPhase = ReturnList(Side, pursuitphase_alive_text, DataType.Alive);
                        units.SetAlivePursuitPhase (Alive_PursuitPhase);

                        string mainphase_kills_text = all_kills_texts[0].Groups[1].Value;
                        Kills_MainPhase = ReturnList(Side, mainphase_kills_text, DataType.Kills);
                        units.SetKillsMainPhase(Kills_MainPhase);

                        string pursuitphase_kills_text = all_kills_texts[1].Groups[1].Value;
                        Kills_PursuitPhase = ReturnList(Side, pursuitphase_kills_text, DataType.Kills);
                        units.SetKillsPursuitPhase(Kills_PursuitPhase) ;
                    }
 
                    reader.Close();
                    logFile.Close();


                }

                Side.UnitsResults = units;


                
            }
            catch
            {
                Console.WriteLine("Error reading Attila results!");
                throw new Exception();
                
            }
       
        }

        private enum DataType
        {
            Alive,
            Kills
        }

        private static List<(string, string)> ReturnList(ICharacter Side, string text, DataType list_type)
        {

            List<(string, string)> list = new List<(string, string)> ();

            if(Side is Player)
            {
                MatchCollection pattern;
                switch (list_type)
                {
                    case DataType.Alive:
                        pattern = Regex.Matches(text, "\\n(?<Unit>player_.+)-(?<Remaining>.+)");
                        foreach (Match match in pattern)
                        {
                            string unit_name = match.Groups["Unit"].Value;
                            string remaining = match.Groups["Remaining"].Value;

                            list.Add((unit_name, remaining));
                        }
                        break;
                    case DataType.Kills:
                        pattern = Regex.Matches(text, "(?<Unit>kills_player_.+)-(?<Remaining>.+)");
                        foreach (Match match in pattern)
                        {
                            string unit_name = match.Groups["Unit"].Value;
                            string remaining = match.Groups["Remaining"].Value;

                            list.Add((unit_name, remaining));
                        }
                        break;
                }
            }
            //Enemy
            else
            {
                MatchCollection pattern;
                switch (list_type)
                {
                    case DataType.Alive:
                        pattern = Regex.Matches(text, "\\n(?<Unit>enemy_.+)-(?<Remaining>.+)");
                        foreach (Match match in pattern)
                        {
                            string unit_name = match.Groups["Unit"].Value;
                            string remaining = match.Groups["Remaining"].Value;

                            list.Add((unit_name, remaining));
                        }
                        break;
                    case DataType.Kills:
                        pattern = Regex.Matches(text, "(?<Unit>kills_enemy_.+)-(?<Remaining>.+)");
                        foreach (Match match in pattern)
                        {
                            string unit_name = match.Groups["Unit"].Value;
                            string remaining = match.Groups["Remaining"].Value;

                            list.Add((unit_name, remaining));
                        }
                        break;
                }
            }



            return list;
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
