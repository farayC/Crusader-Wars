using Crusader_Wars.client;
using Crusader_Wars.data.save_file;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;

namespace Crusader_Wars.unit_mapper
{
    internal static class UnitMappers_BETA
    {
        /*----------------------------------------------------------------
         * TO DO:
         * Enable by year;
         * Terrains files reader ;
         ----------------------------------------------------------------*/


        static List<string> UnitMappers_FolderPaths { get; set; }
        static string LoadedUnitMapper_FolderPath { get; set; }
        public static void ReadUnitMappers()
        {
            UnitMappers_FolderPaths = new List<string>();
            LoadedUnitMapper_FolderPath = "";
            LoadUnitMapper();

        }

        static void LoadUnitMapper ()
        {
            ReadEnabledMappersFile();

            //One Unit Mapper enabled
            if (UnitMappers_FolderPaths.Count == 1)
            {
                LoadedUnitMapper_FolderPath = UnitMappers_FolderPaths[0];
            }
            //Multiple Unit Mappers enabled
            else if (UnitMappers_FolderPaths.Count > 1)
            {
                //TO DO: load correct based on time period
            }
            //No Unit Mapper enabled
            else
            {

            }
        }

        /*
         * Read all activated unit mappers
         */
        static void ReadEnabledMappersFile()
        {

            using (StreamReader sr = new StreamReader(@".\settings\lastchecked.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line == null) break;
                    UnitMappers_FolderPaths.Add(@".\unit mappers\" + line);
                }
            }
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
                        if (int.TryParse(MaxValue, out int max_int)) return max_int;
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

        static List<(int porcentage, string unit_key, string name)> Levies(XmlDocument factions_file, string attila_faction)
        {
            var levies_nodes = factions_file.SelectNodes($"/FactionsGroups/Faction[@name=\"{attila_faction}\"]/Levies");
            List<(int porcentage, string unit_key, string name)> list = new List<(int porcentage, string unit_key, string name)>();

            if (levies_nodes.Count == 0) 
                return list;


            foreach (XmlNode levies_node in levies_nodes)
            {
                int porcentage = Int32.Parse(levies_node.Attributes["porcentage"].Value);
                string key = levies_node.Attributes["key"].Value;
                string name = $"Levy_{porcentage}";
                list.Add((porcentage, key, name));
            }

            return list;
        }


        public static List<(int porcentage, string unit_key, string name)> GetFactionLevies(string attila_faction)
        {
            string factions_folder_path = LoadedUnitMapper_FolderPath + @"\Factions";
            var files_paths = Directory.GetFiles(factions_folder_path);
            var levies = new List<(int porcentage, string unit_key, string name)>();
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



        public static string GetUnitKey(Unit unit)
        {
            string factions_folder_path = LoadedUnitMapper_FolderPath + @"\Factions";
            var files_paths = Directory.GetFiles(factions_folder_path);

            string unit_key = "";
            foreach (var xml_file in files_paths)
            {
                if (Path.GetExtension(xml_file) == ".xml")
                {
                    XmlDocument FactionsFile = new XmlDocument();
                    FactionsFile.Load(xml_file);

                    //LEVIES
                    if (unit.GetRegimentType() == RegimentType.Levy) continue;



                    //MAA|COMMANDER|KNIGHT
                    foreach (XmlNode element in FactionsFile.DocumentElement.ChildNodes)
                    {
                        if (element is XmlComment) continue;
                        string faction = element.Attributes["name"].Value;

                        //Store Default unit key first
                        if(faction == "Default" || faction == "DEFAULT")
                        {
                            foreach (XmlNode node in element.ChildNodes)
                            {
                                if (node is XmlComment) continue;
                                if (node.Name == "General" || node.Name == "Levies") continue;

                                //Knights
                                if (node.Name == "Knights" && unit.GetRegimentType() == RegimentType.Knight)
                                {
                                    unit_key = node.Attributes["key"].Value;
                                }

                                if(node.Name == "MenAtArm")
                                {
                                    if (node.Attributes["type"].Value == unit.GetName() && unit.GetRegimentType() == RegimentType.MenAtArms)
                                    {
                                        unit_key = node.Attributes["key"].Value;
                                    }
                                }

                            }
                        }
                        //Then stores culture specific unit key
                        else if (faction == unit.GetAttilaFaction())
                        {
                            foreach (XmlNode node in element.ChildNodes)
                            {
                                if (node is XmlComment) continue;
                                if (node.Name == "General" || node.Name == "Levies") continue;
                               
                                //Knights
                                if(node.Name == "Knights"  && unit.GetRegimentType() == RegimentType.Knight)
                                {
                                    unit_key = node.Attributes["key"].Value;
                                }

                                //MAA
                                if (node.Name == "MenAtArm")
                                {
                                    if (node.Attributes["type"].Value == unit.GetName() && unit.GetRegimentType() == RegimentType.MenAtArms)
                                    {
                                        unit_key = node.Attributes["key"].Value;
                                    }
                                }

                            }
                        }    
                    }
                }
            }

            return unit_key;
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
    }
}
