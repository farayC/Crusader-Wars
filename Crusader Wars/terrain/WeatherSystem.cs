using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crusader_Wars.terrain
{
    public static class Weather
    {
        static string Season { get;set; }
        static bool HasWinter { get;set; }
        static string Winter_Severity { get; set; }

        struct MildWinter
        {
            public static List<string> GetWeathers()
            {
                return new List<string> {"Weather\\ROME_Destruct\\default\\land\\day\\dry\\snow_morning.environment",
                                         "Weather\\default\\default\\land\\day\\dry\\snow_dry_0_light.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\snow_dry_1_light.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\spring_1_dry_snow.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\spring_2_dry_snow.environment",
                                        "Weather\\default\\default\\land\\day\\snow\\spring_snow_3.environment"};
            }
        };
       
        struct NormalWinter
        {
            public static List<string> GetWeathers()
            {
                return new List<string> {"Weather\\default\\default\\land\\day\\dry\\snow_01_light.environment",
                                         "Weather\\default\\default\\land\\day\\dry\\snow_dry_2_light.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\snow_dry_colour.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\snow_ground.environment",
                                        "Weather\\ROME_Destruct\\default\\land\\day\\snow\\snow.environment",
                                        "Weather\\default\\default\\land\\day\\snow\\snow_01_light.environment"};
            }
        };

        struct HarshWinter
        {
            public static List<string> GetWeathers()
            {
                return new List<string> {"Weather\\ROME_Med\\default\\Land\\day\\snow\\default_attila.environment",
                                         "Weather\\default\\default\\land\\day\\fog\\snow_fog_ground.environment",
                                        "Weather\\default\\default\\land\\day\\snow\\snow.environment",
                                        "Weather\\default\\default\\land\\day\\snow\\snow_3_heavy.environment",
                                        "Weather\\default\\default\\land\\day\\snow\\snow_4_heavy.environment",
                                        "Weather\\default\\default\\land\\day\\snow\\snow_5_heavy.environment"};
            }
        };

        struct Winter
        {
            public static List<string> GetWeathers()
            {
                return new List<string> {"Weather\\ROME_Destruct\\default\\land\\day\\rain\\default_day_rain.environment",
                                         "Weather\\ROME_Med\\default\\Land\\day\\fog\\default_attila.environment",
                                        "Weather\\ROME_Destruct\\default\\land\\day\\fog\\default_day_fog.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\CC2_3_day_dark_3.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\CC3_day_dark_2_med.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\CC4_day_dark_1.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\CC4_day_dry_3.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\morn_north_1.environment",
                                        "Weather\\ROME_Med\\default\\Land\\day\\dry\\morn_med_1.environment",
                                        "Weather\\ROME_Med\\default\\Land\\day\\dry\\morn_med_2.environment" };
            }
        }

        struct Spring
        {
            public static List<string> GetWeathers()
            {
                return new List<string> {"Weather\\ROME_Med\\default\\Land\\day\\rain\\default_attila.environment",
                                         "Weather\\default\\default\\land\\day\\dry\\CC1_2_day_dry_2.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\day_north_1.environment",
                                        "Weather\\ROME_Destruct\\default\\land\\day\\dry\\DEMO_morning.environment",
                                        "Weather\\ROME_Med\\default\\Land\\day\\dry\\day_med_clear.environment",
                                        "Weather\\ROME_Med\\default\\Land\\day\\dry\\morn_med_3.environment" };
            }
        };

        struct Fall
        {
            public static List<string> GetWeathers()
            {
                return new List<string> {"Weather\\ROME_Med\\default\\Land\\day\\rain\\morn_9.environment",
                                         "Weather\\default\\default\\land\\day\\rain\\default_day_rain.environment",
                                        "Weather\\default\\default\\land\\day\\rain\\morn_9.environmentt",
                                        "Weather\\default\\default\\land\\day\\dry\\CC1_2_day_dry_1.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\CC4_day_dry_1.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\CC4_day_dry_2.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\evening_north_1.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\morn_north_2.environment",
                                        "Weather\\ROME_Destruct\\default\\land\\day\\dry\\day_dark_4c.environment",
                                        "Weather\\ROME_Med\\default\\Land\\day\\dry\\evening_med_2.environment",
                                        "Weather\\ROME_Med\\default\\Land\\day\\dry\\evening_med_3.environment" };
            }
        }

        struct Summer
        {
            public static List<string> GetWeathers()
            {
                return new List<string> {"Weather\\ROME_Desert\\default\\Land\\day\\dry\\desert_morning.environment",
                                         "Weather\\default\\default\\land\\day\\dry\\evening_north_2.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\evening_north_3.environment",
                                        "Weather\\default\\default\\land\\day\\dry\\morn_6.environment",
                                        "Weather\\ROME_Desert\\default\\Land\\day\\dry\\desert_day.environment",
                                        "Weather\\ROME_Desert\\default\\Land\\day\\dry\\desert_evening.environment",
                                        "Weather\\ROME_Desert\\default\\Land\\day\\dry\\desert_eveningB.environment",
                                        "Weather\\ROME_Destruct\\default\\land\\day\\dry\\day_dark_4.environment",
                                        "Weather\\ROME_Med\\default\\Land\\day\\dry\\day_med_3.environment" };
            }
        }

        public static string GetRandomWeather()
        {
            return RandomizeWeather();
        }


        public static void SetSeason(string season) 
        {
            Reset();
            Season = season;
        }
        public static string GetWeather()
        {
            return ChooseWeather();
        }

        //string[3] severities
        //[0] = Mild
        //[1] = Normal
        //[2] = Harsh
        public static void SetWinterSeverity(string winterSeverity)
        {
            if(winterSeverity != String.Empty) 
            {
                HasWinter = true;
                if (winterSeverity == Languages.Terrain.AllWinter[0]) { Winter_Severity = "MildWinter"; return; }
                if (winterSeverity == Languages.Terrain.AllWinter[1]) { Winter_Severity = "NormalWinter"; return; }
                if (winterSeverity == Languages.Terrain.AllWinter[2]) { Winter_Severity = "HarshWinter"; return; }
            
            } else { HasWinter = false; }
            
        }

        static void Reset()
        {
            HasWinter = false;
            Winter_Severity = null;
            Season = null;
        }

        static string RandomizeWeather()
        {
            List<string> all = new List<string>();
            all.AddRange(Summer.GetWeathers());
            all.AddRange(Winter.GetWeathers());
            all.AddRange(Fall.GetWeathers());
            all.AddRange(Spring.GetWeathers());

            Random random = new Random();
            string random_weather = all[random.Next(0, all.Count)];

            return random_weather;
        }

        static string ChooseWeather()
        {

            Random random = new Random();
            if (HasWinter)
            {
                switch(Winter_Severity)
                {
                    case "MildWinter":
                        List<string> list_mild_winter = MildWinter.GetWeathers();
                        int l = random.Next(0, list_mild_winter.Count);
                        return list_mild_winter[l];
                    case "NormalWinter":
                        List<string> list_normal_winter = NormalWinter.GetWeathers();
                        int i = random.Next(0, list_normal_winter.Count);
                        return list_normal_winter[i];
                    case "HarshWinter":
                        List<string> list_harsh_winter = HarshWinter.GetWeathers();
                        int x = random.Next(0, list_harsh_winter.Count);
                        return list_harsh_winter[x];
                    default:
                        List<string> list_ = NormalWinter.GetWeathers();
                        int t = random.Next(0, list_.Count);
                        return list_[t];

                }
            }

            if(Season != null) 
            {
                switch (Season)
                {
                    case "winter":
                        List<string> list_winter = Winter.GetWeathers();
                        int l = random.Next(0, list_winter.Count);
                        return list_winter[l];

                    case "spring":
                        List<string> list_spring = Spring.GetWeathers();
                        int i = random.Next(0, list_spring.Count);
                        return list_spring[i];

                    case "fall":
                        List<string> list_fall = Fall.GetWeathers();
                        int x = random.Next(0, list_fall.Count);
                        return list_fall[x];

                    case "summer":
                        List<string> list_summer = Summer.GetWeathers();
                        int t = random.Next(0, list_summer.Count);
                        return list_summer[t];
                    default:
                        return GetRandomWeather();
                }
            }

            return "Weather\\ROME_Med\\default\\Land\\day\\dry\\day_med_2.environment";
        }


    }
}
