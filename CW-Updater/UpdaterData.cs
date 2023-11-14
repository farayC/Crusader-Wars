using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.IO.Compression;
using System.Diagnostics;

namespace CW_Updater
{
    internal class UpdaterData
    {

        private static void ExtractFiles(string zipFilePath, string extractFolderPath)
        {
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    bool isFirstDirectory = true;
                    File.Delete(@".\Crusader Wars.exe");
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (isFirstDirectory)
                        {
                            // Skip the first directory
                            isFirstDirectory = false;
                            continue;
                        }

                        // Create a file path by combining the extract folder path with the entry name
                        string filePath = Path.Combine(extractFolderPath, entry.FullName)
                                              .Replace("\\CW-Updater-main", "");
                        if (entry.Name == "")
                        {
                            // Create the directory for empty entries
                            Directory.CreateDirectory(filePath);
                        }
                        else
                        {
                            //If files are from github, skip
                            if (entry.Name == ".gitattributes" || entry.Name == "README.md") continue;

                            // Extract the entry to the file path
                            entry.ExtractToFile(filePath, overwrite: true);
                        }
                    }
                }

                File.Delete(zipFilePath);
            }
            catch 
            {
                MessageBox.Show("Error updating!", "Updater Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                
                if(File.Exists(zipFilePath)) File.Delete(zipFilePath);

                Application.Exit();
                return;
            }

        }

        public static void UpdateApp(AutoUpdater main, string ModVersion)
        {

            try
            {
                using (var webClient = new WebClient())
                {
                    string folderUrl = "https://github.com/farayC/CW-Updater/archive/refs/heads/main.zip";
                    string zipFilePath = Directory.GetCurrentDirectory() + "\\Updater.zip";

                    // Subscribe to the DownloadProgressChanged event
                    webClient.DownloadProgressChanged += (sender, e) =>
                    {
                        Console.WriteLine($"Downloaded {e.BytesReceived}/{e.TotalBytesToReceive} bytes ({e.ProgressPercentage}%).");
                    };


                    string extractPath = Directory.GetCurrentDirectory();
                    webClient.DownloadFileCompleted += (sender, e) =>
                    {
                        ExtractFiles(zipFilePath, extractPath);
                        
                        //Update .txt file version
                        string version_path = Directory.GetCurrentDirectory() + "\\app_version.txt";
                        File.WriteAllText(version_path, $"version=\"{ModVersion}\"");
                        
                        //Reopen CW
                        Process.Start(@".\Crusader Wars.exe");
                        main.Close();
                    };

                    System.Uri url = new Uri(folderUrl);
                    webClient.DownloadFileAsync(url, zipFilePath);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating!", "Updater Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                main.Close();
                return;
            }
        }
    }
}
