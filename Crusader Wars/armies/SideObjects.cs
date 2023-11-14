using Crusader_Wars.armies;
using System.Collections.Generic;

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
        Supplys Supplys { get; set; }

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
        public Supplys Supplys { get; set; }

        public Enemy()
        {
        }
    }


}
