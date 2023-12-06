using Crusader_Wars.terrain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        public static List<string> LivingList { get; private set; }
        static List<(string name, int index)> WoundedTraits_List { get; set; }
        public static void ReadAll()
        {

            //---------Living text----------

            long startMemory1 = GC.GetTotalMemory(false);
            MatchCollection allLiving = Regex.Matches(Data.String_Living, @"(?s)\d+={.*?(?=\d+={|\z)");
            LivingList = new List<string>();
            foreach (Match match in allLiving)
            {
                LivingList.Add(match.Value);
            }
            long endMemory1 = GC.GetTotalMemory(false);
            long memoryUsage1 = endMemory1 - startMemory1;
            Console.WriteLine($"----\nAdding each char to Living list...\nMemory Usage: {memoryUsage1 / 1048576} mb");


        }

        public static void ReadWoundedTraits()
        {
            //---------Wounded Traits----------

            MatchCollection allTraits = Regex.Matches(Data.String_Traits, @" (\w+)");
            string[] traitList = new string[allTraits.Count];
            for (int i = 0; i < allTraits.Count; i++)
            {
                traitList[i] = allTraits[i].Groups[1].Value;
            }

            List<(string name, int index)> wounded_traits;
            wounded_traits = new List<(string name, int index)>();

            for (int i = 0; i < traitList.Length; i++)
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
            if (str.Contains($" {ids[0]} ") || str.Contains($" {ids[0]}"))
            {
                already_wounded_trait = ids[0];
            }
            else if (str.Contains($" {ids[1]} ") || str.Contains($" {ids[1]}"))
            {
                already_wounded_trait = ids[1];
            }
            else if (str.Contains($" {ids[2]} ") || str.Contains($" {ids[2]}"))
            {
                already_wounded_trait = ids[2];
            }
            

            //if its not wounded, give wounded trait
            if (already_wounded_trait == string.Empty)
            {
                //Add trait if doesn't have one
                str += $" {trait}";

                return str;
            }
            else //if its already wounded, increase wound
            {
                var match_1 = Regex.Match(str, $" {already_wounded_trait} ");
                if(match_1.Success)
                {
                    string edited_str = Regex.Replace(str, $" {already_wounded_trait} ", $" {Traits.Brutally_Mauled()} ");
                    return edited_str;
                }


                var match_2 = Regex.Match(str, $" {already_wounded_trait}");
                if (match_2.Success)
                {
                    string edited_str = Regex.Replace(str, $" {already_wounded_trait}", $" {Traits.Brutally_Mauled()} ");
                    return edited_str;
                }

                return str;

            }
        }
    }




    public class CommanderSystem
    {
        private class CourtPosition
        {
            private string Profession { get; set; }
            private string Employee_ID { get; set; }


            public CourtPosition(string profession, string employee_ID)
            {
                Profession = profession;
                Employee_ID = employee_ID;
            }
        }



        public string CommanderID { get; private set; }
        public string Name { get; private set; }
        public int Rank { get; private set; }
        public int Martial { get; private set; }
        public int Prowess { get;private set; }
        public List<string> Traits_List { get; private set; }
        private List<CourtPosition> Employees { get; set; }
        private bool hasFallen { get; set; }

        public void SetID(string charachter_id)
        {
            CommanderID= charachter_id;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetRank(int rank)
        {
            Rank = rank;
        }

        public void SetMartial(int martial)
        {
            Martial = martial;
        }

        public void SetProwess(int prowess)
        {
            Prowess = prowess;
        }

        
        public void AddCourtPosition(string profession, string id )
        {
            if(Employees is null) Employees = new List<CourtPosition>();
            Employees.Add(new CourtPosition(profession, id));
        }

        public int GetUnitSoldiers()
        {
            return UnitSoldiers();
        }


        public int GetUnitsExperience()
        {
            return MartialArmyExperience();
        }

        public int GetCommanderExperience()
        {
            return (int)Math.Round(ProwessExperience() + (ProwessExperience() * MartialExperience()));
        }

        public int GetCommanderStarRating()
        {
            return StarExperience();
        }

        public void SetTraits(List<string> traits)
        {
            Traits_List = traits;
        }

        private int UnitSoldiers()
        {

            //Title rank soldiers
            int soldiers = 0;
            switch (Rank) 
            {
                case 1:
                    soldiers = 10;
                    break;
                case 2:
                    soldiers = 20;
                    break;
                case 3:
                    soldiers = 30;
                    break;
                case 4:
                    soldiers = 50;
                    break;
                case 5:
                    soldiers = 70;
                    break;
                case 6:
                    soldiers = 90;
                    break;
            }
            
            //Prowess soldiers
            int prowess = Prowess;
            if (prowess <= 3)
            {
                soldiers += 0;

            }
            else if (prowess >= 4 && prowess <= 7)
            {
                soldiers += 5;
            }
            else if (prowess >= 8 && prowess <= 11)
            {
                soldiers += 10;
            }
            else if (prowess >= 12 && prowess <= 15)
            {
                soldiers += 15;
            }
            else if (prowess >= 16)
            {
                soldiers += 20;
            }
            
            //Court positions soldiers
            if (Employees != null) {
                int courtiers = Employees.Count * 5;
                soldiers += courtiers;
            }

            //Health soldiers debuff
            if(Traits_List.Contains(Traits.Wounded().ToString())) soldiers += -5;
            if(Traits_List.Contains(Traits.Severely_Injured().ToString())) soldiers += -10;
            if (Traits_List.Contains(Traits.Brutally_Mauled().ToString())) soldiers += -15;
            if (Traits_List.Contains(Traits.Maimed().ToString())) soldiers += -10;
            if (Traits_List.Contains(Traits.One_Eyed().ToString())) soldiers += -5;
            if (Traits_List.Contains(Traits.One_Legged().ToString())) soldiers += -10;
            if (Traits_List.Contains(Traits.Disfigured().ToString())) soldiers += -5;

            //Minimum of 1 soldier
            if (soldiers < 1) soldiers = 1;

            return soldiers;
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

        private double ProwessExperience()
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

        private double MartialExperience()
        {
            int martial = Martial;
            double value = 0;

            if (martial <= 3)
            {
                value += 0.0;

            }
            else if (martial >= 4 && martial <= 7)
            {
                value += 0.2;
            }
            else if (martial >= 8 && martial <= 11)
            {
                value += 0.4;
            }
            else if (martial >= 12 && martial <= 15)
            {
                value += 0.6;
            }
            else if (martial >= 16)
            {
                value += 0.8;
            }


            return value;
        }
        
        private int MartialArmyExperience()
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

            //50% Chance Harsh Wounds
            const int Severely_InjuredChance = 70; // 20%
            const int Brutally_MauledChance = 90; // 20%

            //10% Chance Extreme Wounds
            const int MaimedChance = 93; // 3%
            const int One_LeggedChance = 96;// 3%
            const int One_EyedChance = 99;// 3%
            const int Disfigured = 100; //1%

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

    public static class AccoladesSystem
    {
        struct Abilities
        {
            public static string RaiseBanner1() { return "att_gen_raise_01"; }
            public static string Fear1() { return "att_gen_fear_01"; }
            public static string Inspire1() { return "att_gen_inspire_01"; }
            public static string Encourage() { return "chant"; }
            public static string SecondWind1() { return "att_gen_second_01"; }
            public static string RallyInspire() { return "com_rally_and_inspire"; }
            public static string Presence1() { return "att_gen_presence_01"; }
            public static string BattleRhythm() { return "com_battle_rhythm"; }
            public static string Pride() { return "com_pride"; }
            public static string WarCryGroup() { return "com_war_cry_group"; }
            public static string Brace1() { return "att_gen_brace_01"; }
            public static string WarCry1() { return "att_gen_war_01"; }
            public static string Push() { return "com_push"; }
            public static string Reconnaissance1() { return "att_gen_recon_01"; }
        };

        struct Attributes
        {
            public static string Marauder() { return "marauder_attribute"; }
            public static string Idealist() { return "idealist_attribute"; }
            public static string Charmer() { return "charmer_attribute"; }
            public static string Thug() { return "thug_attribute"; }
            public static string Disciplinarian() { return "disciplinarian_attribute"; }
            public static string Fanatic() { return "fanatic_attribute"; }
            public static string Valiant() { return "valiant_attribute"; }
            public static string Stalwart() { return "stalwart_attribute"; }
            public static string Scoundrel() { return "scoundrel_attribute"; }
            public static string Politicker() { return "politicker_attribute"; }
            public static string Tactician() { return "tactician_attribute"; }
            public static string Reeve() { return "reeve_attribute"; }
            public static string Manipulator() { return "manipulator_attribute"; }
            public static string Mentor() { return "mentor_attribute"; }
            public static string Contender() { return "contender_attribute"; }
        }

        public static void GetSpecialAbility(string attribute_key)
        {
            if(attribute_key == Attributes.Marauder())
            {

            }
            else if( attribute_key == Attributes.Idealist())
            {

            }
            else if (attribute_key == Attributes.Charmer())
            {

            }
            else if (attribute_key == Attributes.Thug())
            {

            }
            else if (attribute_key == Attributes.Disciplinarian())
            {

            }
            else if (attribute_key == Attributes.Fanatic())
            {

            }
            else if (attribute_key == Attributes.Stalwart())
            {

            }
            else if (attribute_key == Attributes.Scoundrel())
            {

            }
            else if (attribute_key == Attributes.Politicker())
            {

            }
            else if (attribute_key == Attributes.Tactician())
            {

            }
            else if (attribute_key == Attributes.Reeve())
            {

            }
            else if (attribute_key == Attributes.Manipulator())
            {

            }
            else if (attribute_key == Attributes.Mentor())
            {

            }
            else if (attribute_key == Attributes.Contender())
            {

            }


        }



    }

    public class KnightSystem
    {

        private List<(string ID, int Soldiers,int Prowess, List<string> Traits,BaseSkills BaseSkill,bool isAccolade)> Knights { get; set; }
        private List<(string PrimaryAttribute, string SecundaryAttribute, string Honor)> Accolades { get; set; }
        private int UnitSoldiers { get; set; }

        private int Effectiveness { get; set; }

        private List<string> KilledKnights { get; set; }
        private bool HasKnights { get; set; }
        
        public List<(string, int,int, List<string>,BaseSkills, bool)> GetKnightsList()
        {
            return Knights;
        }
        public void SetAccolades(List<(string, string, string)> accolades_list)
        {
            Accolades = accolades_list;
        }


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


        public void Health()
        {
            if(HasKnights)
            {
                //50% Chance Light Wounds
                const int WoundedChance = 50; // 50%

                //40% Chance Harsh Wounds
                const int Severely_InjuredChance = 70; // 20%
                const int Brutally_MauledChance = 90; // 20%

                //10% Chance Extreme Wounds
                const int MaimedChance = 93; // 3%
                const int One_LeggedChance = 96;// 3%
                const int One_EyedChance = 99;// 3%
                const int Disfigured = 100; //1%

                var Chance = new Random();
                int RandomNumber;

                Console.WriteLine("----------------------------------");
                Console.WriteLine($"KNIGHTS FALLEN - {KilledKnights}\n");

                for (int i = 0; i < KilledKnights.Count; i++)
                {
                    //Random Knight
                    string id = KilledKnights[i];
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
                KilledKnights = new List<string>();

                int totalSoldiers = UnitSoldiers;
                int remainingSoldiers = remaining;


                //random knight
                int soldiers_lost = totalSoldiers - remainingSoldiers;
                while(soldiers_lost > 0)
                {
                    Random random = new Random();
                    int random_index = random.Next(Knights.Count);
                    var knight = Knights[random_index];

                    soldiers_lost -= knight.Soldiers;

                    if (soldiers_lost <= 0) break;

                    KilledKnights.Add(knight.ID);

                    Knights.Remove(knight);
                }


            }


        }


        public int SetExperience()
        {
            int prowess_level = (int)ProwessExperience();
            double effectiveness = KnightEffectiveness(prowess_level);
            int level = (int)Math.Round(prowess_level + effectiveness);
            
            return level;
        }

        public int GetKnightsSoldiers()
        {
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


        private void SetKnightsCount()
        {
            UnitSoldiers = 0;

            if(HasKnights)
            {
                for(int i = 0; i < Knights.Count; i++)
                {
                    int prowess = Knights[i].Prowess;
                    var traits = Knights[i].Traits;
                    Knights[i] = (Knights[i].ID, CalculeKnightStrenght(prowess), prowess, Knights[i].Traits, Knights[i].BaseSkill,Knights[i].isAccolade);
                    UnitSoldiers += Knights[i].Soldiers;
                }

            }            
        }

        public void WoundedDebuffs()
        {
            if(HasKnights)
            {
                int debuff = 0;
                for (int i = 0; i < Knights.Count; i++)
                {
                    var knight = Knights[i];
                    var traits = knight.Traits;

                    //Health soldiers debuff
                    if (traits.Contains(Traits.Wounded().ToString())) debuff += -1;
                    if (traits.Contains(Traits.Severely_Injured().ToString())) debuff += -2;
                    if (traits.Contains(Traits.Brutally_Mauled().ToString())) debuff += -3;
                    if (traits.Contains(Traits.Maimed().ToString())) debuff += -2;
                    if (traits.Contains(Traits.One_Eyed().ToString())) debuff += -1;
                    if (traits.Contains(Traits.One_Legged().ToString())) debuff += -2;
                    if (traits.Contains(Traits.Disfigured().ToString())) debuff += -1;

                    Knights[i] = (knight.ID, knight.Soldiers - debuff, knight.Prowess, knight.Traits, knight.BaseSkill, knight.isAccolade);
                }

            }



        }

        int CalculeKnightStrenght(int knight_prowess)
        {
            int value = 0;
            if (knight_prowess <= 3)
            {
                value += 0;
            }
            else if (knight_prowess >= 4 && knight_prowess <= 7)
            {
                value += 1;
            }
            else if (knight_prowess >= 8 && knight_prowess <= 11)
            {
                value += 2;
            }
            else if (knight_prowess >= 12 && knight_prowess <= 15)
            {
                value += 3;
            }
            else if (knight_prowess >= 16)
            {
                value += 4;
            }

            return 3 + value;
        }

        public void SetTraits(string id, List<string> traits)
        {
            var knight = Knights.FirstOrDefault(x => x.ID == id);
            Knights[Knights.IndexOf(knight)] = (knight.ID, knight.Soldiers, knight.Prowess, traits, knight.BaseSkill,knight.isAccolade);
        }
        public void SetSkills(string id, BaseSkills skills)
        {
            var knight = Knights.FirstOrDefault(x => x.ID == id);
            Knights[Knights.IndexOf(knight)] = (knight.ID, knight.Soldiers, knight.Prowess, knight.Traits, skills, knight.isAccolade);
        }
       

        public void SetData(List<(string, int, int, List<string>, BaseSkills ,bool)> data, int effectiveness)
        {
            if(data.Count > 0)
            {
                Knights = data;
                Effectiveness = effectiveness;
                HasKnights = true;
                SetKnightsCount();
            }
            else
            {
                HasKnights = false;
            }
            
        }

    }

    //this are the base values from a character skills
    //modifiers do not count here
    public class BaseSkills
    {
        public int diplomacy { get; private set; }
        public int martial { get; private set; }
        public int stewardship { get; private set; }
        public int intrige { get; private set; }
        public int learning { get; private set; }
        public int prowess { get; private set; }

        public BaseSkills(List<string> skills_collection)
        {

            this.diplomacy = Int32.Parse(skills_collection[0]);
            this.martial = Int32.Parse(skills_collection[1]);
            this.stewardship = Int32.Parse(skills_collection[2]);
            this.intrige = Int32.Parse(skills_collection[3]);
            this.learning = Int32.Parse(skills_collection[4]);
            this.prowess = Int32.Parse(skills_collection[5]);
        }
    }


}
