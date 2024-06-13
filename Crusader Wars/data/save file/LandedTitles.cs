using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;

namespace Crusader_Wars.data.save_file
{
    
    //Change to internal later
    public static class LandedTitles
    {
        static List<string> EnabledMods;

        public static void ReadProvinces(List<Army> attacker_armies, List<Army> defender_armies)
        {
            ReadEnabledMods();
            ReadEachModPath();
            ReadModsLandedTitles(attacker_armies, defender_armies);
            ReadDefaultCK3LandedTitles(attacker_armies, defender_armies);
        }
        static void ReadEnabledMods()
        {
            string ck3_mods_folder_path = Properties.Settings.Default.VAR_log_ck3;
            ck3_mods_folder_path = Regex.Replace(ck3_mods_folder_path, @"console_history.txt", "mod");

            string ck3_mods_enabled_file_path = Regex.Replace(ck3_mods_folder_path, "mod", "dlc_load.json");
            string enabled_mods_json_text = File.ReadAllText(ck3_mods_enabled_file_path);
            var json = JsonSerializer.Deserialize<Dictionary<string, string[]>>(enabled_mods_json_text);
            EnabledMods = json.ElementAt(0).Value.ToList();
            for(int i = 0; i < EnabledMods.Count; i++)
            {
                EnabledMods[i] = EnabledMods[i].Replace("mod/", "");
            }
        }

        static List<string> EnabledMods_Folders_Paths = new List<string>();
        static void ReadEachModPath()
        {
            string ck3_mods_folder_path = Properties.Settings.Default.VAR_log_ck3;
            ck3_mods_folder_path = Regex.Replace(ck3_mods_folder_path, @"console_history.txt", "mod");
            var all_mods_paths = Directory.GetFiles(ck3_mods_folder_path);

            
            foreach(var mod_path in all_mods_paths)
            {
                foreach(string enabled_mod in EnabledMods)
                {
                    if(mod_path.Contains(enabled_mod))
                    {
                        using (StreamReader sr = new StreamReader(mod_path))
                        {
                            while(!sr.EndOfStream)
                            {
                                string line = sr.ReadLine();
                                if (line == null) break;

                                if(line.StartsWith("path="))
                                {
                                    string path = Regex.Match(line, @""".+""").Value;

                                    //NON-STEAM VERSION
                                    if (path.StartsWith("mod/"))
                                    {
                                        string non_steam_path;
                                        non_steam_path = ck3_mods_folder_path.Replace("mod", "") + path.Replace('/', '\\');
                                        EnabledMods_Folders_Paths.Add(non_steam_path);
                                        Console.WriteLine(non_steam_path);
                                    }
                                    //STEAM VERSION
                                    else
                                    {
                                        string steam_path;
                                        steam_path = path.Replace('/', '\\');
                                        EnabledMods_Folders_Paths.Add(steam_path);
                                        Console.WriteLine(steam_path);
                                    }


                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }


        static void ReadModsLandedTitles(List<Army> attacker_armies, List<Army> defender_armies)
        {
            foreach(var mod_folder_path in EnabledMods_Folders_Paths)
            {
                string landed_titles_path = mod_folder_path + @"\common\landed_titles\00_landed_titles.txt";
                if (File.Exists(landed_titles_path))
                {
                    ReadLandedTitles(landed_titles_path, attacker_armies, defender_armies);
                }
                else continue;
            }
        }

        static void ReadDefaultCK3LandedTitles(List<Army> attacker_armies, List<Army> defender_armies)
        {
            string ck3_exe_path = Properties.Settings.Default.VAR_ck3_path;
            string default_landed_titles_path = Regex.Replace(ck3_exe_path, @"binaries\\ck3.exe", @"game\\common\\landed_titles\\00_landed_titles.txt");

            if (File.Exists(default_landed_titles_path))
            {
                ReadLandedTitles(default_landed_titles_path, attacker_armies, defender_armies);
            }
        }

        static void ReadLandedTitles(string file_path, List<Army> attacker_armies, List<Army> defender_armies)
        {
            using (StreamReader reader = new StreamReader(file_path))
            {
                string line = reader.ReadLine();
                string county = "";
                bool county_started = false;
                bool barony_started = false;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line == null) break;

                    //County Key
                    if (Regex.IsMatch(line, @"c_.+ = {"))
                    {
                        county = Regex.Match(line, @"(c_.+) = {").Groups[1].Value;
                        county_started = true;
                    }
                    //Barony Key
                    else if(county_started && Regex.IsMatch(line, @"b_.+ = {"))
                    {
                        barony_started = true;
                    }
                    else if (county_started && barony_started && line.Contains("province ="))
                    {
                        string province_id = Regex.Match(line, @"\d+").Value;
                        SetRegimentsCountiesKeys(attacker_armies, county, province_id);
                        SetRegimentsCountiesKeys(defender_armies, county, province_id);

                        barony_started = false;
                    }
                }
                reader.Close();
            }
        }
   

        static void SetRegimentsCountiesKeys(List<Army> armies, string county_key, string province_id)
        {
            //Armies
            for (int i = 0; i < armies.Count; i++)
            {
                //Army Regiments
                for (int x = 0; x < armies[i].ArmyRegiments.Count; x++)
                {
                    //Regiments
                    if (armies[i].ArmyRegiments[x].Regiments != null)
                    {
                        for (int y = 0; y < armies[i].ArmyRegiments[x].Regiments.Count; y++)
                        {
                            //if county key is set, skip
                            if (!string.IsNullOrEmpty(armies[i].ArmyRegiments[x].Regiments[y].GetCountyKey()))
                            {
                                continue;
                            }
                            string origin = armies[i].ArmyRegiments[x].Regiments[y].Origin;

                            //Regiment Province Origin
                            if (province_id == origin && !armies[i].ArmyRegiments[x].Regiments[y].isMercenary())
                            {
                                armies[i].ArmyRegiments[x].Regiments[y].StoreCountyKey(county_key);
                            }
                        }
                    }

                }
            }
        }
    }
}
