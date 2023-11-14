using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;

namespace Crusader_Wars
{
    public static class PackFile
    {

        public static void PackFileCreator()
        {
            // Create and import .pack file

            string create_path =Directory.GetCurrentDirectory() + @"\CrusaderWars.pack";
            string add_path = Directory.GetCurrentDirectory() + @"\battle files";
            string thumbnail_path = Directory.GetCurrentDirectory() + @"\Settings\CrusaderWars.png";

            string create_command = $@"--game attila pack create -p ""{create_path}""";
           // string add_command = $@"--game attila pack add -p ""{create_path}"" -t ""{add_path}\""";
            string add_command = $@"--game attila pack add -p ""{create_path}"" -F ""{add_path}""";

            CreatePackFile(create_command);
            CreatePackFile(add_command);

            string pack_dir_path = Path.GetDirectoryName(Properties.Settings.Default.VAR_attila_path) + @"\data";
            string pack_to_move_path = create_path;
            string pack_file_path = pack_dir_path + @"\CrusaderWars.pack";
            string thumb_file_path = pack_dir_path + @"\CrusaderWars.png";

            if (File.Exists(pack_file_path))
            {
                File.Delete(pack_file_path);
            }
            File.Move(pack_to_move_path, pack_file_path);

            if(!File.Exists(thumb_file_path))
            {
                File.Copy(thumbnail_path, thumb_file_path);
            }
            
        }

        private static string CreatePackFile(string command)
        {
            string rpfm_client_path = Directory.GetCurrentDirectory() + @"\"+Directory.GetFiles("rpfm", "rpfm_cli.exe", SearchOption.AllDirectories)[0];

            ProcessStartInfo procStartInfo = new ProcessStartInfo(rpfm_client_path, command)

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
    }
}
