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
using Crusader_Wars.mod_manager;

namespace Crusader_Wars
{
    public partial class Options : Form
    {
        private string CK3_Path { get; set; }
        private string Attila_Path { get; set; }



        UserControl General_Tab;
        UserControl Units_Tab;
        UserControl BattleScale_Tab;

        UC_UnitMapper CrusaderKings_Tab;
        UC_UnitMapper TheFallenEagle_Tab;
        UC_UnitMapper RealmsInExile_Tab;
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
            SaveValuesToOptionsFile();
            ReadOptionsFile();
            ModOptions.StoreOptionsValues(optionsValuesCollection);
            WriteUnitMappersOptions();
            
            AttilaModManager.SaveActiveMods();
            AttilaModManager.CreateUserModsFile();
            ChangeTabsStates();

            this.Close();
        }


        private void Options_Load(object sender, EventArgs e)
        {
            General_Tab = new UC_GeneralOptions();
            Units_Tab = new UC_UnitsOptions();
            BattleScale_Tab = new UC_BattleScaleOptions();

            //ReadTimePeriods();
            ReadUnitMappersOptions();
            ReadOptionsFile();
            SetOptionsUIData();
            Status_Refresh();


            AttilaModManager.SetControlRefence(ModManager);
            AttilaModManager.ReadInstalledMods();
            
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
                var CulturalPreciseness = xmlDoc.SelectSingleNode("//Option [@name='CulturalPreciseness']").InnerText;

                var LeviesMax_Value = xmlDoc.SelectSingleNode("//Option [@name='LeviesMax']").InnerText;
                var RangedMax_Value = xmlDoc.SelectSingleNode("//Option [@name='RangedMax']").InnerText;
                var InfantryMax_Value = xmlDoc.SelectSingleNode("//Option [@name='InfantryMax']").InnerText;
                var CavalryMax_Value = xmlDoc.SelectSingleNode("//Option [@name='CavalryMax']").InnerText;

                var BattleScale_Value = xmlDoc.SelectSingleNode("//Option [@name='BattleScale']").InnerText;
                var AutoScaleUnits_Value = xmlDoc.SelectSingleNode("//Option [@name='AutoScaleUnits']").InnerText;
                var SeparateArmies_Value = xmlDoc.SelectSingleNode("//Option [@name='SeparateArmies']").InnerText;

                optionsValuesCollection.AddRange(new List<(string, string)>
                {
                    ("CloseAttila", CloseAttila_Value),
                    ("FullArmies", FullArmies_Value),
                    ("TimeLimit", TimeLimit_Value),
                    ("BattleMapsSize", BattleMapsSize_Value) ,
                    ("DefensiveDeployables", DefensiveDeployables_Value),
                    ("UnitCards", UnitCards_Value),
                    ("SeparateArmies", SeparateArmies_Value),
                    ("CulturalPreciseness", CulturalPreciseness),

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
            var SeparateArmies_ComboBox = General_Tab.Controls[0].Controls.Find("OptionSelection_SeparateArmies", true).FirstOrDefault() as ComboBox;
            var CulturalPreciseness_TrackBar = General_Tab.Controls[0].Controls.Find("OptionSelection_CulturalPreciseness", true).FirstOrDefault() as TrackBar;

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
            SeparateArmies_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "SeparateArmies").value;
            CulturalPreciseness_TrackBar.Value = Int32.Parse(optionsValuesCollection.FirstOrDefault(x => x.option == "CulturalPreciseness").value);

            LeviesMax_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "LeviesMax").value;
            RangedMax_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "RangedMax").value;
            InfantryMax_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "InfantryMax").value;
            CavalryMax_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "CavalryMax").value;

            BattleScale_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "BattleScale").value;
            AutoScaleUnits_ComboBox.SelectedItem = optionsValuesCollection.FirstOrDefault(x => x.option == "AutoScaleUnits").value;
            

            ChangeOptionsTab(General_Tab);
            ChangeUnitMappersTab(CrusaderKings_Tab);
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
            var SeparateArmies_ComboBox = General_Tab.Controls.Find("OptionSelection_SeparateArmies", true)[0] as ComboBox;
            var CulturalPreciseness_TrackBar = General_Tab.Controls.Find("OptionSelection_CulturalPreciseness", true)[0] as TrackBar;

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
            var SeparateArmies_Node = xmlDoc.SelectSingleNode("//Option [@name='SeparateArmies']");
            SeparateArmies_Node.InnerText = SeparateArmies_ComboBox.Text;
            var CulturalPreciseness_Node = xmlDoc.SelectSingleNode("//Option [@name='CulturalPreciseness']");
            CulturalPreciseness_Node.InnerText = CulturalPreciseness_TrackBar.Value.ToString();

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
        /*
        void ReadTimePeriods()
        {
            timePeriodCollecion = new List<(string mapper, string start_year, string end_year)>();

            string mappers_folder = Directory.GetCurrentDirectory() + @"\unit mappers";
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
        */


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


        /*##############################################
         *####              MOD MANAGER             #### 
         *####--------------------------------------####
         *####         Mod Manager Section          ####
         *##############################################
         */

        private void ModManager_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == ModManager.Columns[0].Index && e.RowIndex != -1)
            {
                int rowIndex = e.RowIndex;
                AttilaModManager.ChangeEnabledState(ModManager.Rows[rowIndex]);
            }
        }
        private void ModManager_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // End of edition on each click on column of checkbox
            if (e.ColumnIndex == ModManager.Columns[0].Index && e.RowIndex != -1)
            {
                ModManager.EndEdit();
            }
        }

        private void ModManager_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // End of edition on each click on column of checkbox
            if (e.ColumnIndex == ModManager.Columns[0].Index && e.RowIndex != -1)
            {
                ModManager.EndEdit();
            }
        }


        /*##############################################
         *####             UNIT MAPPERS             #### 
         *####--------------------------------------####
         *####         Unit Mappers Section         ####
         *##############################################
         */
        private void Btn_CK3Tab_Click(object sender, EventArgs e)
        {
            if (UMpanel.Controls.Count > 0 && UMpanel.Controls[0] != CrusaderKings_Tab)
                ChangeUnitMappersTab(CrusaderKings_Tab);
            ChangeTabsStates();
        }

        private void Btn_TFETab_Click(object sender, EventArgs e)
        {
            if (UMpanel.Controls.Count > 0 && UMpanel.Controls[0] != TheFallenEagle_Tab)
                ChangeUnitMappersTab(TheFallenEagle_Tab);
            ChangeTabsStates();
        }

        private void Btn_LOTRTab_Click(object sender, EventArgs e)
        {
            if (UMpanel.Controls.Count > 0 && UMpanel.Controls[0] != RealmsInExile_Tab)
                ChangeUnitMappersTab(RealmsInExile_Tab);
            ChangeTabsStates();
        }

        void ChangeTabsStates() // <--- fix this!
        {
            switch(CrusaderKings_Tab.GetState())
            {
                case true:
                    TheFallenEagle_Tab.SetState(false);
                    RealmsInExile_Tab.SetState(false);
                    break;
            }
            switch (TheFallenEagle_Tab.GetState())
            {
                case true:
                    CrusaderKings_Tab.SetState(false);
                    RealmsInExile_Tab.SetState(false);
                    break;
            }
            switch (RealmsInExile_Tab.GetState())
            {
                case true:
                    TheFallenEagle_Tab.SetState(false);
                    CrusaderKings_Tab.SetState(false);
                    break;
            }
        }

        void ChangeUnitMappersTab(Control control)
        {
            control.Dock = DockStyle.Fill;
            UMpanel.Controls.Clear();
            UMpanel.Controls.Add(control);
            control.BringToFront();
        }

        void ReadUnitMappersOptions()
        {
            string file = @".\Settings\UnitMappers.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);

            var ck3ToggleStateStr = xmlDoc.SelectSingleNode("//UnitMappers [@name='DefaultCK3']").InnerText;
            var tfeToggleStateStr = xmlDoc.SelectSingleNode("//UnitMappers [@name='TheFallenEagle']").InnerText;
            var lotrToggleStateStr = xmlDoc.SelectSingleNode("//UnitMappers [@name='RealmsInExile']").InnerText;

            bool ck3ToggleState = false; bool tfeToggleState = false; bool lotrToggleState = false;
            if (ck3ToggleStateStr == "True") ck3ToggleState = true; else ck3ToggleState = false;
            if (tfeToggleStateStr == "True") tfeToggleState = true; else tfeToggleState = false;
            if (lotrToggleStateStr == "True") lotrToggleState = true; else lotrToggleState = false;

            CrusaderKings_Tab = new UC_UnitMapper(Properties.Resources._default, "", ck3ToggleState);
            TheFallenEagle_Tab = new UC_UnitMapper(Properties.Resources.tfe, "", tfeToggleState);
            RealmsInExile_Tab = new UC_UnitMapper(Properties.Resources.LOTR, "https://steamcommunity.com/sharedfiles/filedetails/?id=3211765434", lotrToggleState);

        }

        void WriteUnitMappersOptions()
        {
            string file = @".\Settings\UnitMappers.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);

            var CrusaderKings_Node = xmlDoc.SelectSingleNode("//UnitMappers [@name='DefaultCK3']");
            CrusaderKings_Node.InnerText = CrusaderKings_Tab.GetState().ToString();
            var TheFallenEagle_Node = xmlDoc.SelectSingleNode("//UnitMappers [@name='TheFallenEagle']");
            TheFallenEagle_Node.InnerText = TheFallenEagle_Tab.GetState().ToString();
            var RealmsInExile_Node = xmlDoc.SelectSingleNode("//UnitMappers [@name='RealmsInExile']");
            RealmsInExile_Node.InnerText = RealmsInExile_Tab.GetState().ToString();
            xmlDoc.Save(file);
        }
    }
}
