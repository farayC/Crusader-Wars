using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Media;
using System.Linq;
using System.Drawing;
using System.Xml.Linq;
using System.Windows;
using System.Text.RegularExpressions;
using Crusader_Wars.client;
using Crusader_Wars.client.RequiredMods;
using Crusader_Wars.locs;
using Crusader_Wars.data.attila_settings;
using Crusader_Wars.data.save_file;
using Crusader_Wars.unit_mapper;
using Crusader_Wars.terrain;

namespace Crusader_Wars
{
    public partial class HomePage : Form
    {


        const string SEARCH_KEY = "CRUSADERWARS3";

        private int _myVariable = 0;
        private Timer _timer;
        public HomePage()
        {
            InitializeComponent();
            
            System.Threading.Thread.Sleep(1000);

            //Show message box for older users to download the new launcher or it will no work
            using (FileStream configFile = File.Open(@".\Crusader Wars.exe.config", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader reader = new StreamReader(configFile))
            {

                string older_version_key = "            <setting name=\"MESSAGE_KEY\" serializeAs=\"String\">\r\n                <value>True</value>\r\n            </setting>";

                if(reader.ReadToEnd().Contains(older_version_key))
                {
                    MessageBox.Show("You have an older launcher, for Crusader Wars to work download the new one from our website.", "Older launcher detected!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }

                reader.Close();
                configFile.Close();
            }


            documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            debugLog_Path = documentsPath + "\\Paradox Interactive\\Crusader Kings III\\console_history.txt";
            saveGames_Path = documentsPath + "\\Paradox Interactive\\Crusader Kings III\\save games";
            //Icon
            this.Icon = Properties.Resources.logo;

            Properties.Settings.Default.VAR_log_attila = string.Empty;
            Properties.Settings.Default.VAR_dir_save = string.Empty;
            Properties.Settings.Default.VAR_log_ck3 = string.Empty;


            Properties.Settings.Default.VAR_dir_save = saveGames_Path;
            Properties.Settings.Default.VAR_log_ck3 = debugLog_Path;
            Properties.Settings.Default.Save();

            Updater.CheckAppVersion(this);       

            labelVersion.Text = $"V{Updater.AppVersion}";

            _timer = new Timer();
            _timer.Interval = 500; // check variable every second
            _timer.Tick += Timer_Tick;
            _timer.Start();
            Original_Color = infoLabel.ForeColor;
        }



        Color Original_Color;
        private void Timer_Tick(object sender, EventArgs e)
        {

            if (_myVariable == 0)
            {
                if (Properties.Settings.Default.VAR_ck3_path.Contains("ck3.exe") && Properties.Settings.Default.VAR_attila_path.Contains("Attila.exe"))
                {
                    ExecuteButton.Enabled = true;
                    infoLabel.Text = "Ready to Start!";
                    infoLabel.ForeColor = Original_Color;

                }
                else
                {
                    ExecuteButton.Enabled = false;
                    infoLabel.Text = "Games Paths Missing!";
                    infoLabel.ForeColor = Color.Red;
                }
            }

        }

        string path_editedSave;

        static string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        static string debugLog_Path = documentsPath + "\\Paradox Interactive\\Crusader Kings III\\console_history.txt";
        string saveGames_Path = documentsPath + "\\Paradox Interactive\\Crusader Kings III\\save games";
        private void Form1_Load(object sender, EventArgs e)
        {

            //Load Game Paths
            Options.ReadGamePaths();

            //Hide debug button
            btt_debug.Visible = true;

            Color myColor = Color.FromArgb(53, 25, 5, 5);
            infoLabel.BackColor = myColor;
            labelVersion.BackColor = myColor;

            Options.ReadOptionsFile();
            ModOptions.StoreOptionsValues(Options.optionsValuesCollection);
            AttilaPreferences.ChangeUnitSizes();


        }

        //---------------------------------//
        //----------DEBUG BUTTON-----------//
        //---------------------------------//

        private void btt_debug_Click(object sender, EventArgs e)
        {
        }
        

        Player Player;
        Enemy Enemy;

        List<Army> attacker_armies;
        List<Army> defender_armies;
        private void HomePage_Shown(object sender, EventArgs e)
        {
            infoLabel.Text = "Loading DLLs...";
            ExecuteButton.Enabled= false;
            LoadDLLs();
            ExecuteButton.Enabled= true;
            infoLabel.Text = "Ready to Start!";

            if (debugLog_Path != null)
                DataSearch.ClearLogFile();


        }

        private void CheckForMappers()
        {
            string filePath = @".\Settings\lastchecked.txt"; ;
            List<string> CheckedMappers;
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

            if(CheckedMappers.Count < 1)
            {
                throw new Exception();
            }
            else
            {
                return;
            }
        }


        string log;
        LoadingScreen LoadingScreen;
        private async void ExecuteButton_Click(object sender, EventArgs e)
        {
            sounds = new SoundPlayer(@".\data\sounds\sword-slash-with-metal-shield-impact-185433.wav");
            sounds.Play();

            _myVariable = 1;

            ExecuteButton.Enabled = false;
            ExecuteButton.BackgroundImage = Properties.Resources.start_new_disabled;
            this.Text = "Crusader Wars (Waiting for battle...)";

            ProcessCommands.ResumeProcess();

            while (true)
            {

                try
                {
                    CheckForMappers();
                }
                catch
                {
                    MessageBox.Show("No Unit Mapper is enabled!", "Data Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    infoLabel.Text = "Ready to start!";
                    ExecuteButton.Enabled = true;
                    this.Text = "Crusader Wars";
                    break;
                }

                DataSearch.ClearLogFile();
                DeclarationsFile.Erase();
                BattleScript.EraseScript();
                BattleResult.ClearAttilaLog();

                try
                {

                }
                catch
                {
                    MessageBox.Show("No Log File Found!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    infoLabel.Text = "Ready to start!";
                    ExecuteButton.Enabled = true;
                    this.Text = "Crusader Wars";
                    break;
                }

                try
                {
                    //Open Crusader Kings 3
                    Games.StartCrusaderKingsProcess();
                }
                catch
                {
                    MessageBox.Show("Couldn't find 'ck3.exe'. Change the Crusader Kings 3 path. ", "Path Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    infoLabel.Text = "Ready to start!";
                    ExecuteButton.Enabled = true;
                    this.Text = "Crusader Wars";
                    break;
                }

                BattleFile.ClearFile();

                bool battleHasStarted = false;

                //Read log file and get all data from CK3
                using (FileStream logFile = File.Open(debugLog_Path, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite))
                {

                    using (StreamReader reader = new StreamReader(logFile))
                    {
                        logFile.Position = 0;
                        reader.DiscardBufferedData();

                        infoLabel.Text = "Waiting for battle...";
                        try
                        {
                            //Wait for CW keyword
                            while (!battleHasStarted)
                            {
                                //Read each line
                                while (!reader.EndOfStream)
                                {
                                    string line = reader.ReadLine();

                                    //If Battle Started
                                    if (line.Contains(SEARCH_KEY))
                                    {
                                        battleHasStarted = true;
                                        break;
                                    }

                                }
                                logFile.Position = 0;
                                reader.DiscardBufferedData();
                                await Task.Delay(1);
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Error searching for battle. ", "Critical Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            infoLabel.Text = "Ready to start!";
                            ExecuteButton.Enabled = true;
                            this.Text = "Crusader Wars";
                            LoadingScreen.Close();
                            break;
                        }


                        try
                        {
                            LoadingScreen = new LoadingScreen();
                            LoadingScreen.Show();
                            this.Hide();

                            infoLabel.Text = "Battle found...";
                            logFile.Position = 0;
                            reader.DiscardBufferedData();
                            log = reader.ReadToEnd();
                            log = RemoveASCII(log);

                            if (battleHasStarted)
                            {
                                LoadingScreen.ChangeMessage("Reading battle data...");

                                infoLabel.Text = "Reading battle data...";
                                this.Text = "Crusader Wars (Reading battle data...)";

                                Player = new Player();
                                Enemy = new Enemy();

                                DataSearch.SearchLanguage(); if (Languages.Language != "l_english") Languages.ShowWarningMessage();
                                DataSearch.Search(log, Player, Enemy);

                            }
                        }
                        catch
                        {
                            this.Show();
                            LoadingScreen.Close();
                            MessageBox.Show("Error reading battle data.", "Data Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            infoLabel.Text = "Waiting for battle...";
                            this.Text = "Crusader Wars (Waiting for battle...)";

                            //Data Clear
                            Data.Reset();
                            Player = new Player();
                            Enemy = new Enemy(); 

                            continue;
                        }

                        logFile.Position = 0;
                        reader.DiscardBufferedData();

                        reader.Close();
                        logFile.Close();

                    }

                }



                try
                {
                    await Task.Delay(3000);
                    ProcessCommands.SuspendProcess();

                    path_editedSave = Properties.Settings.Default.VAR_dir_save + @"\CrusaderWars_Battle.ck3";

                    LoadingScreen.ChangeMessage("Reading save file data...");
                    Reader.ReadFile(path_editedSave);
                    BattleResult.GetPlayerCombatResult();
                    BattleResult.ReadPlayerCombat(Player.ID.ToString());
                }
                catch
                {
                    this.Show();
                    LoadingScreen.Close();
                    MessageBox.Show("Error reading the save file. Disable Ironman or Debug Mode.", "Save File Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    infoLabel.Text = "Waiting for battle...";
                    ProcessCommands.ResumeProcess();

                    //Data Clear
                    Data.Reset();
                    Player = new Player();
                    Enemy = new Enemy();

                    continue;

                }

                //1.0 Beta Debug
                var armies = ArmiesReader.ReadBattleArmies();
                attacker_armies = armies.attacker;
                defender_armies = armies.defender;

                if (ModOptions.UnitCards())
                {
                    LoadingScreen.ChangeMessage("Changing unit cards names...");
                    UnitsCardsNames.ChangeUnitsCardsNames(UnitMapper.LoadedMapper, Player, Enemy);
                }

                LoadingScreen.ChangeMessage("Adding battle details...");


                var left_side = ArmiesReader.GetSideArmies("left");
                var right_side = ArmiesReader.GetSideArmies("left");
                int left_side_total = left_side.Sum(army => army.GetTotalSoldiers());
                int right_side_total = right_side.Sum(army => army.GetTotalSoldiers());
                BattleDetails.ChangeBattleDetails(left_side_total, right_side_total);

                try
                {

                }
                catch
                {
                    this.Show();
                    LoadingScreen.Close();
                    MessageBox.Show("Error reading the battle armies.", "Beta Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    ProcessCommands.ResumeProcess();
                    infoLabel.Text = "Waiting for battle...";
                    this.Text = "Crusader Wars (Waiting for battle...)";

                    //Data Clear
                    Data.Reset();
                    Player = new Player();
                    Enemy = new Enemy();

                    continue;
                }

                Games.CloseTotalWarAttilaProcess();
                LoadingScreen.ChangeMessage("Creating battle in Total War: Attila...");

                //Create Declarations
                DeclarationsFile.CreateAlliances(attacker_armies, defender_armies);

                //Create Remaining Soldiers Script
                BattleScript.CreateScript();

                // Set Battle Scale
                int total_soldiers = attacker_armies.SelectMany(army => army.Units).Sum(unit => unit.GetSoldiers()) +
                                     defender_armies.SelectMany(army => army.Units).Sum(unit => unit.GetSoldiers());
                ArmyProportions.AutoSizeUnits(total_soldiers);
                foreach (var army in attacker_armies) army.ScaleUnits(ModOptions.GetBattleScale());
                foreach (var army in defender_armies) army.ScaleUnits(ModOptions.GetBattleScale());

                //Create Battle
                BattleFile.BETA_CreateBattle(attacker_armies, defender_armies);


                //Close Script
                BattleScript.CloseScript();

                //Set Units Kills Script
                BattleScript.SetLocalsKills(Data.units_scripts);

                //Close Script
                BattleScript.CloseScript();

                //Creates .pack mod file
                PackFile.PackFileCreator();
                try
                {

                }
                catch
                {
                    this.Show();
                    LoadingScreen.Close();
                    MessageBox.Show("Error creating the battle", "Data Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    ProcessCommands.ResumeProcess();
                    infoLabel.Text = "Waiting for battle...";
                    this.Text = "Crusader Wars (Waiting for battle...)";

                    //Data Clear
                    Data.Reset();
                    Player = new Player();
                    Enemy = new Enemy();


                    continue;
                }

                try
                {
                    //Open Total War Attila
                    Games.StartTotalWArAttilaProcess();

                    LoadingScreen.ChangeMessage("Done!");
                    LoadingScreen.Close();
                    this.Show();
                }
                catch
                {
                    this.Show();
                    LoadingScreen.Close();
                    MessageBox.Show("Couldn't find 'Attila.exe'. Change the Total War Attila path. ", "Path Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    infoLabel.Text = "Ready to start!";
                    ProcessCommands.ResumeProcess();
                    ExecuteButton.Enabled = true;
                    this.Text = "Crusader Wars";
                    break;
                }

                try
                {
                    DataSearch.ClearLogFile();
                    DeclarationsFile.Erase();
                    BattleScript.EraseScript();
                    BattleResult.ClearAttilaLog();
                    UnitMapper.ShowRequiredMods(); //<-- button ok
                }
                catch
                {
                    
                    MessageBox.Show("Error", "Application Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Games.CloseTotalWarAttilaProcess();
                    Games.StartCrusaderKingsProcess();
                    infoLabel.Text = "Waiting for battle...";
                    this.Text = "Crusader Wars (Waiting for battle...)";

                    //Data Clear
                    Data.Reset();
                    Player = new Player();
                    Enemy = new Enemy();

                    continue;
                }

                Games.CloseCrusaderKingsProcess();

                Console.WriteLine("Conversion Completed");
                infoLabel.Text = "Battle Converted!";

                //               Retrieve battle result to ck3
                //-----------------------------------------------------------
                //                       Battle Results                     |
                //-----------------------------------------------------------

                string attilaLogPath = Properties.Settings.Default.VAR_log_attila;
                
                bool battleEnded = false;

                infoLabel.Text = "Waiting battle to end...";
                this.Text = "Crusader Wars (Waiting battle to end...)";

                while (battleEnded == false)
                {
                    battleEnded = BattleResult.HasBattleEnded(attilaLogPath);
                    await Task.Delay(10);
                }

                if(battleEnded)
                {
                    ModOptions.CloseAttila();
                    RequiredModsMessage.CloseAllWindows();

                    infoLabel.Text = "Battle has ended!";
                    this.Text = "Battle has eneded";
                    string path_log_attila = Properties.Settings.Default.VAR_log_attila;


                    //Attila Remaining Soldiers
                    foreach(var army in attacker_armies)
                    {
                        BattleResult.GetUnitsData(army, path_log_attila);
                    }
                    foreach(var army in defender_armies)
                    {
                        BattleResult.GetUnitsData(army, path_log_attila);
                    }

                    
                    
                }

                /*
                if (battleEnded)
                {
                    ModOptions.CloseAttila();
                    RequiredModsMessage.CloseAllWindows();

                    infoLabel.Text = "Battle has ended!";
                    string path_log_attila = Properties.Settings.Default.VAR_log_attila;

                    List<(string Name, string Remaining)> Player_Remaining;
                    List<(string Name, string Remaining)> Enemy_Remaining;

                    string remaining_player_knights = "";
                    string remaining_enemy_knights = "";

                    string winner = "";

                    try
                    {
                        //Attila Remaining Soldiers

                        //Remaining_Soldiers_List = BattleResult.GetRemainingSoldiersData(path_log_attila);
                        BattleResult.GetUnitsData(Player, path_log_attila);
                        BattleResult.GetUnitsData(Enemy, path_log_attila);


                        Player_Remaining = new List<(string Name, string Remaining)>();
                        Enemy_Remaining = new List<(string Name, string Remaining)>();

                        if(Player.UnitsResults.Alive_PursuitPhase != null )
                        {
                            Player_Remaining = Player.UnitsResults.Alive_PursuitPhase;
                        }
                        else
                        {
                            Player_Remaining = Player.UnitsResults.Alive_MainPhase;
                        }
                        if (Enemy.UnitsResults.Alive_PursuitPhase != null)
                        {
                            Enemy_Remaining = Enemy.UnitsResults.Alive_PursuitPhase;
                        }
                        else
                        {
                            Enemy_Remaining = Enemy.UnitsResults.Alive_MainPhase;
                        }

                        //Army Ratio
                        for (int i = 0; i < Player_Remaining.Count; i++)
                        {
                            int number;
                            if (Int32.TryParse(Player_Remaining[i].Remaining, out number))
                            {
                                if (!Player_Remaining[i].Name.Contains("general"))
                                    number = ArmyProportions.SetResultsRatio(number);

                                Player_Remaining[i] = (Player_Remaining[i].Name, number.ToString());
                            }
                            else
                            {
                                continue;
                            }
                        }
                        for (int i = 0; i < Enemy_Remaining.Count; i++)
                        {
                            int number;
                            if (Int32.TryParse(Enemy_Remaining[i].Remaining, out number))
                            {
                                if (!Enemy_Remaining[i].Name.Contains("general"))
                                    number = ArmyProportions.SetResultsRatio(number);

                                Enemy_Remaining[i] = (Enemy_Remaining[i].Name, number.ToString());
                            }
                            else
                            {
                                continue;
                            }
                        }


                        remaining_player_knights = Player_Remaining.Where(x => x.Name.Contains("player_general_knights"))
                                           .Select(p => p.Remaining)
                                           .FirstOrDefault();

                        remaining_enemy_knights = Enemy_Remaining.Where(x => x.Name.Contains("enemy_general_knights"))
                                                                     .Select(p => p.Remaining)
                                                                     .FirstOrDefault();



                        Player.Commander.HasGeneralFallen(path_log_attila, Player);
                        Enemy.Commander.HasGeneralFallen(path_log_attila, Enemy);


                        if (remaining_player_knights != null) Player.Knights.GetKilled(Int32.Parse(remaining_player_knights));

                        if (remaining_enemy_knights != null) Enemy.Knights.GetKilled(Int32.Parse(remaining_enemy_knights));

                        
                        //Commanders Health System
                        //Player.Commander.Health();
                        //Enemy.Commander.Health();

                        //Knights Health System
                        //Player.Knights.Health();
                        //Enemy.Knights.Health();

                        winner = BattleResult.GetAttilaWinner(attilaLogPath, Player.CombatSide, Enemy.CombatSide);

                    }
                    catch
                    {
                        MessageBox.Show("Error reading the battle results", "Battle Results Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        Games.CloseTotalWarAttilaProcess();
                        Games.StartCrusaderKingsProcess();
                        infoLabel.Text = "Waiting for battle...";
                        this.Text = "Crusader Wars (Waiting for battle...)";

                        //Data Clear
                        Data.Reset();
                        Player = new Player();
                        Enemy = new Enemy();

                        continue;
                    }




                    try
                    {
                        bool side = false; ;
                        if (Player.CombatSide == "attacker") side = false;//Attacker Player and Defender Enemy
                        else side = true; //Defender Player and Attacker Enemy
                        BattleResult.EditCombatResults(Player, Enemy, side);

                        BattleResult.SetWinner(winner);

                        BattleResult.SendToSaveFile(path_editedSave);
                        Games.LoadBattleResults();
                    }
                    catch
                    {
                        MessageBox.Show("Corrupted save.", "Battle Results Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        Games.CloseTotalWarAttilaProcess();
                        Games.StartCrusaderKingsProcess();
                        infoLabel.Text = "Waiting for battle...";
                        this.Text = "Crusader Wars (Waiting for battle...)";

                        //Data Clear
                        Data.Reset();
                        Player = new Player();
                        Enemy = new Enemy();

                        continue;
                    }


                    ProcessCommands.ResumeProcess();
                    infoLabel.Text = "Battle results sent!";

                }
                */

                await Task.Delay(10);

                //Clear data
                Player = new Player();
                Enemy = new Enemy();
                UnitMapper.ClearData();
                DeclarationsFile.Declarations.Clear();
                ArmyProportions.ResetUnitSizes();
                GC.Collect();

            }

          
            
        }
        

        /*---------------------------------------------
         * :::::::::::::PROCESS COMMANDS:::::::::::::::
         ---------------------------------------------*/

        struct ProcessCommands
        {
            private static string ProcessRuntime(string command)
            {
                //Get User Path
                string filePath = Directory.GetFiles(@".\data\runtime", "pssuspend64.exe", SearchOption.AllDirectories)[0];
                ProcessStartInfo procStartInfo = new ProcessStartInfo(filePath, command)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true

                };

                using (Process proc = new Process())
                {
                    proc.StartInfo = procStartInfo;
                    proc.Start();
                    return proc.StandardOutput.ReadToEnd();
                }

            }
            public static void SuspendProcess()
            {
                ProcessRuntime("ck3.exe");

            }

            public static void ResumeProcess()
            {
                ProcessRuntime("/r ck3.exe");
            }


        }

        


        /*---------------------------------------------
         * :::::::::::GAMES INICIALIZATION:::::::::::::
         ---------------------------------------------*/
        struct Games
        {
            public static void StartCrusaderKingsProcess()
            {

                Process[] process_ck3 = Process.GetProcessesByName("ck3");
                if (process_ck3.Length == 0)
                {
                    Process.Start(Properties.Settings.Default.VAR_ck3_path);
                    DataSearch.ClearLogFile();
                }

            }

            public static void CloseCrusaderKingsProcess()
            {
                Process[] process_ck3 = Process.GetProcessesByName("ck3");
                foreach (Process worker in process_ck3)
                {
                    worker.Kill();
                    worker.WaitForExit();
                    worker.Dispose();
                }

             
            }

            public static void LoadBattleResults()
            {
                string ck3_path = Properties.Settings.Default.VAR_ck3_path;
                Process.Start(ck3_path, "--continuelastsave");
            }

            public static async void StartTotalWArAttilaProcess()
            {
                Process.Start("steam://rungameid/325610");
                await Task.Delay(2000);
                Process[] process_launcher = Process.GetProcessesByName("launcher");
                if (process_launcher.Length == 0)
                {
                    Process[] process_attila = Process.GetProcessesByName("Total War: Attila");
                    if (process_attila.Length == 0)
                    {
                        Process.Start(Properties.Settings.Default.VAR_attila_path);

                    }
                }
            }

            public async static void CloseTotalWarAttilaProcess()
            {
                Process[] process_attila = Process.GetProcessesByName("Attila");
                foreach (Process worker in process_attila)
                {
                    worker.Kill();
                    worker.WaitForExit();
                    worker.Dispose();
                }

                await Task.Delay(1000);

            }
        };


      /*---------------------------------------------
       * :::::::::::LOW-LEVEL FUNCTIONS  :::::::::::::
       ---------------------------------------------*/
        private string RemoveASCII(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            string apostrophe = "'";
            foreach (char c in inputString)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == '\n' || c == '-' || c == ':' || c == ' '|| char.IsLetter(c) || c == '?' || c == apostrophe[0] || c== '%')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private void LoadDLLs()
        {
            try
            {
                string dll_folder = @".\data\dlls";
                foreach (string dllFile in Directory.GetFiles(dll_folder, "*.dll"))
                {
                    Assembly assembly = Assembly.LoadFrom(dllFile);
                    AppDomain.CurrentDomain.Load(assembly.GetName());
                }
            }
            catch
            {
                return;
            }

        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            SettingsBtn.BackgroundImage = Properties.Resources.options_btn_new_click;
            sounds = new SoundPlayer(@".\data\sounds\metal-dagger-hit-185444.wav");
            sounds.Play();
            Options optionsChild = new Options();
            optionsChild.ShowDialog();
        }


        private void HomePage_FormClosing(object sender, FormClosingEventArgs e)
        {
            ProcessCommands.ResumeProcess();
        }

        SoundPlayer sounds;

        private void patreonBtn_Click(object sender, EventArgs e)
        {
            patreonBtn.BackgroundImage = Properties.Resources.patreon_btn_clickpng;
            sounds = new SoundPlayer(@".\data\sounds\metal-dagger-hit-185444.wav");
            sounds.Play();
            Process.Start("https://www.patreon.com/user?u=83859552");

        }

        private void WebsiteBTN_Click(object sender, EventArgs e)
        {
            WebsiteBTN.BackgroundImage = Properties.Resources.website_btn_new_click;
            sounds = new SoundPlayer(@".\data\sounds\metal-dagger-hit-185444.wav");
            sounds.Play();
            Process.Start("https://www.crusaderwars.com");

        }

        private void SteamBTN_Click(object sender, EventArgs e)
        {
            SteamBTN.BackgroundImage = Properties.Resources.steam_btn_new_click;
            sounds = new SoundPlayer(@".\data\sounds\metal-dagger-hit-185444.wav");
            sounds.Play();
            Process.Start("https://steamcommunity.com/sharedfiles/filedetails/?id=2977969008");
        }

        private void patreonBtn_MouseEnter(object sender, EventArgs e)
        {
            patreonBtn.BackgroundImage = Properties.Resources.patreon_btn_hover;
        }

        private void patreonBtn_MouseLeave(object sender, EventArgs e)
        {
            patreonBtn.BackgroundImage = Properties.Resources.patreon_btn_new;
        }

        private void patreonBtn_MouseHover_1(object sender, EventArgs e)
        {
            patreonBtn.BackgroundImage = Properties.Resources.patreon_btn_hover;
        }

        private void ExecuteButton_MouseEnter(object sender, EventArgs e)
        {
            if (ExecuteButton.Enabled)
                ExecuteButton.BackgroundImage = Properties.Resources.start_new_hover;
        }

        private void ExecuteButton_MouseHover(object sender, EventArgs e)
        {
            if(ExecuteButton.Enabled)
            ExecuteButton.BackgroundImage = Properties.Resources.start_new_hover;
        }

        private void ExecuteButton_MouseLeave(object sender, EventArgs e)
        {
            if (ExecuteButton.Enabled)
                ExecuteButton.BackgroundImage = Properties.Resources.start_new;
        }

        private void SettingsBtn_MouseHover(object sender, EventArgs e)
        {
            SettingsBtn.BackgroundImage = Properties.Resources.options_btn_new_hover;
        }

        private void SettingsBtn_MouseLeave(object sender, EventArgs e)
        {
            SettingsBtn.BackgroundImage = Properties.Resources.options_btn_new;
        }

        private void SettingsBtn_MouseEnter(object sender, EventArgs e)
        {
            SettingsBtn.BackgroundImage = Properties.Resources.options_btn_new_hover;
        }

        private void WebsiteBTN_MouseEnter(object sender, EventArgs e)
        {
            WebsiteBTN.BackgroundImage = Properties.Resources.website_btn_new_hover1;
        }

        private void WebsiteBTN_MouseHover(object sender, EventArgs e)
        {
            WebsiteBTN.BackgroundImage = Properties.Resources.website_btn_new_hover1;
        }

        private void WebsiteBTN_MouseLeave(object sender, EventArgs e)
        {
            WebsiteBTN.BackgroundImage = Properties.Resources.website_btn_new;
        }

        private void SteamBTN_MouseEnter(object sender, EventArgs e)
        {
            SteamBTN.BackgroundImage = Properties.Resources.steam_btn_new_hover1;
        }

        private void SteamBTN_MouseHover(object sender, EventArgs e)
        {
            SteamBTN.BackgroundImage = Properties.Resources.steam_btn_new_hover1;
        }

        private void SteamBTN_MouseLeave(object sender, EventArgs e)
        {
            SteamBTN.BackgroundImage = Properties.Resources.steam_btn_new;
        }
    }
}