﻿using Crusader_Wars.client;
using Crusader_Wars.data.save_file;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace Crusader_Wars.unit_mapper
{
    class TerrainsUM
    {
        string AttilaMap { get; set; }
        List<(string building, string x, string y)> HistoricalMaps { get; set; }
        List<(string terrain, string x, string y)> NormalMaps { get; set; }

        internal TerrainsUM(string attilaMap, List<(string building, string x, string y)> historicalMaps, List<(string terrain, string x, string y)> normalMaps)
        {
            AttilaMap = attilaMap;
            HistoricalMaps = historicalMaps;    
            NormalMaps = normalMaps;
        }

        public string GetAttilaMap() { return AttilaMap; }
        public List<(string building, string x, string y)> GetHistoricalMaps() { return HistoricalMaps; }
        public List<(string terrain, string x, string y)> GetNormalMaps() { return NormalMaps; }

    }
    internal static class UnitMappers_BETA
    {
        /*----------------------------------------------------------------
         * TO DO:
         * House files reader for AGOT
         ----------------------------------------------------------------*/

        public static TerrainsUM Terrains { get;private set; }  
        static string LoadedUnitMapper_FolderPath { get; set; }

        public static string GetLoadedUnitMapperName() { return Path.GetFileName(LoadedUnitMapper_FolderPath); }
        public static string GetLoadedUnitMapperString() { 
            switch(GetLoadedUnitMapperName())
            {
                case "OfficialCW_EarlyMedieval_919Mod":
                    return "EARLY MEDIEVAL";
                case "OfficialCW_HighMedieval_MK1212Mod":
                    return "HIGH MEDIEVAL";
                case "OfficialCW_LateMedieval_MK1212Mod":
                    return "LATE MEDIEVAL";
                case "OfficialCW_Renaissance_MK1212Mod":
                    return "RENAISSANCE";
                case "OfficialCW_FallenEagle_AgeOfJustinian":
                    return "DARK AGES";
                case "OfficialCW_FallenEagle_FallofTheEagle":
                case "OfficialCW_FallenEagle_Fireforged-Empire":
                    return "LATE ANTIQUITY";
                case "OfficialCW_RealmsInExile_TheDawnlessDays":
                    return "SECOND AGE";
                default:
                    return null;
            }
            
        }

        private static void ReadTerrainsFile()
        {
            if(!Directory.Exists($@"{LoadedUnitMapper_FolderPath}\terrains")) { Terrains = null; return; }

            var terrainFiles = Directory.GetFiles($@"{LoadedUnitMapper_FolderPath}\terrains");

            try
            {
                string attilaMap = "";
                var historicMaps = new List<(string building, string x, string y)>();
                var normalMaps = new List<(string terrain, string x, string y)>();

                foreach (var file in terrainFiles)
                {
                    XmlDocument TerrainsFile = new XmlDocument();
                    TerrainsFile.Load(file);
                    foreach (XmlElement Element in TerrainsFile.DocumentElement.ChildNodes)
                    {
                        if (Element.Name == "Attila_Map")
                        {
                            attilaMap = Element.InnerText;
                        }
                        else if (Element.Name == "Historic_Maps")
                        {
                            foreach (XmlElement historic_map in Element.ChildNodes)
                            {
                                string building = historic_map.Attributes["ck3_building_key"].Value;
                                string x = historic_map.Attributes["x"].Value;
                                string y = historic_map.Attributes["y"].Value;
                                historicMaps.Add((building, x, y));
                            }
                        }
                        else if (Element.Name == "Normal_Maps")
                        {
                            foreach (XmlElement terrain_type in Element.ChildNodes)
                            {
                                string terrain = terrain_type.Attributes["ck3_name"].Value;
                                foreach (XmlElement map in terrain_type.ChildNodes)
                                {
                                    string x = map.Attributes["x"].Value;
                                    string y = map.Attributes["y"].Value;
                                    normalMaps.Add((terrain, x, y));

                                }
                            }
                        }
                    }
                }

                Terrains = new TerrainsUM(attilaMap, historicMaps, normalMaps);
            }
            catch
            {
                MessageBox.Show($"Error reading {GetLoadedUnitMapperName()} terrains file!", "Unit Mapper Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

        }

        public static List<string> GetUnitMapperModFromTagAndTimePeriod(string tag)
        {
            var unit_mappers_folder = Directory.GetDirectories(@".\unit mappers");
            List<string> requiredMods = new List<string>();

            foreach (var mapper in unit_mappers_folder)
            {
                string mapperName = Path.GetDirectoryName(mapper);
                var files = Directory.GetFiles(mapper);
                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    if (fileName == "tag.txt")
                    {
                        string fileTag = File.ReadAllText(file);
                        if (tag == fileTag)
                        {
                            // TIME PERIOD
                            int startYear = -1, endYear = -1;
                            bool isDefault = false;
                            string timePeriodPath = mapper + @"\Time Period.xml";
                            if (File.Exists(timePeriodPath))
                            {
                                XmlDocument xmlDocument = new XmlDocument();
                                xmlDocument.Load(timePeriodPath);
                                string startYearStr = xmlDocument.SelectSingleNode("//StartDate").InnerText;
                                string endYearStr = xmlDocument.SelectSingleNode("//EndDate").InnerText;
                                
                                if(startYearStr == "Default" || startYearStr == "DEFAULT")
                                {
                                    isDefault = true;
                                    startYear = 0;
                                    endYear = 0;
                                }

                                if(!int.TryParse(startYearStr, out startYear))
                                {
                                    isDefault = true;
                                    startYear = 0;
                                    endYear = 0;
                                }

                                if (!int.TryParse(endYearStr, out endYear))
                                {
                                    isDefault = true;
                                    startYear = 0;
                                    endYear = 0;
                                }

                                if(startYear != -1 && endYear != -1)
                                {

                                    if((Date.Year >= startYear && Date.Year <= endYear) || isDefault)
                                    {
                                        //  MODS
                                        string modsPath = mapper + @"\Mods.xml";
                                        if (File.Exists(modsPath))
                                        {
                                            XmlDocument xmlMods = new XmlDocument();
                                            xmlMods.Load(modsPath);
                                            foreach (XmlNode node in xmlMods.DocumentElement.ChildNodes)
                                            {
                                                if (node is XmlComment) continue;
                                                if (node.Name == "Mod")
                                                {
                                                    requiredMods.Add(node.InnerText);
                                                }
                                            }

                                            LoadedUnitMapper_FolderPath = mapper;
                                            ReadTerrainsFile();
                                            return requiredMods;
                                        }
                                        else
                                        {
                                            MessageBox.Show($"Mods.xml was not found in {mapper}", "Unit Mappers Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Time Period.xml was not found in {mapper}", "Unit Mappers Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            }
                            break;
                        }
                    }
                }
            }

            return requiredMods;
        }

        struct MaxType
        {
            public static int GetMax(string MaxValue)
            {
                switch (MaxValue)
                {
                    case "INFANTRY":
                        return ModOptions.GetInfantryMax();
                    case "RANGED":
                        return ModOptions.GetRangedMax();
                    case "CAVALRY":
                        return ModOptions.GetCavalryMax();
                    case "LEVY":
                        return ModOptions.GetLevyMax();
                    case "SPECIAL":
                        return 1111;
                    default:
                        if (int.TryParse(MaxValue, out int max_int)) 
                            return max_int;
                        else
                            return ModOptions.GetInfantryMax();

                }
            }
        };
        public static int GetMax(Unit unit)
        {
            string factions_folder_path = LoadedUnitMapper_FolderPath + @"\Factions";
            var files_paths = Directory.GetFiles(factions_folder_path);

            int max = 0;
            foreach (var xml_file in files_paths)
            {
                if (Path.GetExtension(xml_file) == ".xml")
                {
                    XmlDocument FactionsFile = new XmlDocument();
                    FactionsFile.Load(xml_file);

                    foreach (XmlNode element in FactionsFile.DocumentElement.ChildNodes)
                    {
                        if (element is XmlComment) continue;
                        string faction = element.Attributes["name"].Value;

                        //Store Default unit max first
                        if (faction == "Default" || faction == "DEFAULT")
                        {
                            foreach (XmlNode node in element.ChildNodes)
                            {
                                if (node is XmlComment) continue;
                                if (node.Name == "General" || node.Name == "Knights") continue;

                                if (node.Name == "Levies" &&
                                    node.Attributes["max"] != null)
                                {
                                    if (unit.GetRegimentType() == RegimentType.Levy)
                                    {
                                        max = MaxType.GetMax(node.Attributes["max"].Value);
                                        continue;
                                    }
                                    else
                                        continue;
                                }

                                if (node.Attributes["type"].Value == unit.GetName())
                                {
                                    max = MaxType.GetMax(node.Attributes["max"].Value);
                                }
                            }
                        }
                        //Then stores culture specific unit max
                        else if (faction == unit.GetAttilaFaction())
                        {
                            foreach (XmlNode node in element.ChildNodes)
                            {
                                if (node is XmlComment) continue;
                                if (node.Name == "General" || node.Name == "Knights") continue;

                                if (node.Name == "Levies")
                                {
                                    if(unit.GetRegimentType() == RegimentType.Levy && node.Attributes["max"] != null) {
                                        max = MaxType.GetMax(node.Attributes["max"].Value); 
                                        continue;
                                    }
                                    else
                                        continue;
                                }

                                if (node.Attributes["type"].Value == unit.GetName())
                                {
                                    if(node.Attributes["max"] != null)
                                    {
                                        max = MaxType.GetMax(node.Attributes["max"].Value);
                                    }
                                    else
                                    {
                                        break;
                                    }   
                                }
                            }
                        }
                    }
                }
            }

            return max;
        }

        static List<(int porcentage, string unit_key, string name, string max)> Levies(XmlDocument factions_file, string attila_faction)
        {
            var levies_nodes = factions_file.SelectNodes($"/FactionsGroups/Faction[@name=\"{attila_faction}\"]/Levies");
            List<(int porcentage, string unit_key, string name, string max)> list = new List<(int porcentage, string unit_key, string name, string max)>();

            if (levies_nodes.Count == 0) 
                return list;


            foreach (XmlNode levies_node in levies_nodes)
            {
                int porcentage = Int32.Parse(levies_node.Attributes["porcentage"].Value);
                string key = levies_node.Attributes["key"].Value;
                string name = $"Levy_{porcentage}";
                string max = MaxType.GetMax("LEVY").ToString();
                if (levies_node.Attributes["max"] != null)
                    max = MaxType.GetMax(levies_node.Attributes["max"].Value).ToString();

                list.Add((porcentage, key, name, max));
            }

            return list;
        }


        public static List<(int porcentage, string unit_key, string name, string max)> GetFactionLevies(string attila_faction)
        {
            string factions_folder_path = LoadedUnitMapper_FolderPath + @"\Factions";
            var files_paths = Directory.GetFiles(factions_folder_path);
            var levies = new List<(int porcentage, string unit_key, string name, string max)>();
            foreach (var xml_file in files_paths)
            {
                if (Path.GetExtension(xml_file) == ".xml")
                {
                    XmlDocument FactionsFile = new XmlDocument();
                    FactionsFile.Load(xml_file);
                    if (levies.Count == 0)
                        levies = Levies(FactionsFile, attila_faction);
                    else
                        return levies;

                }
            }


            return levies;


        }



        static string SearchInTitlesFile(Unit unit)
        {
            string titles_folder_path = LoadedUnitMapper_FolderPath + @"\Titles";
            if (!Directory.Exists(titles_folder_path)) return "";
            var files_paths = Directory.GetFiles(titles_folder_path);

            if(unit.GetOwner() == null || unit.GetOwner().GetPrimaryTitleKey() == string.Empty)
                return string.Empty;
            
            //LEVIES skip
            if (unit.GetRegimentType() == RegimentType.Levy) return "";

            string unit_key = "";
            foreach (var xml_file in files_paths)
            {
                if (Path.GetExtension(xml_file) == ".xml")
                {
                    XmlDocument TitlesFile = new XmlDocument();
                    TitlesFile.Load(xml_file);

                    //MAA|COMMANDER|KNIGHT
                    foreach (XmlNode element in TitlesFile.DocumentElement.ChildNodes)
                    {
                        if (element as XmlNode is XmlComment) continue;
                        string titleKey = element.Attributes["title_key"].Value;

                        //Then stores culture specific unit key
                        if (titleKey == unit.GetOwner().GetPrimaryTitleKey())
                        {
                            foreach (XmlNode node in element.ChildNodes)
                            {
                                unit_key = FindUnitKeyInFaction(element, unit);
                                return unit_key;
                            }
                        }
                    }
                }
            }

            return unit_key;
        }

        static string SearchInFactionFiles(Unit unit)
        {
            string factions_folder_path = LoadedUnitMapper_FolderPath + @"\Factions";
            var files_paths = Directory.GetFiles(factions_folder_path);

            //LEVIES skip
            if (unit.GetRegimentType() == RegimentType.Levy) return "" ;

            string unit_key = "";
            foreach (var xml_file in files_paths)
            {
                if (Path.GetExtension(xml_file) == ".xml")
                {
                    XmlDocument FactionsFile = new XmlDocument();
                    FactionsFile.Load(xml_file);

                    //MAA|COMMANDER|KNIGHT
                    foreach (XmlNode element in FactionsFile.DocumentElement.ChildNodes)
                    {
                        if (element is XmlComment) continue;
                        if(element is XmlElement)
                        {
                            string faction = element.Attributes["name"].Value;

                            //Store Default unit key first
                            if (faction == "Default" || faction == "DEFAULT")
                            {
                                unit_key = FindUnitKeyInFaction(element, unit);
                            }
                            //Then stores culture specific unit key
                            else if (faction == unit.GetAttilaFaction())
                            {
                                string foundKey = FindUnitKeyInFaction(element, unit);
                                if (!string.IsNullOrEmpty(foundKey))
                                    return foundKey;
                            }
                        }
                    }

                    if (unit_key == string.Empty)
                        continue;
                    else
                        return unit_key;
                }
            }

            return unit_key;
        }

        static string FindUnitKeyInFaction(XmlNode factionElement, Unit unit)
        {
            string unit_key = "";
            foreach (XmlNode node in factionElement.ChildNodes)
            {
                if (node is XmlComment) continue;
                if (node.Name == "Levies") continue;

                //General
                if (node.Name == "General" && unit.GetRegimentType() == RegimentType.Commander)
                {
                    unit_key = node.Attributes["key"].Value;
                    return unit_key;
                }
                //Knights
                else if (node.Name == "Knights" && unit.GetRegimentType() == RegimentType.Knight)
                {
                    unit_key = node.Attributes["key"].Value;
                    return unit_key;
                }
                //MenAtArms
                else if (node.Name == "MenAtArm" && unit.GetRegimentType() == RegimentType.MenAtArms)
                {
                    if (node.Attributes["type"].Value == unit.GetName())
                    {
                        unit_key = node.Attributes["key"].Value;
                        return unit_key;
                    }
                }
            }

            return unit_key;
        }


        public static string GetUnitKey(Unit unit)
        {
            string unit_key = SearchInTitlesFile(unit);
            if (!string.IsNullOrEmpty(unit_key))
            {
                return unit_key;
            }
            else
            {
                unit_key = SearchInFactionFiles(unit);
                return unit_key;
            }
                

        }

        public static string GetAttilaFaction(string CultureName, string HeritageName)
        {
            string faction = "";
            string cultures_folder_path = LoadedUnitMapper_FolderPath + @"\Cultures";
            
            var files_paths = Directory.GetFiles(cultures_folder_path);
            foreach (var xml_file in files_paths)
            {
                if(Path.GetExtension(xml_file) == ".xml")
                {
                    XmlDocument CulturesFile = new XmlDocument();
                    CulturesFile.Load(xml_file);

                    foreach(XmlNode heritage in CulturesFile.DocumentElement.ChildNodes)
                    {
                        if (heritage is XmlComment) continue;

                        string heritage_name = heritage.Attributes["name"].Value;                       

                        if(heritage_name == HeritageName)
                        {
                            faction = heritage.Attributes["faction"].Value;
                        }

                        foreach(XmlNode culture in heritage.ChildNodes)
                        {
                            if (culture is XmlComment) continue; 
                            string culture_name = culture.Attributes["name"].Value;

                            if (culture_name == CultureName)
                            {
                                faction = culture.Attributes["faction"].Value;
                            }
                        }
                    }
                }
            }

            return faction;
        }

        public static void SetMapperImage()
        {

            try
            {
                //Copy mapper image to files
                var image_path = Directory.GetFiles(LoadedUnitMapper_FolderPath).Where(x => x.EndsWith(".png")).FirstOrDefault();
                string destination_path = Directory.GetCurrentDirectory() + @"\data\battle Files\campaign_maps\main_attila_map\main_attila_map.png";
                File.Copy(image_path, destination_path, true);
                return;
            }
            catch
            {
                //In case of error, use default image

                string default_image_path = Directory.GetCurrentDirectory() + "\\settings\\main_attila_map.png";
                string destination_path = Directory.GetCurrentDirectory() + @"\data\battle Files\campaign_maps\main_attila_map\main_attila_map.png";
                File.Copy(default_image_path, destination_path, true);
                return;
            }


        }
    }
}
