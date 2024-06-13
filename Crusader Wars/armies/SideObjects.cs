using Crusader_Wars.armies;
using Crusader_Wars.data.save_file;
using Crusader_Wars.twbattle;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
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
        UnitsResults UnitsResults {  get; set; }
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
        public UnitsResults UnitsResults { get; set; }
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
        public UnitsResults UnitsResults { get; set; }
        public Supplys Supplys { get; set; }

        public Enemy()
        {
        }
    }

    public class Army
    {
        public string ID { get; set; }
        public string Owner { get; set; }
        public string ArmyUnitID { get; set; }

        public List<ArmyRegiment> ArmyRegiments { get; private set; }
        public List<Unit> Units { get; private set; }
        public KnightSystem Knights { get; private set; }
        public CommanderSystem Commander { get; private set; }

        public string CommanderID { get; set; }
        public bool isMainArmy { get; private set; }
        bool IsHumanPlayer { get; set; }
        bool IsMainEnemy { get; set; }



        public string RealmName { get; set; }
        public string CombatSide { get; set; }
        public UnitsResults UnitsResults { get; set; }


        //public DefensiveSystem Defences { get; set; }
        //public Modifiers Modifiers { get; set; }


        public Army(string id, string combat_side, bool is_main)
        {
            ID = id;
            CombatSide = combat_side;
            isMainArmy = is_main;
        }

        //Getters
        public bool IsEnemy() { return IsMainEnemy; }
        public bool IsPlayer() { return IsHumanPlayer; }


        //Setters
        public void IsPlayer(bool u) { IsHumanPlayer = u; }
        public void IsEnemy(bool u) { IsMainEnemy = u; }
        public void SetUnits(List<Unit> l) { Units = l; }

        public void SetArmyRegiments(List<ArmyRegiment> list)
        {
            ArmyRegiments = list;
        }
        public void SetKnights(KnightSystem knights)
        {
            Knights = knights;
        }
        public void ClearNullRegiments()
        {
            for (int i = 0; i < ArmyRegiments.Count; i++)
            {
                if (ArmyRegiments[i].Regiments is null)
                {
                    ArmyRegiments.Remove(ArmyRegiments[i]);
                }
            }
        }

        public void ClearEmptyRegimnts()
        {
            for (int i = 0; i < ArmyRegiments.Count; i++)
            {
                var t = ArmyRegiments[i].Regiments.Where(origin => string.IsNullOrEmpty(origin.CurrentNum));
                for (int x = 0; x < t.Count(); x++)
                {
                    ArmyRegiments[i].Regiments.Remove(t.ElementAt(x));
                }
            }


        }

        public void RemoveNullUnits()
        {
            var ascending_list = Units.OrderBy(x => x.GetSoldiers()).ToList();
            var major_levy_culture = ascending_list[0];

            int total_soldiers = 0;
            for(int i = 0; i< Units.Count;i++)
            {
                var unit = Units[i];
                if(unit.GetObjCulture() == null)
                {
                    int soldiers = unit.GetSoldiers();
                    total_soldiers += soldiers;
                    Units.Remove(unit);
                }
            }

            foreach(var unit in Units)
            {
                if(major_levy_culture == unit)
                {
                    unit.ChangeSoldiers(unit.GetSoldiers()+total_soldiers);
                }
            }
        }

        public void PrintUnits()
        {
           
            Console.WriteLine($"ARMY - {ID} | {CombatSide}");

            if (Knights.GetKnightsList() != null)
            {
                foreach (var knight in Knights.GetKnightsList())
                {
                    
                    if(knight.IsAccolade())
                    {
                        Console.WriteLine($"## ACCOLADE | Name: {knight.GetName()} | Soldiers: {knight.GetSoldiers()} | Culture: {knight.GetCultureName()} | Heritage: {knight.GetHeritageName()}");
                    }
                    else
                    {
                        Console.WriteLine($"## KNIGHT | Name: {knight.GetName()} | Soldiers: {knight.GetSoldiers()} | Culture: {knight.GetCultureName()} | Heritage: {knight.GetHeritageName()}");
                    }
                }
            }
            foreach (var unit in Units)
            {
                if (unit.IsMerc())
                {
                    Console.WriteLine($"## Hired {unit.GetRegimentType()} | Name: {unit.GetName()} |Soldiers: {unit.GetSoldiers()} | Culture: {unit.GetCulture()} | Heritage: {unit.GetHeritage()} | Unit Key: {unit.GetAttilaUnitKey()}");
                }
                else
                {
                    Console.WriteLine($"## {unit.GetRegimentType()} | Name: {unit.GetName()} |Soldiers: {unit.GetSoldiers()} | Culture: {unit.GetCulture()} | Heritage: {unit.GetHeritage()} | Unit Key: {unit.GetAttilaUnitKey()}");
                }
            }
            Console.WriteLine();
        }


    }
}
