using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using Crusader_Wars.client.BETAUPDATE_Message;
using System.Web;
using Microsoft.SqlServer.Server;

namespace Crusader_Wars
{
    public static class Updater
    {
        public static string AppVersion { get; set; }
        private static string ModVersion { get; set; }

        private static void GetAppVersion()
        {
            string version_path = Directory.GetCurrentDirectory() + "\\app_version.txt";
            if (File.Exists(version_path))
            {
                AppVersion = File.ReadAllText(version_path);
                AppVersion = Regex.Match(AppVersion, "\"(.+)\"").Groups[1].Value;
            }
        }
        private static void GetModVersion()
        {
            string steam_mod_path = Properties.Settings.Default.VAR_log_ck3;
            steam_mod_path = steam_mod_path.Replace("console_history.txt", @"Mod\ugc_2977969008.mod");

            string non_steam_mod_path = Properties.Settings.Default.VAR_log_ck3;
            non_steam_mod_path = non_steam_mod_path.Replace("console_history.txt", @"Mod\Crusader Wars.mod");

            string version_path = steam_mod_path;
            string other_version_path = non_steam_mod_path;
            if (File.Exists(version_path))
            {
                string[] allLines = File.ReadAllLines(version_path); 
                ModVersion = allLines[0];
                ModVersion = Regex.Match(ModVersion, "\"(.+)\"").Groups[1].Value;
            }
            else if(File.Exists(other_version_path))
            {
                string[] allLines = File.ReadAllLines(other_version_path);
                ModVersion = allLines[0];
                ModVersion = Regex.Match(ModVersion, "\"(.+)\"").Groups[1].Value;
            }
            else 
            {
                ModVersion = string.Empty;
            }
        }

        private static bool IsMostRecentUpdate()
        {
            string[] AppComponents = AppVersion.Split('.');
            string[] ModComponents = ModVersion.Split('.');

            for (int i = 0; i < Math.Max(AppComponents.Length, ModComponents.Length); i++)
            {
                int v1 = i < AppComponents.Length ? int.Parse(AppComponents[i]) : 0;
                int v2 = i < ModComponents.Length ? int.Parse(ModComponents[i]) : 0;

                if (v2 > v1)
                {
                    return true;
                }
                else if (v1 > v2)
                {
                    return false;
                }
            }

            return false;
        }

        private static bool HasInternetConnection()
        {
            Ping myPing = new Ping();
            String host = "google.com";
            byte[] buffer = new byte[32];
            int timeout = 2;
            PingOptions pingOptions = new PingOptions();
            try
            {
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public static void CheckAppVersion(HomePage main)
        {
            AppVersion = String.Empty;
            ModVersion = String.Empty;

            GetAppVersion();
            GetModVersion();

            if(ModVersion == "1.0")
            {
                string text = "The mod has updated to the official Crusader Wars v1.0 \"Warfare\"!\n" +
                              "For this you need to uninstall this Launcher by just deleting the folder." +
                              "Then you need to download the new setup from our website!"
                              ;
               // BETAUPDATE_Message.ShowWarningMessage(text);
            }


            if(ModVersion != String.Empty)
            {
                if(IsMostRecentUpdate() && HasInternetConnection()) 
                {
                    try
                    {

                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = @".\CW-Updater.exe";
                        startInfo.Arguments = $"{AppVersion} {ModVersion}";
                        Process.Start(startInfo);

                        main.Close();
                    }
                    catch
                    {
                        return;
                    }
                }

            }

        }

    }

   
}
