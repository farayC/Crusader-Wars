using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crusader_Wars
{
    public class CommanderSystem
    {
        private class CourtPosition
        {
            string Profession { get; set; }
            string Employee_ID { get; set; }


            public CourtPosition(string profession, string employee_ID)
            {
                Profession = profession;
                Employee_ID = employee_ID;
            }
        }



        public string ID { get; private set; }
        public string Name { get; private set; }
        public int Rank { get; private set; }
        public int Martial { get; private set; }
        public int Prowess { get; private set; }
        public List<string> Traits_List { get; private set; }
        private List<CourtPosition> Employees { get; set; }
        private (string PrimaryAttribute, string SecundaryAttribute, string Honor) Accolade { get; set; }
        private bool hasFallen { get; set; }
        private bool MainCommander {  get; set; }

        public CommanderSystem(string name, string id, int prowess, int martial, int rank, bool mainCommander)
        {
            Name = name;
            ID = id;
            Prowess = prowess;
            Martial = martial;
            Rank = rank;
            MainCommander = mainCommander;
        }
        public bool IsMainCommander() { return MainCommander; }
        public void SetAccolade((string PrimaryAttribute, string SecundaryAttribute, string Honor) accolade)
        {
            Accolade = accolade;
        }

        public (string,string, string) GetAccolade()
        {
            return Accolade;
        }


        public void AddCourtPosition(string profession, string id)
        {
            if (Employees is null) Employees = new List<CourtPosition>();
            Employees.Add(new CourtPosition(profession, id));
        }

        public int GetUnitSoldiers()
        {
            return UnitSoldiers();
        }


        public int GetUnitsExperience()
        {
            return MartialArmyExperience();
        }

        public int GetCommanderExperience()
        {
            return (int)Math.Round(ProwessExperience() + (ProwessExperience() * MartialExperience()));
        }

        public int GetCommanderStarRating()
        {
            return StarExperience();
        }

        public void SetTraits(List<string> traits)
        {
            Traits_List = traits;
        }

        int UnitSoldiers()
        {

            //Title rank soldiers
            int soldiers = 0;
            switch (Rank)
            {
                case 1:
                    soldiers = 10;
                    break;
                case 2:
                    soldiers = 20;
                    break;
                case 3:
                    soldiers = 30;
                    break;
                case 4:
                    soldiers = 50;
                    break;
                case 5:
                    soldiers = 70;
                    break;
                case 6:
                    soldiers = 90;
                    break;
            }

            //Prowess soldiers
            int prowess = Prowess;
            if (prowess <= 4)
            {
                soldiers += 0;

            }
            else if (prowess >= 5 && prowess <= 8)
            {
                soldiers += 5;
            }
            else if (prowess >= 9 && prowess <= 12)
            {
                soldiers += 10;
            }
            else if (prowess >= 13 && prowess <= 16)
            {
                soldiers += 15;
            }
            else if (prowess >= 17)
            {
                soldiers += 20;
            }

            //Court positions soldiers
            if (Employees != null)
            {
                int courtiers = Employees.Count * 5;
                soldiers += courtiers;
            }

            //Health soldiers debuff
            if (Traits_List.Contains(Traits.Wounded().ToString())) soldiers += -5;
            if (Traits_List.Contains(Traits.Severely_Injured().ToString())) soldiers += -10;
            if (Traits_List.Contains(Traits.Brutally_Mauled().ToString())) soldiers += -15;
            if (Traits_List.Contains(Traits.Maimed().ToString())) soldiers += -10;
            if (Traits_List.Contains(Traits.One_Eyed().ToString())) soldiers += -5;
            if (Traits_List.Contains(Traits.One_Legged().ToString())) soldiers += -10;
            if (Traits_List.Contains(Traits.Disfigured().ToString())) soldiers += -5;

            //Minimum of 1 soldier
            if (soldiers < 1) soldiers = 1;

            return soldiers;
        }


        int StarExperience()
        {
            int martial = Martial;
            int value = 0;

            if (martial <= 3)
            {
                //Terrible Martial
                value += 1;

            }
            else if (martial >= 4 && martial <= 7)
            {
                //Poor Martial
                value += 2;
            }
            else if (martial >= 8 && martial <= 11)
            {
                //Averege Martial
                value += 3;
            }
            else if (martial >= 12 && martial <= 15)
            {
                //Good Martial
                value += 4;
            }
            else if (martial >= 16)
            {
                //Excelent Martial
                value += 5;
            }

            if (value > 9) value = 9;
            return value;
        }

        double ProwessExperience()
        {
            int prowess = Prowess;
            int value = 0;
            if (prowess <= 4)
            {
                value += MartialSkill.Terrible();

            }
            else if (prowess >= 5 && prowess <= 8)
            {
                value += MartialSkill.Poor();
            }
            else if (prowess >= 9 && prowess <= 12)
            {
                value += MartialSkill.Average();
            }
            else if (prowess >= 13 && prowess <= 16)
            {
                value += MartialSkill.Good();
            }
            else if (prowess >= 17)
            {
                value += MartialSkill.Excellent();
            }

            return value;
        }

        double MartialExperience()
        {
            int martial = Martial;
            double value = 0;

            if (martial <= 3)
            {
                value += 0.0;

            }
            else if (martial >= 4 && martial <= 7)
            {
                value += 0.2;
            }
            else if (martial >= 8 && martial <= 11)
            {
                value += 0.4;
            }
            else if (martial >= 12 && martial <= 15)
            {
                value += 0.6;
            }
            else if (martial >= 16)
            {
                value += 0.8;
            }


            return value;
        }

        int MartialArmyExperience()
        {
            int martial = Martial;
            int value = 0;

            if (martial <= 3)
            {
                value += MartialSkill.Terrible();

            }
            else if (martial >= 4 && martial <= 7)
            {
                value += MartialSkill.Poor();
            }
            else if (martial >= 8 && martial <= 11)
            {
                value += MartialSkill.Average();
            }
            else if (martial >= 12 && martial <= 15)
            {
                value += MartialSkill.Good();
            }
            else if (martial >= 16)
            {
                value += MartialSkill.Excellent();
            }


            return value;

        }

        public void HasGeneralFallen(string path_attila_log, ICharacter Side)
        {
            using (FileStream logFile = File.Open(path_attila_log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(logFile))
            {
                string str = reader.ReadToEnd();

                if (Side is Player)
                {
                    if (str.Contains("Player general has fallen"))
                    {
                        hasFallen = true;
                        reader.Close();
                        logFile.Close();
                        return;
                    }
                }
                else
                {
                    if (str.Contains("Enemy general has fallen"))
                    {
                        hasFallen = true;
                        reader.Close();
                        logFile.Close();
                        return;
                    }
                }

                hasFallen = false;
            }
        }

        public void Health()
        {
            //50% Chance Light Wounds
            const int WoundedChance = 50; // 50%

            //50% Chance Harsh Wounds
            const int Severely_InjuredChance = 70; // 20%
            const int Brutally_MauledChance = 90; // 20%

            //10% Chance Extreme Wounds
            const int MaimedChance = 93; // 3%
            const int One_LeggedChance = 96;// 3%
            const int One_EyedChance = 99;// 3%
            const int Disfigured = 100; //1%

            var Chance = new Random();
            var RandomNumber = Chance.Next(101);

            string id = ID;
            if (hasFallen)
            {

                // Determine which option to set based on its percentage chance
                if (RandomNumber >= 0 && RandomNumber <= WoundedChance)
                {
                    //SaveFile.SetTraits(id, Traits.Wounded().ToString());
                    Console.Write("Wounded ");
                    return;
                }
                else if (RandomNumber > WoundedChance && RandomNumber <= Severely_InjuredChance)
                {
                    //SaveFile.SetTraits(id, Traits.Severely_Injured().ToString());
                    Console.Write("Severely_Injured ");
                    return;
                }
                else if (RandomNumber > Severely_InjuredChance && RandomNumber <= Brutally_MauledChance)
                {
                    //SaveFile.SetTraits(id, Traits.Brutally_Mauled().ToString());
                    Console.Write("Brutally Mauled ");
                    return;
                }
                else if (RandomNumber > Brutally_MauledChance && RandomNumber <= MaimedChance)
                {
                    //SaveFile.SetTraits(id, Traits.Maimed().ToString());
                    Console.Write("Maimed ");
                    return;
                }
                else if (RandomNumber > MaimedChance && RandomNumber <= One_LeggedChance)
                {
                    //SaveFile.SetTraits(id, Traits.One_Legged().ToString());
                    Console.Write("One Legged ");
                    return;
                }
                else if (RandomNumber > One_LeggedChance && RandomNumber <= One_EyedChance)
                {
                    //SaveFile.SetTraits(id, Traits.One_Eyed().ToString());
                    Console.Write("One Eyed ");
                    return;
                }
                else if (RandomNumber > One_EyedChance && RandomNumber <= Disfigured)
                {
                    //SaveFile.SetTraits(id, Traits.Disfigured().ToString());
                    Console.Write("Disfigured ");
                    return;
                }

            }



        }
    }
}
