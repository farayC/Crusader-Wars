using Crusader_Wars.client;
using Crusader_Wars.client.RequiredMods;
using Crusader_Wars.data.save_file;

using Crusader_Wars.terrain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using System.Data.SqlTypes;

namespace Crusader_Wars
{
    public static class UnitMapper
    {
        public static string LoadedMapper { get; private set; }
        public static List<(string Heritage, string Faction)> Heritages { get; private set; }
        public static List<(string Cultures, string Faction)> Cultures { get; private set; }

        public static List<(string Type, string Key, int Max, string Script)> PlayerUnits { get; private set; }

        public static List<(string Type, string Key, int Max, string Script)> EnemyUnits { get; private set; }

        public static string Attila_Map { get; private set; }
        public static List<(string building, string x, string y)> Historic_Maps { get; private set; }
        public static List<(string terrain, string x, string y)> Normal_Maps { get;  set; }

        private static List<string> CheckedMappers { get; set; }

        //Function to read activated mappers
        private static void ReadCheckedMappers()
        {
            try
            {
                string filePath = @".\Settings\lastchecked.txt";
                //Read checked unit mappers file
                using (StreamReader reader = new StreamReader(filePath))
                {
                    var all = File.ReadAllLines(filePath);
                    CheckedMappers = new List<string>();

                    //For each checked unit mapper on the file
                    foreach (var line in all)
                    {
                        if (line != null)
                        {
                            //Add to list
                            CheckedMappers.Add(line);
                        }
                    }
                    reader.Close();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred while reading to the file:");
                Console.WriteLine(e.Message);
            }
        }

        //Load the Cultures and Default Units from a Mapper
        public static void LoadMapper()
        {
            ReadCheckedMappers();

            if (CheckedMappers.Count == 1)
            {
                //Only one selected mapper
                //load only mapper
                LoadSingleMapper(CheckedMappers[0]);
            }
            else if (CheckedMappers.Count > 1)
            {
                //Multiple selected mappers
                //select one by time period
                LoadMapperByTimePeriod();
            }
            else
            {
                //Zero selected mappers
                //load default one
                LoadDefaultMapper();
            }


        }

        static void ReadMapperImage(string folderName)
        {

            try
            {
                //Copy mapper image to files

                string mapper_path = Directory.GetCurrentDirectory() + $"\\Mappers\\{folderName}";
                var image_path = Directory.GetFiles(mapper_path).Where(x => x.EndsWith(".png")).FirstOrDefault();
                string destination_path = Directory.GetCurrentDirectory() + @"\Battle Files\campaign_maps\main_attila_map\main_attila_map.png";
                File.Copy(image_path, destination_path, true);
                return;
            }
            catch
            {
                //In case of error, use default image

                string default_image_path = Directory.GetCurrentDirectory() + "\\Settings\\main_attila_map.png";
                string destination_path = Directory.GetCurrentDirectory() + @"\Battle Files\campaign_maps\main_attila_map\main_attila_map.png";
                File.Copy(default_image_path, destination_path, true);
                return;
            }


        }

        //Load a specific Faction units on top of the Default ones
        static XmlDocument[] MapperFactionFiles { get; set; }


        private static void LoadSingleMapper(string folderName)
        {

            //Cultures & Heritages
            ReadCulturesFile(folderName);

            //Factions
            ReadFactionsFile(folderName);

            //Terrains
            ReadTerrainsFile(folderName);

            //Required Mods Message Box
            ReadRequiredModsFile(folderName);

            //Image background
            ReadMapperImage(folderName);
            folder = folderName;

            LoadedMapper = folderName;
        }
        private static void LoadMapperByTimePeriod()
        {
            foreach(var folderName in CheckedMappers)
            {
                string timeperiod_path = GetXmlFilePath(folderName, FileType.TimePeriod())[0];
                XmlDocument TimePeriodFile = new XmlDocument();
                TimePeriodFile.Load(timeperiod_path);

                XmlNode startDateNode = TimePeriodFile.SelectSingleNode("/TimePeriod/StartDate");
                XmlNode endDateNode = TimePeriodFile.SelectSingleNode("/TimePeriod/EndDate");

                if (startDateNode != null && endDateNode != null)
                {
                    //Load default Mapper
                    if(startDateNode.InnerText == "DEFAULT" || endDateNode.InnerText == "DEFAULT") 
                    {
                        LoadSingleMapper(folderName);
                        return;
                    }
                    //Load Mapper based on year
                    else
                    {
                        int StartDate = Int32.Parse(startDateNode.InnerText);
                        int EndDate = Int32.Parse(endDateNode.InnerText);

                        if (Date.Year >= StartDate && Date.Year <= EndDate)
                        {
                            LoadSingleMapper(folderName);
                            return;
                        }
                    }

                }

            }
            try { throw new NotImplementedException(); }
            catch
            {
                MessageBox.Show("None of the Unit Mappers enabled is for your time period! Enable one and try again!", "No Mappers Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

        }

        private static List<string> RequiredMods { get; set; }
        private static void ReadRequiredModsFile(string folderName)
        {
            string mods_path;
            try
            {
                //Get path if exists
                mods_path = GetXmlFilePath(folderName, FileType.Mods())[0];
            }
            catch
            {
                //If it's an error, value should be empty
                mods_path = "";
            }
            

            if(File.Exists(mods_path))
            {
                XmlDocument ModsFile = new XmlDocument();
                ModsFile.Load(mods_path);

                RequiredMods = new List<string>();
                foreach (XmlElement mod in ModsFile.DocumentElement.ChildNodes)
                {
                    var name = mod.InnerText;
                    RequiredMods.Add(name);
                }
            }
            else
            {
                RequiredMods = null;
            }
        }

        static string folder;
        public async static void ShowRequiredMods()
        {
            if(RequiredMods != null)
            {
               StringBuilder sb = new StringBuilder();
               foreach(string mod in RequiredMods)
                {
                    sb.AppendLine(mod);
                }
               string mods = sb.ToString();

                await Task.Delay(1);

                //MessageBox.Show($"Loaded Mapper: {folder}\nRequired Mods:\n" + mods, "Load these required mods!");
                RequiredModsMessage.ShowRequiredMods(folder, mods);
            }
            
        }

        private static void LoadDefaultMapper()
        {
            //Read the first one that finds
            try
            {
                var path = Directory.GetCurrentDirectory() + @"\Mappers";
                var folder = Directory.GetDirectories(path)[0];
                LoadSingleMapper(folder);
            }
            catch 
            {
                MessageBox.Show("Zero Unit Mappers on the folder!", "No Mappers Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

            
        }


        private static void ReadTerrainsFile(string folderName)
        {
            List<string> terrains_files;

            //if file exists
            try
            {
                terrains_files = GetXmlFilePath($"{folderName}\\terrains", FileType.Terrains());
            }
            catch
            {
                return;
            }
            


            try
            {
                

                Attila_Map = "";
                Historic_Maps = new List<(string building, string x, string y)>();
                Normal_Maps = new List<(string terrain, string x, string y)>();

                foreach (var file in terrains_files)
                {
                    XmlDocument TerrainsFile = new XmlDocument();
                    TerrainsFile.Load(file);
                    foreach (XmlElement Element in TerrainsFile.DocumentElement.ChildNodes)
                    {
                         if(Element.Name == "Attila_Map")
                         {
                            string map = Element.InnerText;
                            Attila_Map = map;
                         }
                         else if (Element.Name == "Historic_Maps")
                         {
                            foreach(XmlElement historic_map in Element.ChildNodes)
                            {
                                string building = historic_map.Attributes["ck3_building_key"].Value;
                                string x = historic_map.Attributes["x"].Value;
                                string y = historic_map.Attributes["y"].Value;
                                Historic_Maps.Add((building, x, y));
                            }
                         }
                         else if(Element.Name == "Normal_Maps")
                         {
                            foreach(XmlElement terrain_type in Element.ChildNodes)
                            {
                                string terrain = terrain_type.Attributes["ck3_name"].Value;
                                foreach(XmlElement map in terrain_type.ChildNodes)
                                {
                                    string x = map.Attributes["x"].Value;
                                    string y = map.Attributes["y"].Value;
                                    Normal_Maps.Add((terrain, x, y));

                                }
                            }
                         }
                    }
                }
            }
            catch
            {
                MessageBox.Show($"Error reading {folderName} terrains file!", "Unit Mapper Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

        }


        private static void ReadCulturesFile(string folderName)
        {
            try
            {
                var cultures_files = GetXmlFilePath(folderName+@"\Cultures", FileType.Culture());
                Heritages = new List<(string Heritage, string Faction)>();
                Cultures = new List<(string Heritage, string Faction)>();
                foreach (var file in cultures_files)
                {
                    XmlDocument CulturesFile = new XmlDocument();
                    CulturesFile.Load(file);
                    foreach (XmlElement Heritage in CulturesFile.DocumentElement.ChildNodes)
                    {
                        Heritages.Add((Heritage.Attributes["name"].Value, Heritage.Attributes["faction"].Value));

                        foreach (XmlNode Culture in Heritage)
                        {
                            Cultures.Add((Culture.Attributes["name"].Value, Culture.Attributes["faction"].Value));
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show($"Error reading {folderName} cultural file!", "Unit Mapper Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

        }

        private static void ReadFactionsFile(string folderName)
        {

            try
            {
                var factions_files = GetXmlFilePath(folderName+@"\Factions", FileType.Factions());
                PlayerUnits = new List<(string Type, string Key, int Max, string Script)>();
                EnemyUnits = new List<(string Type, string Key, int Max, string Script)>();
                var files_list = new List<XmlDocument>();
                foreach (var file in factions_files)
                {
                    XmlDocument FactionsFile = new XmlDocument();
                    FactionsFile.Load(file);
                    files_list.Add(FactionsFile);

                    foreach (XmlElement Faction in FactionsFile.DocumentElement.ChildNodes)
                    {
                        //Default Units
                        if (Faction.Attributes["name"].Value is "Default")
                        {
                            foreach (XmlNode Unit in Faction.ChildNodes)
                            {
                                if (Unit.Name is "General" || Unit.Name is "Knights")
                                {
                                    int max_g = MaxType.GetMax(Unit.Attributes["max"].Value);
                                    string key_g = TrimKey(Unit.Attributes["key"].Value);
                                    PlayerUnits.Add((Unit.Name, key_g, max_g, Unit.Attributes["script"].Value));
                                    EnemyUnits.Add((Unit.Name, key_g, max_g, Unit.Attributes["script"].Value));
                                    continue;
                                }

                                int max = MaxType.GetMax(Unit.Attributes["max"].Value);
                                string key = TrimKey(Unit.Attributes["key"].Value);
                                PlayerUnits.Add((Unit.Attributes["type"].Value, key, max, Unit.Attributes["script"].Value));
                                EnemyUnits.Add((Unit.Attributes["type"].Value, key, max, Unit.Attributes["script"].Value));
                            }
                        }
                    }
                }

                MapperFactionFiles = files_list.ToArray();

            }
            catch
            {
                MessageBox.Show($"Error reading {folderName} faction file!", "Unit Mapper Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }


        }

        //Removes whitespaces from unit keys to prevent Attila Crashes
        static string TrimKey(string key)
        {
            string trimmed = string.Concat(key.Where(c => !char.IsWhiteSpace(c)));
            return trimmed;
        }
        



        public static List<string> GetXmlFilePath(string folderName, string fileType)
        {
            string mapper_path = Directory.GetCurrentDirectory() + $@"\unit mappers\{folderName}";
            string languages_path = $@".\Languages\{Languages.Language}";
            var files = Directory.GetFiles(mapper_path, "*.xml");
            switch (fileType)
            {
                //Search for Cultures Files
                case "Cultures":
                    //For each xml file in Unit Mapper folder
                    List<string> cultures_files = new List<string>();
                    foreach (var xmlfile in files)
                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.Load(xmlfile);

                        //If xml file is for cultures
                        if (xmldoc.DocumentElement.Name == "Cultures")
                        {
                            //Return file path
                            cultures_files.Add(xmlfile);
                        }
                        else { continue; }
                    }
                    return cultures_files;
                //Search for Factions File
                case "Factions":
                    //For each xml file in Unit Mapper folder
                    List<string> factions_files = new List<string>();
                    foreach (var xmlfile in files)
                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.Load(xmlfile);

                        //If xml file is for factions
                        if (xmldoc.DocumentElement.Name == "FactionsGroups")
                        {
                            //Return file path
                            factions_files.Add(xmlfile);
                        }
                        else { continue; }
                    }
                    return factions_files;
                //Search for TimePeriod File
                case "TimePeriod":
                    //For each xml file in Unit Mapper folder
                    List<string> timeperiod_file = new List<string>();
                    foreach (var xmlfile in files)
                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.Load(xmlfile);

                        //If xml file is for time period
                        if (xmldoc.DocumentElement.Name == "TimePeriod")
                        {
                            //Return file path
                            timeperiod_file.Add(xmlfile);
                            return timeperiod_file;
                        }
                        else { continue; }
                    }
                    break;

                //Search for RequiredMods File
                case "RequiredMods":
                    //For each xml file in Unit Mapper folder
                    List<string> mods_file = new List<string>();
                    foreach (var xmlfile in files)
                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.Load(xmlfile);

                        //If xml file is for time period
                        if (xmldoc.DocumentElement.Name == "RequiredMods")
                        {
                            //Return file path
                            mods_file.Add(xmlfile);
                            return mods_file;
                        }
                        else { continue; }
                    }
                    break;
                case "Terrains":
                    //For each xml file in Unit Mapper folder
                    List<string> terrains_file = new List<string>();
                    foreach (var xmlfile in files)
                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.Load(xmlfile);

                        //If xml file is for time period
                        if (xmldoc.DocumentElement.Name == "Terrains")
                        {
                            //Return file path
                            terrains_file.Add(xmlfile);
                            return terrains_file;
                        }
                        else { continue; }
                    }
                    break;
            }

            return new List<string>();
        }

        public static void ClearData()
        {
            PlayerUnits = new List<(string Type, string Key, int Max, string Script)>();
            EnemyUnits = new List<(string Type, string Key, int Max, string Script)> ();

            Heritages = new List<(string Heritage, string Faction)>();
            Cultures = new List<(string Cultures, string Faction)> ();

            CheckedMappers = new List<string>();
            RequiredMods = new List<string>();

        }


 

        struct FileType
        {
            public static string Culture() { return "Cultures"; }
            public static string Factions() { return "Factions"; }
            public static string TimePeriod() { return "TimePeriod"; }
            public static string Mods() { return "RequiredMods"; }
            public static string Terrains() { return "Terrains"; }
        };

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
                        if(int.TryParse(MaxValue, out int max_int)) return max_int;
                        return ModOptions.GetInfantryMax();

                }
            }
        };

    }
}
