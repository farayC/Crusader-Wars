using Crusader_Wars.twbattle;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
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
        private static ICharacter Player { get; set; }
        private static ICharacter Enemy { get; set; }

        /// <summary>  
        /// Sets essential data to the Reader. Important to set this before using the Reader!  
        /// </summary>  
        /// <param name="player">Player side object</param>  
        /// <param name="enemy">Enemy side object</param>  
        public static void SetData(ICharacter player, ICharacter enemy)
        {
            Player = player;
            Enemy = enemy;
        }

        /// <summary>  
        /// Reads the ck3 save file for all the needed data.  
        /// </summary>  
        /// <param name="savePath">Path to the ck3 save file</param>  
        public static void ReadFile(string savePath)
        {
            //Clear Battle Results File
            File.WriteAllText(@".\data\save_file_data\BattleResults.txt", "");
            //Clear Battle Results TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\BattleResults.txt", "");
            //Clear Combats File
            File.WriteAllText(@".\data\save_file_data\Combats.txt", "");
            //Clear Combats TEMP File
            File.WriteAllText(@".\data\save_file_data\temp\Combats.txt", "");


            using (FileStream saveFile = File.Open(savePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(saveFile))
            {
                string line = reader.ReadLine();
                while (line != null && !reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    //GetterKeys.ReadProvinceBuildings(line, "5984");
                    GetterKeys.ReadAccolades(line, Player, Enemy);
                    GetterKeys.ReadCourtPositions(line, Player, Enemy);
                    GetterKeys.ReadLivingCharacters(line, Player, Enemy);

                    SearchKeys.TraitsList(line);
                    
                    SearchKeys.BattleResults(line);
                    SearchKeys.Combats(line);
                    SearchKeys.Regiments(line);
                    SearchKeys.ArmyRegiments(line);
                    SearchKeys.Living(line);


                    //SearchKeys.Armies(line);
                    //SearchKeys.Counties(line);
                    //SearchKeys.Cultures(line);
                    //SearchKeys.Mercenaries(line);

                }
                Player = null;
                Enemy = null;

                reader.Close();
                saveFile.Close();
            }

            Data.ConvertDataToString();
            SaveFile.ReadWoundedTraits();
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

        //Sieges
        public static List<string> Province_Buildings = new List<string>();

        public static StringBuilder Traits = new StringBuilder();
        public static StringBuilder Living = new StringBuilder();
        public static StringBuilder Armies = new StringBuilder();
        public static StringBuilder ArmyRegiments = new StringBuilder();
        public static StringBuilder Regiments = new StringBuilder();
        public static StringBuilder Counties = new StringBuilder();
        public static StringBuilder Cultures = new StringBuilder();
        public static StringBuilder Mercenaries = new StringBuilder();
        //public static int BattleID = 0;

        public static string String_Traits;
        public static string String_Living;
        public static string String_Armies;
        public static string String_ArmyRegiments;
        public static string String_Regiments;
        public static string String_Counties;
        public static string String_Cultures;
        public static string String_Mercenaries;

        public static void ConvertDataToString()
        {
            
            long startMemory = GC.GetTotalMemory(false);

            String_Traits = Traits.ToString();
            String_Living = Living.ToString();
            String_ArmyRegiments = ArmyRegiments.ToString();
            String_Armies = Armies.ToString();
            String_Regiments = Regiments.ToString();
            String_Counties = Counties.ToString();
            String_Cultures = Cultures.ToString();
            String_Mercenaries = Mercenaries.ToString();
            string path = @".\data\save_file_data\Mercenaries.txt";



            long endMemory = GC.GetTotalMemory(false);
            long memoryUsage = endMemory - startMemory;

            Console.WriteLine($"----\nConvert all data to string\nMemory Usage: {memoryUsage / 1048576} mb");
        }

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

            Traits = new StringBuilder();
            Living = new StringBuilder();
            Armies = new StringBuilder();
            ArmyRegiments = new StringBuilder ();
            Regiments = new StringBuilder ();

            Counties = new StringBuilder();
            Cultures = new StringBuilder();
            Mercenaries = new StringBuilder();

            String_Traits = "";
            String_Living = "";
            String_Armies = "";
            String_ArmyRegiments= "";
            String_Regiments = "";
            String_Counties = "";
            String_Cultures = "";
            String_Mercenaries = "";

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
        static List<(string, int, int, List<string>, BaseSkills, bool)> player_knights_list;
        static List<(string, int, int, List<string>, BaseSkills, bool)> enemy_knights_list;
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
                if(line == ($"\t{Player.Commander.CommanderID}={{") || line == $"{Player.Commander.CommanderID}={{")
                {
                    StartCharacterSearchAllowed = true;
                    isPlayerCommander=true;
                    char_id = Player.Commander.CommanderID;
                }
                else if (line == $"\t{Enemy.Commander.CommanderID}={{" || line == $"{Enemy.Commander.CommanderID}={{")
                {
                    StartCharacterSearchAllowed = true;
                    isEnemyCommander=true;
                    char_id = Enemy.Commander.CommanderID;
                }
                
                if(player_knights_list != null)
                {
                    foreach (var knight in player_knights_list)
                    {
                        if (line == $"\t{knight.Item1}={{" || line == $"{knight.Item1}={{")
                        {
                            StartCharacterSearchAllowed = true;
                            isPlayerKnight = true;
                            char_id = knight.Item1;
                        }
                    }
                }

                if( enemy_knights_list != null )
                {
                    foreach (var knight in enemy_knights_list)
                    {
                        if (line == $"\t{knight.Item1}={{" || line == $"{knight.Item1}={{")
                        {
                            StartCharacterSearchAllowed = true;
                            isEnemyKnight = true;
                            char_id = knight.Item1;
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
                        Player.Knights.SetTraits(char_id, traits_collection);
                    }
                    else if(isEnemyKnight)
                    {
                        Enemy.Knights.SetTraits(char_id, traits_collection);
                    }

                }


                if(line.Contains("\tskill={"))
                {
                    var skills_collection = Regex.Matches(line, @"\d+").Cast<Match>()
                                                .Select(m => m.Value)
                                                .ToList<string>();

                    if (isPlayerKnight)
                    {
                        BaseSkills skills = new BaseSkills(skills_collection);
                        Player.Knights.SetSkills(char_id, skills);
                    }
                    else if (isEnemyKnight)
                    {
                        BaseSkills skills = new BaseSkills(skills_collection);
                        Enemy.Knights.SetSkills(char_id, skills);
                    }
                }

                //set the knight as accolade if true and add id to a list
                if (line.Contains("\t\taccolade="))
                {
                   
                    (string, int, int, List<string>, BaseSkills, bool) accolade_knight;

                    if (isPlayerKnight)
                    {
                        try
                        {
                            var player_knights = Player.Knights.GetKnightsList();
                            accolade_knight = player_knights.First(x => x.Item1 == char_id);

                            int index = player_knights.IndexOf(accolade_knight);
                            accolade_knight = ((accolade_knight.Item1, 4, accolade_knight.Item3, accolade_knight.Item4, accolade_knight.Item5, true));
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
                            accolade_knight = enemy_knights.First(x => x.Item1 == char_id);

                            int index = enemy_knights.IndexOf(accolade_knight);
                            accolade_knight = ((accolade_knight.Item1, 4, accolade_knight.Item3, accolade_knight.Item4, accolade_knight.Item5, true));
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
                        Start_TraitsFound = true; Console.WriteLine("TRAITS START KEY FOUND!");
                    }
                    else { Start_TraitsFound = false; }
                }

                if (Start_TraitsFound && !End_TraitsFound)
                {
 
                    if (line == "provinces={")
                    {
                        End_TraitsFound = true;
                        Console.WriteLine("TRAITS END KEY FOUND!");
                        return;
                    }
                    else { End_TraitsFound = false; }

                    Data.Traits.Append(line + "\n");

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
                        Start_CombatsFound = true; Console.WriteLine("COMBAT START KEY FOUND!"); 
                    }
                    else { Start_CombatsFound = false; }
                }

                if(Start_CombatsFound && !End_CombatsFound)
                {
                    if (line == "pending_character_interactions={") 
                    {
                        End_CombatsFound = true; 
                        Console.WriteLine("COMBAT END KEY FOUND!");
                        return;
                    }
                    else { End_CombatsFound = false; }

                    using (StreamWriter sw = File.AppendText(@".\data\save_file_data\Combats.txt"))
                    {
                        sw.WriteLine(line);
                    }

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
                        Start_BattleResultsFound = true; Console.WriteLine("COMBAT RESULT START KEY FOUND!");
                    }
                    else { Start_BattleResultsFound = false; }
                }
                


                if (Start_BattleResultsFound && !End_BattleResultsFound)
                {
                    if (line == "\tcombats={")
                    {
                        End_BattleResultsFound = true;
                        Console.WriteLine("COMBAT RESULT END KEY FOUND!");
                        return;
                    }
                    else { End_BattleResultsFound = false; }

                    using (StreamWriter sw = File.AppendText(@".\data\save_file_data\BattleResults.txt")) 
                    {
                        sw.WriteLine(line);
                    }

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
                    if (line == "\tregiments={") { Start_RegimentsFound = true; Console.WriteLine("REGIMENTS START KEY FOUND!"); }
                    else { Start_RegimentsFound = false; }
                }

                if (Start_RegimentsFound && !End_RegimentsFound)
                {
                  
                    if (line == "\tarmy_regiments={") 
                    { 
                        End_RegimentsFound = true; 
                        Console.WriteLine("REGIMENTS END KEY FOUND!");
                        return;
                    }
                    else { End_RegimentsFound = false; }

                    Data.Regiments.Append(line + "\n");
                }

                if (End_RegimentsFound)
                {
                    HasRegimentsExtracted = true;
                    Start_RegimentsFound = false; 
                    End_RegimentsFound = false;
                    //Console.WriteLine(Data.Regiments);
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
                    if (line == "\tarmy_regiments={") { Start_ArmyRegimentsFound = true; Console.WriteLine("ARMY REGIMENTS START KEY FOUND!"); }
                    else { Start_ArmyRegimentsFound = false; }
                }

                if (Start_ArmyRegimentsFound && !End_ArmyRegimentsFound)
                {
                    
                    if (line == "\tarmies={") 
                    { 
                        End_ArmyRegimentsFound = true; 
                        Console.WriteLine("ARMY REGIMENTS END KEY FOUND!");
                        return;
                    }
                    else { End_ArmyRegimentsFound = false; }

                    Data.ArmyRegiments.Append(line + "\n");
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
                    if (line == "\tarmies={") { Start_ArmiesFound = true; Console.WriteLine("ARMIES START KEY FOUND!"); }
                    else { Start_ArmiesFound = false; }
                }

                if (Start_ArmiesFound && !End_ArmiesFound)
                {

                    if (line == "\t}")
                    {
                        End_ArmiesFound = true;
                        Console.WriteLine("ARMIES END KEY FOUND!");
                        return;
                    }
                    else { End_ArmiesFound = false; }

                    Data.Armies.Append(line + "\n");
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
                    if (line == "living={") { Start_LivingFound = true; Console.WriteLine("LIVING START KEY FOUND!"); }
                    else { Start_LivingFound = false; }
                }

                if (Start_LivingFound && !End_LivingFound)
                {
                    if (line == "dead_unprunable={") 
                    { 
                        End_LivingFound = true; 
                        Console.WriteLine("LIVING END KEY FOUND!");
                        return;
                    }
                    else { End_LivingFound = false; }

                    Data.Living.Append(line + "\n");
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
                    { Start_CountiesFound = true; Console.WriteLine("COUNTIES START KEY FOUND!"); }
                    else { Start_CountiesFound = false; }
                }

                if (Start_CountiesFound && !End_CountiesFound)
                {
                    if (line == "}")
                    {
                        End_CountiesFound = true;
                        Console.WriteLine("COUNTIES END KEY FOUND!");
                        return;
                    }
                    else { End_CountiesFound = false; }

                    Data.Counties.Append(line + "\n");
                }

                if (End_CountiesFound)
                {
                    HasCountiesExtracted = true;
                    Start_CountiesFound = false;
                    End_CountiesFound = false;
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
                    { Start_CulturesFound = true; Console.WriteLine("CULTURES START KEY FOUND!"); }
                    else { Start_CulturesFound = false; }
                }

                if (Start_CulturesFound && !End_CulturesFound)
                {
                    if (line == "}")
                    {
                        End_CulturesFound = true;
                        Console.WriteLine("CULTURES END KEY FOUND!");
                        return;
                    }
                    else { End_CulturesFound = false; }

                    Data.Cultures.Append(line + "\n");
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
                    { Start_MercenariesFound = true; Console.WriteLine("MERCENARIES START KEY FOUND!"); }
                    else { Start_MercenariesFound = false; }
                }

                if (Start_MercenariesFound && !End_MercenariesFound)
                {

                    if (line == "}")
                    {
                        HasMercenariesExtracted = true;
                        Console.WriteLine("MERCENARIES END KEY FOUND!");
                        return;
                    }
                    else { HasMercenariesExtracted = false; }
                    
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
