using Crusader_Wars.data.save_file;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crusader_Wars
{
    public class Knight
    {
        string Name { get; set; }
        string ID { get; set; }
        Culture CultureObj { get; set; }
        int Prowess { get; set; }
        int Soldiers { get; set; }
        List<string> Traits { get; set; }
        BaseSkills BaseSkill { get; set; }
        bool isAccolade { get; set; }
        bool hasFallen { get; set; }    

        public string GetName() {  return Name; }
        public string GetID() { return ID; }
        public string GetCultureName() { return CultureObj.GetCultureName(); }
        public string GetHeritageName() { return CultureObj.GetHeritageName(); }
        public Culture GetCultureObj() { return CultureObj; }
        public int GetSoldiers() { return Soldiers; }
        public int GetProwess() { return Prowess; }
        public bool IsAccolade() { return isAccolade; }
        public bool HasFallen() { return hasFallen; }

        public void HasFallen(bool yn) { hasFallen = yn; }
        public void ChangeCulture(Culture cul) { CultureObj = cul; }
        public void SetTraits(List<string> list_trait) { Traits = list_trait; }


        internal Knight(string name, string id, Culture culture, int prowess, int soldiers) { 
            Name = name;
            ID = id;
            CultureObj = culture;
            Prowess = prowess;
            Soldiers = SetStrengh(soldiers);
        }

        internal Knight(string name, string id, Culture culture, int prowess, int soldiers, bool accolade)
        {
            Name = name;
            ID = id;
            CultureObj = culture;
            Prowess = prowess;
            Soldiers = SetStrengh(soldiers);
            isAccolade = accolade;
        }



        int SetStrengh(int soldiers)
        {
            int value = 0;
            if (Prowess <= 4)
            {
                value += 0;
            }
            else if (Prowess >= 5 && Prowess <= 8)
            {
                value += 1;
            }
            else if (Prowess >= 9 && Prowess <= 12)
            {
                value += 2;
            }
            else if (Prowess >= 13 && Prowess <= 16)
            {
                value += 3;
            }
            else if (Prowess >= 17)
            {
                value += 4;
            }

            return soldiers + value;
        }

        public string Health(string traits_line)
        {
            if (hasFallen)
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

                RandomNumber = Chance.Next(101);

                // Determine which option to set based on its percentage chance
                if (RandomNumber >= 0 && RandomNumber <= WoundedChance)
                {
                    Console.WriteLine("Wounded ");
                    return CharacterWounds.VerifyTraits(traits_line, WoundedTraits.Wounded().ToString());
                }
                else if (RandomNumber > WoundedChance && RandomNumber <= Severely_InjuredChance)
                {
                    Console.WriteLine("Severely_Injured ");
                    return CharacterWounds.VerifyTraits(traits_line, WoundedTraits.Severely_Injured().ToString());
                }
                else if (RandomNumber > Severely_InjuredChance && RandomNumber <= Brutally_MauledChance)
                {
                    Console.WriteLine("Brutally Mauled ");
                    return CharacterWounds.VerifyTraits(traits_line, WoundedTraits.Brutally_Mauled().ToString());
                }
                else if (RandomNumber > Brutally_MauledChance && RandomNumber <= MaimedChance)
                {
                    Console.WriteLine("Maimed ");
                    return CharacterWounds.VerifyTraits(traits_line, WoundedTraits.Maimed().ToString());
                }
                else if (RandomNumber > MaimedChance && RandomNumber <= One_LeggedChance)
                {
                    Console.WriteLine("One Legged ");
                    return CharacterWounds.VerifyTraits(traits_line, WoundedTraits.One_Legged().ToString());
                }
                else if (RandomNumber > One_LeggedChance && RandomNumber <= One_EyedChance)
                {
                    Console.WriteLine("One Eyed ");
                    return CharacterWounds.VerifyTraits(traits_line, WoundedTraits.One_Eyed().ToString());
                }
                else if (RandomNumber > One_EyedChance && RandomNumber <= Disfigured)
                {
                    Console.WriteLine("Disfigured ");
                    return CharacterWounds.VerifyTraits(traits_line, WoundedTraits.Disfigured().ToString());
                }
            }

            return traits_line;

        }

    }
    public class KnightSystem
    {
        private List<Knight> Knights { get; set; }
        private Culture MajorCulture { get; set; }
        private List<(string PrimaryAttribute, string SecundaryAttribute, string Honor)> Accolades { get; set; }
        private int UnitSoldiers { get; set; }

        private int Effectiveness { get; set; }

        //private List<Knight> KilledKnights { get; set; }
        private bool hasKnights { get; set; }


        public KnightSystem(List<Knight> data, int effectiveness)
        {
            if (data.Count > 0)
            {
                Knights = data;
                Effectiveness = effectiveness;
                hasKnights = true;
                SetKnightsCount();
            }
            else
            {
                hasKnights = false;
            }

        }

        public bool HasKnights() { return hasKnights; }
        public Culture GetMajorCulture() { return MajorCulture; }
        public List<Knight> GetKnightsList()
        {
            return Knights;
        }

        public List<(string PrimaryAttribute, string SecundaryAttribute, string Honor)> GetAccolades()
        {
            return Accolades;
        }

        public void SetMajorCulture()
        {
              MajorCulture = Knights.GroupBy(knight => knight.GetCultureObj())
                                    .OrderByDescending(group => group.Count())
                                    .Select(group => group.Key)
                                    .FirstOrDefault();

              if(MajorCulture == null)
                MajorCulture = Knights.FirstOrDefault(x => x.GetCultureObj() != null).GetCultureObj();
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



        public void GetKilled(int remaining)
        {
            if (hasKnights)
            {
                //KilledKnights = new List<Knight>();

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

                    soldiers_lost -= knight.GetSoldiers();

                    if (soldiers_lost <= 0) break;

                    //KilledKnights.Add(knight);

                    Knights[random_index].HasFallen(true);
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
            if (hasKnights)
            {
                double value = 0;

                foreach (var knight_value in Knights)
                {
                    if (knight_value.GetProwess() <= 4)
                    {
                        value += Strength.Terrible();
                    }
                    else if (knight_value.GetProwess() >= 5 && knight_value.GetProwess() <= 8)
                    {
                        value += Strength.Poor();
                    }
                    else if (knight_value.GetProwess() >= 9 && knight_value.GetProwess() <= 12)
                    {
                        value += Strength.Average();
                    }
                    else if (knight_value.GetProwess() >= 13 && knight_value.GetProwess() <= 16)
                    {
                        value += Strength.Good();
                    }
                    else if (knight_value.GetProwess() >= 17)
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

            if (hasKnights)
            {
                for (int i = 0; i < Knights.Count; i++)
                {
                    UnitSoldiers += Knights[i].GetSoldiers();
                }

            }
        }

        public void WoundedDebuffs()
        {
            /*
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

                    Knights[i] = (knight.ID, knight.Soldiers - debuff, knight.Prowess, knight.Traits, knight.BaseSkill, knight.isAccolade, knight.Name);
                }

            }
            */


        }


    }
}
