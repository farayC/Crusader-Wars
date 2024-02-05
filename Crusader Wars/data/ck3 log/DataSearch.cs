using Crusader_Wars.armies;
using Crusader_Wars.client;
using Crusader_Wars.locs;
using Crusader_Wars.terrain;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Crusader_Wars
{
    public static class DataSearch
    {

        static string LogPath = Properties.Settings.Default.VAR_log_ck3;

        public static void SearchLanguage()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string languages_file_path = documentsPath + "\\Paradox Interactive\\Crusader Kings III\\pdx_settings.txt";

            using (FileStream language_file = File.Open(languages_file_path, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(language_file))
                {
                    string text = reader.ReadToEnd();
                    
                    if(text.Contains("l_english"))
                    {
                        Languages.SetLanguage("l_english");
                        return;
                    }
                    else if(text.Contains("l_spanish"))
                    {
                        Languages.SetLanguage("l_spanish");
                        return;
                    }
                    else if (text.Contains("l_french"))
                    {
                        Languages.SetLanguage("l_french");
                        return;
                    }
                    else if (text.Contains("l_german"))
                    {
                        Languages.SetLanguage("l_german");
                        return;
                    }
                    else if (text.Contains("l_russian"))
                    {
                        Languages.SetLanguage("l_russian");
                        return;
                    }
                    else if (text.Contains("l_korean"))
                    {
                        Languages.SetLanguage("l_korean");
                        return;
                    }
                    else if (text.Contains("l_simp_chinese"))
                    {
                        Languages.SetLanguage("l_simp_chinese");
                        return;
                    }
                }
            }

        }



        public static void Search(string log, Player Player, Enemy Enemy)
        {
            /*---------------------------------------------
             * ::::::::::::::::Army Ratio::::::::::::::::::
             ---------------------------------------------*/

            //Get Army Ratio in log file...
            ArmyProportions.SetRatio(ModOptions.GetBattleScale());

            /*---------------------------------------------
             * :::::::::::::::::::Geral Data:::::::::::::::
             ---------------------------------------------*/

             DateSearch(log);
            BattleNameSearch(log);


            /*---------------------------------------------
             * ::::::::::::Commanders ID's:::::::::::::::::
             ---------------------------------------------*/

            //Search player ID
            string player_id = Regex.Match(log, @"PlayerID:(\d+)").Groups[1].Value;
            Player.ID = Int32.Parse(player_id);

            //Search enemy ID
            string enemy_id = Regex.Match(log, @"EnemyID:(\d+)").Groups[1].Value;

            //No Commander Crash Fix 
            int commander_id;
            if (int.TryParse(enemy_id, out commander_id))
                Enemy.ID = commander_id;
            else
                Enemy.ID = 0;

           /*---------------------------------------------
             * :::::::::::::::Unit Mapper::::::::::::::::::
             ---------------------------------------------*/

            UnitMapper.LoadMapper();

            /*---------------------------------------------
             * :::::::::::::::::Terrain::::::::::::::::::::
             ---------------------------------------------*/

            TerrainSearch(log);

            UniqueMapsSearch(log);

            /*---------------------------------------------
             * ::::::::::::::Player Army:::::::::::::::::::
             ---------------------------------------------*/

            string PlayerArmy = Regex.Match(log, @"---------Player Army---------([\s\S]*?)---------Enemy Army---------").Groups[1].Value;


            /*---------------------------------------------
             * :::::::::Player Culture & Heritage::::::::::
             ---------------------------------------------*/

             CultureAndHeritageSearch(PlayerArmy, Player);

            /*---------------------------------------------
             * ::::::::::Player Commander System:::::::::::
             ---------------------------------------------*/

            CommanderSearch(PlayerArmy, Player);

            /*---------------------------------------------
             * :::::::::::Player Knight System:::::::::::::
             ---------------------------------------------*/

            KnightsSearch(PlayerArmy, Player);

            /*---------------------------------------------
             * ::::::::::::Player Composition::::::::::::::
             ---------------------------------------------*/

             UnitMapper.LoadCultureMAA(Player);
             ArmyCompositionSearch(PlayerArmy, Player);

            /*---------------------------------------------
             * :::::::::::::Player Modifiers:::::::::::::::
             ---------------------------------------------*/
             
            ArmyModifiersSearch(log, Player);

            /*---------------------------------------------
             * ::::::::::::::::Enemy Army::::::::::::::::::
             ---------------------------------------------*/

            string EnemyArmy = Regex.Match(log, @"---------Enemy Army---------([\s\S]*?)---------Completed---------").Groups[1].Value;

            /*---------------------------------------------
             * :::::::::Enemy Culture & Heritage::::::::::
             ---------------------------------------------*/

             CultureAndHeritageSearch(EnemyArmy, Enemy);

            /*---------------------------------------------
             * ::::::::::Enemy Commander System:::::::::::
             ---------------------------------------------*/

            CommanderSearch(EnemyArmy, Enemy);

            /*---------------------------------------------
             * :::::::::::Enemy Knight System:::::::::::::
             ---------------------------------------------*/

            KnightsSearch(EnemyArmy, Enemy);

            /*---------------------------------------------
             * ::::::::::::Enemy Composition::::::::::::::
             ---------------------------------------------*/

            UnitMapper.LoadCultureMAA(Enemy);
             ArmyCompositionSearch(EnemyArmy, Enemy);

            /*---------------------------------------------
             * :::::::::::::Enemy Modifiers:::::::::::::::
             ---------------------------------------------*/

             ArmyModifiersSearch(log, Enemy);

            /*---------------------------------------------
             * ::::::::::::::::Army Names::::::::::::::::::
             ---------------------------------------------*/

            RealmsNamesSearch(log, Player, Enemy);
            CharNamesSearch(log, Player, Enemy);
            TitlesRanksSearch(log, Player, Enemy);

        }

        private static void BattleNameSearch(string log)
        {
            string battle_name = Regex.Match(log, "BattleName:(?<BattleName>.+)\n").Groups["BattleName"].Value;
            BattleDetails.SetBattleName(battle_name);
        }

        private static void DateSearch(string log)
        {
            string month;
            string year;
            try
            {
                month = Regex.Match(log, Languages.SearchPatterns.date).Groups["Month"].Value;
                year = Regex.Match(log, Languages.SearchPatterns.date).Groups["Year"].Value;
                Date.Month = month;
                Date.Year = Int32.Parse(year);

                string season = Date.GetSeason();
                Weather.SetSeason(season);
            }
            catch
            {
                month = "January";
                year = "1300";
                Date.Month = month;
                Date.Year = Int32.Parse(year);
                Weather.SetSeason("random");
            }

        }

        private static void TerrainSearch(string log)
        {
            TerrainGenerator.TerrainType = SearchForTerrain(log);
            Weather.SetWinterSeverity(SearchForWinter(log));
        }
        private static void CommanderSearch(string army_data, ICharacter Side)
        {

            Side.Commander = new CommanderSystem();
            Side.Commander.SetID(Side.ID.ToString());

            Match martial_match = Regex.Match(army_data, Languages.SearchPatterns.martial_skill);
              if (martial_match.Success)
            {
                string martial_str = martial_match.Groups["Martial"].Value;
                int martial = Int32.Parse(martial_str);
                Side.Commander.SetMartial(martial);
            }
            else
            {
                Side.Commander.SetMartial(0);
            }


            string pattern = @"";
            if(Side is Player) { pattern = @"PlayerProwess:(?<Num>\d+)"; }
            else if (Side is Enemy) { pattern = @"EnemyProwess:(?<Num>\d+)"; }

            Match prowess_match = Regex.Match(army_data, pattern);
            if (prowess_match.Success)
            {
                string prowess_str = prowess_match.Groups["Num"].Value;
                int prowess = Int32.Parse(prowess_str);
                Side.Commander.SetProwess(prowess);
            }
            else
            {
                Side.Commander.SetProwess(0);
            }

        }
        private static void ArmyCompositionSearch(string army_data, ICharacter side)
        {
            string ArmyComposition = Regex.Match(army_data, Languages.SearchPatterns.army_composition).Groups["ArmyComposition"].Value;

            //Search player total soldiers
            int TotalNumber = Int32.Parse(Regex.Match(army_data, Languages.SearchPatterns.total_soldiers).Groups["SoldiersNum"].Value);
            side.TotalNumber = TotalNumber;

            //Search army composition of player side

            List<(string Type, int Number)> FoundMAA = new List<(string Type, int Number)>();

            //Levies
            MatchCollection levies_found = Regex.Matches(ArmyComposition, ModOptions.FullArmiesLevies(ArmyComposition));
            int levies = 0;
            foreach (Match match in levies_found)
            {
                if (match.Success)
                {
                    string num_of_soldiers = match.Groups["SoldiersNum"].Value;
                    levies += Int32.Parse(num_of_soldiers);
                }
            }
            if (levies > 0)
            {
                levies = ArmyProportions.SetSoldiersRatio(levies);
                FoundMAA.Add(("Levy", levies));
            }



            MatchCollection maa_matches = Regex.Matches(ArmyComposition, ModOptions.FullArmiesMAA());

            foreach (Match maa in maa_matches)
            {
                try
                {
                    string type = maa.Groups["MenAtArms"].Value;
                    int soldiers = Int32.Parse(maa.Groups["SoldiersNum"].Value);

                    bool containsSame = FoundMAA.Any(item => item.Type == type);

                    //Repetead maa
                    //Increase soldiers num
                    if(containsSame)
                    {
                        var same_maa = FoundMAA.FirstOrDefault(x => x.Type == type);
                        int i = FoundMAA.IndexOf(same_maa);

                        soldiers = ArmyProportions.SetSoldiersRatio(soldiers);
                        same_maa = (same_maa.Type, same_maa.Number + soldiers);
                        FoundMAA[i] = same_maa;
                        continue;
                    }

                    //Add to found maa
                    if (soldiers > 0)
                    {
                        soldiers = ArmyProportions.SetSoldiersRatio(soldiers);
                        FoundMAA.Add((type, soldiers));
                    }
                }
                catch
                {
                    continue;
                }
            }

            //Populate the side object with the army composition
            side.Army = new List<(string Type, string Key, int Max, string Script, int Soldiers)>();

            if (side is Player)
            {
                //General
                //if army has a commander
                if(side.ID != 0)
                {
                    var general_unit = UnitMapper.PlayerUnits.FirstOrDefault(item => item.Type == "General");
                    side.Army.Add((general_unit.Type, general_unit.Key,general_unit.Max, $"player_{general_unit.Script}", 50));
                }

                //Knights
                //if army has knights
                /*
                if (side.Knights.GetKnightsSoldiers() > 0)
                {

                }*/
                var knights_unit = UnitMapper.PlayerUnits.FirstOrDefault(item => item.Type == "Knights");
                side.Army.Add((knights_unit.Type, knights_unit.Key, side.Knights.GetKnightsSoldiers(), $"player_{knights_unit.Script}", side.Knights.GetKnightsSoldiers()));

                foreach (var unit in UnitMapper.PlayerUnits)
                {
                    for (int i = 0; i < FoundMAA.Count; i++)
                    {
                        string found_type = FoundMAA[i].Type;
                        int found_soldiers = FoundMAA[i].Number;
                        if (unit.Type == found_type || (unit.Type.Contains("Levy") && FoundMAA[0].Type == "Levy"))
                        {
                            side.Army.Add((unit.Type, unit.Key, unit.Max, $"player_{unit.Script}", found_soldiers));
                            break;
                        }
                    }
                }
            }
            if (side is Enemy)
            {
                //General
                //if army has a commander
                if (side.ID != 0)
                {
                    var general_unit = UnitMapper.EnemyUnits.FirstOrDefault(item => item.Type == "General");
                    side.Army.Add((general_unit.Type, general_unit.Key, general_unit.Max, $"enemy_{general_unit.Script}", 50));
                }

                //Knights
                var knights_unit = UnitMapper.EnemyUnits.FirstOrDefault(item => item.Type == "Knights");
                side.Army.Add((knights_unit.Type, knights_unit.Key, side.Knights.GetKnightsSoldiers(), $"enemy_{knights_unit.Script}", side.Knights.GetKnightsSoldiers()));

                foreach (var unit in UnitMapper.EnemyUnits)
                {
                    for (int i = 0; i < FoundMAA.Count; i++)
                    {
                        string found_type = FoundMAA[i].Type;
                        int found_soldiers = FoundMAA[i].Number;
                        if (unit.Type == found_type || (unit.Type.Contains("Levy") && FoundMAA[0].Type == "Levy"))
                        {
                            side.Army.Add((unit.Type, unit.Key, unit.Max, $"enemy_{unit.Script}", found_soldiers));
                            break;
                        }
                    }
                }
            }
        }
        static void RealmsNamesSearch(string log, Player Player, Enemy Enemy)
        {
            string text = Regex.Match(log, "(Log[\\s\\S]*?)---------Player Army---------[\\s\\S]*?").Groups[1].Value;
            MatchCollection found_armies = Regex.Matches(text, "L (.+)");
            if(found_armies.Count >= 1)
            {
                string player_army = found_armies[0].Groups[1].Value;
                string enemy_army = found_armies[1].Groups[1].Value;

                Player.RealmName =  player_army;
                Enemy.RealmName = enemy_army;
            }

        }

        static void TitlesRanksSearch(string log, Player Player, Enemy Enemy)
        {
            int player_rank = Int32.Parse(Regex.Match(log, @"PlayerRank:(?<Name>.+)").Groups["Name"].Value);
            int enemy_rank = Int32.Parse(Regex.Match(log, @"EnemyRank:(?<Name>.+)").Groups["Name"].Value);

            Player.Commander.SetRank(player_rank);
            Enemy.Commander.SetRank(enemy_rank);
        }

        static void CharNamesSearch(string log, Player player, Enemy enemy)
        {

            string player_name = Regex.Match(log, @"PlayerName:(?<Name>.+)").Groups["Name"].Value;
            string enemy_name = Regex.Match(log, @"EnemyName:(?<Name>.+)").Groups["Name"].Value;

            player.Commander.SetName(player_name);
            enemy.Commander.SetName(enemy_name);
        }

        private static void KnightsSearch(string army_data, ICharacter side)
        {
            string Knights = Regex.Match(army_data, @"(?<Knights>ONCLICK:CHARACTER[\s\S]*?)\z[\s\S]*?").Groups["Knights"].Value;
            

            side.Knights = new KnightSystem();

            //Search Knights
            List<(string, int, int, List<string>,BaseSkills, bool)> KnightsList = new List<(string, int, int, List<string>, BaseSkills,bool)>();
            MatchCollection knights_collection = Regex.Matches(Knights, "(?<Prowess>\\d+) E TOOLTIP");
            MatchCollection knights_id_collection = Regex.Matches(Knights, "CHARACTER(?<ID>\\d+) TOOLTIP");


            for (int i = 0; i < knights_collection.Count; i++)
            {
                string id = knights_id_collection[i].Groups["ID"].Value;
                if (id == side.ID.ToString()) continue; // if commander is on the list knight, skip it.
                int knight_prowess = Int32.Parse(knights_collection[i].Groups["Prowess"].Value);
                KnightsList.Add((id,3,knight_prowess, new List<string>(), null,false));
            }

            MatchCollection knight_effectiveness = Regex.Matches(Knights, @"(?<Effectiveness>\d+)%");
            int effectiveness = 0;
            foreach (Match effect in knight_effectiveness)
            {
                int value = Int32.Parse(effect.Groups["Effectiveness"].Value);
                effectiveness += value;
            }


            

            side.Knights.SetData(KnightsList, effectiveness);

        }

        //No commander bug error is here!!
        private static void CultureAndHeritageSearch(string side_army, ICharacter side)
        {
            try
            {
                string CulturesText = Regex.Match(side_army, Languages.SearchPatterns.cultures).Groups["CulturesText"].Value;

                MatchCollection FoundCultureHeritage = Regex.Matches(CulturesText, "L (?<Match>.+)");
                string Heritage = FoundCultureHeritage[0].Groups["Match"].Value;
                string Culture = FoundCultureHeritage[1].Groups["Match"].Value;

                string AttilaFaction;
                for (int i = 0; i < UnitMapper.Heritages.Count; i++)
                {
                    if (Heritage == UnitMapper.Heritages[i].Heritage)
                    {
                        AttilaFaction = UnitMapper.Heritages[i].Faction;
                        side.AttilaFaction = AttilaFaction;
                        break;
                    }
                }

                for (int i = 0; i < UnitMapper.Cultures.Count; i++)
                {
                    if (Culture == UnitMapper.Cultures[i].Cultures)
                    {
                        AttilaFaction = UnitMapper.Cultures[i].Faction;
                        side.AttilaFaction = AttilaFaction;
                        break;
                    }
                }
            }
            catch 
            {
                MessageBox.Show("The enemy side doesn't have a commander, unfortunaly this battle is impossible to fight....", "Impossible Battle Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                throw;
            }
 

        }

        private static void ArmyModifiersSearch(string log, ICharacter Side)
        {
            Modifiers army_modifiers = new Modifiers();
            army_modifiers.ReadModifiers(log, Side);
            Side.Modifiers = army_modifiers;
        }

        private static void UniqueMapsSearch(string log)
        {


            Match match = Regex.Match(log, @"SpecialBuilding:(.+)");
            if(match.Success)
            {
                string building_key = match.Groups[1].Value;
                UniqueMaps.ReadSpecialBuilding(building_key);
            }
        }


        private static string SearchForTerrain(string content)
        {
            string terrain_data = Regex.Match(content, "---------Completed---------([\\s\\S]*?)PlayerID").Groups[1].Value;
            
            string region_data = Regex.Match(terrain_data, @"Region:(.+)").Groups[1].Value;
            TerrainGenerator.SetRegion(region_data);

            string terrain = Regex.Match(terrain_data, "L (?<Terrain>.+)").Groups["Terrain"].Value;

            //Check if exists
            for (int i = 0; i < Languages.Terrain.AllTerrains.Length; i++)
            {
                if (terrain == Languages.Terrain.AllTerrains[i])
                {
                    return terrain;
                }
            }

            return terrain;

        }

        public static void ClearLogFile()
        {

            using (var fileStream = new FileStream(LogPath, FileMode.Truncate))
            {
                // Truncate the file, effectively clearing its contents
                fileStream.Close();
            }


        }

        private static string SearchForWinter(string content)
        {
            string terrain_data = Regex.Match(content, "---------Completed---------([\\s\\S]*?)PlayerID").Groups[1].Value;

            for (int i = 0; i < Languages.Terrain.AllWinter.Length; i++)
            {
                Match hasFound = Regex.Match(terrain_data, $"{Languages.Terrain.AllWinter[i]}");

                if (hasFound.Success)
                {
                    string winter = hasFound.Value;
                    return winter;
                }
            }

            return string.Empty;

        }


       
    }
}
