using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crusader_Wars.locs
{
    static class UnitsCardsNames
    {
        /*
         * Whats missing:
         *  -Change Unit Card names based on the loaded Unit Mapper
         *  -Commanders Unit Card name based on their culture title tier name
         *  -Knights Unit Card name based on their culture name
         */

        public static void ChangeUnitsCardsNames(string Mapper_Name, Player Player, Enemy Enemy)
        {
            switch(Mapper_Name)
            {
                case "OfficialCW_DarkAges_AOC":
                case "OfficialCW_EarlyMedieval_919Mod":

                    string attila_loc = @".\data\units_cards_names\land_units.loc.tsv";
                    string mod919_loc = @".\data\units_cards_names\919_land_units.loc.tsv";
                    string[] loc_files = new string []{mod919_loc,attila_loc };
                    EditUnitCardsFiles(loc_files, Player, Enemy);
                    break;
                case "OfficialCW_HighMedieval_MK1212":
                case "OfficialCW_LateMedieval_MK1212":
                case "OfficialCW_Renaissance_MK1212":
                    string mk1212_loc = @".\data\units_cards_names\mk1212_land_units.loc.tsv";
                    string submod_loc = @".\data\units_cards_names\mk1212submod_land_units.loc.tsv";
                    string[] mk1212_loc_files = new string[] { mk1212_loc, submod_loc };
                    EditUnitCardsFiles(mk1212_loc_files, Player, Enemy);
                    break;

            }

        }

        private static void EditUnitCardsFiles(string[] unit_cards_files, Player Player, Enemy Enemy)
        {
            for (int i = 0; i < unit_cards_files.Length - 1; i++)
            {
                string loc_file_path = unit_cards_files[i];
                string loc_file_name = Path.GetFileName(loc_file_path);
                string file_to_edit_path = $@".\data\{loc_file_name}";

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

                        foreach (var regiment in Player.Army)
                        {
                            if (line.Contains($"land_units_onscreen_name_{regiment.Key}\t"))
                            {
                                //Commander
                                if (regiment.Type == "General")
                                {
                                    string commander_name = ReturnGeneralName(Player);
                                    line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{commander_name}\t");
                                    break;
                                }

                                //Knights
                                if (regiment.Type == "Knights" && regiment.SoldiersNum > 0)
                                {

                                }


                                //Levies
                                if (regiment.Type.Contains("Levy"))
                                {
                                    string levies_name = ReturnLeviesName(regiment.Type);

                                    line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{levies_name}\t");
                                    break;
                                }

                                //Men-At-Arms
                                line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{regiment.Type}\t");
                                break;
                            }
                        }

                        foreach (var regiment in Enemy.Army)
                        {
                            if (line.Contains($"land_units_onscreen_name_{regiment.Key}\t"))
                            {
                                //Commander
                                if (regiment.Type == "General")
                                {
                                    string commander_name = ReturnGeneralName(Enemy);
                                    line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{commander_name}\t");
                                    break;
                                }

                                //Knights
                                if (regiment.Type == "Knights" && regiment.SoldiersNum > 0)
                                {

                                }


                                if (regiment.Type.Contains("Levy"))
                                {
                                    string levies_name = ReturnLeviesName(regiment.Type);

                                    line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{levies_name}\t");
                                    break;
                                }

                                line = Regex.Replace(line, @"\t(?<UnitName>.+)\t", $"\t{regiment.Type}\t");
                                break;
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


        private static string ReturnLeviesName(string levy_type)
        {
            string levies_name = "Levies";
            switch (levy_type)
            {
                case "Levy Spearmen":
                    levies_name = "Levy Peasantry";
                    break;
                case "Levy Infantry":
                    levies_name = "Levy Burghers";
                    break;
                case "Levy Ranged":
                    levies_name = "Levy Skirmishers";
                    break;
            }
            return levies_name;
        }


        private static string ReturnGeneralName(ICharacter Side)
        {

            string name="Commander";
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

    }
}
