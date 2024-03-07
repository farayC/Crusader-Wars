using System;
using System.IO;
using System.Text.RegularExpressions;


namespace Crusader_Wars.data.attila_settings
{
    static class AttilaPreferences
    {

        static string preferences_file_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\The Creative Assembly\Attila\scripts\preferences.script.txt";
        public static void ChangeUnitSizes()
        {
            if(!isUnitsSetToUltra())
            {
                string new_data = "";
                using (FileStream attila_settings_file = File.Open(preferences_file_path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(attila_settings_file))
                {
                    string line = "";
                    while (line != null && !reader.EndOfStream)
                    {
                        line = reader.ReadLine();

                        if (line.Contains("gfx_unit_size"))
                        {
                            line = Regex.Replace(line, @"gfx_unit_size (\d)", @"gfx_unit_size 3");
                        }

                        new_data += line + "\n";
                    }

                    reader.Close();
                    attila_settings_file.Close();
                }

                File.Create(preferences_file_path).Close();
                File.WriteAllText(preferences_file_path, new_data);
            }
            else
            {
                return;
            }

        }

        private static bool isUnitsSetToUltra()
        {

            if (!File.Exists(preferences_file_path)) return true;
            
            string unit_size_setting = "";
            using (FileStream attila_settings_file = File.Open(preferences_file_path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(attila_settings_file))
            {
                string line = "";
                while (line != null && !reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line.Contains("gfx_unit_size"))
                    {
                        unit_size_setting = line;
                        break;
                    }
                }

                reader.Close();
                attila_settings_file.Close();
            }

            Match isCorrect = Regex.Match(unit_size_setting, @"gfx_unit_size 3");
            if(isCorrect.Success) 
            {
                return true;
            }
            else 
            { 
                return false; 
            }

        }
    }
}
