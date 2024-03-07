using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crusader_Wars
{
    public class KnightSystem
    {

        private List<(string ID, int Soldiers, int Prowess, List<string> Traits, BaseSkills BaseSkill, bool isAccolade)> Knights { get; set; }
        private List<(string PrimaryAttribute, string SecundaryAttribute, string Honor)> Accolades { get; set; }
        private int UnitSoldiers { get; set; }

        private int Effectiveness { get; set; }

        private List<string> KilledKnights { get; set; }
        private bool HasKnights { get; set; }

        public List<(string, int, int, List<string>, BaseSkills, bool)> GetKnightsList()
        {
            return Knights;
        }

        public List<(string PrimaryAttribute, string SecundaryAttribute, string Honor)> GetAccolades()
        {
            return Accolades;
        }
        public void SetAccolades(List<(string, string, string)> accolades_list)
        {
            Accolades = accolades_list;
        }


        private double KnightEffectiveness(int level)
        {
            if (level > 0)
            {
                double mulltiplier = Effectiveness / 100;

                double value_to_increase = level * mulltiplier;
                return value_to_increase;
            }
            else
            {
                return 0;
            }

        }


        public void Health()
        {
            if (HasKnights)
            {
                //50% Chance Light Wounds
                const int WoundedChance = 50; // 50%

                //40% Chance Harsh Wounds
                const int Severely_InjuredChance = 70; // 20%
                const int Brutally_MauledChance = 90; // 20%

                //10% Chance Extreme Wounds
                const int MaimedChance = 93; // 3%
                const int One_LeggedChance = 96;// 3%
                const int One_EyedChance = 99;// 3%
                const int Disfigured = 100; //1%

                var Chance = new Random();
                int RandomNumber;

                Console.WriteLine("----------------------------------");
                Console.WriteLine($"KNIGHTS FALLEN - {KilledKnights}\n");

                for (int i = 0; i < KilledKnights.Count; i++)
                {
                    //Random Knight
                    string id = KilledKnights[i];
                    RandomNumber = Chance.Next(101);

                    // Determine which option to set based on its percentage chance
                    if (RandomNumber >= 0 && RandomNumber <= WoundedChance)
                    {
                        SaveFile.SetTraits(id, Traits.Wounded().ToString());
                        Console.Write("Wounded ");
                        continue;
                    }
                    else if (RandomNumber > WoundedChance && RandomNumber <= Severely_InjuredChance)
                    {
                        SaveFile.SetTraits(id, Traits.Severely_Injured().ToString());
                        Console.Write("Severely_Injured ");
                        continue;
                    }
                    else if (RandomNumber > Severely_InjuredChance && RandomNumber <= Brutally_MauledChance)
                    {
                        SaveFile.SetTraits(id, Traits.Brutally_Mauled().ToString());
                        Console.Write("Brutally Mauled ");
                        continue;
                    }
                    else if (RandomNumber > Brutally_MauledChance && RandomNumber <= MaimedChance)
                    {
                        SaveFile.SetTraits(id, Traits.Maimed().ToString());
                        Console.Write("Maimed ");
                        continue;
                    }
                    else if (RandomNumber > MaimedChance && RandomNumber <= One_LeggedChance)
                    {
                        SaveFile.SetTraits(id, Traits.One_Legged().ToString());
                        Console.Write("One Legged ");
                        continue;
                    }
                    else if (RandomNumber > One_LeggedChance && RandomNumber <= One_EyedChance)
                    {
                        SaveFile.SetTraits(id, Traits.One_Eyed().ToString());
                        Console.Write("One Eyed ");
                        continue;
                    }
                    else if (RandomNumber > One_EyedChance && RandomNumber <= Disfigured)
                    {
                        SaveFile.SetTraits(id, Traits.Disfigured().ToString());
                        Console.Write("Disfigured ");
                        continue;
                    }

                    Console.WriteLine("No Trait Set! - ERROR");
                }

            }

        }


        public void GetKilled(int remaining)
        {
            if (HasKnights)
            {
                KilledKnights = new List<string>();

                int totalSoldiers = UnitSoldiers;
                int remainingSoldiers = remaining;


                //random knight
                int soldiers_lost = totalSoldiers - remainingSoldiers;
                while (soldiers_lost > 0)
                {
                    if (Knights.Count == 0) break;

                    Random random = new Random();
                    int random_index = random.Next(Knights.Count);
                    var knight = Knights[random_index];

                    soldiers_lost -= knight.Soldiers;

                    if (soldiers_lost <= 0) break;

                    KilledKnights.Add(knight.ID);

                    Knights.Remove(knight);
                }


            }


        }


        public int SetExperience()
        {
            int prowess_level = (int)ProwessExperience();
            double effectiveness = KnightEffectiveness(prowess_level);
            int level = (int)Math.Round(prowess_level + effectiveness);

            return level;
        }

        public int GetKnightsSoldiers()
        {
            return UnitSoldiers;
        }


        private double ProwessExperience()
        {
            if (HasKnights)
            {
                double value = 0;

                foreach (var knight_value in Knights)
                {
                    if (knight_value.Prowess <= 4)
                    {
                        value += Strength.Terrible();
                    }
                    else if (knight_value.Prowess >= 5 && knight_value.Prowess <= 8)
                    {
                        value += Strength.Poor();
                    }
                    else if (knight_value.Prowess >= 9 && knight_value.Prowess <= 12)
                    {
                        value += Strength.Average();
                    }
                    else if (knight_value.Prowess >= 13 && knight_value.Prowess <= 16)
                    {
                        value += Strength.Good();
                    }
                    else if (knight_value.Prowess >= 17)
                    {
                        value += Strength.Excellent();
                    }


                    //Max level of experience
                    if (value >= 9)
                    {
                        value = 9;
                        break;
                    }
                }

                double rounded = Math.Round(value);
                return rounded;
            }
            else
            {
                return 0;
            }


        }


        private void SetKnightsCount()
        {
            UnitSoldiers = 0;

            if (HasKnights)
            {
                for (int i = 0; i < Knights.Count; i++)
                {
                    int prowess = Knights[i].Prowess;
                    var traits = Knights[i].Traits;
                    Knights[i] = (Knights[i].ID, CalculeKnightStrenght(prowess), prowess, Knights[i].Traits, Knights[i].BaseSkill, Knights[i].isAccolade);
                    UnitSoldiers += Knights[i].Soldiers;
                }

            }
        }

        public void WoundedDebuffs()
        {
            if (HasKnights)
            {
                int debuff = 0;
                for (int i = 0; i < Knights.Count; i++)
                {
                    var knight = Knights[i];
                    var traits = knight.Traits;

                    //Health soldiers debuff
                    if (traits.Contains(Traits.Wounded().ToString())) debuff += -1;
                    if (traits.Contains(Traits.Severely_Injured().ToString())) debuff += -2;
                    if (traits.Contains(Traits.Brutally_Mauled().ToString())) debuff += -3;
                    if (traits.Contains(Traits.Maimed().ToString())) debuff += -2;
                    if (traits.Contains(Traits.One_Eyed().ToString())) debuff += -1;
                    if (traits.Contains(Traits.One_Legged().ToString())) debuff += -2;
                    if (traits.Contains(Traits.Disfigured().ToString())) debuff += -1;

                    Knights[i] = (knight.ID, knight.Soldiers - debuff, knight.Prowess, knight.Traits, knight.BaseSkill, knight.isAccolade);
                }

            }



        }

        int CalculeKnightStrenght(int knight_prowess)
        {
            int value = 0;
            if (knight_prowess <= 4)
            {
                value += 0;
            }
            else if (knight_prowess >= 5 && knight_prowess <= 8)
            {
                value += 1;
            }
            else if (knight_prowess >= 9 && knight_prowess <= 12)
            {
                value += 2;
            }
            else if (knight_prowess >= 13 && knight_prowess <= 16)
            {
                value += 3;
            }
            else if (knight_prowess >= 17)
            {
                value += 4;
            }

            return 3 + value;
        }

        public void SetTraits(string id, List<string> traits)
        {
            var knight = Knights.FirstOrDefault(x => x.ID == id);
            Knights[Knights.IndexOf(knight)] = (knight.ID, knight.Soldiers, knight.Prowess, traits, knight.BaseSkill, knight.isAccolade);
        }
        public void SetSkills(string id, BaseSkills skills)
        {
            var knight = Knights.FirstOrDefault(x => x.ID == id);
            Knights[Knights.IndexOf(knight)] = (knight.ID, knight.Soldiers, knight.Prowess, knight.Traits, skills, knight.isAccolade);
        }


        public void SetData(List<(string, int, int, List<string>, BaseSkills, bool)> data, int effectiveness)
        {
            if (data.Count > 0)
            {
                Knights = data;
                Effectiveness = effectiveness;
                HasKnights = true;
                SetKnightsCount();
            }
            else
            {
                HasKnights = false;
            }

        }

    }
}
