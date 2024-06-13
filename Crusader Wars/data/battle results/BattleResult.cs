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
        public static twbattle.Date FirstDay_Date { get; set; }


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
                Console.WriteLine("Combat ID - " + battleID);
                File.WriteAllText(@".\data\save_file_data\temp\Combats.txt", Player_Combat);
                File.WriteAllText(@".\data\save_file_data\Combats.txt", "");

                ArmiesReader.ReadCombats(Player_Combat);
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
                        else if (isSearchStarted && line.Contains("\t\t\tstart_date="))
                        {
                            f.AppendLine(line);
                            Match date = Regex.Match(line, @"(?<year>\d+).(?<month>\d+).(?<day>\d+)");
                            string year = date.Groups["year"].Value, month = date.Groups["month"].Value, day = date.Groups["day"].Value;
                            FirstDay_Date = new twbattle.Date(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));

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
                Console.WriteLine("ResultID - " + battle_id);
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

        static void EditRegiments()
        {

        }

        static void EditCombatGUI()
        {

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
 

        public static void SendToSaveFile(string filePath)
        {
            Writter.SendDataToFile(filePath);


            Data.Reset();

            Player_Combat = "";


            GC.Collect();
        }





        //---------------------------------//
        //----------Functions--------------//
        //---------------------------------//


        // Get attila remaining soldiers
        public static void GetUnitsData(Army army, string path_attila_log)
        {
            UnitsResults units = new UnitsResults();
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

                        Alive_MainPhase = ReturnList(army, alive_text, DataType.Alive);
                        units.SetAliveMainPhase(Alive_MainPhase);
                        Kills_MainPhase = ReturnList(army, kills_text, DataType.Kills);
                        units.SetKillsMainPhase(Kills_MainPhase);

                    }
                    else if(remaining_soldiers_ocurrences > 1 && kills_ocurrences > 1) 
                    {
                        string mainphase_alive_text = all_alive_texts[0].Groups[2].Value;
                        Alive_MainPhase = ReturnList(army, mainphase_alive_text, DataType.Alive);
                        units.SetAliveMainPhase(Alive_MainPhase);

                        string pursuitphase_alive_text = all_alive_texts[1].Groups[2].Value;
                        Alive_PursuitPhase = ReturnList(army, pursuitphase_alive_text, DataType.Alive);
                        units.SetAlivePursuitPhase (Alive_PursuitPhase);

                        string mainphase_kills_text = all_kills_texts[0].Groups[1].Value;
                        Kills_MainPhase = ReturnList(army, mainphase_kills_text, DataType.Kills);
                        units.SetKillsMainPhase(Kills_MainPhase);

                        string pursuitphase_kills_text = all_kills_texts[1].Groups[1].Value;
                        Kills_PursuitPhase = ReturnList(army, pursuitphase_kills_text, DataType.Kills);
                        units.SetKillsPursuitPhase(Kills_PursuitPhase) ;
                    }
 
                    reader.Close();
                    logFile.Close();


                }

                army.UnitsResults = units;


                
            }
            catch
            {
                MessageBox.Show("Error reading Attila results", "Battle Results Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                throw new Exception();
                
            }
       
        }

        private enum DataType
        {
            Alive,
            Kills
        }

        private static List<(string, string)> ReturnList(Army army, string text, DataType list_type)
        {

            List<(string, string)> list = new List<(string, string)> ();

            if(army.IsPlayer())
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
