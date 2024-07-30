using Crusader_Wars.client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Crusader_Wars.mod_manager
{
    public partial class UC_UnitMapper : UserControl
    {
        string SteamCollectionLink {  get; set; }
        List<string> RequiredModsList { get; set; }
        
        public UC_UnitMapper(Bitmap image, string steamCollectionLink, bool state)
        {
            InitializeComponent();

            pictureBox1.BackgroundImage = image;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            SteamCollectionLink = steamCollectionLink;
            uC_Toggle1.SetState(state);

            RequiredModsList = new List<string>
            {
                "1051_0.pack",
                "1051_1.pack",
                "ad_1051.pack"
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(SteamCollectionLink);
        }

        public bool GetState()
        {
            return uC_Toggle1.State;
        }

        public void SetState(bool state)
        {
            uC_Toggle1.SetState(state);
        }

        private void BtnVerifyMods_Click(object sender, EventArgs e)
        {
            if(RequiredModsList != null)
            {
                List<string> notFoundMods = new List<string>();
                notFoundMods.AddRange(RequiredModsList);

                //Verify data folder
                string data_folder_path = Properties.Settings.Default.VAR_attila_path.Replace("Attila.exe", @"data\");
                if(Directory.Exists(data_folder_path))
                {
                    var dataModsPaths = Directory.GetFiles(data_folder_path);
                    foreach (var file in dataModsPaths)
                    {
                        var fileName = Path.GetFileName(file);
                        foreach(var mod in RequiredModsList)
                        {
                            if(mod == fileName && Path.GetExtension(fileName) == ".pack")
                                notFoundMods.Remove(mod);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Error reading Attila data folder. This is caused by wrong Attila path.", "Game Paths Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }

                //Verify workshop folder
                string workshop_folder_path = Properties.Settings.Default.VAR_attila_path.Replace(@"common\Total War Attila\Attila.exe", @"workshop\content\325610\");
                if (Directory.Exists(workshop_folder_path))
                {
                    var steamModsFoldersPaths = Directory.GetDirectories(workshop_folder_path);
                    foreach (var folder in steamModsFoldersPaths)
                    {
                        var files = Directory.GetFiles(folder);
                        foreach (var file in files)
                        {
                            var fileName = Path.GetFileName(file);
                            foreach(var mod in RequiredModsList)
                            {
                                if (mod == fileName && Path.GetExtension(fileName) == ".pack")
                                    notFoundMods.Remove(mod);
                            }
                        }
                    }
                }


                //Print Message
                if(notFoundMods.Count > 0) // not all installed
                {
                    string missingMods = "";
                    foreach (var mod in notFoundMods)
                        missingMods += $"{mod}\n";

                    MessageBox.Show($"You are missing these mods:\n{missingMods}", "Missing Mods!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                else if (notFoundMods.Count == 0) // all installed
                {
                    MessageBox.Show("All mods are installed, you are good to go!", "All mods installed!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
        }
    }
}
