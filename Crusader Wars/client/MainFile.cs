using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Xml.Linq;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

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
            RemoveFiles();            

            labelVersion.Text = $"V{Updater.AppVersion}";

            _timer = new Timer();
            _timer.Interval = 500; // check variable every second
            _timer.Tick += Timer_Tick;
            _timer.Start();
            Original_Color = infoLabel.ForeColor;
        }


        void RemoveFiles()
        {

            string remove_file = @".\Settings\filestoremove.txt";
            if (File.Exists(remove_file))
            {
                string[] files=  File.ReadAllLines(remove_file);
                foreach (string file in files) 
                {
                    try
                    {
                        //to delete files
                        if(File.Exists(file))
                        {
                            File.Delete(file);
                            continue;
                        }
                        //to delete folders
                        if(Directory.Exists(file))
                        {
                            Directory.Delete(file);
                            continue;
                        }
                        
                    }
                    catch
                    {
                        continue;
                    }

                }
                
                //Auto destroy itself
                File.Delete(remove_file);
            }
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
            btt_debug.Visible = false;

            Color myColor = Color.FromArgb(53, 25, 5, 5);
            infoLabel.BackColor = myColor;
            labelVersion.BackColor = myColor;


        }

        //---------------------------------//
        //----------DEBUG BUTTON-----------//
        //---------------------------------//

        private void btt_debug_Click(object sender, EventArgs e)
        {

        }
        

        Player Player;
        Enemy Enemy;

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
        private async void ExecuteButton_Click(object sender, EventArgs e)
        {
            _myVariable = 1;

            ExecuteButton.Enabled = false;


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
                    break;
                }

                try
                {
                    DataSearch.ClearLogFile();
                    DeclarationsFile.Erase();
                    BattleScript.EraseScript();
                    BattleResult.ClearAttilaLog();
                }
                catch
                {
                    MessageBox.Show("No Log File Found!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    infoLabel.Text = "Ready to start!";
                    ExecuteButton.Enabled = true;
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

                                    DataSearch.SettingsSearch(line);

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
                            break;
                        }
                        infoLabel.Text = "Battle found...";
                        logFile.Position = 0;
                        reader.DiscardBufferedData();
                        log = reader.ReadToEnd();
                        log = RemoveASCII(log);

                        if (battleHasStarted)
                        {
                            infoLabel.Text = "Reading data...";

                            Player = new Player();
                            Enemy = new Enemy();

                            DataSearch.SearchLanguage(); if (Languages.Language != "l_english") Languages.ShowWarningMessage();
                            DataSearch.Search(log, Player, Enemy);
                        }
                        try
                        {

                        }
                        catch
                        {
                            MessageBox.Show("Error reading battle data.", "Data Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            infoLabel.Text = "Waiting for battle...";
                            continue;
                        }

                        logFile.Position = 0;
                        reader.DiscardBufferedData();

                        reader.Close();
                        logFile.Close();

                    }

                }

                await Task.Delay(3000);
                ProcessCommands.SuspendProcess();

                path_editedSave = Properties.Settings.Default.VAR_dir_save + @"\CrusaderWars_Battle.ck3";
                try
                {
                    long startMemoryTotal = GC.GetTotalMemory(false);



                    BattleResult.LoadSaveFile(path_editedSave);

                    long endMemory10 = GC.GetTotalMemory(false);
                    long memoryUsage10 = endMemory10 - startMemoryTotal;
                    Console.WriteLine($"----\nReading data from save file ...\nTotal Memory Usage: {memoryUsage10 / 1048576} mb\n----");

                    BattleResult.GetAllCombats();
                    BattleResult.FindPlayerBattle(Player.ID.ToString());
                    BattleResult.GetAttackerRegiments();
                    BattleResult.GetDefenderRegiments();
                    BattleResult.GetAllArmyRegiments();
                    BattleResult.GetAllRegiments();

                }
                catch
                {
                    MessageBox.Show("Error reading the save file. Disable Ironman or Debug Mode.", "Save File Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    infoLabel.Text = "Waiting for battle...";
                    ProcessCommands.ResumeProcess();
                    continue;
                }


                try
                {
                    BattleResult.GetCombatSides(Player, Enemy);
                }
                catch
                {
                    MessageBox.Show("Error reading the save file. Disable Ironman or Debug Mode.", "Save File Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    ProcessCommands.ResumeProcess();
                    infoLabel.Text = "Waiting for battle...";
                    continue;
                }



                try
                {
                    Games.CloseTotalWarAttilaProcess();

                    //Create Declarations
                    DeclarationsFile.CreateAlliances();

                    //Create Remaining Soldiers Script
                    BattleScript.CreateScript();

                    //Create Battle
                    ArmyProportions.AutoSizeUnits(Player.TotalNumber, Enemy.TotalNumber);
                    BattleFile.CreateBattle(Player, Enemy);

                    //Close Script
                    BattleScript.CloseScript();

                    //Set Units Declarations
                    DeclarationsFile.SetDeclarations();

                    //Creates .pack mod file
                    PackFile.PackFileCreator();
                }
                catch
                {
                    MessageBox.Show("Error creating the battle", "Data Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    ProcessCommands.ResumeProcess();
                    infoLabel.Text = "Waiting for battle...";
                    continue;
                }

                try
                {
                    //Open Total War Attila
                    Games.StartTotalWArAttilaProcess();
                }
                catch
                {
                    MessageBox.Show("Couldn't find 'Attila.exe'. Change the Total War Attila path. ", "Path Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    infoLabel.Text = "Ready to start!";
                    ProcessCommands.ResumeProcess();
                    ExecuteButton.Enabled = true;
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
                    continue;
                }

                Games.CloseCrusaderKingsProcess();

                Console.WriteLine("Conversion Completed");
                infoLabel.Text = "Battle Converted!";

                // Retrieve battle result to ck3
                //-----------------------------------------------------------
                //                       Battle Results                     |
                //-----------------------------------------------------------

                string attilaLogPath = Properties.Settings.Default.VAR_log_attila;
                
                bool battleEnded = false;

                infoLabel.Text = "Waiting battle to end...";

                try
                {
                    while (battleEnded == false)
                    {
                        battleEnded = BattleResult.HasBattleEnded(attilaLogPath);
                        await Task.Delay(10);
                    }
                }
                catch
                {
                    MessageBox.Show("Error while waiting for battle results", "Data Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Games.CloseTotalWarAttilaProcess();
                    Games.StartCrusaderKingsProcess();
                    infoLabel.Text = "Waiting for battle...";
                    continue;
                }



                if (battleEnded)
                {

                    infoLabel.Text = "Battle has ended!";
                    string path_log_attila = Properties.Settings.Default.VAR_log_attila;
                    List<(string Name, string Remaining)> Remaining_Soldiers_List;

                    List<(string Name, string Remaining)> Player_Remaining;
                    List<(string Name, string Remaining)> Enemy_Remaining;

                    string remaining_player_knights = "";
                    string remaining_enemy_knights = "";

                    string winner = "";

                    try
                    {
                        //Attila Remaining Soldiers

                        long startMemory = GC.GetTotalMemory(false);
                        Remaining_Soldiers_List = BattleResult.GetRemainingSoldiersData(path_log_attila);
                        long endMemory = GC.GetTotalMemory(false);
                        long memoryUsage = endMemory - startMemory;

                        Console.WriteLine($"----\nGetting from Attila log the remaining soldiers...\nMemory Usage: {memoryUsage / 1048576} mb\n----");

                        Player_Remaining = new List<(string Name, string Remaining)>();
                        Enemy_Remaining = new List<(string Name, string Remaining)>();

                        //Army Ratio
                        for (int i = 0; i < Remaining_Soldiers_List.Count; i++)
                        {
                            int number;
                            if (Int32.TryParse(Remaining_Soldiers_List[i].Remaining, out number))
                            {
                                if (!Remaining_Soldiers_List[i].Name.Contains("general"))
                                    number = ArmyProportions.SetResultsRatio(number);

                                Remaining_Soldiers_List[i] = (Remaining_Soldiers_List[i].Name, number.ToString());
                            }
                            else
                            {
                                continue;
                            }
                        }

                        foreach (var unit in Remaining_Soldiers_List)
                        {

                            if (unit.Name.StartsWith("player"))
                            {
                                Player_Remaining.Add(unit);
                            }

                            if (unit.Name.StartsWith("enemy"))
                            {
                                Enemy_Remaining.Add(unit);
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
                        SaveFile.ReadAll(); //memory intensive
                        Player.Commander.Health();
                        Enemy.Commander.Health();

                        //Knights Health System
                        Player.Knights.Health(Player.ID.ToString());
                        Enemy.Knights.Health(Enemy.ID.ToString());
                        SaveFile.SendToFile(); //hmmm sus...

                        winner = BattleResult.GetAttilaWinner(attilaLogPath, Player.CombatSide, Enemy.CombatSide);

                        switch (Player.CombatSide)
                        {
                            case "attacker":

                                BattleResult.SetRemainingAttacker(Player_Remaining);
                                BattleResult.SetRemainingDefender(Enemy_Remaining);

                                break;
                            case "defender":

                                BattleResult.SetRemainingAttacker(Enemy_Remaining);
                                BattleResult.SetRemainingDefender(Player_Remaining);

                                break;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Error reading the battle results", "Battle Results Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        Games.CloseTotalWarAttilaProcess();
                        Games.StartCrusaderKingsProcess();
                        infoLabel.Text = "Waiting for battle...";
                        continue;
                    }




                    try
                    {
                        BattleResult.SetAttackerGUIRegiments();
                        BattleResult.SetDefenderGUIRegiments();
                        BattleResult.SetWinner(Player.ID.ToString(), winner);
                        BattleResult.SetAttackerDATA();
                        BattleResult.SetDefenderDATA();
                        BattleResult.SendToSaveFile(path_editedSave); //hmmmm sus sus...
                        Games.LoadBattleResults();
                    }
                    catch
                    {
                        MessageBox.Show("Corrupted save.", "Battle Results Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        Games.CloseTotalWarAttilaProcess();
                        Games.StartCrusaderKingsProcess();
                        infoLabel.Text = "Waiting for battle...";
                        continue;
                    }


                    ProcessCommands.ResumeProcess();
                    infoLabel.Text = "Battle results sent!";

                }

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
                string filePath = Directory.GetFiles("runtime", "pssuspend64.exe", SearchOption.AllDirectories)[0];
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
        

  
        private string RemoveASCII(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in inputString)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == '\n' || c == '-' || c == ':' || c == ' '|| char.IsLetter(c) || c == '?' )
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
                string dll_folder = @".\dlls";
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
            Options optionsChild = new Options();
            optionsChild.ShowDialog();
        }

        private void Info_Status_MouseHover(object sender, EventArgs e)
        {
            InformationToolTip.ToolTipTitle = "Crusader Wars Pre Beta";

            InformationToolTip.SetToolTip(Info_Status, "Tips to work properly:\n" + 
                                                       "1-Disable '1-1212scripts.pack' for the mod to work\n" +
                                                       "2-In Total War Attila set Unit Size to Ultra\n");
        }

        private void HomePage_FormClosing(object sender, FormClosingEventArgs e)
        {
            ProcessCommands.ResumeProcess();
        }


        private void ExecuteButton_MouseHover(object sender, EventArgs e)
        {
            if(ExecuteButton.Enabled)
            {
                InformationToolTip.ToolTipTitle = "";
                InformationToolTip.SetToolTip(ExecuteButton, "Start");
            }
            else
            {
                InformationToolTip.ToolTipTitle = "";
                InformationToolTip.SetToolTip(ExecuteButton, "Running...");
            }

        }


        private void btnWebsite_Click(object sender, EventArgs e)
        {
            Process.Start("https://crusaderwars.com/");
        }

        private void btnWebsite_MouseHover(object sender, EventArgs e)
        {
            InformationToolTip.ToolTipTitle = "Check our website!";
            InformationToolTip.SetToolTip(btnWebsite, "You can find useful information about the mod.");
        }

        private void btnDiscord_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/8tVhPMRT9A");
        }

        private void btnDiscord_MouseHover(object sender, EventArgs e)
        {
            InformationToolTip.ToolTipTitle = "Join our community!";
            InformationToolTip.SetToolTip(btnDiscord, "If you need support ask us, we answer fast.");
        }

        private void btnSteam_Click(object sender, EventArgs e)
        {
            Process.Start("https://steamcommunity.com/sharedfiles/filedetails/?id=2977969008");
        }

        private void btnSteam_MouseHover(object sender, EventArgs e)
        {
            InformationToolTip.ToolTipTitle = "Crusader Kings 3 required mod";
            InformationToolTip.SetToolTip(btnSteam, "Subscribe to Crusader Wars mod.");
        }

        private void btnPatch_Click(object sender, EventArgs e)
        {
            Process.Start("https://crusaderwars.com/patches/");
        }

        private void btnPatch_MouseHover(object sender, EventArgs e)
        {
            InformationToolTip.ToolTipTitle = "Check recent update changes!";
            InformationToolTip.SetToolTip(btnPatch, "See what we added, fixed, improved, etc..");
        }
    }
}