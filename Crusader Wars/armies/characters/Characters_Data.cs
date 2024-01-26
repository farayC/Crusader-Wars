using Crusader_Wars.armies;
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
