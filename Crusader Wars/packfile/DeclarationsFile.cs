using System.IO;
using System.Collections.Generic;


namespace Crusader_Wars
{
    public static class DeclarationsFile
    {
     
        static string filePath = Directory.GetFiles("Battle Files\\script", "tut_declarations.lua", SearchOption.AllDirectories)[0];

        public static List<string> Declarations = new List<string>();

        /*
        public DeclarationsFile()
        {
            using (FileStream fs = File.Create(filePath)) ;
        }
        */

        public static void SetDeclarations()
        {
            AddArray();
        }

        public static void CreateAlliances()
        {
            string alliances = "--\r\n-- Alliance declarations\r\n--\r\n\r\nAlliances = bm:alliances();\r\n\r\nAlliance_Stark = Alliances:item(1);\r\nAlliance_Bolton = Alliances:item(2);\r\n\r\nStark = Alliance_Stark:armies():item(1);\r\nBolton = Alliance_Bolton:armies():item(1);";
            File.AppendAllText(filePath, alliances+"\n\n");
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

        private static void AddArray()
        {
            string identifier = "\n\nUNIT_All = {";

            string declaration_str; 
            foreach (string declariation in Declarations)
            {
                declaration_str = $"\n\t{declariation},\n";
                identifier = identifier.Insert(identifier.Length, declaration_str);
            }
            identifier = identifier.Remove(identifier.Length-2, 1);
            string close = "\n}";
            identifier = identifier.Insert(identifier.Length, close);

            File.AppendAllText(filePath, identifier);
        }
        
        public static void Erase()
        {
            File.Create(filePath);
        }




    }
}
