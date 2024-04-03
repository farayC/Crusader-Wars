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

                    if (line == "\tcombat_results={")
                    {
                        resultsFound = true;
                        streamWriter.WriteLine(line);
                    }
                    //Combat Result
                    else if (line == $"\t\t{BattleResult.ResultID}={{" && resultsFound && !CombatResults_NeedsSkiping)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        using (StreamReader sr = new StreamReader(@".\data\save_file_data\BattleResults.txt"))
                        {
                            while (true)
                            {
                                string l = sr.ReadLine();

                                if (l is null) break;
                                if (l == "\t\t}") continue;

                                stringBuilder.AppendLine(l);
                            }
                        }

                        streamWriter.WriteLine(stringBuilder.ToString());
                        Console.WriteLine("EDITED BATTLE RESULTS SENT!");
                        CombatResults_NeedsSkiping = true;
                    }
                    else if (line == "\tcombats={")
                    {
                        combatsFound = true;
                        streamWriter.WriteLine(line);
                    }
                    //Combat
                    else if (line == $"\t\t{BattleResult.CombatID}={{" && combatsFound && !Combats_NeedsSkiping)
                    {
                        StringBuilder sb = new StringBuilder();
                        using (StreamReader sr = new StreamReader(@".\data\save_file_data\Combats.txt"))
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
                        Console.WriteLine("EDITED COMBATS SENT!");
                        Combats_NeedsSkiping = true;
                    }
                    else if (line == "\tregiments={" && !NeedSkiping)
                    {
                        streamWriter.WriteLine(Data.String_Regiments);
                        Console.WriteLine("EDITED REGIMENTS SENT!");
                        NeedSkiping = true;
                    }
                    else if (line == "\tarmy_regiments={" && !NeedSkiping)
                    {
                        streamWriter.WriteLine(Data.String_ArmyRegiments);
                        Console.WriteLine("EDITED ARMY REGIMENTS SENT!");
                        NeedSkiping = true;
                    }
                    else if (line == "living={" && !NeedSkiping)
                    {
                        streamWriter.WriteLine(Data.String_Living);
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

    }
}

