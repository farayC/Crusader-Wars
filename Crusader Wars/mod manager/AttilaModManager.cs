using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Crusader_Wars.mod_manager
{
    enum ModLocalization
    {
        Steam,
        Data
    };
    class Mod
    {
        bool Enabled { get; set; }
        Bitmap Image { get; set; }
        string Name { get; set; }
        ModLocalization Localization { get; set; }
        string FullPath {  get; set; }
        public Mod(bool isEnabled, Bitmap pngImg, string name, ModLocalization local) { 
            Enabled = isEnabled;
            Image = pngImg;
            Name = name;
            Localization = local;
        }
        public Mod(bool isEnabled, Bitmap pngImg, string name, ModLocalization local, string fullPath)
        {
            Enabled = isEnabled;
            Image = pngImg;
            Name = name;
            Localization = local;
            FullPath = fullPath;
        }
        public void StoreFullPath(string path) { FullPath =  path; }
        public void ChangeEnabledState(bool yn) {  Enabled = yn; }

        public bool IsEnabled() { return Enabled; }
        public Bitmap GetThumbnail() { return Image; }
        public string GetName() { return Name; }
        public ModLocalization GetLocalization() { return Localization; }
        public string GetFullPath() { return FullPath; }
    }

    public static class AttilaModManager
    {
        static DataGridView ModManagerControl { get; set; }
        public static void SetControlRefence(DataGridView t) { ModManagerControl = t; }

        static List<Mod> ModsPaths { get; set; }

        static void RemoveRequiredMods()
        {
            string[] unitMappers_folders = Directory.GetDirectories(@".\unit mappers\");
            ModsPaths.RemoveAll(x => x.GetName() == "CrusaderWars.pack");

            foreach (var mapper in unitMappers_folders)
            {
                string[] files  = Directory.GetFiles(mapper);
                foreach(var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    if(fileName == "Mods.xml")
                    {
                        XmlDocument ModsFile = new XmlDocument();
                        ModsFile.Load(file);
                        foreach(XmlNode modNode in ModsFile.DocumentElement.ChildNodes)
                        {
                            ModsPaths.RemoveAll(x => x.GetName() == modNode.InnerText);
                        }                        
                    }
                }
            }
        }

        public static void CreateUserModsFile()
        {
            // Create essential file to open Attila.exe automatically
            string steam_app_id_path = Properties.Settings.Default.VAR_attila_path.Replace("Attila.exe", "steam_appid.txt");
            if (!File.Exists(steam_app_id_path))
            {
                File.WriteAllText(steam_app_id_path, "325610");
            }

            string userMods_path = Properties.Settings.Default.VAR_attila_path.Replace("Attila.exe", "used_mods_cw.txt");
            string[] workingDirectories = null;
            string[] steamModNames = null;
            string[] dataModNames = null;

            //Working Directories
            var steamMods = ModsPaths.Where(x => x.GetLocalization() == ModLocalization.Steam && x.IsEnabled()).ToList();
            if(steamMods != null)
            {
                workingDirectories = steamMods.Select(x => x.GetFullPath()).ToArray();
                steamModNames = steamMods.Select(x => x.GetName()).ToArray();
            }

            //Data Mods
            var dataMods = ModsPaths.Where(x => x.GetLocalization() == ModLocalization.Data && x.IsEnabled()).ToList();
            if(dataMods != null)
            {
                dataModNames = dataMods.Select(x => x.GetName()).ToArray();
            }
                       
            if(!File.Exists(userMods_path)) { File.Create(userMods_path); }
            using(StreamWriter sw = new StreamWriter(userMods_path))
            {
                sw.NewLine = "\n";
                foreach(string wD  in workingDirectories)
                {
                    string t = wD.Replace(@"\", @"/");
                    sw.WriteLine($"add_working_directory \"{t}\";");
                }

                sw.WriteLine($"mod \"CrusaderWars.pack\";");

                foreach (string mod in dataModNames)
                {
                    sw.WriteLine($"mod \"{mod}\";");
                }
                foreach (string mod in steamModNames)
                {
                    sw.WriteLine($"mod \"{mod}\";");
                }

                sw.Close();
            };

        }

        public static void ReadInstalledMods()
        {
            string data_folder_path = Properties.Settings.Default.VAR_attila_path.Replace("Attila.exe", @"data\");
            string workshop_folder_path = Properties.Settings.Default.VAR_attila_path.Replace(@"common\Total War Attila\Attila.exe", @"workshop\content\325610\");

            ModsPaths = new List<Mod>();
            Bitmap null_img = new Bitmap(@".\data\mod manager\noimage.png");
            Bitmap img1 = null;

            //Read data folder
            var dataModsPaths = Directory.GetFiles(data_folder_path);
            foreach(var file in dataModsPaths)
            {
                var fileName = Path.GetFileName(file);
                if(Path.GetExtension(fileName) == ".pack")
                {
                    // Skip Attila Packs
                    if(fileName == "belisarius.pack" ||
                       fileName == "boot.pack" ||
                       fileName == "charlemagne.pack"||
                       fileName == "data.pack" ||
                       fileName == "local_en.pack" ||
                       fileName == "local_en_shared_rome2.pack" ||
                       fileName == "models.pack" ||
                       fileName == "models1.pack" ||
                       fileName == "models2.pack" ||
                       fileName == "models3.pack" ||
                       fileName == "movies.pack" ||
                       fileName == "music.pack" ||
                       fileName == "music_en_shared_rome2.pack" ||
                       fileName == "slavs.pack" ||
                       fileName == "sound.pack" ||
                       fileName == "terrain.pack" ||
                       fileName == "terrain2.pack" ||
                       fileName == "tiles.pack" ||
                       fileName == "tiles2.pack" ||
                       fileName == "tiles3.pack" ||
                       fileName == "tiles4.pack" ||
                       fileName == "blood.pack")
                    {
                        continue;
                    }
                    else
                    {
                        ModsPaths.Add(new Mod(false, null_img, fileName, ModLocalization.Data));
                    }
                }
            }

            // Read steam workshop folder
            if(Directory.Exists(workshop_folder_path))
            {
                var steamModsFoldersPaths = Directory.GetDirectories(workshop_folder_path);
                foreach (var folder in steamModsFoldersPaths)
                {
                    var files = Directory.GetFiles(folder);
                    string name = "";
                    Bitmap bmp = null;
                    string fullPath = "";
                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileName(file);

                        if (Path.GetExtension(fileName) == ".pack")
                        {
                            name = fileName;
                            fullPath = file.Replace($@"\{fileName}", "");
                        }
                        else if (Path.GetExtension(fileName) == ".png")
                        {
                            bmp = new Bitmap(file);
                        }
                    }
                    if(name != string.Empty)
                        ModsPaths.Add(new Mod(false, bmp, name, ModLocalization.Steam, fullPath)); 
                            

                }
            }

            //  REMOVE REQUIRED MODS
            //  to only show optional mods
            RemoveRequiredMods();


            //SET ACTIVE MODS
            SetActiveMods();


            //  SET AT MOD MANAGER
            foreach (var mod in ModsPaths)
            {
                if(mod.GetLocalization() == ModLocalization.Steam)
                    img1 = new Bitmap(@".\data\mod manager\steamlogo.png");
                else
                    img1 = new Bitmap(@".\data\mod manager\folder.png");

                ModManagerControl.Rows.Add(mod.IsEnabled(), mod.GetThumbnail(), mod.GetName(), img1);
            }
        }


        public static void SaveActiveMods()
        {
            var activeMods = ModsPaths.Where(mod => mod.IsEnabled()).Select(x => x.GetName()).ToArray();
            File.WriteAllLines(@".\data\mod manager\active_mods.txt", activeMods);
        }

        public static void ChangeEnabledState(DataGridViewRow row)
        {
            string checkboxValue = (String)row.Cells[0].Value;
            bool value;
            if (checkboxValue == "Active")
                value = true;
            else
                value = false;
                
            string name = (String)row.Cells[2].Value;

            ModsPaths.FirstOrDefault(x => x.GetName() == name)?.ChangeEnabledState(value);
        }

        static void SetActiveMods()
        {
            var activeMods = File.ReadAllLines(@".\data\mod manager\active_mods.txt").ToList();
            foreach(Mod mod in ModsPaths) { 
                string name = mod.GetName();
                foreach(var x in activeMods) { 
                    if(name == x)
                    {
                        mod.ChangeEnabledState(true);
                        break;
                    }
                }
            }
        }
    }
}
