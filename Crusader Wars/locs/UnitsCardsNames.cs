using Crusader_Wars.data.save_file;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crusader_Wars.locs
{
    /*
     *      REWORK ALL OF THIS SECTION FOR BETA
     * 
     */
    static class UnitsCardsNames
    {

        private static string Player_KnightsName { get; set; }
        public static string Enemy_KnightsName { get; set; }

        public static void SetPlayerKnightsName(string name)
        {
            Player_KnightsName = name;
        }
        public static void SetEnemyKnightsName(string name)
        {
            Enemy_KnightsName = name;
        }

        /*
        public static void ChangeUnitsCardsNames(string Mapper_Name, Player Player, Enemy Enemy)
        {
            switch (Mapper_Name)
            {
                case "OfficialCW_DarkAges_AOC":
                case "OfficialCW_EarlyMedieval_919Mod":
                    string[] loc_files = Directory.GetFiles(@".\data\units_cards_names\anno domini\");
                    //EditUnitCardsFiles(loc_files, Player, Enemy);
                    break;
                case "OfficialCW_HighMedieval_MK1212":
                case "OfficialCW_LateMedieval_MK1212":
                case "OfficialCW_Renaissance_MK1212":
                    string[] mk1212_loc_files = Directory.GetFiles(@".\data\units_cards_names\mk1212\");
                    //EditUnitCardsFiles(mk1212_loc_files, Player, Enemy);
                    break;
                case "xCW_FallenEagle_AgeOfJustinian":
                    string[] aoj_loc_files = Directory.GetFiles(@".\data\units_cards_names\age of justinian\");
                    //EditUnitCardsFiles(aoj_loc_files, Player, Enemy);
                    break;
                case "xCW_FallenEagle_FallofTheEagle":
                    string[] fte_loc_files = Directory.GetFiles(@".\data\units_cards_names\fall of the eagles\");
                    //EditUnitCardsFiles(fte_loc_files, Player, Enemy);
                    break;
                case "xCW_RealmsInExile_TheDawnlessDays":
                    string[] lotr_loc_files = Directory.GetFiles(@".\data\units_cards_names\dawnless days\");
                    //EditUnitCardsFiles(lotr_loc_files, Player, Enemy);
                    break;
            }

        }
        */

        /*
        private static void EditUnitCardsFiles(string[] unit_cards_files, List<Army> attacker, List<Army> defender)
        {
            for (int i = 0; i < unit_cards_files.Length; i++)
            {
                string loc_file_path = unit_cards_files[i];
                string loc_file_name = Path.GetFileName(loc_file_path);
                string file_to_edit_path = $@".\data\{loc_file_name}";

                
                if(File.Exists(file_to_edit_path)) File.Delete(file_to_edit_path);
                
                //Copy original loc file
                File.Copy(loc_file_path, file_to_edit_path);

                //Clears the new one
                File.WriteAllText(file_to_edit_path, string.Empty);

                string edited_names = "";
                using (FileStream units_file = File.Open(loc_file_path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(units_file))
                {
                    string line = "";
                    while (line != null && !reader.EndOfStream)
                    {
                        line = reader.ReadLine();

                        foreach(var army in attacker)
                        {
                            foreach(var unit in army.Units)
                            {
                                if (line.Contains($"land_units_onscreen_name_{unit.GetAttilaUnitKey()}\t"))
                                {
                                    //Commander
                                    if (unit.GetRegimentType() == RegimentType.Knight)
                                    {
                                        string commander_name = ReturnGeneralName(Player);
                                        line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{commander_name}\t");
                                        break;
                                    }

                                    //Knights
                                    if (unit.GetRegimentType() == RegimentType.Knight && unit.GetSoldiers() > 0)
                                    {
                                        string knights_name = RemoveDiacritics(Player_KnightsName);
                                        line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{knights_name}\t");
                                        break;
                                    }


                                    //Levies
                                    if (unit.GetRegimentType() == RegimentType.Levy)
                                    {
                                        string levies_name = ReturnLeviesName(regiment.Type);

                                        line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{levies_name}\t");
                                        break;
                                    }

                                    //Men-At-Arms
                                    line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{unit.GetName()}\t");
                                    break;
                                }
                            }

                        }

                        foreach (var army in defender)
                        {
                            foreach (var unit in army.Units)
                            {
                                if (line.Contains($"land_units_onscreen_name_{unit.GetAttilaUnitKey()}\t"))
                                {
                                    //Commander
                                    if (unit.GetRegimentType() == RegimentType.Knight)
                                    {
                                        string commander_name = ReturnGeneralName(Player);
                                        line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{commander_name}\t");
                                        break;
                                    }

                                    //Knights
                                    if (unit.GetRegimentType() == RegimentType.Knight && unit.GetSoldiers() > 0)
                                    {
                                        string knights_name = RemoveDiacritics(Player_KnightsName);
                                        line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{knights_name}\t");
                                        break;
                                    }


                                    //Levies
                                    if (unit.GetRegimentType() == RegimentType.Levy)
                                    {
                                        string levies_name = ReturnLeviesName(regiment.Type);

                                        line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{levies_name}\t");
                                        break;
                                    }

                                    //Men-At-Arms
                                    line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{unit.GetName()}\t");
                                    break;
                                }
                            }

                        }

                        edited_names += line + "\n"; // write to string every line
                    }

                    reader.Close();
                    units_file.Close();
                }

                string battle_files_path = $@".\battle files\text\db\{loc_file_name}";

                File.WriteAllText(file_to_edit_path, edited_names);
                if(File.Exists(battle_files_path))File.Delete(battle_files_path);
                File.Move(file_to_edit_path, battle_files_path);

            }
            


        }
        */



        private static string ReturnLeviesName(string levy_type)
        {
            string levies_name = "Levies";
            switch (levy_type)
            {
                case "Levy Spearmen":
                    levies_name = "Levy Peasantry";
                    break;
                case "Levy Infantry":
                    levies_name = "Levy Landowners";
                    break;
                case "Levy Ranged":
                    levies_name = "Levy Skirmishers";
                    break;
            }
            return levies_name;
        }


        /*
        private static string ReturnGeneralName(ICharacter Side)
        {

            string name="Commander's Retinue";

            
            switch(Side.Commander.Rank)
            {
                case 1:
                    name = "Commander's Retinue";
                    break;
                case 2:
                    name = "Baronial Retinue";
                    break;
                case 3:
                    name = "Lord's Retinue";
                    break;
                case 4:
                    name = "Ducal Retinue";
                    break;
                case 5:
                    name = "Royal Retinue";
                    break;
                case 6:
                    name = "Imperial Retinue";
                    break;
            }


            return name;
        }
        */


        static string RemoveDiacritics(string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

    }
}
