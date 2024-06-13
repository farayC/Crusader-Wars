using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Diagnostics.Eventing.Reader;


namespace Crusader_Wars
{
    public static class DeclarationsFile
    {
     
        static string filePath = Directory.GetFiles(".\\data\\battle files\\script", "tut_declarations.lua", SearchOption.AllDirectories)[0];

        public static List<string> Declarations = new List<string>();


        static List<Army> stark_armies = null;
        static List<Army> bolton_armies = null;
        public static void CreateAlliances(List<Army>attacker, List<Army>defender)
        {
            string alliances_ = "--\r\n-- Alliance declarations\r\n--\r\n\r\nAlliances = bm:alliances();\r\n\r\nAlliance_Stark = Alliances:item(1);\r\nAlliance_Bolton = Alliances:item(2);\r\n\r\n";
            
            /*
             *  SET STARK AND BOLTON ARMIES
             */

            foreach (var army in attacker)
            {
                if (army.IsPlayer())
                {
                    stark_armies = attacker;
                    break;
                }
                else if(army.IsEnemy())
                {
                    bolton_armies = attacker;
                    break;
                }
            }
            foreach (var army in defender)
            {
                if (army.IsPlayer())
                {
                    stark_armies = defender;
                    break;
                }
                else if (army.IsEnemy())
                {
                    bolton_armies = defender;
                    break;
                }
            }

            for (int i = 1; i <= stark_armies.Count; i++)
            {
                string t = $"Stark_Army{i} = Alliance_Stark:armies():item({i});\r\n";
                alliances_ += t ;
            }

            for (int i = 1; i <= bolton_armies.Count; i++)
            {
                string t = $"Bolton_Army{i} = Alliance_Bolton:armies():item({i});\r\n";
                alliances_ += t;
            }

            File.AppendAllText(filePath, alliances_ + "\n\n");
        }

        public static void AddUnitDeclaration(string unit_name, string unit_script_name)
        {

            // side = Stark / Bolton
            string side;
            if (unit_name.Contains("player")) side = "Stark";
            else { side = "Bolton"; }

            string unit_declaration = $"\n{unit_name} = script_unit:new({side}, \"{unit_script_name}\");";
            Declarations.Add(unit_name);
            File.AppendAllText(filePath, unit_declaration);
            
        }
        
        public static void Erase()
        {
            File.Create(filePath);
        }




    }
}
