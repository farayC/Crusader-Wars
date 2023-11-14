using Crusader_Wars;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crusader_Wars
{
    public static class Languages
    {


        public static string Language { get; private set; }

        public static void ShowWarningMessage()
        {
            var y = MessageBox.Show("Your Crusader Kings 3 language is not in english! This cause bugs and crashes, do you want to continue?", "Warning!",
            MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

            //Restarts the mod
            if(y == DialogResult.No)
            {
                throw new Exception();
            }

        }

        public static void SetLanguage(string language)
        {
            Language = language;
            switch(Language) 
            {
                case "l_english":
                    SearchPatterns.English.Set();
                    Terrain.English.Set();
                    break;
                case "l_spanish":
                    SearchPatterns.Spanish.Set();
                    Terrain.Spanish.Set();
                    break;
                case "l_french":
                    SearchPatterns.French.Set();
                    Terrain.French.Set();
                    break;
                case "l_german":
                    SearchPatterns.German.Set();
                    Terrain.German.Set();
                    break;
                case "l_russian":
                    SearchPatterns.Russian.Set();
                    Terrain.Russian.Set();
                    break;
                case "l_korean":
                    SearchPatterns.Korean.Set();
                    Terrain.Korean.Set();
                    break;

                default:
                    SearchPatterns.English.Set();
                    Terrain.English.Set();
                    break;


            }
        }

        public static class SearchPatterns
        {
            public static string date { get; private set; }
            public static string army_composition { get; private set; }
            public static string commander { get; private set; }
            public static string martial_skill { get; private set; }
            public static string total_soldiers { get; private set; }
            public static string levies { get; private set; }
            public static string cultures { get; private set; }

            public struct English
            {
                public static void Set()
                {
                    date = @"Current date: \d+ (?<Month>\p{L}+) (?<Year>\d+)";
                    army_composition = @"(?<ArmyComposition>T Total Soldiers:[\s\S]*?)T Commander[\s\S]*?";
                    commander = @"(?<Commander>T Commander[\s\S]*?)ONCLICK[\s\S]*?";
                    martial_skill = @"Martial skill: \w+ (?<Martial>\d+)";
                    total_soldiers = "Total Soldiers.+?(?=)(?<SoldiersNum>\\d+)";
                    levies = "Levies.+?(?=)(?<SoldiersNum>\\d+)";
                    cultures = @"\n(?<CulturesText>[\s\S]+)\nT Total Soldiers";
                }
            }
            public struct Spanish
            {
                public static void Set()
                {
                    date = @"Fecha actual: \d+ (?<Month>\p{L}+) (?<Year>\d+)";
                    army_composition = @"(?<ArmyComposition>T Total de soldados:[\s\S]*?)T E TOOLTIP[\s\S]*?";
                    commander = @"(?<Commander>T E TOOLTIP[\s\S]*?)Base[\s\S]*?";
                    martial_skill = @"Habilidad marcial: \w+ (?<Martial>\d+)";
                    total_soldiers = "Total de soldados.+?(?=)(?<SoldiersNum>\\d+)";
                    levies = "Levas.+?(?=)(?<SoldiersNum>\\d+)";
                    cultures = @"\n(?<CulturesText>[\s\S]+)\nT Total de soldados";
                }
            }

            public struct French
            {
                public static void Set()
                {
                    date = @"Date actuelle: \d+ (?<Month>\p{L}+) (?<Year>\d+)";
                    army_composition = @"(?<ArmyComposition>T Nombre total de soldats:[\s\S]*?)T E TOOLTIP[\s\S]*?";
                    commander = @"(?<Commander>T E TOOLTIP[\s\S]*?)Base[\s\S]*?";
                    martial_skill = @"Aptitude martiale: \w+ (?<Martial>\d+)";
                    total_soldiers = "Nombre total de soldats.+?(?=)(?<SoldiersNum>\\d+)";
                    levies = "Levées.+?(?=)(?<SoldiersNum>\\d+)";
                    cultures = @"\n(?<CulturesText>[\s\S]+)\nT Nombre total de soldats";
                }
            }

            public struct German
            {
                public static void Set()
                {
                    date = @"Aktuelles Datum: \d+. (?<Month>\p{L}+) (?<Year>\d+)";
                    army_composition = @"(?<ArmyComposition>T Soldaten gesamt:[\s\S]*?)T Befehlshaber[\s\S]*?";
                    commander = @"(?<Commander>T Befehlshaber[\s\S]*?)Grundwert[\s\S]*?";
                    martial_skill = @"Kriegsführungswert: \w+ (?<Martial>\d+)";
                    total_soldiers = "Soldaten gesamt.+?(?=)(?<SoldiersNum>\\d+)";
                    levies = "Aufgebote.+?(?=)(?<SoldiersNum>\\d+)";
                    cultures = @"\n(?<CulturesText>[\s\S]+)\nT Soldaten gesamt";

                }
            }
            
            public struct Russian
            {
                public static void Set()
                {
                    date = @"Дата: \d+ (?<Month>\p{L}+) (?<Year>\d+)";
                    army_composition = @"(?<ArmyComposition>T Всего солдат:[\s\S]*?)T E TOOLTIP[\s\S]*?";
                    commander = @"(?<Commander>T E TOOLTIP[\s\S]*?)Базовое[\s\S]*?";
                    martial_skill = @"Навык «Военное дело»: \w+ (?<Martial>\d+)";
                    total_soldiers = "Всего солдат.+?(?=)(?<SoldiersNum>\\d+)";
                    levies = "Ополчение.+?(?=)(?<SoldiersNum>\\d+)";
                    cultures = @"\n(?<CulturesText>[\s\S]+)\nT Всего солдат";
                }
            }

            public struct Korean
            {
                public static void Set()
                {
                    date = @"开始于(?<Year>\d+)年(?<Month>\d+)";
                    army_composition = @"(?<ArmyComposition>T 总士兵[\s\S]*?)T 将领[\s\S]*?";
                    commander = @"(?<Commander>T 将领E[\s\S]*?)基础[\s\S]*?";
                    martial_skill = @"军事能力\w+ (?<Martial>\d+)";
                    total_soldiers = "总士兵.+?(?=)(?<SoldiersNum>\\d+)";
                    levies = "征召兵.+?(?=)(?<SoldiersNum>\\d+)";
                    cultures = @"\n(?<CulturesText>[\s\S]+)\nT 总士兵";
                }
            }

            public struct Chinese
            {
                public static void Set()
                {
                    date = @"开始于(?<Year>\d+)年(?<Month>\d+)";
                    army_composition = @"(?<ArmyComposition>T 总士兵[\s\S]*?)T 将领[\s\S]*?";
                    commander = @"(?<Commander>T 将领E[\s\S]*?)基础[\s\S]*?";
                    martial_skill = @"军事能力\w+ (?<Martial>\d+)";
                    total_soldiers = "总士兵.+?(?=)(?<SoldiersNum>\\d+)";
                    levies = "征召兵.+?(?=)(?<SoldiersNum>\\d+)";
                    cultures = @"\n(?<CulturesText>[\s\S]+)\nT 总士兵";
                }
            }

        };

        public static class Terrain
        {
            public static string[] AllTerrains { get; private set; }
            public static string[] AllWinter { get; private set; }


            public struct English
            {
                public static void Set()
                {
                    AllTerrains = new string[] {"Desert", "Desert Mountains", "Drylands", "Farmlands",
                                        "Forest", "Hills", "Mountains", "Plains" , "Steppe",
                                        "Taiga", "Wetlands", "Jungle", "Oasis", "Floodplains"};
                    
                    AllWinter = new string[] {"Mild", "Normal", "Harsh" };
                }
            }

            public struct Spanish
            {
                public static void Set()
                {
                    
                    AllTerrains = new string[] {"Desierto", "Montaña desértica", "Tierras áridas", "Tierra de cultivo",
                                        "Bosque", "Colina", "Montaña", "Llanura" , "Estepa",
                                        "Taiga", "Pantano", "Selva", "Oasis", "Llanura aluvial"};
                    
                    AllWinter = new string[] { "suave", "normal", "duro" };
                }
            }

            public struct French
            {
                public static void Set()
                {
                    AllTerrains = new string[] {"Désert", "Montagnes désertiques", "Terres arides", "Terres arables",
                                        "Forêt", "Collines", "Montagnes", "Plaines" , "Steppe",
                                        "Taïga", "Marécages", "Jungle", "Oasis", "Plaine inondable"};

                    AllWinter = new string[] { "Hiver doux", "Hiver normal", "Hiver rude" };
                }
            }
            public struct German
            {
                public static void Set()
                {
                    AllTerrains = new string[] {"Wüste", "Bergwüste", "Trockengebiet", "Ackerland",
                                        "Wald", "Hügel", "Berge", "Ebenen" , "Steppe",
                                        "Taiga", "Feuchtgebiet", "Dschungel", "Oase", "Auen"};

                    AllWinter = new string[] { "Milder", "Normaler", "Rauer" };
                }
            }

            public struct Russian
            {
                public static void Set()
                {
                    AllTerrains = new string[] {"Пустыня", "Пустынные горы", "Засушливые земли", "Пахотные земли",
                                        "Лес", "Холмы", "Горы", "Равнины" , "Степь",
                                        "Тайга", "Болота", "Джунгли", "Оазис", "Поймы рек"};

                    AllWinter = new string[] { "Мягкие", "Обычные", "Суровые" };
                }
            }

            public struct Korean
            {
                public static void Set()
                {
                    AllTerrains = new string[] {"사막", "사막 산악", "건조지", "농지",
                                        "삼림", "구릉지", "산악", "평야" , "초원",
                                        "침엽수림", "습지대", "밀림", "오아시스", "범람원"};

                    AllWinter = new string[] { "温暖的", "普通的", "严酷的" };
                }
            }

        };

    }
}