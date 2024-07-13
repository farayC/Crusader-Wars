using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crusader_Wars.data.save_file
{
    public static class Writter
    {
        static bool NeedSkiping { get; set; }
        static bool CombatResults_NeedsSkiping { get; set; }
        static bool Combats_NeedsSkiping { get; set; }
        public static void SendDataToFile(string savePath)
        {
            bool resultsFound = false;
            bool combatsFound = false;

            long startMemory = GC.GetTotalMemory(false);
            string tempFilePath = Directory.GetCurrentDirectory() + "\\CrusaderWars_Battle.ck3";

            using (var inputFileStream = new FileStream(savePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var outputFileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var streamReader = new StreamReader(inputFileStream))
            using (StreamWriter streamWriter = new StreamWriter(outputFileStream))
            {
                streamWriter.NewLine = "\n";
                string line;
                while ((line = streamReader.ReadLine()) != null || !streamReader.EndOfStream)
                {

                    //Line Skipper
                    if (NeedSkiping && line == "pending_character_interactions={")
                    {
                        NeedSkiping = false;
                    }
                    else if (CombatResults_NeedsSkiping && line == "\t\t}")
                    {
                        CombatResults_NeedsSkiping = false;
                        resultsFound = false;
                    }
                    else if (Combats_NeedsSkiping && line == "\t\t}")
                    {
                        Combats_NeedsSkiping = false;
                        combatsFound = false;
                    }
                    else if (NeedSkiping && line == "\tarmy_regiments={")
                    {
                        NeedSkiping = false;
                    }
                    else if (NeedSkiping && line == "\tarmies={")
                    {
                        NeedSkiping = false;
                    }
                    else if (NeedSkiping && line == "dead_unprunable={")
                    {
                        NeedSkiping = false;
                    }

                    //Achievements
                    if(line == "\tcan_get_achievements=no")
                    {
                        streamWriter.WriteLine("\tcan_get_achievements=yes");
                    }

                    //Combat Result START
                    if (line == "\tcombat_results={")
                    {
                        resultsFound = true;
                        streamWriter.WriteLine(line);
                    }
                    //Combat Result
                    else if (line == $"\t\t{BattleResult.ResultID}={{" && resultsFound && !CombatResults_NeedsSkiping)
                    {
                        WriteDataToSaveFile(streamWriter, DataFilesPaths.CombatResults_Path());
                        Console.WriteLine("EDITED BATTLE RESULTS SENT!");
                        CombatResults_NeedsSkiping = true;
                    }
                    //Combat START
                    else if (line == "\tcombats={")
                    {
                        combatsFound = true;
                        streamWriter.WriteLine(line);
                    }
                    //Combat
                    else if (line == $"\t\t{BattleResult.CombatID}={{" && combatsFound && !Combats_NeedsSkiping)
                    {
                        WriteDataToSaveFile(streamWriter, DataFilesPaths.Combats_Path());
                        Console.WriteLine("EDITED COMBATS SENT!");
                        Combats_NeedsSkiping = true;
                    }
                    else if (line == "\tregiments={" && !NeedSkiping)
                    {
                        //streamWriter.WriteLine(Data.String_Regiments);
                        Console.WriteLine("EDITED REGIMENTS SENT!");
                        NeedSkiping = true;
                    }
                    else if (line == "\tarmy_regiments={" && !NeedSkiping)
                    {
                        //streamWriter.WriteLine(Data.String_ArmyRegiments);
                        Console.WriteLine("EDITED ARMY REGIMENTS SENT!");
                        NeedSkiping = true;
                    }
                    else if (line == "living={" && !NeedSkiping)
                    {
                        //streamWriter.WriteLine(Data.String_Living);
                        Console.WriteLine("EDITED LIVING SENT!");
                        NeedSkiping = true;
                    }
                    else if (!NeedSkiping && !CombatResults_NeedsSkiping && !Combats_NeedsSkiping)
                    {
                        streamWriter.WriteLine(line);
                    }

                }

                streamWriter.Close();
                streamReader.Close();
                outputFileStream.Close();
                inputFileStream.Close();
            }

            string save_games_path = Properties.Settings.Default.VAR_dir_save;
            string editedSavePath = save_games_path + "\\CrusaderWars_Battle.ck3";

            File.Delete(savePath);
            File.Move(tempFilePath, editedSavePath);

            long endMemory = GC.GetTotalMemory(false);
            long memoryUsage = endMemory - startMemory;

            Console.WriteLine($"----\nWritting data to save file\nMemory Usage: {memoryUsage / 1048576} megabytes");
        }



        static void WriteDataToSaveFile(StreamWriter streamWriter, string data_file_path)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(data_file_path))
            {
                while (true)
                {
                    string l = sr.ReadLine();
                    if (l is null) break;
                    if (l == "\t\t}") continue;
                    sb.AppendLine(l);
                }
            }

            streamWriter.WriteLine(sb.ToString());
        }

        public struct DataFilesPaths
        {
            public static string CombatResults_Path() { return @".\data\save_file_data\BattleResults.txt"; }
            public static string Combats_Path() { return @".\data\save_file_data\Combats.txt"; }
            public static string Regiments_Path() { return @".\data\save_file_data\Regiments.txt"; }
            public static string ArmyRegiments_Path() { return @".\data\save_file_data\ArmyRegiments.txt"; }
            public static string Living_Path() { return @".\data\save_file_data\Living.txt"; }
            public static string Cultures_Path() { return @".\data\save_file_data\Cultures.txt"; }
            public static string Mercenaries_Path() { return @".\data\save_file_data\Mercenaries.txt"; }
            public static string Armies_Path() { return @".\data\save_file_data\Armies.txt"; }
            public static string Counties_Path() { return @".\data\save_file_data\Counties.txt"; }
            public static string Traits_Path() { return @".\data\save_file_data\Traits.txt"; }
            public static string Units_Path() { return @".\data\save_file_data\Units.txt"; }

        }

        public struct DataTEMPFilesPaths
        {
            public static string CombatResults_Path() { return @".\data\save_file_data\temp\BattleResults.txt"; }
            public static string Combats_Path() { return @".\data\save_file_data\temp\Combats.txt"; }
            public static string Regiments_Path() { return @".\data\save_file_data\temp\Regiments.txt"; }
            public static string ArmyRegiments_Path() { return @".\data\save_file_data\temp\ArmyRegiments.txt"; }
            public static string Living_Path() { return @".\data\save_file_data\temp\Living.txt"; }
            public static string Cultures_Path() { return @".\data\save_file_data\temp\Cultures.txt"; }
            public static string Mercenaries_Path() { return @".\data\save_file_data\temp\Mercenaries.txt"; }
            public static string Armies_Path() { return @".\data\save_file_data\temp\Armies.txt"; }
            public static string Counties_Path() { return @".\data\save_file_data\temp\Counties.txt"; }
            public static string Traits_Path() { return @".\data\save_file_data\temp\Traits.txt"; }
            public static string Units_Path() { return @".\data\save_file_data\temp\Units.txt"; }

        }

    }
}

