using Crusader_Wars.armies;
using Crusader_Wars.data.save_file;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text.RegularExpressions;

namespace Crusader_Wars
{
    public interface ICharacter
    {
        int ID { get; set; }
        string RealmName { get; set; }
        int TotalNumber { get; set; }
        string Heritage { get; set; }
        string Culture { get; set; }
        string AttilaFaction { get; set; }
        List<(string Type, string Key, int Max, string Script, int SoldiersNum)> Army { get; set; }
        string CombatSide { get; set; }
        CommanderSystem Commander { get; set; }
        KnightSystem Knights { get; set; }
        DefensiveSystem Defences { get; set; }
        Modifiers Modifiers { get; set; }
        Units UnitsResults {  get; set; }
        Supplys Supplys { get; set; }


    }

    public class Army
    {
        public string ID { get; set; }
        public string CommanderID { get;set;}
        bool IsMercenaryArmy {  get; set; }
        public List<ArmyRegiment> ArmyRegiments { get; private set; }
        public string RealmName { get; set; }
        public int TotalNumber { get; set; }
        public string Heritage { get; set; }
        public string Culture { get; set; }
        public string AttilaFaction { get; set; }
        public List<(string Type, string Key, int Max, string Script, int SoldiersNum)> CK3_Composition { get; set; }
        public string CombatSide { get; set; }
        public bool isMainArmy {  get; set; }
        public CommanderSystem Commander { get; set; }
        public KnightSystem Knights { get; set; }
        public DefensiveSystem Defences { get; set; }
        public Modifiers Modifiers { get; set; }
        public Units UnitsResults { get; set; }
        public Supplys Supplys { get; set; }


        

        public Army(string id, string combat_side, bool is_main) {
            ID = id;
            CombatSide = combat_side;
            isMainArmy = is_main;
        }

        public void IsMercenary(bool u) { IsMercenaryArmy = u; }
        public bool IsMercenary() { return IsMercenaryArmy; }

        public void SetArmyRegiments(List<ArmyRegiment> list)
        {
            ArmyRegiments = list;
        }

        public void ClearEmptyRegimnts()
        {
            for (int i = 0; i < ArmyRegiments.Count; i++)
            {
                var t = ArmyRegiments[i].Regiments.Where(origin => string.IsNullOrEmpty(origin.Origin) && !origin.isMercenary());
                for (int x = 0; x < t.Count(); x++)
                {
                    ArmyRegiments[i].Regiments.Remove(t.ElementAt(x));
                }
            }


        }


    }

    public class Player : ICharacter
	{
        public int ID { get; set; }
        public string RealmName { get; set; }
        public int TotalNumber { get; set; }
		public string Heritage { get; set; }
		public string Culture { get; set; }
		public string AttilaFaction { get; set; }
		public List<(string Type, string Key, int Max, string Script, int SoldiersNum)> Army { get; set; }
		public string CombatSide { get; set; }
        public CommanderSystem Commander { get; set; }
        public KnightSystem Knights { get; set; }
        public DefensiveSystem Defences { get; set; }
        public Modifiers Modifiers { get; set; }
        public Units UnitsResults { get; set; }
        public Supplys Supplys { get; set; }


        public Player()
		{

		}

	}

    public class Enemy : ICharacter
    {
        public int ID { get; set; }
        public string RealmName { get; set; }
        public int TotalNumber { get; set; }
        public string Heritage { get; set; }
        public string Culture { get; set; }
        public string AttilaFaction { get; set; }
        public List<(string Type, string Key, int Max, string Script, int SoldiersNum)> Army { get; set; }
        public string CombatSide { get; set; }
        public CommanderSystem Commander { get; set; }
        public KnightSystem Knights { get; set; }
        public DefensiveSystem Defences { get; set; }
        public Modifiers Modifiers { get; set; }
        public Units UnitsResults { get; set; }
        public Supplys Supplys { get; set; }

        public Enemy()
        {
        }
    }


}
