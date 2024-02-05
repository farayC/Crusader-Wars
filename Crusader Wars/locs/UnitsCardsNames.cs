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

        public static void ChangeUnitsCardsNames(Player Player, Enemy Enemy)
        {
            string pattern = @"name_(?<AttilaUnit>\w+)\t(?<UnitName>.+)\t";

            string edited_file = @".\Settings\919_land_units.loc.tsv";

            string unit_919AD_path =@".\battle files\text\db\919_land_units.loc.tsv";


            File.Copy(unit_919AD_path, edited_file);
            File.WriteAllText(edited_file, string.Empty);

            string edited_names="";
            using (FileStream units_file = File.Open(unit_919AD_path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(units_file))
            {
                string line = "";
                while(line != null && !reader.EndOfStream)
                {
                    line = reader.ReadLine();
                        
                    foreach(var regiment in Player.Army)
                    {
                        if(line.Contains($"land_units_onscreen_name_{regiment.Key}\t"))
                        {
                            //Commander
                            if(regiment.Type == "General")
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

                    foreach(var regiment in Enemy.Army)
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

                    edited_names += line+"\n"; // write to string every line
                }

                reader.Close();
                units_file.Close();
            }

            File.WriteAllText (edited_file, edited_names);
            File.Delete(unit_919AD_path);
            File.Move(edited_file, unit_919AD_path);
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
