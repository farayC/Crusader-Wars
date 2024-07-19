using Crusader_Wars.twbattle;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;

namespace Crusader_Wars
{
    /*
     * IMPORTANT NOTE
     * ----------------------------
     * The writter gives some extra new lines '\n'
     * might remove them later
     */
    internal static class Reader
    {

        static void ClearFilesData()
        {
            //Clear Battle Results File
            File.WriteAllText(@".\data\save_file_data\BattleResults.txt", "");
            //Clear Battle Results TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\BattleResults.txt", "");

            //Clear Combats File
            File.WriteAllText(@".\data\save_file_data\Combats.txt", "");
            //Clear Combats TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\Combats.txt", "");

            //Clear Regiments File
            File.WriteAllText(@".\data\save_file_data\Regiments.txt", "");
            //Clear Regiments TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\Regiments.txt", "");

            //Clear Army Regiments File
            File.WriteAllText(@".\data\save_file_data\ArmyRegiments.txt", "");
            //Clear Army Regiments TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\ArmyRegiments.txt", "");

            //Clear Living Regiments File
            File.WriteAllText(@".\data\save_file_data\Living.txt", "");
            //Clear Living Regiments TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\Living.txt", "");

            //Clear Armies File
            File.WriteAllText(@".\data\save_file_data\Armies.txt", "");
            //Clear Armies TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\Armies.txt", "");

            //Clear Counties File
            File.WriteAllText(@".\data\save_file_data\Counties.txt", "");
            //Clear Counties TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\Counties.txt", "");

            //Clear Cultures File
            File.WriteAllText(@".\data\save_file_data\Cultures.txt", "");
            //Clear Cultures TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\Cultures.txt", "");

            //Clear Mercenaries File
            File.WriteAllText(@".\data\save_file_data\Mercenaries.txt", "");
            //Clear Mercenaries TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\Mercenaries.txt", "");

            //Clear Traits File
            File.WriteAllText(@".\data\save_file_data\Traits.txt", "");
            //Clear Traits TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\Traits.txt", "");
            //Clear Units File
            File.WriteAllText(@".\data\save_file_data\Units.txt", "");
            //Clear Units TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\Units.txt", "");

            //Clear Court Positions File
            File.WriteAllText(@".\data\save_file_data\CourtPositions.txt", "");
            //Clear Court Positions TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\CourtPositions.txt", "");
        }

        /// <summary>  
        /// Reads the ck3 save file for all the needed data.  
        /// </summary>  
        /// <param name="savePath">Path to the ck3 save file</param>  
        public static void ReadFile(string savePath)
        {
            //Clean all data in save file data files
            ClearFilesData();

            using (FileStream saveFile = File.Open(savePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(saveFile))
            {
                string line = reader.ReadLine();
                while (line != null && !reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    //GetterKeys.ReadProvinceBuildings(line, "5984");

                    /*TO DO:
                     * REWORK THIS LATER*/

                    //GetterKeys.ReadAccolades(line, Player, Enemy);
                    //GetterKeys.ReadCourtPositions(line, Player, Enemy);
                    //GetterKeys.ReadLivingCharacters(line, Player, Enemy);

                    SearchKeys.TraitsList(line);
                    
                    SearchKeys.BattleResults(line);
                    SearchKeys.Combats(line);
                    SearchKeys.Regiments(line);
                    SearchKeys.ArmyRegiments(line);
                    SearchKeys.Living(line);


                    SearchKeys.Armies(line);
                    SearchKeys.Counties(line);
                    SearchKeys.Cultures(line);
                    SearchKeys.Mercenaries(line);
                    SearchKeys.Units(line);
                    SearchKeys.CourtPositions(line);
                    
                }
               
                reader.Close();
                saveFile.Close();
            }

            GC.Collect();

        }
    }




    internal static class Data
    {
        public static List<(string unitName, string declarationName)> units_scripts = new List<(string unitName, string declarationName)>();


        public static List<string> PlayerIDsAccolades = new List<string>();
        public static List<string> EnemyIDsAccolades = new List<string>();
        public static List<(string,string,string)> PlayerAccolades = new List<(string,string,string)>();
        public static List<(string,string,string)> EnemysAccolades = new List<(string,string,string)>();


        public static string PlayerCommanderAccoladeID = "";
        public static string EnemyCommanderAccoladeID = "";
        public static (string, string, string) PlayerCommanderAccolade;
        public static (string, string, string) EnemyCommanderAccolade;

        public static StringBuilder SB_Living = new StringBuilder();
        public static StringBuilder SB_Regiments = new StringBuilder();
        public static StringBuilder SB_ArmyRegiments = new StringBuilder();
        public static StringBuilder SB_Armies = new StringBuilder();
        public static StringBuilder SB_CombatResults = new StringBuilder();
        public static StringBuilder SB_Combats = new StringBuilder();
        public static StringBuilder SB_Counties = new StringBuilder();
        public static StringBuilder SB_Cultures = new StringBuilder();
        public static StringBuilder SB_Mercenaries = new StringBuilder();
        public static StringBuilder SB_Units = new StringBuilder();
        public static StringBuilder SB_CourtPositions = new StringBuilder();



        //Sieges
        public static List<string> Province_Buildings = new List<string>();


        public static void Reset()
        {

            PlayerIDsAccolades = new List<string> ();
            EnemyIDsAccolades = new List<string>();
            PlayerAccolades = new List<(string, string, string)> ();
            EnemysAccolades = new List<(string, string, string)>();

            PlayerCommanderAccoladeID = "";
            EnemyCommanderAccoladeID = "";
            PlayerCommanderAccolade = ("","","");
            EnemyCommanderAccolade = ("","","");


            SearchKeys.HasTraitsExtracted = false;
            SearchKeys.HasCombatsExtracted = false;
            SearchKeys.HasLivingExtracted = false;
            SearchKeys.HasArmyRegimentsExtracted = false;
            SearchKeys.HasRegimentsExtracted = false;
            SearchKeys.HasArmiesExtracted = false;
            SearchKeys.HasBattleResultsExtracted = false;
            SearchKeys.HasCountiesExtracted = false;
            SearchKeys.HasCulturesExtracted = false;
            SearchKeys.HasMercenariesExtracted = false;
            SearchKeys.HasUnitsExtracted = false;
            SearchKeys.HasCourtPositionsExtracted = false;
        }
    }

    struct GetterKeys
    {
        static bool isSearchPermitted = false;
        static bool isSearchBuildingsPermitted = false;
        static bool isExtractBuildingsPermitted = false;
        public static void ReadProvinceBuildings(string line, string province_id)
        {
            
            if(line.Contains("provinces={"))
            {
                isSearchPermitted = true;
            }

            
            if(isSearchPermitted && line.Contains($"\t{province_id}={{"))
            {
                isSearchBuildingsPermitted = true;
            }

            if(isSearchBuildingsPermitted && line.Contains("buildings={"))
            {
                isExtractBuildingsPermitted = true;
            }

            if(isExtractBuildingsPermitted)
            {
                if (line.Contains("type="))
                {

                    string building_key = Regex.Match(line, @"=(.+)").Groups[1].Value.Trim('"').Trim('/');
                    Data.Province_Buildings.Add(building_key);
                }


            }


            //last line of the province data
            //stop searching
            if( isSearchBuildingsPermitted && line.Contains("fort_level="))
            {
                string fort_level = Regex.Match(line, @"=(.+)").Groups[1].Value.Trim('"').Trim('/');
                if(int.TryParse(fort_level, out int level))
                {
                    Sieges.SetFortLevel(level);
                }
                else
                {
                    Sieges.SetFortLevel(0);
                }
                
                isExtractBuildingsPermitted = false;
                isSearchBuildingsPermitted = false;
                isSearchPermitted = false;
                return;

            }
        }



        static bool isSearchPermittedAccolades = false;
        static bool StartAccoladeSearchAllowed = false;
        static string found_accolade_id;
        static string primary_attribute;
        static string secundary_attribute;
        static string glory;
        static bool isAccoladePlayer = false;
        static bool isAccoladeEnemy = false;
        static bool isAccoladePlayerCommander = false;
        static bool isAccoladeEnemyCommander = false;
        public static void ReadAccolades(string line, ICharacter player, ICharacter enemy)
        {
            //if there is a accolade id found
            if (Data.PlayerIDsAccolades.Count > 0 || Data.EnemyIDsAccolades.Count > 0)
            {
                if (line == "accolades={")
                {
                    isSearchPermittedAccolades = true;
                }

                //find accolade on the battle
                if (isSearchPermittedAccolades && !StartAccoladeSearchAllowed)
                {
                    foreach (var id in Data.PlayerIDsAccolades)
                    {
                        if (line == $"\t\t{id}={{")
                        {
                            isAccoladePlayer = true;
                            found_accolade_id = id;
                            StartAccoladeSearchAllowed = true;
                            break;
                        }
                    }

                    foreach (var id in Data.EnemyIDsAccolades)
                    {
                        if (line == $"\t\t{id}={{")
                        {
                            isAccoladeEnemy = true;
                            found_accolade_id = id;
                            StartAccoladeSearchAllowed = true;
                            break;
                        }
                    }

                    if (line == $"\t\t{Data.PlayerCommanderAccoladeID}={{")
                    {
                        isAccoladePlayerCommander = true;
                        found_accolade_id = Data.PlayerCommanderAccoladeID;
                        StartAccoladeSearchAllowed = true;
                    }
                    else if (line == $"\t\t{Data.EnemyCommanderAccoladeID}={{")
                    {
                        isAccoladeEnemyCommander = true;
                        found_accolade_id = Data.EnemyCommanderAccoladeID;
                        StartAccoladeSearchAllowed = true;
                    }

                }
                    //search for attributes and glory
                    if (StartAccoladeSearchAllowed && line.Contains("\t\t\tprimary="))
                    {
                        primary_attribute = Regex.Match(line, @"\w+", RegexOptions.RightToLeft).Value;
                    }
                    if (StartAccoladeSearchAllowed && line.Contains("\t\t\tsecondary="))
                    {
                        secundary_attribute = Regex.Match(line, @"\w+", RegexOptions.RightToLeft).Value;
                    }
                    if (StartAccoladeSearchAllowed && line.Contains("\t\t\tglory="))
                    {
                        glory = Regex.Match(line, @"\d+").Value;
                    }


                    //add data to list
                    if (primary_attribute != String.Empty && secundary_attribute != String.Empty && glory != String.Empty)
                    {
                        if (isAccoladePlayer)
                        {
                            Data.PlayerAccolades.Add((primary_attribute, secundary_attribute, glory));
                            Console.WriteLine($"Player Accolade: ID- {found_accolade_id}\tPrimary- {primary_attribute}\tSecundary-{secundary_attribute}\tHonor-{glory}");
                        }
                        else if(isAccoladeEnemy)
                        {
                            Data.EnemysAccolades.Add((primary_attribute, secundary_attribute, glory));
                            Console.WriteLine($"Enemy Accolade: ID- {found_accolade_id}\tPrimary- {primary_attribute}\tSecundary-{secundary_attribute}\tHonor-{glory}");
                        }
                        else if(isAccoladePlayerCommander)
                        {
                            player.Commander.SetAccolade((primary_attribute, secundary_attribute, glory));
                            Console.WriteLine($"Player Commander Accolade: ID- {found_accolade_id}\tPrimary- {primary_attribute}\tSecundary-{secundary_attribute}\tHonor-{glory}");
                        }
                        else if(isAccoladeEnemyCommander)
                        {
                            enemy.Commander.SetAccolade((primary_attribute, secundary_attribute, glory));
                            Console.WriteLine($"Enemmy Commander Accolade: ID- {found_accolade_id}\tPrimary- {primary_attribute}\tSecundary-{secundary_attribute}\tHonor-{glory}");
                        }


                        primary_attribute = "";
                        secundary_attribute = "";
                        glory = "";
                        isAccoladeEnemy = false;
                        isAccoladePlayer = false;
                        isAccoladePlayerCommander = false;
                        isAccoladeEnemyCommander = false;
                    }

                    //accolade data end line
                    if (StartAccoladeSearchAllowed && line == "\t\t}")
                    {
                        StartAccoladeSearchAllowed = false;
                        found_accolade_id = "";
                    }

                    //accolades data group end line
                    if (isSearchPermittedAccolades && line == "tax_slot_manager={")
                    {
                        //clear empty items
                        Data.PlayerAccolades.RemoveAll(t => string.IsNullOrEmpty(t.Item1) || string.IsNullOrEmpty(t.Item2) || string.IsNullOrEmpty(t.Item3));
                        Data.EnemysAccolades.RemoveAll(t => string.IsNullOrEmpty(t.Item1) || string.IsNullOrEmpty(t.Item2) || string.IsNullOrEmpty(t.Item3));

                        player.Knights.SetAccolades(Data.PlayerAccolades);
                        enemy.Knights.SetAccolades(Data.EnemysAccolades);

                        StartAccoladeSearchAllowed = false;
                        isSearchPermittedAccolades = false;


                    }

                
            }
        }

        static bool isSearchPermittedLiving = false;
        static bool StartCharacterSearchAllowed = false;
        static List<Knight> player_knights_list;
        static List<Knight> enemy_knights_list;
        static bool isPlayerCommander = false;
        static bool isEnemyCommander = false;
        static bool isPlayerKnight = false;
        static bool isEnemyKnight = false;
        static string char_id = "";
        public static void ReadLivingCharacters(string line, ICharacter Player, ICharacter Enemy)
        {


            if (line == "living={")
            {
                isSearchPermittedLiving = true;

                player_knights_list = Player.Knights.GetKnightsList();
                enemy_knights_list = Enemy.Knights.GetKnightsList();
            }

            if (isSearchPermittedLiving && !StartCharacterSearchAllowed)
            {
                if(line == ($"\t{Player.Commander.ID}={{") || line == $"{Player.Commander.ID}={{")
                {
                    StartCharacterSearchAllowed = true;
                    isPlayerCommander=true;
                    char_id = Player.Commander.ID;
                }
                else if (line == $"\t{Enemy.Commander.ID}={{" || line == $"{Enemy.Commander.ID}={{")
                {
                    StartCharacterSearchAllowed = true;
                    isEnemyCommander=true;
                    char_id = Enemy.Commander.ID;
                }
                
                if(player_knights_list != null)
                {
                    foreach (var knight in player_knights_list)
                    {
                        if (line == $"\t{knight.GetID()}={{" || line == $"{knight.GetID()}={{")
                        {
                            StartCharacterSearchAllowed = true;
                            isPlayerKnight = true;
                            char_id = knight.GetID();
                        }
                    }
                }

                if( enemy_knights_list != null )
                {
                    foreach (var knight in enemy_knights_list)
                    {
                        if (line == $"\t{knight.GetID()}={{" || line == $"{knight.GetID()}={{")
                        {
                            StartCharacterSearchAllowed = true;
                            isEnemyKnight = true;
                            char_id = knight.GetID();
                        }
                    }
                }

            }

            if(StartCharacterSearchAllowed)
            {
                if(line.Contains("\ttraits={"))
                {
                    var traits_collection = Regex.Matches(line, @"\d+").Cast<Match>()
                                                                        .Select(m => m.Value)
                                                                        .ToList<string>();
                    
                    if (isPlayerCommander)
                    {
                        Player.Commander.SetTraits(traits_collection);
                    }
                    else if(isEnemyCommander)
                    {
                        Enemy.Commander.SetTraits(traits_collection);
                    }
                    else if(isPlayerKnight)
                    {
                        //Player.Knights.SetTraits(char_id, traits_collection);
                    }
                    else if(isEnemyKnight)
                    {
                        //Enemy.Knights.SetTraits(char_id, traits_collection);
                    }

                }
                if(line.Contains("\tculture="))
                {
                    string culture = Regex.Match(line, @"\d+").Value;

                }


                if(line.Contains("\tskill={"))
                {
                    var skills_collection = Regex.Matches(line, @"\d+").Cast<Match>()
                                                .Select(m => m.Value)
                                                .ToList<string>();

                    if (isPlayerKnight)
                    {
                        BaseSkills skills = new BaseSkills(skills_collection);
                        //Player.Knights.SetSkills(char_id, skills);
                    }
                    else if (isEnemyKnight)
                    {
                        BaseSkills skills = new BaseSkills(skills_collection);
                        //Enemy.Knights.SetSkills(char_id, skills);
                    }
                }

                //set the knight as accolade if true and add id to a list
                if (line.Contains("\t\taccolade="))
                {
                   
                    Knight accolade_knight;

                    if (isPlayerKnight)
                    {
                        try
                        {
                            var player_knights = Player.Knights.GetKnightsList();
                            accolade_knight = player_knights.First(x => x.GetID() == char_id);

                            int index = player_knights.IndexOf(accolade_knight);
                            accolade_knight = new Knight(accolade_knight.GetName(), accolade_knight.GetID(), null, accolade_knight.GetProwess(), 5, true);
                            player_knights[index] = accolade_knight;

                            string accolade_id = Regex.Match(line, @"\d+").Value;
                            Data.PlayerIDsAccolades.Add(accolade_id);
                            Console.WriteLine("Player accolade found with an id of - " + char_id);
                        }
                        catch
                        {
                            Console.WriteLine("");
                        }

                    }
                    else if (isEnemyKnight)
                    {
                        try
                        {
                            var enemy_knights = Enemy.Knights.GetKnightsList();
                            accolade_knight = enemy_knights_list.First(x => x.GetID() == char_id);

                            int index = enemy_knights.IndexOf(accolade_knight);
                            accolade_knight = new Knight(accolade_knight.GetName(), accolade_knight.GetID(), null, accolade_knight.GetProwess(), 5, true);
                            enemy_knights[index] = accolade_knight;

                            string accolade_id = Regex.Match(line, @"\d+").Value;
                            Data.EnemyIDsAccolades.Add(accolade_id);
                            Console.WriteLine("Enemy accolade found with an id of - " + char_id);
                        }
                        catch 
                        {
                            Console.WriteLine("");
                        }

                    }
                    else if(isPlayerCommander) 
                    {
                        string accolade_id = Regex.Match(line, @"\d+").Value;
                        Data.PlayerCommanderAccoladeID = accolade_id;
                        Console.WriteLine("Player Accolade Commander found with an id of - "+ char_id);
                    }
                    else if (isEnemyCommander)
                    {
                        string accolade_id = Regex.Match(line, @"\d+").Value;
                        Data.EnemyCommanderAccoladeID = accolade_id;
                        Console.WriteLine("Enemy Accolade Commander found with an id of - " + char_id);
                    }

                }


            }

            //end line to specific court position data
            if (StartCharacterSearchAllowed && line == "}")
            {
                isEnemyCommander = false;
                isPlayerCommander = false;
                isEnemyKnight = false;
                isPlayerKnight = false;

                char_id = "";
                StartCharacterSearchAllowed = false;
            }

            //end line to all court positions data
            if (isSearchPermittedLiving && line == "dead_unprunable={")
            {
                player_knights_list = null;
                enemy_knights_list = null;

                isEnemyCommander = false;
                isPlayerCommander = false;
                isEnemyKnight = false;
                isPlayerKnight = false;

                char_id = "";
                StartCharacterSearchAllowed = false;
                isSearchPermittedLiving = false;

            }
        }

        static bool isSearchPermittedCourtPositions = false;
        static bool StartCourtPositionsSearchAllowed = false;
        static string employee;
        static string profession;
        public static void ReadCourtPositions(string line, ICharacter Player, ICharacter Enemy)
        {
            if (line == "court_positions={")
            {
                isSearchPermittedCourtPositions = true;
            }

            if (isSearchPermittedCourtPositions && !StartCourtPositionsSearchAllowed)
            {
                if (line == "\t\t\tcourt_position=\"bodyguard_court_position\"")
                {
                    profession = "bodyguard";
                    StartCourtPositionsSearchAllowed = true;
                }
                else if (line == "\t\t\tcourt_position=\"champion_court_position\"")
                {
                    profession = "personal_champion";
                    StartCourtPositionsSearchAllowed = true;
                }
                else if (line == "\t\t\tcourt_position=\"garuva_warrior_court_position\"")
                {
                    profession = "garuva_warrior";
                    StartCourtPositionsSearchAllowed = true;
                }

            }

            if (StartCourtPositionsSearchAllowed)
            {
                if (line.Contains("\t\t\temployee="))
                {
                    employee = Regex.Match(line, @"=(.+)").Groups[1].Value;
                }


                if (line == $"\t\t\temployer={Player.ID}")
                {
                    Player.Commander.AddCourtPosition(profession, employee);
                }

                if (line == $"\t\t\temployer={Enemy.ID}")
                {
                    Enemy.Commander.AddCourtPosition(profession, employee);
                }
            }

            //end line to specific court position data
            if (StartCourtPositionsSearchAllowed && line == "\t\t}")
            {
                employee = "";
                profession = "";
                StartCourtPositionsSearchAllowed = false;
            }

            //end line to all court positions data
            if (isSearchPermittedCourtPositions && line == "}")
            {
                employee = "";
                profession = "";
                StartCourtPositionsSearchAllowed = false;
                isSearchPermittedCourtPositions = false;
            }
        }

    };

    struct SearchKeys
    {
        private static bool Start_TraitsFound { get; set; }
        private static bool End_TraitsFound { get; set; }
        public static bool HasTraitsExtracted { get; set; }

        public static void TraitsList(string line)
        {
            if (!HasTraitsExtracted)
            {
                if (!Start_TraitsFound)
                {
                    if (line.Contains("traits_lookup={"))
                    {
                        Start_TraitsFound = true;
                    }
                    else { Start_TraitsFound = false; }
                }

                if (Start_TraitsFound && !End_TraitsFound)
                {
 
                    if (line == "provinces={")
                    {
                        End_TraitsFound = true;
                        //SaveFile.ReadWoundedTraits();
                        return;
                    }
                    else { End_TraitsFound = false; }

                    using (StreamWriter sw = File.AppendText(@".\data\save_file_data\Traits.txt"))
                    {
                        sw.WriteLine(line);
                    }

                }

                if (End_TraitsFound)
                {
                    HasTraitsExtracted = true;
                    Start_TraitsFound = false;
                    End_TraitsFound = false;
                }
            }
        }

        private static bool Start_CombatsFound { get; set; }
        private static bool End_CombatsFound { get; set; }
        public static bool HasCombatsExtracted { get; set; }

        public static void Combats(string line)
        {
            if(!HasCombatsExtracted)
            {
                if (!Start_CombatsFound)
                {
                    if (line == "\tcombats={") { 
                        Start_CombatsFound = true; 
                    }
                    else { Start_CombatsFound = false; }
                }

                if(Start_CombatsFound && !End_CombatsFound)
                {
                    if (line == "pending_character_interactions={") 
                    {
                        //Write Combats Data to txt file
                        using (StreamWriter sw = File.AppendText(@".\data\save_file_data\Combats.txt"))
                        {
                            sw.Write(Data.SB_Combats);
                            sw.Close();
                        }
                        Data.SB_Combats = null;
                        GC.Collect();

                        End_CombatsFound = true; 
                        return;
                    }
                    else { End_CombatsFound = false; }

                    Data.SB_Combats.AppendLine(line);

                }

                if(End_CombatsFound)
                {
                    HasCombatsExtracted = true;
                    Start_CombatsFound = false; 
                    End_CombatsFound = false;
                }
            }
        }

        private static bool Start_BattleResultsFound { get; set; }
        private static bool End_BattleResultsFound { get; set; }
        public static bool HasBattleResultsExtracted { get; set; }

        public static void BattleResults(string line)
        {
            if (!HasBattleResultsExtracted)
            {
                if (!Start_BattleResultsFound)
                {
                    if (line == "\tcombat_results={")
                    {
                        Start_BattleResultsFound = true;
                    }
                    else { Start_BattleResultsFound = false; }
                }
                


                if (Start_BattleResultsFound && !End_BattleResultsFound)
                {
                    if (line == "\tcombats={")
                    {
                        //Write CombatResults Data to txt file
                        using (StreamWriter sw = File.AppendText(@".\data\save_file_data\BattleResults.txt"))
                        {
                            sw.Write(Data.SB_CombatResults);
                            sw.Close();
                        }
                        Data.SB_CombatResults = null;
                        GC.Collect();

                        End_BattleResultsFound = true;
                        return;
                    }
                    else { End_BattleResultsFound = false; }

                    Data.SB_CombatResults.AppendLine(line);

                }

                if (End_BattleResultsFound)
                {
                    HasBattleResultsExtracted = true;
                    Start_BattleResultsFound = false;
                    End_BattleResultsFound = false;
                }
            }
        }

        private static bool Start_RegimentsFound { get; set; }
        private static bool End_RegimentsFound { get; set; }
        public static bool HasRegimentsExtracted { get; set; }
        public static void Regiments(string line)
        {
            if (!HasRegimentsExtracted)
            {
                if (!Start_RegimentsFound)
                {
                    if (line == "\tregiments={") { Start_RegimentsFound = true; }
                    else { Start_RegimentsFound = false; }
                }

                if (Start_RegimentsFound && !End_RegimentsFound)
                {
                  
                    if (line == "\tarmy_regiments={") 
                    {
                        //Write Regiments Data to txt file
                        using (StreamWriter sw = File.AppendText(@".\data\save_file_data\Regiments.txt"))
                        {
                            sw.Write(Data.SB_Regiments);
                            sw.Close();
                        }
                        Data.SB_Regiments = null;
                        GC.Collect();
                        End_RegimentsFound = true; 
                        return;
                    }
                    else { End_RegimentsFound = false; }

                    Data.SB_Regiments.AppendLine(line);
                }

                if (End_RegimentsFound)
                {
                    HasRegimentsExtracted = true;
                    Start_RegimentsFound = false; 
                    End_RegimentsFound = false;
                }
            }
        }

        private static bool Start_ArmyRegimentsFound { get; set; }
        private static bool End_ArmyRegimentsFound { get; set; }
        public static bool HasArmyRegimentsExtracted { get; set; }
        public static void ArmyRegiments(string line)
        {
            if (!HasArmyRegimentsExtracted)
            {
                if (!Start_ArmyRegimentsFound)
                {
                    if (line == "\tarmy_regiments={") { Start_ArmyRegimentsFound = true; }
                    else { Start_ArmyRegimentsFound = false; }
                }

                if (Start_ArmyRegimentsFound && !End_ArmyRegimentsFound)
                {
                    
                    if (line == "\tarmies={") 
                    {
                        //Write ArmyRegiments Data to txt file
                        using (StreamWriter sw = File.AppendText(@".\data\save_file_data\ArmyRegiments.txt"))
                        {
                            sw.Write(Data.SB_ArmyRegiments);
                            sw.Close();
                        }
                        Data.SB_ArmyRegiments = null;
                        GC.Collect();
                        End_ArmyRegimentsFound = true; 
                        return;
                    }
                    else { End_ArmyRegimentsFound = false; }

                    Data.SB_ArmyRegiments.AppendLine(line);
                }

                if (End_ArmyRegimentsFound)
                {
                    HasArmyRegimentsExtracted = true;
                    Start_ArmyRegimentsFound = false;
                    End_ArmyRegimentsFound = false;
                }
            }
        }

        private static bool Start_ArmiesFound { get; set; }
        private static bool End_ArmiesFound { get; set; }
        public static bool HasArmiesExtracted { get; set; }
        public static void Armies(string line)
        {
            if (!HasArmiesExtracted)
            {
                if (!Start_ArmiesFound)
                {
                    if (line == "\tarmies={") { Start_ArmiesFound = true; }
                    else { Start_ArmiesFound = false; }
                }

                if (Start_ArmiesFound && !End_ArmiesFound)
                {

                    if (line == "\t}")
                    {
                        //Write Armies Data to txt file
                        using (StreamWriter sw = File.AppendText(@".\data\save_file_data\Armies.txt"))
                        {
                            sw.Write(Data.SB_Armies);
                            sw.Close();
                        }
                        Data.SB_Armies = null;
                        GC.Collect();
                        End_ArmiesFound = true;
                        return;
                    }
                    else { End_ArmiesFound = false; }

                    Data.SB_Armies.AppendLine(line);
                }

                if (End_ArmiesFound)
                {
                    HasArmiesExtracted = true;
                    Start_ArmiesFound = false;
                    End_ArmiesFound = false;
                }
            }
        }

        private static bool Start_LivingFound { get; set; }
        private static bool End_LivingFound { get; set; }
        public static bool HasLivingExtracted { get; set; }
        public static void Living(string line)
        {
            if (!HasLivingExtracted)
            {
                if (!Start_LivingFound)
                {
                    //Match start = Regex.Match(line, @"living={");
                    if (line == "living={") { Start_LivingFound = true; }
                    else { Start_LivingFound = false; }
                }

                if (Start_LivingFound && !End_LivingFound)
                {
                    if (line == "dead_unprunable={") 
                    {
                        //Write Living Data to txt file
                        using (StreamWriter sw = File.AppendText(@".\data\save_file_data\Living.txt"))
                        {
                            sw.Write(Data.SB_Living);
                            sw.Close();
                        }
                        Data.SB_Living = null;
                        GC.Collect();
                        End_LivingFound = true;
                        return;
                    }
                    else { End_LivingFound = false; }
                    
                    Data.SB_Living.AppendLine(line);

                }

                if (End_LivingFound)
                {
                    HasLivingExtracted = true;
                    Start_LivingFound = false;
                    End_LivingFound = false;
                }
            }
        }

        private static bool Start_CountiesFound { get; set; }
        private static bool End_CountiesFound { get; set; }
        public static bool HasCountiesExtracted { get; set; }
        public static void Counties(string line)
        {
            if (!HasCountiesExtracted)
            {
                if (!Start_CountiesFound)
                {
                    //Match start = Regex.Match(line, @"living={");
                    if (line == "\tcounties={") 
                    { Start_CountiesFound = true; }
                    else { Start_CountiesFound = false; }
                }

                if (Start_CountiesFound && !End_CountiesFound)
                {
                    if (line == "}")
                    {
                        //Write Counties Data to txt file
                        using (StreamWriter sw = File.AppendText(@".\data\save_file_data\Counties.txt"))
                        {
                            sw.Write(Data.SB_Counties);
                            sw.Close();
                        }
                        Data.SB_Counties = null;
                        GC.Collect();
                        End_CountiesFound = true;
                        return;
                    }
                    else { End_CountiesFound = false; }

                    Data.SB_Counties.AppendLine(line);
                }

                if (End_CountiesFound)
                {
                    HasCountiesExtracted = true;
                    Start_CountiesFound = false;
                    End_CountiesFound = false;
                }
            }
        }

        private static bool Start_UnitsFound { get; set; }
        private static bool End_UnitsFound { get; set; }
        public static bool HasUnitsExtracted { get; set; }
        public static void Units(string line)
        {
            if (!HasUnitsExtracted)
            {
                if (!Start_UnitsFound)
                {
                    //Match start = Regex.Match(line, @"living={");
                    if (line == "units={")
                    { Start_UnitsFound = true; }
                    else { Start_UnitsFound = false; }
                }

                if (Start_UnitsFound && !End_UnitsFound)
                {
                    if (line == "}")
                    {
                        //Write Units Data to txt file
                        using (StreamWriter sw = File.AppendText(@".\data\save_file_data\Units.txt"))
                        {
                            sw.Write(Data.SB_Units);
                            sw.Close();
                        }
                        Data.SB_Units = null;
                        GC.Collect();
                        End_UnitsFound = true;
                        return;
                    }
                    else { End_UnitsFound = false; }

                    Data.SB_Units.AppendLine(line);
                }

                if (End_UnitsFound)
                {
                    HasUnitsExtracted = true;
                    Start_UnitsFound = false;
                    End_UnitsFound = false;
                }
            }
        }

        private static bool Start_CourtPositionsFound { get; set; }
        private static bool End_CourtPositionsFound { get; set; }
        public static bool HasCourtPositionsExtracted { get; set; }
        public static void CourtPositions(string line)
        {
            if (!HasCourtPositionsExtracted)
            {
                if (!Start_CourtPositionsFound)
                {
                    //Match start = Regex.Match(line, @"living={");
                    if (line == "court_positions={")
                    { Start_CourtPositionsFound = true; }
                    else { Start_CourtPositionsFound = false; }
                }

                if (Start_CourtPositionsFound && !End_CourtPositionsFound)
                {
                    if (line == "}")
                    {
                        //Write Units Data to txt file
                        using (StreamWriter sw = File.AppendText(@".\data\save_file_data\CourtPositions.txt"))
                        {
                            sw.Write(Data.SB_CourtPositions);
                            sw.Close();
                        }
                        Data.SB_CourtPositions = null;
                        GC.Collect();
                        End_CourtPositionsFound = true;
                        return;
                    }
                    else { End_CourtPositionsFound = false; }

                    Data.SB_CourtPositions.AppendLine(line);
                }

                if (End_CourtPositionsFound)
                {
                    HasCourtPositionsExtracted = true;
                    Start_CourtPositionsFound = false;
                    End_CourtPositionsFound = false;
                }
            }
        }

        private static bool Start_CulturesFound { get; set; }
        private static bool End_CulturesFound { get; set; }
        public static bool HasCulturesExtracted { get; set; }
        public static void Cultures(string line)
        {
            if (!HasCulturesExtracted)
            {
                if (!Start_CulturesFound)
                {
                    //Match start = Regex.Match(line, @"living={");
                    if (line == "culture_manager={")
                    { Start_CulturesFound = true; }
                    else { Start_CulturesFound = false; }
                }

                if (Start_CulturesFound && !End_CulturesFound)
                {
                    if (line == "}")
                    {
                        //Write Cultures Data to txt file
                        using (StreamWriter sw = File.AppendText(@".\data\save_file_data\Cultures.txt"))
                        {
                            sw.Write(Data.SB_Cultures);
                            sw.Close();
                        }
                        Data.SB_Cultures = null;
                        GC.Collect();
                        End_CulturesFound = true;
                        return;
                    }
                    else { End_CulturesFound = false; }

                    Data.SB_Cultures.AppendLine(line);
                
                }

                if (End_CulturesFound)
                {
                    HasCulturesExtracted = true;
                    Start_CulturesFound = false;
                    End_CulturesFound = false;
                }
            }
        }

        private static bool Start_MercenariesFound { get; set; }
        private static bool End_MercenariesFound { get; set; }
        public static bool HasMercenariesExtracted { get; set; }
        public static void Mercenaries(string line)
        {

            if (!HasMercenariesExtracted)
            {
                if (!Start_MercenariesFound)
                {
                    //Match start = Regex.Match(line, @"living={");
                    if (line == "mercenary_company_manager={")
                    { Start_MercenariesFound = true; }
                    else { Start_MercenariesFound = false; }
                }

                if (Start_MercenariesFound && !End_MercenariesFound)
                {

                    if (line == "}")
                    {
                        //Write Mercenaries Data to txt file
                        using (StreamWriter sw = File.AppendText(@".\data\save_file_data\Mercenaries.txt"))
                        {
                            sw.Write(Data.SB_Mercenaries);
                            sw.Close();
                        }
                        Data.SB_Mercenaries = null;
                        GC.Collect();
                        HasMercenariesExtracted = true;
                        return;
                    }
                    else { HasMercenariesExtracted = false; }
                    
                    Data.SB_Mercenaries.AppendLine(line);

                }

                if (End_MercenariesFound)
                {
                    HasMercenariesExtracted = true;
                    Start_MercenariesFound = false;
                    End_MercenariesFound = false;
                }
            }
        }
    }
}
