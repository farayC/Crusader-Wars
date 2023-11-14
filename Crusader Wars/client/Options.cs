using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Control = System.Windows.Forms.Control;

namespace Crusader_Wars
{
    public partial class Options : Form
    {
        private string CK3_Path { get; set; }
        private string Attila_Path { get; set; }
        public Options()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.logo;
        }
        private void CloseBtn_Click(object sender, EventArgs e)
        {
            SaveCheckedStates();
            WriteLastChecked();
            this.Close();
        }


        private void Options_Load(object sender, EventArgs e)
        {
            ReadTimePeriods();
            RetrieveCheckedStates();
            CreateDynamicListControl();
            Status_Refresh();
            ReadLastChecked();
            LoadMappersDescritions();
            UpdateStatusLabel();


        }

        //This is for unit mapper tooltip
        List<(string mapper, string start_year, string end_year)> timePeriodCollecion;
        void ReadTimePeriods()
        {
            timePeriodCollecion = new List<(string mapper, string start_year, string end_year)>();

            string mappers_folder = Directory.GetCurrentDirectory() + @"\Mappers";
            var folderNames = Directory.GetDirectories(mappers_folder).Select(Path.GetFileName).ToArray();
            foreach (string folder in folderNames)
            {
                string time_period_path = UnitMapper.GetXmlFilePath(folder, "TimePeriod")[0];
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(time_period_path);

                string startDateNode = xmlDoc.SelectSingleNode("/TimePeriod/StartDate").InnerText;
                string endDateNode = xmlDoc.SelectSingleNode("/TimePeriod/EndDate").InnerText;

                if((startDateNode == "DEFAULT" || startDateNode == "default") || (endDateNode == "DEFAULT" || endDateNode == "default"))
                {

                    string text = "Anytime";
                    timePeriodCollecion.Add((folder, text, text));
                }
                else
                {
                    timePeriodCollecion.Add((folder, startDateNode,  endDateNode));

                }


            }
        }



        private static Dictionary<string, bool> folderCheckStates = new Dictionary<string, bool>();
        private void CreateDynamicListControl()
        {
            string directoryPath = Directory.GetCurrentDirectory() + @"\Mappers";
            List<string> folderNames = new List<string>();
            folderNames.AddRange(Directory.GetDirectories(directoryPath));

            foreach (string folderName in folderNames)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Width = MappersControl.Width;
                checkBox.Text = Path.GetFileName(folderName);

                // Add event handlers for drag and drop functionality
                checkBox.MouseDown += CheckBox_MouseDown;
                checkBox.MouseHover += CheckBox_MouseHover;
                checkBox.MouseClick += CheckBox_MouseClick;

                // Check if the folder has a stored state
                if (folderCheckStates.ContainsKey(folderName))
                {
                    checkBox.Checked = folderCheckStates[folderName];
                }

                // Add an event handler to handle checkbox state changes
                checkBox.CheckedChanged += (sender, e) =>
                {
                    folderCheckStates[folderName] = checkBox.Checked;
                };

                MappersControl.Controls.Add(checkBox);
            }
        }

        private void CheckBox_MouseClick(object sender, MouseEventArgs e)
        {
            UpdateStatusLabel();
        }

        List<(string mapper ,string description)> MappersDescriptions  { get; set; }

        private void LoadMappersDescritions()
        {
            MappersDescriptions = new List<(string mapper, string description)>();

            var all_mappers_folders = Directory.GetDirectories(@".\Mappers");
            foreach(var folder in all_mappers_folders)
            {
                string mapper_name = Path.GetFileName(folder);
                try 
                {
                    var txt_files_path = Directory.GetFiles(folder).Where(x => x.EndsWith(".txt"));


                    using (FileStream logFile = File.Open(txt_files_path.FirstOrDefault(), FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite))
                    using (StreamReader reader = new StreamReader(logFile))
                    {
                        string description = reader.ReadToEnd();
                        MappersDescriptions.Add((mapper_name, description));
                    }

                }
                catch 
                {
                    continue;
                }


            }


        }

        private void CheckBox_MouseHover(object sender, EventArgs e)
        {
            var control = (CheckBox)sender;

            var period = timePeriodCollecion.FirstOrDefault(x => x.mapper == control.Text);
            var description = MappersDescriptions.FirstOrDefault(x => x.mapper == control.Text).description;

            ToolTip_UnitMappers.ToolTipTitle = "";
            ToolTip_UnitMappers.SetToolTip(control, $"{description}\n{period.start_year} - {period.end_year}");
        }


        // Save the checked states before closing the form
        private static Dictionary<string, bool> lastFolderCheckStates = new Dictionary<string, bool>();
        private void SaveCheckedStates()
        {
            lastFolderCheckStates = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, bool> entry in folderCheckStates)
            {
                string folderName = entry.Key;
                bool isChecked = entry.Value;
                lastFolderCheckStates.Add(folderName, isChecked);
            }
        }

        // Retrieve the checked states when opening the form
        private void RetrieveCheckedStates()
        {
            folderCheckStates = lastFolderCheckStates;
        }
        private CheckBox selectedCheckBox; // Track the currently selected checkbox

        private void CheckBox_MouseDown(object sender, MouseEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;


            // Unselect the previously selected checkbox, if any
            if (selectedCheckBox != null && selectedCheckBox != checkBox)
            {
                selectedCheckBox.BackColor = SystemColors.Control; // Restore the default background color
            }

            // Update the selected checkbox and its visual appearance
            selectedCheckBox = checkBox;
            selectedCheckBox.BackColor = Color.LightBlue; // Set the background color for the selected item




        }

        void UpdateStatusLabel()
        {
            //Update status text
            int disabled = 0;
            int enabled = 0;
            foreach (var item in folderCheckStates)
            {
                if (item.Value is false)
                {
                    disabled++;
                }
                else
                {
                    enabled++;
                }
            }

            if (disabled == folderCheckStates.Count) 
            { 
                Label_MapperStatus.Text = "No Mapper enabled!";
                Label_MapperStatus.ForeColor = Color.Red;
            }
            if (enabled == 1)
            {
                Label_MapperStatus.Text = $"Mapper loaded!";
                Label_MapperStatus.ForeColor = SystemColors.ControlText;
            }
            if (enabled > 1)
            {
                Label_MapperStatus.Text = "Loading according to time period!";
                Label_MapperStatus.ForeColor = SystemColors.ControlText;
            }
        }

        private void WriteLastChecked()
        {
            string filePath = @".\Settings\lastchecked.txt";

            try
            {
                // Create a new text file or overwrite if it already exists
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var control in MappersControl.Controls)
                    {
                        var check_box = (CheckBox)control;

                        if(check_box.Checked) 
                        {
                            writer.WriteLine(check_box.Text);
                        }
                    }

                    writer.Close();
                }

            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred while writing to the file:");
                Console.WriteLine(e.Message);
            }

        }

        private void ReadLastChecked()
        {
            string filePath = @".\Settings\lastchecked.txt";

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    var all = File.ReadAllLines(filePath);
                    
                    foreach(var line in all)
                    {
                        if (line != null)
                        {
                            for (int i = 0; i < MappersControl.Controls.Count; i++)
                            {
                                var check_box = (CheckBox)MappersControl.Controls[i];
                                if (line == check_box.Text)
                                {
                                    check_box.Checked = true;
                                    break;
                                }
                            }

                        }
                    }

                }
            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred while reading to the file:");
                Console.WriteLine(e.Message);
            }
        }


        private void Status_Refresh()
        {
            //Path Status
            //Ck3
            if (Properties.Settings.Default.VAR_ck3_path.Contains("ck3.exe"))
            {
                Status_Ck3_Icon.BackgroundImage = Crusader_Wars.Properties.Resources.correct;
            }
            else
            {
                Status_Ck3_Icon.BackgroundImage = Crusader_Wars.Properties.Resources.warning__1_;
            }

            //Attila
            if (Properties.Settings.Default.VAR_attila_path.Contains("Attila.exe"))
            {
                Status_Attila_Icon.BackgroundImage = Crusader_Wars.Properties.Resources.correct;
            }
            else
            {
                Status_Attila_Icon.BackgroundImage = Crusader_Wars.Properties.Resources.warning__1_;
            }
        }


        private void ck3Btn_Click(object sender, EventArgs e)
        {
            string game_node = "CrusaderKings";

            // Open the file explorer dialog
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select 'ck3.exe' from the installation folder";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                CK3_Path = openFileDialog1.FileName; // Get the selected file path
                Properties.Settings.Default.VAR_ck3_path = CK3_Path;
                ChangePathSettings(game_node, CK3_Path);
                Properties.Settings.Default.Save();
            }

            Status_Refresh();

        }

        private void AttilaBtn_Click(object sender, EventArgs e)
        {
            string game_node = "TotalWarAttila";

            // Open the file explorer dialog
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select 'Attila.exe' from the installation folder";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                Attila_Path = openFileDialog1.FileName; // Get the selected file path
                Properties.Settings.Default.VAR_attila_path = Attila_Path;
                if (Attila_Path.Contains("Attila.exe"))
                {
                    Properties.Settings.Default.VAR_log_attila = Attila_Path.Substring(0, Attila_Path.IndexOf("Attila.exe")) + "data\\BattleResults_log.txt";
                }
                ChangePathSettings(game_node, Attila_Path);
                Properties.Settings.Default.Save();
            }

            Status_Refresh();

        }

        private void ChangePathSettings(string game, string new_path)
        {
            try
            {
                string file = @".\Settings\Paths.xml";
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(file);

                XmlNode node = xmlDoc.SelectSingleNode($"Paths/{game}");
                node.Attributes["path"].Value = new_path;
                xmlDoc.Save(file);
            }
            catch 
            {
                MessageBox.Show("Error setting game paths. Restart the mod and try again", "Data Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Application.Exit();
            }


        }

        /*
         * To read after each update, so that the user
         * doesnt need to always set the game paths
         */
        public static void ReadGamePaths()
        {
            try
            {
                string file = @".\Settings\Paths.xml";
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(file);

                //Read Attila Path
                XmlNode attila_node = xmlDoc.SelectSingleNode("Paths/TotalWarAttila");
                Properties.Settings.Default.VAR_attila_path = attila_node.Attributes["path"].Value;
                Properties.Settings.Default.Save();

                //Read CK3 Path
                XmlNode ck3_node = xmlDoc.SelectSingleNode("Paths/CrusaderKings");
                Properties.Settings.Default.VAR_ck3_path = ck3_node.Attributes["path"].Value;
                Properties.Settings.Default.Save();
            }
            catch 
            {
                MessageBox.Show("Error reading game paths. Restart the mod and try again", "Data Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Application.Exit();
            }


        }



        
        Point mouseOffset;
        private void Options_MouseDown(object sender, MouseEventArgs e)
        {
            mouseOffset = new Point(-e.X, -e.Y);
        }

        private void Options_MouseMove(object sender, MouseEventArgs e)
        {
            // Move the form when the left mouse button is down
            if (e.Button == MouseButtons.Left)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void AttilaBtn_MouseHover(object sender, EventArgs e)
        {
            ToolTip_Attila.ToolTipTitle = "Attila Path";
            ToolTip_Attila.SetToolTip(AttilaBtn, Properties.Settings.Default.VAR_attila_path);
        }

        private void ck3Btn_MouseHover(object sender, EventArgs e)
        {
            ToolTip_Attila.ToolTipTitle = "Crusader Kings 3 Path";
            ToolTip_Attila.SetToolTip(ck3Btn, Properties.Settings.Default.VAR_ck3_path);
        }


        private void Options_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void infoBox_MouseHover(object sender, EventArgs e)
        {
            ToolTip_UnitMappers.ToolTipTitle = "What are Unit Mappers?";

            ToolTip_UnitMappers.SetToolTip(infoBox, "This is how the mod assigns ck3 cultures and men-at-arms to Attila units.\n" +
                                                    "Think of them like little unit mod packs. You choose the desired one for how you want your units to look\n" +
                                                    "You can have multiple enabled and the mod will load the one that the time period is more close to your campaign date.\n\n" +
                                                    "Select only one mapper if you only want that one to load.\n" +
                                                    "Select multiple if they have a continuation of time periods.\n\n" +
                                                    "The officials unit mappers are meant to all of them being enabled, so you can see your units evolve.\n" +
                                                    "You can change the timeperiod they start by going to the unit mapper files and change the years.");
        }
    }
}
