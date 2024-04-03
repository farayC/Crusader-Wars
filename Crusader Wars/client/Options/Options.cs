using Crusader_Wars.client;
using Crusader_Wars.client.WarningMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Media;
using Control = System.Windows.Forms.Control;

namespace Crusader_Wars
{
    public partial class Options : Form
    {
        private string CK3_Path { get; set; }
        private string Attila_Path { get; set; }

        UserControl General_Tab;
        UserControl Units_Tab;
        UserControl BattleScale_Tab;
        public Options()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.logo;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            SaveCheckedStates();
            WriteLastChecked();
            SaveValuesToOptionsFile();
            ReadOptionsFile();
            ModOptions.StoreOptionsValues(optionsValuesCollection);
            this.Close();
        }


        private void Options_Load(object sender, EventArgs e)
        {
            General_Tab = new UC_GeneralOptions();
            Units_Tab = new UC_UnitsOptions();
            BattleScale_Tab = new UC_BattleScaleOptions();

            ReadTimePeriods();
            ReadOptionsFile();
            SetOptionsUIData();
            RetrieveCheckedStates();
            CreateDynamicListControl();
            Status_Refresh();
            ReadLastChecked();
            LoadMappersDescritions();
            UpdateStatusLabel();


        }

        //this is to read the options values on the .xml file
        public static List<(string option, string value)> optionsValuesCollection { get; private set; }
        public static void ReadOptionsFile()
        {

            try
            {
                string file = @".\Settings\Options.xml";
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(file);

                optionsValuesCollection = new List<(string option, string value)>();
                var CloseAttila_Value = xmlDoc.SelectSingleNode("//Option [@name='CloseAttila']").InnerText;
                var FullArmies_Value = xmlDoc.SelectSingleNode("//Option [@name='FullArmies']").InnerText;
                var TimeLimit_Value = xmlDoc.SelectSingleNode("//Option [@name='TimeLimit']").InnerText;
                var BattleMapsSize_Value = xmlDoc.SelectSingleNode("//Option [@name='BattleMapsSize']").InnerText;
                var DefensiveDeployables_Value = xmlDoc.SelectSingleNode("//Option [@name='DefensiveDeployables']").InnerText;
                var UnitCards_Value = xmlDoc.SelectSingleNode("//Option [@name='UnitCards']").InnerText;

                var LeviesMax_Value = xmlDoc.SelectSingleNode("//Option [@name='LeviesMax']").InnerText;
                var RangedMax_Value = xmlDoc.SelectSingleNode("//Option [@name='RangedMax']").InnerText;
                var InfantryMax_Value = xmlDoc.SelectSingleNode("//Option [@name='InfantryMax']").InnerText;
                var CavalryMax_Value = xmlDoc.SelectSingleNode("//Option [@name='CavalryMax']").InnerText;

                var BattleScale_Value = xmlDoc.SelectSingleNode("//Option [@name='BattleScale']").InnerText;
                var AutoScaleUnits_Value = xmlDoc.SelectSingleNode("//Option [@name='AutoScaleUnits']").InnerText;

                optionsValuesCollection.AddRange(new List<(string, string)>
                {
                    ("CloseAttila", CloseAttila_Value),
                    ("FullArmies", FullArmies_Value),
                    ("TimeLimit", TimeLimit_Value),
                    ("BattleMapsSize", BattleMapsSize_Value) ,
                    ("DefensiveDeployables", DefensiveDeployables_Value),
                    ("UnitCards", UnitCards_Value),

                    ("LeviesMax", LeviesMax_Value),
                    ("RangedMax", RangedMax_Value),
                    ("InfantryMax", InfantryMax_Value),
                    ("CavalryMax", CavalryMax_Value),

                    ("BattleScale", BattleScale_Value),
                    ("AutoScaleUnits", AutoScaleUnits_Value),
                });


            }
            catch
            {
                MessageBox.Show("Error reading game options. Restart the mod and try again", "Data Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Application.Exit();
            }
        }

        void SetOptionsUIData()
        {
            var CloseAttila_ComboBox = General_Tab.Controls[0].Controls.Find("OptionSelection_CloseAttila", true).FirstOrDefault() as ComboBox;
            var FullArmies_ComboBox = General_Tab.Controls[0].Controls.Find("OptionSelection_FullArmies", true).FirstOrDefault() as ComboBox;
            var TimeLimit_ComboBox = General_Tab.Controls[0].Controls.Find("OptionSelection_TimeLimit", true).FirstOrDefault() as ComboBox;
            var BattleMapsSize_ComboBox = General_Tab.Controls[0].Controls.Find("OptionSelection_BattleMapsSize", true).FirstOrDefault() as ComboBox;
            var DefensiveDeployables_ComboBox = General_Tab.Controls[0].Controls.Find("OptionSelection_DefensiveDeployables", true).FirstOrDefault() as ComboBox;
            var UnitCards_ComboBox = General_Tab.Controls[0].Controls.Find("OptionSelection_UnitCards", true).FirstOrDefault() as ComboBox;

            var LeviesMax_ComboBox = Units_Tab.Controls[0].Controls.Find("OptionSelection_LeviesMax", true).FirstOrDefault() as ComboBox;
            var RangedMax_ComboBox = Units_Tab.Controls[0].Controls.Find("OptionSelection_RangedMax", true).FirstOrDefault() as ComboBox;
            var InfantryMax_ComboBox = Units_Tab.Controls[0].Controls.Find("OptionSelection_InfantryMax", true).FirstOrDefault() as ComboBox;
            var CavalryMax_ComboBox = Units_Tab.Controls[0].Controls.Find("OptionSelection_CavalryMax", true).FirstOrDefault() as ComboBox;

            var BattleScale_ComboBox = BattleScale_Tab.Controls[0].Controls.Find("OptionSelection_BattleSizeScale", true).FirstOrDefault() as ComboBox;            
            var AutoScaleUnits_ComboBox = BattleScale_Tab.Controls[0].Controls.Find("OptionSelection_AutoScale", true).FirstOrDefault() as ComboBox;
            
            CloseAttila_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "CloseAttila").value; 
            FullArmies_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "FullArmies").value;
            TimeLimit_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "TimeLimit").value;
            BattleMapsSize_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "BattleMapsSize").value;
            DefensiveDeployables_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "DefensiveDeployables").value;
            UnitCards_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "UnitCards").value;

            LeviesMax_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "LeviesMax").value;
            RangedMax_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "RangedMax").value;
            InfantryMax_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "InfantryMax").value;
            CavalryMax_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "CavalryMax").value;

            BattleScale_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "BattleScale").value;
            AutoScaleUnits_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "AutoScaleUnits").value;

            ChangeOptionsTab(General_Tab);
        }


        void SaveValuesToOptionsFile()
        {
            string file = @".\Settings\Options.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);

            var CloseAttila_ComboBox = General_Tab.Controls.Find("OptionSelection_CloseAttila", true)[0] as ComboBox;
            var FullArmies_ComboBox = General_Tab.Controls.Find("OptionSelection_FullArmies", true)[0] as ComboBox;
            var TimeLimit_ComboBox = General_Tab.Controls.Find("OptionSelection_TimeLimit", true)[0] as ComboBox;
            var BattleMapsSize_ComboBox = General_Tab.Controls.Find("OptionSelection_BattleMapsSize", true)[0] as ComboBox;
            var DefensiveDeployables_ComboBox = General_Tab.Controls.Find("OptionSelection_DefensiveDeployables", true)[0] as ComboBox;
            var UnitCards_ComboBox = General_Tab.Controls.Find("OptionSelection_UnitCards", true)[0] as ComboBox;

            var LeviesMax_ComboBox = Units_Tab.Controls.Find("OptionSelection_LeviesMax", true)[0] as ComboBox;
            var RangedMax_ComboBox = Units_Tab.Controls.Find("OptionSelection_RangedMax", true)[0] as ComboBox;
            var InfantryMax_ComboBox = Units_Tab.Controls.Find("OptionSelection_InfantryMax", true)[0] as ComboBox;
            var CavalryMax_ComboBox = Units_Tab.Controls.Find("OptionSelection_CavalryMax", true)[0] as ComboBox;

            var BattleScale_ComboBox = BattleScale_Tab.Controls.Find("OptionSelection_BattleSizeScale", true)[0] as ComboBox;
            var AutoScaleUnits_ComboBox = BattleScale_Tab.Controls.Find("OptionSelection_AutoScale", true)[0] as ComboBox;



            var CloseAttila_Node = xmlDoc.SelectSingleNode("//Option [@name='CloseAttila']");
            CloseAttila_Node.InnerText = CloseAttila_ComboBox.Text;
            var FullArmies_Node = xmlDoc.SelectSingleNode("//Option [@name='FullArmies']");
            FullArmies_Node.InnerText = FullArmies_ComboBox.Text;
            var TimeLimit_Node = xmlDoc.SelectSingleNode("//Option [@name='TimeLimit']");
            TimeLimit_Node.InnerText = TimeLimit_ComboBox.Text;
            var BattleMapsSize_Node = xmlDoc.SelectSingleNode("//Option [@name='BattleMapsSize']");
            BattleMapsSize_Node.InnerText = BattleMapsSize_ComboBox.Text;
            var DefensiveDeployables_Node = xmlDoc.SelectSingleNode("//Option [@name='DefensiveDeployables']");
            DefensiveDeployables_Node.InnerText = DefensiveDeployables_ComboBox.Text;
            var UnitCards_Node = xmlDoc.SelectSingleNode("//Option [@name='UnitCards']");
            UnitCards_Node.InnerText = UnitCards_ComboBox.Text;

            var LeviesMax_Node = xmlDoc.SelectSingleNode("//Option [@name='LeviesMax']");
            LeviesMax_Node.InnerText = LeviesMax_ComboBox.Text;
            var RangedMax_Node = xmlDoc.SelectSingleNode("//Option [@name='RangedMax']");
            RangedMax_Node.InnerText = RangedMax_ComboBox.Text;
            var InfantryMax_Node = xmlDoc.SelectSingleNode("//Option [@name='InfantryMax']");
            InfantryMax_Node.InnerText = InfantryMax_ComboBox.Text;
            var CavalryMax_Node = xmlDoc.SelectSingleNode("//Option [@name='CavalryMax']");
            CavalryMax_Node.InnerText = CavalryMax_ComboBox.Text;

            var BattleScale_Node = xmlDoc.SelectSingleNode("//Option [@name='BattleScale']");
            BattleScale_Node.InnerText = BattleScale_ComboBox.Text;
            var AutoScaleUnits_Node = xmlDoc.SelectSingleNode("//Option [@name='AutoScaleUnits']");
            AutoScaleUnits_Node.InnerText = AutoScaleUnits_ComboBox.Text;

            xmlDoc.Save(file);
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

        Dictionary<string, (int start, int end)> YearCollection = new Dictionary<string, (int start, int end)>();
        void UpdateStatusLabel()
        {
            //Update status text
            int disabled = 0;
            int enabled = 0;


            YearCollection = new Dictionary<string, (int start, int end)>();
            foreach (var item in folderCheckStates)
            {
                if (item.Value is false)
                {
                    disabled++;
                }
                else
                {
                    enabled++;


                    var timePeriod_Item = timePeriodCollecion.FirstOrDefault(x => item.Key.Contains(x.mapper));
                    if (timePeriod_Item.mapper != null) {
                        try { YearCollection.Add(item.Key, (Int32.Parse(timePeriod_Item.start_year), Int32.Parse(timePeriod_Item.end_year))); } 
                        catch { }
                    } 
                    

                }

            }


            if (disabled == folderCheckStates.Count) 
            { 
                Label_MapperStatus.Text = "No Mapper enabled!";
                Label_MapperStatus.ForeColor = Color.Red;
            }
            if (enabled == 1)
            {
                string loaded_mapper = YearCollection.First().Key;
                if (loaded_mapper.Contains("OfficialCW"))
                {
                    Label_MapperStatus.Text = $"Single Mapper loaded!";
                    Label_MapperStatus.ForeColor = Color.White;
                }
                else if (loaded_mapper.Contains("xCW_FallenEagle"))
                {
                    Label_MapperStatus.Text = $"Single TFE Mapper loaded!";
                    Label_MapperStatus.ForeColor = Color.White;
                }
                else if (loaded_mapper.Contains("xCW_RealmsInExile"))
                {
                    Label_MapperStatus.Text = $"Single LOTR Mapper loaded!";
                    Label_MapperStatus.ForeColor = Color.White;
                }

            }
            if (enabled > 1)
            {
                //LOTR & TFE mix warning
                try
                {
                    string lotr_one = YearCollection.First(item => item.Key.Contains("xCW_RealmsInExile")).Key;
                    string tfe_one = YearCollection.First(item => item.Key.Contains("xCW_FallenEagle")).Key;

                    Label_MapperStatus.Text = "Incorrect mix of loaded Mappers!";
                    Label_MapperStatus.ForeColor = Color.Red;

                    return;
                }
                catch { }


                //Total Conversion & Offical mix warning
                try
                {
                    string official_one = YearCollection.First(item => item.Key.Contains("OfficialCW")).Key;
                    string totalConversion_one = YearCollection.First(item => item.Key.Contains("xCW")).Key;

                    Label_MapperStatus.Text = "Incorrect Mappers loaded together!";
                    Label_MapperStatus.ForeColor = Color.Red;

                    return;
                }
                catch { }


                //Time Period
                int minimum_year = 0;
                int maximum_year = 0;

                foreach (var item in YearCollection) 
                {

                    int min, max;   
                    min = Math.Min(item.Value.start, item.Value.end);
                    max = Math.Max(item.Value.start, item.Value.end);
                    if(minimum_year == 0) { minimum_year = min; }
                    if (maximum_year == 0) { maximum_year = max; }

                    if (min < minimum_year) { minimum_year = min; }
                    if (max > maximum_year) { maximum_year = max; }
                }
                

                
                Label_MapperStatus.Text = $"Loading Mappers from {minimum_year}AD to {maximum_year}AD!";
                Label_MapperStatus.ForeColor = Color.White;
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


        
        //Warning Messages
        private void Options_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (Label_MapperStatus.Text == "Incorrect Mappers loaded together!")
            {
                string message = "Mixing Total Conversion Unit Mappers with Official ones cause crashes! Disable one of them!";
                WarningMessage.ShowWarningMessage(message);
            }
            else if(Label_MapperStatus.Text == "No Mapper enabled!")
            {
                string message = "You didn't enable a Unit Mapper, enable one or the mod will not work!";
                WarningMessage.ShowWarningMessage(message);
            }
            else if(Label_MapperStatus.Text == "Incorrect mix of loaded Mappers!")
            {
                string message = "You enabled \"Realms in Exile\" Mapper with \"The Fallen Eagle\" Mapper, just enable one of them or the mod will not work!";
                WarningMessage.ShowWarningMessage(message);
            }

            foreach(var mapper_path in YearCollection)
            {
                if (mapper_path.Key.Contains("MK1212"))
                {
                    string text = "MK1212 has updated and it's temporarily not working with Crusader Wars, use the previous version of MK1212, and not to the most recent, to avoid crashes in Attila! You have been warned!";
                    string title = "Warning";
                    WarningMessage.ShowWarningMessage(text, title);
                    break;
                }
            }

            int start =1066;
            foreach (var i in YearCollection)
            {
                if (i.Value.start < start)
                { 
                    start = i.Value.start; 
                    continue; 
                }
                else { 
                    start = Math.Min(i.Value.start, start); 
                }
                
            }
            if (start > 1066 && YearCollection.Count > 1)
            {
                string message = $"Your selected unit mappers only starts at {YearCollection.ElementAtOrDefault(0).Value.start}AD.\n" +
                                 "If your CK3 campaign is below this year, the mod won't work!\n";
                string title = "Empty Time Periods";
                WarningMessage.ShowWarningMessage(message, title);
            }

            
        }

        private void infoBox_MouseHover(object sender, EventArgs e)
        {
            ToolTip_UnitMappers.ToolTipTitle = "What are Unit Mappers?";

            ToolTip_UnitMappers.SetToolTip(infoBox, "This is how the mod assigns ck3 cultures and men-at-arms to Attila units.\n" +
                                                    "Think of them like little unit mod packs. You choose the desired one for how you want your units to look\n" +
                                                    "You can have multiple enabled and the mod will load the one that the time period is more close to your campaign year.\n\n" +
                                                    "Select only one mapper if you only want that one to load.\n" +
                                                    "Select multiple if they have a continuation of time periods.\n\n" +
                                                    "The officials unit mappers are meant to all of them being enabled, so you can see your units evolve.");
        }

        private void Btn_GeneralTab_Click(object sender, EventArgs e)
        {
            if (OptionsPanel.Controls.Count > 0 && OptionsPanel.Controls[0] != General_Tab)
                ChangeOptionsTab(General_Tab);
        }

        private void Btn_UnitsTab_Click(object sender, EventArgs e)
        {
            if (OptionsPanel.Controls.Count > 0 && OptionsPanel.Controls[0] != Units_Tab)
                ChangeOptionsTab(Units_Tab);
        }

        private void Btn_BattleScaleTab_Click(object sender, EventArgs e)
        {
            if (OptionsPanel.Controls.Count > 0 && OptionsPanel.Controls[0] != BattleScale_Tab)
                ChangeOptionsTab(BattleScale_Tab);
        }

        void ChangeOptionsTab(Control control)
        {
            control.Dock = DockStyle.Fill;
            OptionsPanel.Controls.Clear();
            OptionsPanel.Controls.Add(control);
            control.BringToFront();
        }
    }
}
