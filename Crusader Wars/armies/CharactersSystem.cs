using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace Crusader_Wars
{

    struct MartialSkill
    {
        public static int Terrible() { return 0; }
        public static int Poor() { return 0; }
        public static int Average() { return 1; }
        public static int Good() { return 2; }
        public static int Excellent() { return 3; }
    }

    struct DefenseSkill
    {
        public static int Terrible() { return 1; }
        public static int Poor() { return 2; }
        public static int Average() { return 3; }
        public static int Good() { return 4; }
        public static int Excellent() { return 5; }
    }

    struct Strength
    {
        public static double Terrible() { return 0.0; }
        public static double Poor() { return 0.1; }
        public static double Average() { return 0.2; }
        public static double Good() { return 0.3; }
        public static double Excellent() { return 0.4; }
    }

    struct Traits
    {
        public static int Wounded() { return SaveFile.GetWoundedTraitIndex("wounded_1"); }
        public static int Severely_Injured() { return SaveFile.GetWoundedTraitIndex("wounded_2"); }
        public static int Brutally_Mauled() { return SaveFile.GetWoundedTraitIndex("wounded_3"); }
        public static int Maimed() { return SaveFile.GetWoundedTraitIndex("maimed"); }
        public static int One_Legged() { return SaveFile.GetWoundedTraitIndex("one_legged"); }
        public static int One_Eyed() { return SaveFile.GetWoundedTraitIndex("one_eyed"); }
        public static int Disfigured() { return SaveFile.GetWoundedTraitIndex("disfigured"); }
    }

    public static  class SaveFile
    {
        static List<string> LivingList { get; set; }
        static List<(string name, int index)> WoundedTraits_List { get; set; }
        public static void ReadAll()
        {

            //---------Living text----------

            long startMemory1 = GC.GetTotalMemory(false);
            MatchCollection allLiving = Regex.Matches(Data.String_Living, @"(?s)\d+={.*?(?=\d+={|\z)");
            LivingList = new List<string>();
            foreach(Match match in allLiving)
            {
                LivingList.Add(match.Value);
            }
            long endMemory1 = GC.GetTotalMemory(false);
            long memoryUsage1 = endMemory1 - startMemory1;
            Console.WriteLine($"----\nAdding each char to Living list...\nMemory Usage: {memoryUsage1 / 1048576} mb");

            //---------Wounded Traits----------

            long startMemory = GC.GetTotalMemory(false);

            MatchCollection allTraits = Regex.Matches(Data.String_Traits, @" (\w+)");
            string[] traitList = new string[allTraits.Count];
            for(int i = 0; i < allTraits.Count; i++)
            {
                traitList[i] = allTraits[i].Groups[1].Value;
            }

            List<(string name, int index)> wounded_traits;
            wounded_traits = new List<(string name, int index)>();

            for(int i = 0; i < traitList.Length; i++)
            {
                if (traitList[i] == "wounded_1") wounded_traits.Add(("wounded_1", i));
                else if (traitList[i] == "wounded_2") wounded_traits.Add(("wounded_2", i));
                else if (traitList[i] == "wounded_3") wounded_traits.Add(("wounded_3", i));
                else if (traitList[i] == "maimed") wounded_traits.Add(("maimed", i));
                else if (traitList[i] == "one_legged") wounded_traits.Add(("one_legged", i));
                else if (traitList[i] == "one_eyed") wounded_traits.Add(("one_eyed", i));
                else if (traitList[i] == "disfigured") wounded_traits.Add(("disfigured", i));
            }

            WoundedTraits_List = new List<(string name, int index)>();
            WoundedTraits_List.AddRange(wounded_traits);

            long endMemory = GC.GetTotalMemory(false);
            long memoryUsage = endMemory - startMemory;
            Console.WriteLine($"----\nAdding each trait to list...\nMemory Usage: {memoryUsage / 1048576} mb");

        }

        internal static int GetWoundedTraitIndex(string trait_name)
        {
            int index;
            index = WoundedTraits_List.FirstOrDefault(x => x.name == trait_name).index;
            return index;
        }

        public static void SetTraits(string character_id, string trait)
        {

            bool check =  Regex.Match(Data.String_Living, $@"(?<Character>(?<ID>\b{character_id}\b)={{\s+first_name=[\s\S]*?)\d+={{").Success;
            if (check)
            {
                int index = LivingList.IndexOf(LivingList.FirstOrDefault(x => x.StartsWith($"{character_id}" + "={")));
                string match_living = LivingList.FirstOrDefault(x => x.StartsWith($"{character_id}" + "={" ));
                string traits = Regex.Match(match_living, @"traits=\{(?:\s+\d+)*").Value;
                traits = VerifyTraits(traits, trait);
                if (traits[traits.Length - 1] == ' ')
                {
                    string added_trait = Regex.Replace(match_living, @"traits=\{(?:\s+\d+)* }", traits + "}");
                    LivingList[index] = added_trait;
                }
                else
                {
                    string added_trait = Regex.Replace(match_living, @"traits=\{(?:\s+\d+)* }", traits + " }");
                    LivingList[index] = added_trait;
                }


            }            
        }

        public static void SendToFile()
        {
            long startMemory1 = GC.GetTotalMemory(false);
            StringBuilder sb = new StringBuilder();
            
            sb.Append("living={\n");
            foreach (string item in LivingList) 
            {
                sb.Append("\t" + item);
            }
            long endMemory1 = GC.GetTotalMemory(false);
            long memoryUsage1 = endMemory1 - startMemory1;
            Console.WriteLine($"----\nCreate Living string ...\nMemory Usage: {memoryUsage1 / 1048576} mb");


            long startMemory = GC.GetTotalMemory(false);

            Data.String_Living = "";
            
            Data.String_Living = sb.ToString();


            long endMemory = GC.GetTotalMemory(false);
            long memoryUsage = endMemory - startMemory;

            Console.WriteLine($"----\nTurning living text to string\nMemory Usage: {memoryUsage/1048576} mb");


            //Clear data
            LivingList = new List<string>();
            WoundedTraits_List = new List<(string name, int index)>();

        }

        private static string VerifyTraits(string str, string trait)
        {
            string[] ids = { Traits.Wounded().ToString(), Traits.Severely_Injured().ToString(), Traits.Brutally_Mauled().ToString(),
                             Traits.Maimed().ToString(), Traits.One_Eyed().ToString(),
                             Traits.One_Legged().ToString(), Traits.Disfigured().ToString() };

            //check if it is already wounded
            string already_wounded_trait="";
            foreach(string id in ids)
            {
                if (str.Contains($" {id} ") || str.Contains($" {id}"))
                {
                    already_wounded_trait = id;
                }
            }

            //if its not wounded, give wounded trait
            if(already_wounded_trait == string.Empty)
            {
                //Add trait if doesn't have one
                str += $" {trait}";

                return str;
            }
            else //if its already wounded, increase wound
            {
                string increased_wound_trait = Int32.Parse(already_wounded_trait)+1.ToString();
                string edited_str = Regex.Replace(str, $" {already_wounded_trait} | {already_wounded_trait}", $" {increased_wound_trait} ");

                return edited_str;
            }
        }
    }

    

    public class CommanderSystem
    {

        public string CommanderID { get; private set; }
        public string Name { get; private set; }
        public int Martial { get; private set; }
        public int Prowess { get;private set; }
        private bool hasFallen { get; set; }

        public void SetID(string charachter_id)
        {
            CommanderID= charachter_id;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetMartial(int martial)
        {
            Martial = martial;
        }

        public void SetProwess(int prowess)
        {
            Prowess = prowess;
        }

        public int SetUnitsExperience()
        {
            return MartialExperience();
        }

        public int SetCommanderExperience()
        {
            return ProwessExperience();
        }

        public int SetCommanderStarRating()
        {
            return StarExperience();
        }

        private int StarExperience()
        {
            int martial = Martial;
            int value = 0;

            if (martial <= 3)
            {
                //Terrible Martial
                value += 1;

            }
            else if (martial >= 4 && martial <= 7)
            {
                //Poor Martial
                value += 2;
            }
            else if (martial >= 8 && martial <= 11)
            {
                //Averege Martial
                value += 3;
            }
            else if (martial >= 12 && martial <= 15)
            {
                //Good Martial
                value += 4;
            }
            else if (martial >= 16)
            {
                //Excelent Martial
                value += 5;
            }

            if (value > 9) value = 9;
            return value;
        }

        private int ProwessExperience()
        {
            int prowess = Prowess;
            int value = 0;
            if (prowess <= 3)
            {
                value += MartialSkill.Terrible();

            }
            else if (prowess >= 4 && prowess <= 7)
            {
                value += MartialSkill.Poor();
            }
            else if (prowess >= 8 && prowess <= 11)
            {
                value += MartialSkill.Average();
            }
            else if (prowess >= 12 && prowess <= 15)
            {
                value += MartialSkill.Good();
            }
            else if (prowess >= 16)
            {
                value += MartialSkill.Excellent();
            }

            return value;
        }
        
        private int MartialExperience()
        {
            int martial = Martial;
            int value = 0;

            if (martial <= 3)
            {
                value += MartialSkill.Terrible();
                
            }
            else if (martial >= 4 && martial <= 7)
            {
                value += MartialSkill.Poor();
            }
            else if (martial >= 8 && martial <= 11)
            {
                value += MartialSkill.Average();
            }
            else if (martial >= 12 && martial <= 15)
            {
                value += MartialSkill.Good();
            }
            else if (martial >= 16)
            {
                value += MartialSkill.Excellent();
            }


            return value;

        }

        public void HasGeneralFallen(string path_attila_log, ICharacter Side)
        {
            using (FileStream logFile = File.Open(path_attila_log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(logFile))
            {
                string str = reader.ReadToEnd();

                if(Side is Player)
                {
                    if (str.Contains("Player general has fallen"))
                    {
                        hasFallen = true;
                        reader.Close();
                        logFile.Close();
                        return;
                    }
                }
                else
                {
                    if (str.Contains("Enemy general has fallen"))
                    {
                        hasFallen = true;
                        reader.Close();
                        logFile.Close();
                        return;
                    }
                }

                hasFallen = false;
            }
        }

        public void Health()
        {
            //50% Chance Light Wounds
            const int WoundedChance = 50; // 50%

            //30% Chance Harsh Wounds
            const int Severely_InjuredChance = 65; // 15%
            const int Brutally_MauledChance = 80; // 15%

            //20% Chance Extreme Wounds
            const int MaimedChance = 85; // 5%
            const int One_LeggedChance = 90;// 5%
            const int One_EyedChance = 95;// 5%
            const int Disfigured = 100; //5%

            var Chance = new Random();
            var RandomNumber = Chance.Next(101);

            string id = CommanderID;
            if(hasFallen)
            {

                Console.WriteLine($"Some general has fallen! - ");

                // Determine which option to set based on its percentage chance
                if (RandomNumber >= 0 && RandomNumber <= WoundedChance)
                {
                    SaveFile.SetTraits(id, Traits.Wounded().ToString());
                    Console.Write("Wounded ");
                    return;
                }
                else if (RandomNumber > WoundedChance && RandomNumber <= Severely_InjuredChance)
                {
                    SaveFile.SetTraits(id, Traits.Severely_Injured().ToString());
                    Console.Write("Severely_Injured ");
                    return;
                }
                else if (RandomNumber > Severely_InjuredChance && RandomNumber <= Brutally_MauledChance)
                {
                    SaveFile.SetTraits(id, Traits.Brutally_Mauled().ToString());
                    Console.Write("Brutally Mauled ");
                    return;
                }
                else if (RandomNumber > Brutally_MauledChance && RandomNumber <= MaimedChance)
                {
                    SaveFile.SetTraits(id, Traits.Maimed().ToString());
                    Console.Write("Maimed ");
                    return;
                }
                else if (RandomNumber > MaimedChance && RandomNumber <= One_LeggedChance)
                {
                    SaveFile.SetTraits(id, Traits.One_Legged().ToString());
                    Console.Write("One Legged ");
                    return;
                }
                else if (RandomNumber > One_LeggedChance && RandomNumber <= One_EyedChance)
                {
                    SaveFile.SetTraits(id, Traits.One_Eyed().ToString());
                    Console.Write("One Eyed ");
                    return;
                }
                else if (RandomNumber > One_EyedChance && RandomNumber <= Disfigured)
                {
                    SaveFile.SetTraits(id, Traits.Disfigured().ToString());
                    Console.Write("Disfigured ");
                    return;
                }

            }



        }
    }

    public class KnightSystem
    {

        private List<(string ID, int Prowess)> Knights { get; set; }

        private int UnitSoldiers { get; set; }

        private int Effectiveness { get; set; }

        private int KilledKnights { get; set; }
        private bool HasKnights { get; set; }

        private double KnightEffectiveness(int level)
        {
            if(level > 0)
            {
                double mulltiplier = Effectiveness / 100;

                double value_to_increase = level * mulltiplier;
                return value_to_increase;
            }
            else
            {
                return 0;
            }

        }


        public void Health(string side_id)
        {
            if(HasKnights)
            {
                //50% Chance Light Wounds
                const int WoundedChance = 50; // 50%

                //30% Chance Harsh Wounds
                const int Severely_InjuredChance = 65; // 15%
                const int Brutally_MauledChance = 80; // 15%

                //20% Chance Extreme Wounds
                const int MaimedChance = 85; // 5%
                const int One_LeggedChance = 90;// 5%
                const int One_EyedChance = 95;// 5%
                const int Disfigured = 100; //5%

                var Chance = new Random();
                int RandomNumber;

                Console.WriteLine("----------------------------------");
                Console.WriteLine($"KNIGHTS FALLEN - {KilledKnights}\n");

                for (int i = 0; i < KilledKnights; i++)
                {
                    //Random Knight
                    string id = Knights[i].ID;
                    RandomNumber = Chance.Next(101);

                    // Determine which option to set based on its percentage chance
                    if (RandomNumber >= 0 && RandomNumber <= WoundedChance)
                    {
                        SaveFile.SetTraits(id, Traits.Wounded().ToString());
                        Console.Write("Wounded ");
                        continue;
                    }
                    else if (RandomNumber > WoundedChance && RandomNumber <= Severely_InjuredChance)
                    {
                        SaveFile.SetTraits(id, Traits.Severely_Injured().ToString());
                        Console.Write("Severely_Injured ");
                        continue;
                    }
                    else if (RandomNumber > Severely_InjuredChance && RandomNumber <= Brutally_MauledChance)
                    {
                        SaveFile.SetTraits(id, Traits.Brutally_Mauled().ToString());
                        Console.Write("Brutally Mauled ");
                        continue;
                    }
                    else if (RandomNumber > Brutally_MauledChance && RandomNumber <= MaimedChance)
                    {
                        SaveFile.SetTraits(id, Traits.Maimed().ToString());
                        Console.Write("Maimed ");
                        continue;
                    }
                    else if (RandomNumber > MaimedChance && RandomNumber <= One_LeggedChance)
                    {
                        SaveFile.SetTraits(id, Traits.One_Legged().ToString());
                        Console.Write("One Legged ");
                        continue;
                    }
                    else if (RandomNumber > One_LeggedChance && RandomNumber <= One_EyedChance)
                    {
                        SaveFile.SetTraits(id, Traits.One_Eyed().ToString());
                        Console.Write("One Eyed ");
                        continue;
                    }
                    else if (RandomNumber > One_EyedChance && RandomNumber <= Disfigured)
                    {
                        SaveFile.SetTraits(id, Traits.Disfigured().ToString());
                        Console.Write("Disfigured ");
                        continue;
                    }

                    Console.WriteLine("No Trait Set! - ERROR");
                }

            }

        }


        public void GetKilled(int remaining)
        {
            if(HasKnights)
            {
                int totalSoldiers = UnitSoldiers;
                int remainingSoldiers = remaining;

                int knights_killed = 0;

                int numberOfKnights;
                if (Date.Year >= 1066)
                {
                    numberOfKnights = 10;
                }
                else
                {
                    numberOfKnights = 5;
                }

                //For 10 soldiers killed, 1 knight is killed
                for (int i = numberOfKnights; i <= totalSoldiers - remainingSoldiers; i += numberOfKnights)
                {
                    knights_killed++;
                }

                KilledKnights = knights_killed;
            }


        }


        public int SetExperience()
        {
            int prowess_level = (int)ProwessExperience();
            double effectiveness = KnightEffectiveness(prowess_level);
            int level = (int)Math.Round(prowess_level + effectiveness);
            
            return level;
        }

        public int SetKnightsCount()
        {
            GetKnightsCount();
            return UnitSoldiers;
        }

        private double ProwessExperience()
        {
            if(HasKnights)
            {
                double value = 0;

                foreach (var knight_value in Knights)
                {
                    if (knight_value.Prowess <= 3)
                    {
                        value += Strength.Terrible();
                    }
                    else if (knight_value.Prowess >= 4 && knight_value.Prowess <= 7)
                    {
                        value += Strength.Poor();
                    }
                    else if (knight_value.Prowess >= 8 && knight_value.Prowess <= 11)
                    {
                        value += Strength.Average();
                    }
                    else if (knight_value.Prowess >= 12 && knight_value.Prowess <= 15)
                    {
                        value += Strength.Good();
                    }
                    else if (knight_value.Prowess >= 16)
                    {
                        value += Strength.Excellent();
                    }


                    //Max level of experience
                    if (value >= 9)
                    {
                        value = 9;
                        break;
                    }
                }

                double rounded = Math.Round(value);
                return rounded;
            }
            else
            {
                return 0;
            }
 
           
        }


        private void GetKnightsCount()
        {
            UnitSoldiers = 0;

            int numberOfKnights;
            if(Date.Year >= 1066)
            {
                numberOfKnights = 10;
            }
            else
            {
                numberOfKnights = 5;
            }

            if(HasKnights)
            {
                for (int i = 0; i < Knights.Count; i++)
                {
                    UnitSoldiers += numberOfKnights;
                }
            }

        }

       

        public void SetData(List<(string ,int)> data, int effectiveness)
        {
            if(data.Count > 0)
            {
                Knights = data;
                Effectiveness = effectiveness;
                HasKnights = true;
            }
            else
            {
                HasKnights = false;
            }
            
        }

    }
}
