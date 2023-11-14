using Crusader_Wars.twbattle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Crusader_Wars
{
    /*
     * IMPORTANT NOTE
     * ----------------------------
     * The writter gives some extra new lines '\n'
     * might remove them later
     */
    internal static class Reader
    {
        public static void ReadFile(string savePath)
        {
            using (FileStream saveFile = File.Open(savePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(saveFile))
            {
                string line = reader.ReadLine();
                while(line != null && !reader.EndOfStream) 
                {
                    line = reader.ReadLine();
                    GetterKeys.ReadProvinceBuildings(line, "5984");
                    SearchKeys.TraitsList(line);
                    SearchKeys.Combats(line);
                    SearchKeys.Regiments(line);
                    SearchKeys.ArmyRegiments(line);
                    SearchKeys.Living(line);
                }

                reader.Close();
                saveFile.Close();
            }

            Data.ConvertDataToString();
        }


        static bool NeedSkiping { get;set; }
        public static void SendDataToFile(string savePath)
        {

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


                    if (line == "\tcombats={" && !NeedSkiping)
                    {
                        streamWriter.WriteLine(Data.String_Combats);
                        Console.WriteLine("EDITED COMBATS SENT!");
                        NeedSkiping = true;
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
                    else if (!NeedSkiping)
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

            Console.WriteLine($"----\nWritting data to save file\nMemory Usage: {memoryUsage/1048576} megabytes");
        }

    }

    internal static class Data
    {
        //Sieges
        public static List<string> Province_Buildings = new List<string>();

        public static StringBuilder Traits = new StringBuilder();
        public static StringBuilder Combats = new StringBuilder();
        public static StringBuilder Living = new StringBuilder();
        public static StringBuilder ArmyRegiments = new StringBuilder();
        public static StringBuilder Regiments = new StringBuilder();

        public static string String_Traits;
        public static string String_Combats;
        public static string String_Living;
        public static string String_ArmyRegiments;
        public static string String_Regiments;

        public static void ConvertDataToString()
        {
            long startMemory = GC.GetTotalMemory(false);

            String_Traits = Traits.ToString();
            String_Combats = Combats.ToString();
            String_Living = Living.ToString();
            String_ArmyRegiments = ArmyRegiments.ToString();
            String_Regiments = Regiments.ToString();

            long endMemory = GC.GetTotalMemory(false);
            long memoryUsage = endMemory - startMemory;

            Console.WriteLine($"----\nConvert all data to string\nMemory Usage: {memoryUsage / 1048576} mb");
        }

        public static void Reset()
        {
            Traits = new StringBuilder();
            Combats = new StringBuilder();
            Living = new StringBuilder();
            ArmyRegiments = new StringBuilder ();
            Regiments = new StringBuilder ();

            String_Traits = "";
            String_Combats = "";
            String_Living = "";
            String_ArmyRegiments= "";
            String_Regiments = "";

            SearchKeys.HasTraitsExtracted = false;
            SearchKeys.HasCombatsExtracted = false;
            SearchKeys.HasLivingExtracted = false;
            SearchKeys.HasArmyRegimentsExtracted = false;
            SearchKeys.HasRegimentsExtracted = false;
        }
    }

    struct GetterKeys
    {
        static bool isSearchPermitted = false;
        static bool isSearchBuildingsPermitted = false;
        static bool isExtractBuildingsPermitted = false;
        public static void ReadProvinceBuildings(string line, string province_id)
        {
            
            if(line.Contains("provinces={"))
            {
                isSearchPermitted = true;
            }

            
            if(isSearchPermitted && line.Contains($"\t{province_id}={{"))
            {
                isSearchBuildingsPermitted = true;
            }

            if(isSearchBuildingsPermitted && line.Contains("buildings={"))
            {
                isExtractBuildingsPermitted = true;
            }

            if(isExtractBuildingsPermitted)
            {
                if (line.Contains("type="))
                {

                    string building_key = Regex.Match(line, @"=(.+)").Groups[1].Value.Trim('"').Trim('/');
                    Data.Province_Buildings.Add(building_key);
                }


            }


            //last line of the province data
            //stop searching
            if( isSearchBuildingsPermitted && line.Contains("fort_level="))
            {
                string fort_level = Regex.Match(line, @"=(.+)").Groups[1].Value.Trim('"').Trim('/');
                if(int.TryParse(fort_level, out int level))
                {
                    Sieges.SetFortLevel(level);
                }
                else
                {
                    Sieges.SetFortLevel(0);
                }
                
                isExtractBuildingsPermitted = false;
                isSearchBuildingsPermitted = false;
                isSearchPermitted = false;
                return;

            }
        }
    };

    struct SearchKeys
    {
        private static bool Start_TraitsFound { get; set; }
        private static bool End_TraitsFound { get; set; }
        public static bool HasTraitsExtracted { get; set; }

        public static void TraitsList(string line)
        {
            if (!HasTraitsExtracted)
            {
                if (!Start_TraitsFound)
                {
                    if (line.Contains("traits_lookup={"))
                    {
                        Start_TraitsFound = true; Console.WriteLine("TRAITS START KEY FOUND!");
                    }
                    else { Start_TraitsFound = false; }
                }

                if (Start_TraitsFound && !End_TraitsFound)
                {
 
                    if (line == "provinces={")
                    {
                        End_TraitsFound = true;
                        Console.WriteLine("TRAITS END KEY FOUND!");
                        return;
                    }
                    else { End_TraitsFound = false; }

                    Data.Traits.Append(line + "\n");

                }

                if (End_TraitsFound)
                {
                    HasTraitsExtracted = true;
                    Start_TraitsFound = false;
                    End_TraitsFound = false;
                }
            }
        }

        private static bool Start_CombatsFound { get; set; }
        private static bool End_CombatsFound { get; set; }
        public static bool HasCombatsExtracted { get; set; }

        public static void Combats(string line)
        {
            if(!HasCombatsExtracted)
            {
                if (!Start_CombatsFound)
                {
                    if (line == "\tcombats={") { 
                        Start_CombatsFound = true; Console.WriteLine("COMBAT START KEY FOUND!"); 
                    }
                    else { Start_CombatsFound = false; }
                }

                if(Start_CombatsFound && !End_CombatsFound)
                {
                    //Match end = Regex.Match(line, @"pending_character_interactions={");
                    if (line == "pending_character_interactions={") 
                    {
                        End_CombatsFound = true; 
                        Console.WriteLine("COMBAT END KEY FOUND!");
                        return;
                    }
                    else { End_CombatsFound = false; }

                    Data.Combats.Append(line + "\n");
                    
                }

                if(End_CombatsFound)
                {
                    HasCombatsExtracted = true;
                    Start_CombatsFound = false; 
                    End_CombatsFound = false;
                }
            }
        }

        private static bool Start_RegimentsFound { get; set; }
        private static bool End_RegimentsFound { get; set; }
        public static bool HasRegimentsExtracted { get; set; }
        public static void Regiments(string line)
        {
            if (!HasRegimentsExtracted)
            {
                if (!Start_RegimentsFound)
                {
                    if (line == "\tregiments={") { Start_RegimentsFound = true; Console.WriteLine("REGIMENTS START KEY FOUND!"); }
                    else { Start_RegimentsFound = false; }
                }

                if (Start_RegimentsFound && !End_RegimentsFound)
                {
                  
                    if (line == "\tarmy_regiments={") 
                    { 
                        End_RegimentsFound = true; 
                        Console.WriteLine("REGIMENTS END KEY FOUND!");
                        return;
                    }
                    else { End_RegimentsFound = false; }

                    Data.Regiments.Append(line + "\n");
                }

                if (End_RegimentsFound)
                {
                    HasRegimentsExtracted = true;
                    Start_RegimentsFound = false; 
                    End_RegimentsFound = false;
                    //Console.WriteLine(Data.Regiments);
                }
            }
        }

        private static bool Start_ArmyRegimentsFound { get; set; }
        private static bool End_ArmyRegimentsFound { get; set; }
        public static bool HasArmyRegimentsExtracted { get; set; }
        public static void ArmyRegiments(string line)
        {
            if (!HasArmyRegimentsExtracted)
            {
                if (!Start_ArmyRegimentsFound)
                {
                    if (line == "\tarmy_regiments={") { Start_ArmyRegimentsFound = true; Console.WriteLine("ARMY REGIMENTS START KEY FOUND!"); }
                    else { Start_ArmyRegimentsFound = false; }
                }

                if (Start_ArmyRegimentsFound && !End_ArmyRegimentsFound)
                {
                    
                    if (line == "\tarmies={") 
                    { 
                        End_ArmyRegimentsFound = true; 
                        Console.WriteLine("ARMY REGIMENTS END KEY FOUND!");
                        return;
                    }
                    else { End_ArmyRegimentsFound = false; }

                    Data.ArmyRegiments.Append(line + "\n");
                }

                if (End_ArmyRegimentsFound)
                {
                    HasArmyRegimentsExtracted = true;
                    Start_ArmyRegimentsFound = false;
                    End_ArmyRegimentsFound = false;
                    //Console.WriteLine(Data.ArmyRegiments);
                }
            }
        }

        private static bool Start_LivingFound { get; set; }
        private static bool End_LivingFound { get; set; }
        public static bool HasLivingExtracted { get; set; }
        public static void Living(string line)
        {
            if (!HasLivingExtracted)
            {
                if (!Start_LivingFound)
                {
                    //Match start = Regex.Match(line, @"living={");
                    if (line == "living={") { Start_LivingFound = true; Console.WriteLine("LIVING START KEY FOUND!"); }
                    else { Start_LivingFound = false; }
                }

                if (Start_LivingFound && !End_LivingFound)
                {
                    if (line == "dead_unprunable={") 
                    { 
                        End_LivingFound = true; 
                        Console.WriteLine("LIVING END KEY FOUND!");
                        return;
                    }
                    else { End_LivingFound = false; }

                    Data.Living.Append(line + "\n");
                }

                if (End_LivingFound)
                {
                    HasLivingExtracted = true;
                    Start_LivingFound = false;
                    End_LivingFound = false;
                }
            }
        }
    }
}
