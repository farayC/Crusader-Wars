using Crusader_Wars.terrain;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crusader_Wars.data
{
    public static class SiegesSearch
    {
        public static void Search(string log, Player Player, Enemy Enemy)
        {
            /*---------------------------------------------
             * ::::::::::::::::Army Ratio::::::::::::::::::
             ---------------------------------------------*/

            //Get Army Ratio in log file...
            ArmyProportions.SetRatio(Properties.Settings.Default.OPTIONS_RATIO);
            ArmyProportions.isBiggerThanLimit(Player.TotalNumber, Enemy.TotalNumber);

            /*---------------------------------------------
             * :::::::::::::::::::Date:::::::::::::::::::::
             ---------------------------------------------*/

            DateSearch(log);

            /*---------------------------------------------
             * ::::::::::::Commanders ID's:::::::::::::::::
             ---------------------------------------------*/

            //Search player ID
            string player_id = Regex.Match(log, @"AttackerID:(\d+)").Groups[1].Value;
            Player.ID = Int32.Parse(player_id);

            /*---------------------------------------------
             * ::::::::::::::::Army Names::::::::::::::::::
             ---------------------------------------------*/

            RealmsNamesSearch(log, Player, Enemy);

            /*---------------------------------------------
             * :::::::::::::::Unit Mapper::::::::::::::::::
              ---------------------------------------------*/

            UnitMapper.LoadMapper();

        }

        /*
         *                           ||||
         * |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
         * -----------------------------------------------------------
         * |||||||||||||||||||| Side Functions!|||||||||||||||||||||||
         * -----------------------------------------------------------
         * |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
         *                           ||||  
         */

        static void SiegingArmySearch(string log, ICharacter Side)
        {
            //IMPORTANT
            //Need to add error catchers

            string id = Regex.Match(log, @"Attacker ID:(.+)").Groups[1].Value;

            string heritage_text = Regex.Match(log, @"Attacker Heritage:.+").Value;
            string heritage = Regex.Match(heritage_text, @"L (.+)").Groups[1].Value;

            string culture_text = Regex.Match(log, @"Attacker Culture:.+").Value;
            string culture = Regex.Match(culture_text, @"L (.+)").Groups[1].Value;

            string commander_martial_skill_text = Regex.Match(log, @"Martial skill:.+").Value;
            string comander_martial_skill = Regex.Match(commander_martial_skill_text, @"\d+").Value;
         
            string commander_prowess_skill_text = Regex.Match(log, @"Prowess skill:.+").Value;
            string commander_prowess_skill = Regex.Match(commander_prowess_skill_text, @"\d+").Value;

            Side.ID = Int32.Parse(id);
            Side.Heritage = heritage;
            Side.Culture = culture;

            Side.Commander = new CommanderSystem();
            Side.Commander.SetID(id);
            Side.Commander.SetMartial(Int32.Parse(comander_martial_skill));
            Side.Commander.SetProwess(Int32.Parse(commander_prowess_skill));

        }

        static void HoldingSearch(string log)
        {
            string holding_level = Regex.Match(log, @"Holding Level:(.+)").Groups[1].Value;
            string holding_culture; //....
            string holding_sickness = Regex.Match(log, @"Sickness:(.+)").Groups[1].Value.Trim();
            string holding_supplies = Regex.Match(log, @"Supplies:(.+)").Groups[1].Value.Trim();
            string holding_walls = Regex.Match(log, @"Walls:(.+)").Groups[1].Value.Trim();



        }

        static void RealmsNamesSearch(string log, Player Player, Enemy Enemy)
        {
            string text = Regex.Match(log, "(Log[\\s\\S]*?)---------Attacker Army---------[\\s\\S]*?").Groups[1].Value;
            MatchCollection found_armies = Regex.Matches(text, "L (.+)");
            if (found_armies.Count >= 1)
            {
                string player_army = found_armies[0].Groups[1].Value;
                string enemy_army = found_armies[1].Groups[1].Value;

                Player.RealmName = player_army;
                Enemy.RealmName = enemy_army;
            }

        }

        static void DateSearch(string log)
        {
            string month;
            string year;
            try
            {
                month = Regex.Match(log, Languages.SearchPatterns.date).Groups["Month"].Value;
                year = Regex.Match(log, Languages.SearchPatterns.date).Groups["Year"].Value;
                Date.Month = month;
                Date.Year = Int32.Parse(year);

                string season = Date.GetSeason();
                Weather.SetSeason(season);
            }
            catch
            {
                month = "January";
                year = "1300";
                Date.Month = month;
                Date.Year = Int32.Parse(year);
                Weather.SetSeason("random");
            }

        }
    }
}
