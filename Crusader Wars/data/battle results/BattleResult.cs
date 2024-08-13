using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Text;
using Crusader_Wars.data.save_file;
using static Crusader_Wars.data.save_file.Writter;


namespace Crusader_Wars
{
    public static class BattleResult
    {

        public static string CombatID { get; set; }
        public static string ResultID { get; set; }
        public static string ProvinceID { get; set; }
        //public static twbattle.Date FirstDay_Date { get; set; }


        //Combats
        public static string Player_Combat;
        public static void ReadPlayerCombat(string playerID)
        {
            try
            {
                bool isSearchStarted = false;
                string battleID = "";
                StringBuilder sb = new StringBuilder();
                using(StreamReader sr = new StreamReader(DataFilesPaths.Combats_Path()))
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
                File.WriteAllText(DataFilesPaths.Combats_Path(), Player_Combat);

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
                            //FirstDay_Date = new twbattle.Date(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));

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
        public static (List<string> AliveList, List<string> KillsList) GetRemainingAndKills(string path_attila_log)
        {
            string aliveText = "";
            string killsText = "";

            bool aliveSearchStarted = false;
            bool killsSearchStarted = false;

            List<string> alive_list = new List<string>();
            List<string> kills_list = new List<string>();

            using (StreamReader reader = new StreamReader(path_attila_log))
            {
                string line;
                while((line = reader.ReadLine()) != null && !reader.EndOfStream)
                {
                    if(line == "-----REMAINING SOLDIERS-----!!")
                    {
                        aliveSearchStarted = true;
                        killsSearchStarted = false;
                    }

                    else if (line == "-----NUMBERS OF KILLS-----!!")
                    {
                        aliveSearchStarted = false;
                        killsSearchStarted = true;
                    }

                    else if (line == "-----PRINT ENDED-----!!")
                    {
                        alive_list.Add(aliveText);
                        kills_list.Add(killsText);
                        aliveText = "";
                        killsText = "";
                        aliveSearchStarted = false;
                        killsSearchStarted = false;
                    }

                    else if(aliveSearchStarted)
                    {
                        aliveText += line + "\n";
                    }

                    else if(killsSearchStarted && line.StartsWith("kills"))
                    {
                        killsText += line + "\n";
                    }
                }
            }

            return (alive_list, kills_list);
        }

        // Get attila remaining soldiers
        public static void ReadAttilaResults(Army army, string path_attila_log)
        {

            UnitsResults units = new UnitsResults();
            List<(string Script, string Type, string CultureID, string Remaining)> Alive_MainPhase = new List<(string Script, string Type, string CultureID, string Remaining)>();
            List<(string Script, string Type, string CultureID, string Remaining)> Alive_PursuitPhase = new List<(string Script, string Type, string CultureID, string Remaining)>();
            List<(string Script, string Type, string CultureID, string Kills)> Kills_MainPhase = new List<(string Name, string Type, string CultureID, string Kills)>();
            List<(string Script, string Type, string CultureID, string Kills)> Kills_PursuitPhase = new List<(string Name, string Type, string CultureID, string Kills)>();

            var (AliveList, KillsList) = GetRemainingAndKills(path_attila_log);
            if (AliveList.Count == 1)
            {
                Alive_MainPhase = ReturnList(army, AliveList[0], DataType.Alive);
                units.SetAliveMainPhase(Alive_MainPhase);
                Kills_MainPhase = ReturnList(army, KillsList[0], DataType.Kills);
                units.SetKillsMainPhase(Kills_MainPhase);

            }
            else if (AliveList.Count > 1)
            {
                Alive_MainPhase = ReturnList(army, AliveList[0], DataType.Alive);
                units.SetAliveMainPhase(Alive_MainPhase);

                Alive_PursuitPhase = ReturnList(army, AliveList[1], DataType.Alive);
                units.SetAlivePursuitPhase(Alive_PursuitPhase);

                Kills_MainPhase = ReturnList(army, KillsList[0], DataType.Kills);
                units.SetKillsMainPhase(Kills_MainPhase);

                Kills_PursuitPhase = ReturnList(army, KillsList[1], DataType.Kills);
                units.SetKillsPursuitPhase(Kills_PursuitPhase);
            }

            army.UnitsResults = units;
            army.UnitsResults.ScaleTo100Porcent();

            CreateUnitsReports(army);
            ChangeRegimentsSoldiers(army);

            try
            {

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


        private static List<(string, string, string, string)> ReturnList(Army army, string text, DataType list_type)
        {
            var list = new List<(string, string, string, string)> ();

            MatchCollection pattern;
            switch (list_type)
            {
                case DataType.Alive:
                    pattern = Regex.Matches(text, $@"(?<Unit>.+_army{army.ID}_TYPE(?<Type>.+)_CULTURE(?<Culture>.+)_.+)-(?<Remaining>.+)");
                    foreach (Match match in pattern)
                    {
                        string culture_match = match.Groups["Culture"].Value;

                        string unit_script = match.Groups["Unit"].Value;
                        string remaining = match.Groups["Remaining"].Value;
                        string culture_id = Regex.Match(culture_match, @"(?<Name>\D+)(?<ID>\d+)").Groups["ID"].Value;
                        string type = Regex.Match(match.Groups["Type"].Value, @"\D+").Value;

                        list.Add((unit_script, type, culture_id, remaining));
                    }
                    break;
                case DataType.Kills:
                    pattern = Regex.Matches(text, $@"(?<Unit>kills_.+_army{army.ID}_TYPE(?<Type>.+)_CULTURE(?<Culture>.+)_.+)-(?<Kills>.+)");
                    foreach (Match match in pattern)
                    {
                        string culture_match = match.Groups["Culture"].Value;

                        string unit_script = match.Groups["Unit"].Value;
                        string kills = match.Groups["Kills"].Value;
                        string culture_id = Regex.Match(culture_match, @"(?<Name>\D+)(?<ID>\d+)").Groups["ID"].Value;
                        string type = Regex.Match(match.Groups["Type"].Value, @"\D+").Value;

                        list.Add((unit_script, type, culture_id, kills));
                    }
                    break;
            }

            return list;
        }


        static void ChangeRegimentsSoldiers(Army army)
        {
            foreach(ArmyRegiment armyRegiment in army.ArmyRegiments)
            {
                if (armyRegiment.Type == data.save_file.RegimentType.Commander || armyRegiment.Type == data.save_file.RegimentType.Knight) continue;

                foreach (Regiment regiment in armyRegiment.Regiments)
                {
                    if(regiment.Culture is null ) continue; // skip siege maa

                    var unitReport = army.CasualitiesReports.FirstOrDefault(x => x.GetUnitType() == armyRegiment.Type && x.GetCulture().ID == regiment.Culture.ID && x.GetTypeName() == armyRegiment.MAA_Name);
                    if (unitReport == null)
                        continue;

                    int regSoldiers = Int32.Parse(regiment.CurrentNum);
                    int killed = unitReport.GetKilled();

                    while (regSoldiers > 0 && killed > 0)
                    {
                        if (regSoldiers > killed)
                        {
                            regSoldiers -= killed;
                            killed = 0;
                        }
                        else
                        {
                            killed -= regSoldiers;
                            regSoldiers = 0;
                        }
                    }

                    regiment.SetSoldiers(regSoldiers.ToString());
                    unitReport.SetKilled(killed);

                    int army_regiment_total = armyRegiment.Regiments.Where(reg => reg.CurrentNum != null).Sum(x => Int32.Parse(x.CurrentNum));
                    armyRegiment.SetCurrentNum(army_regiment_total.ToString());

                }
            }
        }

        static void CreateUnitsReports(Army army)
        {
            List<UnitCasualitiesReport> reportsList = new List<UnitCasualitiesReport>();

            // Group by Type and CultureID
            var grouped = army.UnitsResults.Alive_MainPhase.GroupBy(item => new { item.Type, item.CultureID });
            var pursuit_grouped = army.UnitsResults.Alive_PursuitPhase?.GroupBy(item => new { item.Type, item.CultureID });

            Console.WriteLine("#############################");
            Console.WriteLine($"REPORT FROM {army.CombatSide.ToUpper()} ARMY {army.ID}");
            foreach(var group in grouped)
            {
                RegimentType unitType;
                if (group.Key.Type.Contains("Levy")) { unitType = RegimentType.Levy; }
                else if (group.Key.Type.Contains("commander") || group.Key.Type == "knights") { continue; }
                else { unitType = RegimentType.MenAtArms; }

                string type = Regex.Match(group.Key.Type, @"\D+").Value;
                Culture culture = army.Units.FirstOrDefault(x => x.GetObjCulture().ID == group.Key.CultureID).GetObjCulture();

                int starting = army.Units.First(x => x.GetRegimentType() == unitType && x.GetObjCulture().ID == culture.ID && x.GetName() == type).GetSoldiers();
                int remaining = group.Sum(x => Int32.Parse(x.Remaining));

                UnitCasualitiesReport unitReport;
                if (pursuit_grouped != null)
                {
                    var pursuitGroup = pursuit_grouped.FirstOrDefault(x => x.Key.Type == group.Key.Type && x.Key.CultureID == group.Key.CultureID);
                    int pursuitRemaining = pursuitGroup.Sum(x => Int32.Parse(x.Remaining));
                    unitReport = new UnitCasualitiesReport(unitType, type, culture, starting, remaining, pursuitRemaining);
                }
                else
                {
                    unitReport = new UnitCasualitiesReport(unitType, type, culture, starting, remaining);
                }

                
                unitReport.PrintReport();

                reportsList.Add(unitReport);
            }

            army.SetCasualitiesReport(reportsList);
        }

        public static void CheckForDeathCommanders(Army army, string path_attila_log)
        {
            if (army.Commander != null)
                army.Commander.HasGeneralFallen(path_attila_log);
        }

        public static void CheckForDeathKnights(Army army)
        {
            if(army.Knights != null && army.Knights.HasKnights())
            {
                int remaining = 0;
                if (army.UnitsResults.Alive_PursuitPhase != null)
                {
                    remaining = Int32.Parse(army.UnitsResults.Alive_PursuitPhase.FirstOrDefault(x => x.Type == "knights").Remaining);
                }
                else
                {
                    remaining = Int32.Parse(army.UnitsResults.Alive_MainPhase.FirstOrDefault(x => x.Type == "knights").Remaining);
                }
                army.Knights.GetKilled(remaining);
            }
        }

        static (bool searchStarted, bool isCommander, CommanderSystem commander, bool isKnight, Knight knight) SearchCharacters(string char_id, List<Army> armies)
        {

            foreach (Army army in armies)
            {
                if (army.Commander != null && army.Commander.ID == char_id)
                {
                    return (true, true, army.Commander, false, null);
                }
                else if (army.Knights.GetKnightsList() != null)
                {
                    foreach(Knight knight_u in army.Knights.GetKnightsList())
                    {
                        if(knight_u.GetID() == char_id)
                        {
                            return (true, false, null, true, knight_u);
                        }
                    }
                }

                if (army.MergedArmies != null)
                {
                    foreach (Army mergedArmy in army.MergedArmies)
                    {
                        if (mergedArmy.Commander != null && mergedArmy.Commander.ID == char_id)
                        {
                            return (true, true, army.Commander, false, null);
                        }
                        else if (mergedArmy.Knights.GetKnightsList() != null)
                        {
                            foreach (Knight knight_u in army.Knights.GetKnightsList())
                            {
                                if (knight_u.GetID() == char_id)
                                {
                                    return (true, false, null, true, knight_u);
                                }
                            }
                        }
                    }
                }
            }

            return (false, false, null, false, null);
        }

        public static void EditLivingFile(List<Army> attacker_armies, List<Army> defender_armies)
        {
            using (StreamReader streamReader = new StreamReader(DataFilesPaths.Living_Path()))
            using (StreamWriter streamWriter = new StreamWriter(DataTEMPFilesPaths.Living_Path()))
            {
                streamWriter.NewLine = "\n";

                bool searchStarted = false;
                bool isCommander = false;
                bool isKnight = false;

                CommanderSystem commander = null;
                Knight knight = null;


                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if(!searchStarted && Regex.IsMatch(line, @"\d+={"))
                    {
                        string char_id = Regex.Match(line, @"\d+").Value;

                        var searchData = SearchCharacters(char_id, attacker_armies);
                        if(searchData.searchStarted)
                        {
                            searchStarted = true;
                            if(searchData.isCommander)
                            {
                                isCommander = true;
                                commander = searchData.commander;
                            }
                            else if(searchData.isKnight)
                            {
                                isKnight = true;
                                knight = searchData.knight;
                            }
                        }
                        else
                        {
                            searchData = SearchCharacters(char_id, defender_armies);
                            if(searchData.searchStarted)
                            {
                                if (searchData.isCommander)
                                {
                                    isCommander = true;
                                    commander = searchData.commander;
                                }
                                else if (searchData.isKnight)
                                {
                                    isKnight = true;
                                    knight = searchData.knight;
                                }
                            }
                        }
                    }

                    else if(searchStarted && line.StartsWith("\ttraits={"))
                    {
                        string edited_line = line;
                        if (isCommander && commander.hasFallen)
                        {
                            edited_line = commander.Health(edited_line);
                        }
                        else if(isKnight)
                        {
                            edited_line = knight.Health(edited_line);
                        }

                        streamWriter.WriteLine(edited_line);
                        continue;
                    }

                    else if(searchStarted && line == "}")
                    {
                        searchStarted = false;
                        isCommander = false;
                        isKnight = false;
                        commander = null;
                        knight = null;
                    }

                    streamWriter.WriteLine(line);
                }
            }
        }

        public static void EditCombatResultsFile(List<Army> attacker_armies, List<Army> defender_armies)
        {
            using (StreamReader streamReader = new StreamReader(DataFilesPaths.CombatResults_Path()))
            using (StreamWriter streamWriter = new StreamWriter(DataTEMPFilesPaths.CombatResults_Path()))
            {
                streamWriter.NewLine = "\n";

                bool isAttacker = false;
                bool isDefender = false;

                bool isLevy = false;
                bool isMAA = false;
                bool isKnight = false;

                string regimentType = "";

                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line == "\t\t\tattacker={")
                    {
                        isAttacker = true; isDefender = false;
                    }

                    else if (line == "\t\t\tdefender={")
                    {
                        isDefender = true; isAttacker = false;

                    }

                    if (isAttacker)
                    {
                        if (line.Contains("\t\t\t\tsurviving_soldiers="))
                        {
                            string edited_line = "\t\t\t\tsurviving_soldiers=" + GetArmiesTotalFightingMen(attacker_armies);
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (line.Contains("\t\t\t\t\t\ttype="))
                        {
                            isMAA = true;
                            regimentType = Regex.Match(line, "\"(.+)\"").Groups[1].Value;
                        }
                        else if (line.Contains("\t\t\t\t\t\tknight="))
                        {
                            //4294967295
                            string id = Regex.Match(line, @"\d+").Value;
                            if (id == "4294967295" && !isMAA)
                            {
                                isLevy = true;
                                regimentType = "Levy";
                            }    
                            else if(id == "4294967295" && isMAA)
                            {
                                isMAA = true;
                            }
                            else
                                isKnight = true;
                        }
                        else if (!isKnight && line.Contains("\t\t\t\t\t\tmain_kills="))
                        {
                            int main_kills = 0;
                            foreach(Army army in attacker_armies)
                            {
                                main_kills += army.UnitsResults.GetKillsAmountOfMainPhase(regimentType);
                                if(army.MergedArmies != null)
                                {
                                    foreach(Army mergedArmy in army.MergedArmies)
                                    {
                                        main_kills += mergedArmy.UnitsResults.GetKillsAmountOfMainPhase(regimentType);
                                    }
                                }
                            }
                            string edited_line = "\t\t\t\t\t\tmain_kills=" + main_kills;
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (!isKnight && line.Contains("\t\t\t\t\t\tpursuit_kills="))
                        {
                            int pursuit_kills = 0;
                            foreach (Army army in attacker_armies)
                            {
                                pursuit_kills += army.UnitsResults.GetKillsAmountOfPursuitPhase(regimentType);
                                if (army.MergedArmies != null)
                                {
                                    foreach (Army mergedArmy in army.MergedArmies)
                                    {
                                        pursuit_kills += mergedArmy.UnitsResults.GetKillsAmountOfPursuitPhase(regimentType);
                                    }
                                }
                            }
                            string edited_line = "\t\t\t\t\t\tpursuit_kills=" + pursuit_kills;
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (!isKnight && line.Contains("\t\t\t\t\t\tmain_losses="))
                        {
                            int main_losses = 0;
                            foreach (Army army in attacker_armies)
                            {
                                main_losses += army.UnitsResults.GetDeathAmountOfMainPhase(army.CasualitiesReports,regimentType);
                                if (army.MergedArmies != null)
                                {
                                    foreach (Army mergedArmy in army.MergedArmies)
                                    {
                                        main_losses += mergedArmy.UnitsResults.GetDeathAmountOfMainPhase(mergedArmy.CasualitiesReports, regimentType);
                                    }
                                }
                            }
                            string edited_line = "\t\t\t\t\t\tmain_losses=" + main_losses;
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (!isKnight && line.Contains("\t\t\t\t\t\tpursuit_losses_maa="))
                        {
                            int pursuit_losses = 0;
                            foreach (Army army in attacker_armies)
                            {
                                pursuit_losses += army.UnitsResults.GetDeathAmountOfPursuitPhase(army.CasualitiesReports, regimentType);
                                if (army.MergedArmies != null)
                                {
                                    foreach (Army mergedArmy in army.MergedArmies)
                                    {
                                        pursuit_losses += mergedArmy.UnitsResults.GetDeathAmountOfPursuitPhase(mergedArmy.CasualitiesReports, regimentType);
                                    }
                                }
                            }
                            string edited_line = "\t\t\t\t\t\tpursuit_losses_maa=" + pursuit_losses;
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (!isKnight && line.Contains("\t\t\t\t\t\tpursuit_losses_self="))
                        {
                            streamWriter.WriteLine(line);
                            continue;
                        }
                        else if (!isKnight && line.Contains("\t\t\t\t\t\tfinal_count="))
                        {
                            streamWriter.WriteLine(line);
                            continue;
                        }
                        else if(line == "\t\t\t\t\t}")
                        {
                            isLevy = false;
                            isKnight = false;
                            isMAA = false;
                        }
                    }
                    else if (isDefender)
                    {
                        if (line.Contains("\t\t\t\tsurviving_soldiers="))
                        {
                            string edited_line = "\t\t\t\tsurviving_soldiers=" + GetArmiesTotalFightingMen(defender_armies);
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (line.Contains("\t\t\t\t\t\ttype="))
                        {
                            isMAA = true;
                            regimentType = Regex.Match(line, "\"(.+)\"").Groups[1].Value;
                        }
                        else if (line.Contains("\t\t\t\t\t\tknight="))
                        {
                            //4294967295
                            string id = Regex.Match(line, @"\d+").Value;
                            if (id == "4294967295" && !isMAA)
                            {
                                isLevy = true;
                                regimentType = "Levy";
                            }
                            else
                                isKnight = true;
                        }
                        else if (!isKnight && line.Contains("\t\t\t\t\t\tmain_kills="))
                        {
                            int main_kills = 0;
                            foreach (Army army in defender_armies)
                            {
                                main_kills += army.UnitsResults.GetKillsAmountOfMainPhase(regimentType);
                                if (army.MergedArmies != null)
                                {
                                    foreach (Army mergedArmy in army.MergedArmies)
                                    {
                                        main_kills += mergedArmy.UnitsResults.GetKillsAmountOfMainPhase(regimentType);
                                    }
                                }
                            }
                            string edited_line = "\t\t\t\t\t\tmain_kills=" + main_kills;
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (!isKnight && line.Contains("\t\t\t\t\t\tpursuit_kills="))
                        {
                            int pursuit_kills = 0;
                            foreach (Army army in defender_armies)
                            {
                                pursuit_kills += army.UnitsResults.GetKillsAmountOfPursuitPhase(regimentType);
                                if (army.MergedArmies != null)
                                {
                                    foreach (Army mergedArmy in army.MergedArmies)
                                    {
                                        pursuit_kills += mergedArmy.UnitsResults.GetKillsAmountOfPursuitPhase(regimentType);
                                    }
                                }
                            }
                            string edited_line = "\t\t\t\t\t\tpursuit_kills=" + pursuit_kills;
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (!isKnight && line.Contains("\t\t\t\t\t\tmain_losses="))
                        {
                            int main_losses = 0;
                            foreach (Army army in defender_armies)
                            {
                                main_losses += army.UnitsResults.GetDeathAmountOfMainPhase(army.CasualitiesReports, regimentType);
                                if (army.MergedArmies != null)
                                {
                                    foreach (Army mergedArmy in army.MergedArmies)
                                    {
                                        main_losses += mergedArmy.UnitsResults.GetDeathAmountOfMainPhase(mergedArmy.CasualitiesReports, regimentType);
                                    }
                                }
                            }
                            string edited_line = "\t\t\t\t\t\tmain_losses=" + main_losses;
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (!isKnight && line.Contains("\t\t\t\t\t\tpursuit_losses_maa="))
                        {
                            int pursuit_losses = 0;
                            foreach (Army army in defender_armies)
                            {
                                pursuit_losses += army.UnitsResults.GetDeathAmountOfPursuitPhase(army.CasualitiesReports, regimentType);
                                if (army.MergedArmies != null)
                                {
                                    foreach (Army mergedArmy in army.MergedArmies)
                                    {
                                        pursuit_losses += mergedArmy.UnitsResults.GetDeathAmountOfPursuitPhase(mergedArmy.CasualitiesReports, regimentType);
                                    }
                                }
                            }
                            string edited_line = "\t\t\t\t\t\tpursuit_losses_maa=" + pursuit_losses;
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (!isKnight && line.Contains("\t\t\t\t\t\tpursuit_losses_self="))
                        {
                            streamWriter.WriteLine(line);
                            continue;
                        }
                        else if (!isKnight && line.Contains("\t\t\t\t\t\tfinal_count="))
                        {
                            streamWriter.WriteLine(line);
                            continue;
                        }
                        else if (line == "\t\t\t\t\t}")
                        {
                            isLevy = false;
                            isKnight = false;
                            isMAA = false;
                            regimentType = "";
                        }
                    }

                    streamWriter.WriteLine(line);
                }
            }
        }

   
        public static void EditCombatFile(List<Army> attacker_armies,List<Army> defender_armies,string player_armies_combat_side, string enemy_armies_combat_side, string path_log_attila)
        {
            string winner = GetAttilaWinner(path_log_attila, player_armies_combat_side, enemy_armies_combat_side);
            SetWinner(winner);

            using (StreamReader streamReader = new StreamReader(DataFilesPaths.Combats_Path()))
            using (StreamWriter streamWriter = new StreamWriter(DataTEMPFilesPaths.Combats_Path()))
            {
                streamWriter.NewLine = "\n";

                bool isAttacker = false;
                bool isDefender = false;
                string army_regiment_id = "";

                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if(line == "\t\t\tattacker={")
                    {
                        isAttacker = true; isDefender = false;
                    }

                    else if (line == "\t\t\tdefender={")
                    {
                        isDefender = true; isAttacker = false;

                    }
                    
                    if(isAttacker)
                    {
                        if (line.Contains("\t\t\t\t\t\tregiment="))
                        {
                            army_regiment_id = Regex.Match(line, @"\d+").Value;
                        }
                        else if (line.Contains("\t\t\t\t\t\tcurrent="))
                        {
                            string edited_line = "\t\t\t\t\t\tcurrent=" + SearchArmyRegiment(attacker_armies, army_regiment_id)?.CurrentNum ?? "0";
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (line.Contains("\t\t\t\t\t\tsoft_casualties="))
                        {
                            string edited_line = "\t\t\t\t\t\tsoft_casualties=" + 0;
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (line.Contains("\t\t\t\ttotal_fighting_men="))
                        {
                            string edited_line = "\t\t\t\ttotal_fighting_men=" + GetArmiesTotalFightingMen(attacker_armies);
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (line.Contains("\t\t\t\total_levy_men="))
                        {
                            string edited_line = "\t\t\t\ttotal_levy_men" + GetArmiesTotalLevyMen(attacker_armies);
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }

                    }
                    else if (isDefender)
                    {
                        if (line.Contains("\t\t\t\t\t\tregiment="))
                        {
                            army_regiment_id = Regex.Match(line, @"\d+").Value;
                        }
                        else if (line.Contains("\t\t\t\t\t\tcurrent="))
                        {
                            string edited_line = "\t\t\t\t\t\tcurrent=" + SearchArmyRegiment(defender_armies, army_regiment_id)?.CurrentNum ?? "0";
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (line.Contains("\t\t\t\t\t\tsoft_casualties="))
                        {
                            string edited_line = "\t\t\t\t\t\tsoft_casualties=" + 0;
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (line.Contains("\t\t\t\ttotal_fighting_men="))
                        {
                            string edited_line = "\t\t\t\ttotal_fighting_men=" + GetArmiesTotalFightingMen(defender_armies);
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                        else if (line.Contains("\t\t\t\total_levy_men="))
                        {
                            string edited_line = "\t\t\t\ttotal_levy_men" + GetArmiesTotalLevyMen(defender_armies);
                            streamWriter.WriteLine(edited_line);
                            continue;
                        }
                    }
                    
                    streamWriter.WriteLine(line);
                }
            }

        }
        static int GetArmiesTotalFightingMen(List<Army> armies)
        {
            int total = 0;
            foreach(Army army in armies)
            {
                total += army.ArmyRegiments.Sum(x => x.CurrentNum);

                if(army.MergedArmies != null)
                {
                    foreach(Army mergedArmy in  army.MergedArmies)
                    {
                        total += mergedArmy.ArmyRegiments.Sum(x => x.CurrentNum);
                    }
                }
            }

            return total;
        }

        static int GetArmiesTotalLevyMen(List<Army> armies)
        {
            int total = 0;
            foreach (Army army in armies)
            {
                total += army.ArmyRegiments.Where(y => y.Type == RegimentType.Levy).Sum(x => x.CurrentNum);

                if (army.MergedArmies != null)
                {
                    foreach (Army mergedArmy in army.MergedArmies)
                    {
                        total += mergedArmy.ArmyRegiments.Where(y => y.Type == RegimentType.Levy).Sum(x => x.CurrentNum);
                    }
                }
            }

            return total;
        }

        static ArmyRegiment SearchArmyRegiment(List<Army> armies, string army_regiment_id)
        {
            foreach (Army army in armies)
            {
                foreach (ArmyRegiment armyRegiment in army.ArmyRegiments)
                {
                    if (armyRegiment.Type == RegimentType.Knight) continue;
                    if (armyRegiment.ID == army_regiment_id)
                    {
                        return armyRegiment;
                    }
                }

                if(army.MergedArmies != null)
                {
                    foreach (Army merged_army in army.MergedArmies)
                    {
                        foreach (ArmyRegiment armyRegiment in merged_army.ArmyRegiments)
                        {
                            if (armyRegiment.Type == RegimentType.Knight) continue;
                            if (armyRegiment.ID == army_regiment_id)
                            {
                                return armyRegiment;
                            }
                        }
                    }
                }
            }

            return null;
        }

        static void SetWinner(string winner)
        {
            try
            {
                //Set pursuit phase
                Player_Combat = Regex.Replace(Player_Combat, @"(phase=)\w+", "$1" + "pursuit");

                //Set last day of phase
                Player_Combat = Regex.Replace(Player_Combat, @"(days=\d+)", "days=3\n\t\t\twiped=no");

                //Set winner
                Player_Combat = Regex.Replace(Player_Combat, @"(base_combat_width=\d+)", "$1\n\t\t\twinning_side=" + winner);

                Player_Combat = Player_Combat.Replace("\r", "");

                File.WriteAllText(DataFilesPaths.Combats_Path(), Player_Combat);

                Console.WriteLine("Winner of battle set sucessfully");
            }
            catch
            {
                Console.WriteLine("Error setting winner of battle!");
            }

        }

        //Get winner from Attila
        static string GetAttilaWinner(string path_log_attila, string player_armies_combat_side, string enemy_armies_combat_side)
        {
            string winner = "";
            using (FileStream logFile = File.Open(path_log_attila, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(logFile))
            {
                string line;
                //winning_side=attacker/defender
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("Victory")) { winner = player_armies_combat_side; break; }
                    else if (line.Contains("Defeat")) { winner = enemy_armies_combat_side; break; }
                    else winner = enemy_armies_combat_side;
                }

                reader.Close();
                logFile.Close();

                return winner;
            }
        }

        public static void EditArmyRegimentsFile(List<Army> attacker_armies, List<Army> defender_armies)
        {
            bool editStarted = false;
            ArmyRegiment editArmyRegiment = null;

            using (StreamReader streamReader = new StreamReader(DataFilesPaths.ArmyRegiments_Path()))
            using (StreamWriter streamWriter = new StreamWriter(DataTEMPFilesPaths.ArmyRegiments_Path()))
            {
                streamWriter.NewLine = "\n";

                string line;
                while ((line = streamReader.ReadLine()) != null || !streamReader.EndOfStream)
                {

                    //Regiment ID line
                    if (!editStarted && Regex.IsMatch(line, @"\t\t\d+={"))
                    {
                        string army_regiment_id = Regex.Match(line, @"\d+").Value;


                        var searchingData = SearchArmyRegimentsFile(attacker_armies, army_regiment_id);
                        if (searchingData.editStarted)
                        {
                            editStarted = true;
                            editArmyRegiment = searchingData.editArmyRegiment;
                        }
                        else
                        {
                            searchingData = SearchArmyRegimentsFile(defender_armies, army_regiment_id);
                            if (searchingData.editStarted)
                            {
                                editStarted = true;
                                editArmyRegiment = searchingData.editArmyRegiment;
                            }
                        }

                    }

                    else if (editStarted == true && line.Contains("\t\t\t\tcurrent="))
                    {
                        string edited_line = "\t\t\t\tcurrent=" + editArmyRegiment.CurrentNum;
                        streamWriter.WriteLine(edited_line);
                        continue;
                    }

                    //End Line
                    else if (editStarted && line == "\t\t}")
                    {
                        editStarted = false; editArmyRegiment = null;
                    }

                    streamWriter.WriteLine(line);
                }
            }
        }

        static (bool editStarted, ArmyRegiment editArmyRegiment) SearchArmyRegimentsFile(List<Army> armies, string army_regiment_id)
        {
            bool editStarted = false;
            ArmyRegiment editRegiment = null;

            foreach (Army army in armies)
            {
                foreach (ArmyRegiment army_regiment in army.ArmyRegiments)
                {
                    if (army_regiment.Type == RegimentType.Knight) continue;
                    if (army_regiment.ID == army_regiment_id)
                    {
                        editStarted = true;
                        editRegiment = army_regiment;
                        return (editStarted, editRegiment);
                    }
                }

                if (army.MergedArmies != null)
                {
                    foreach (Army merged_army in army.MergedArmies)
                    {
                        foreach (ArmyRegiment army_regiment in army.ArmyRegiments)
                        {
                            if (army_regiment.Type == RegimentType.Knight) continue;
                            if (army_regiment.ID == army_regiment_id)
                            {
                                editStarted = true;
                                editRegiment = army_regiment;
                                return (editStarted, editRegiment);
                            }
                        }
                    }
                }
            }

            return (false, null);
        }

        public static void EditRegimentsFile(List<Army> attacker_armies, List<Army> defender_armies)
        {
            bool editStarted = false;
            bool editIndex = false;
            Regiment editRegiment = null;

            int index = -1;
            

            using (StreamReader streamReader = new StreamReader(DataFilesPaths.Regiments_Path()))
            using (StreamWriter streamWriter = new StreamWriter(DataTEMPFilesPaths.Regiments_Path()))
            { 
                streamWriter.NewLine = "\n";

                string line;
                while((line = streamReader.ReadLine()) != null || !streamReader.EndOfStream)
                {

                    //Regiment ID line
                    if(!editStarted && Regex.IsMatch(line, @"\t\t\d+={"))
                    {
                        string regiment_id = Regex.Match(line, @"\d+").Value;


                        var searchingData = SearchRegimentsFile(attacker_armies,regiment_id);
                        if(searchingData.editStarted)
                        {
                            editStarted = true;
                            editRegiment = searchingData.editRegiment;
                        }
                        else
                        {
                            searchingData = SearchRegimentsFile(defender_armies,regiment_id);
                            if(searchingData.editStarted)
                            {
                                editStarted = true;
                                editRegiment = searchingData.editRegiment;
                            }
                        }

                    }

                    //Index Counter
                    else if(editStarted && line == "\t\t\t\t{")
                    {
                        index++;
                        if (editRegiment.Index == "") 
                            editRegiment.ChangeIndex(0.ToString());
                        if(index.ToString() == editRegiment.Index)
                        {
                            editIndex = true;
                        }
                    }

                    else if((editStarted==true && editIndex==true) && line.Contains("\t\t\t\t\tcurrent="))
                    {
                        string edited_line = "\t\t\t\t\tcurrent="+editRegiment.CurrentNum;
                        streamWriter.WriteLine(edited_line);
                        continue;
                    }

                    //End Line
                    else if(editStarted && line == "\t\t}")
                    {
                        editStarted = false; editRegiment = null; editIndex = false; index = -1;
                    }

                    streamWriter.WriteLine(line);
                }
            }
        }

        static (bool editStarted, Regiment editRegiment) SearchRegimentsFile(List<Army> armies, string regiment_id)
        {
            bool editStarted = false;
            Regiment editRegiment = null;

            foreach (Army army in armies)
            {
                foreach (ArmyRegiment army_regiment in army.ArmyRegiments)
                {
                    foreach (Regiment regiment in army_regiment.Regiments)
                    {
                        if (regiment.ID == regiment_id)
                        {
                            editStarted = true;
                            editRegiment = regiment;
                            return (editStarted, editRegiment);
                        }
                    }
                }

                if (army.MergedArmies != null)
                {
                    foreach (Army merged_army in army.MergedArmies)
                    {
                        foreach (ArmyRegiment army_regiment in merged_army.ArmyRegiments)
                        {
                            foreach (Regiment regiment in army_regiment.Regiments)
                            {
                                if (regiment.ID == regiment_id)
                                {
                                    editStarted = true;
                                    editRegiment = regiment;
                                    return (editStarted, editRegiment);
                                }
                            }
                        }
                    }
                }
            }

            return (false, null);
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
