using Crusader_Wars.armies;
using Crusader_Wars.armies.commander_traits;
using Crusader_Wars.data.save_file;
using Crusader_Wars.twbattle;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;

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


    }



    public class Army
    {
        public string ID { get; set; }
        public string Owner { get; set; }
        public Culture OwnerCulture { get; set; }
        public string ArmyUnitID { get; set; }

        public List<Army> MergedArmies { get; private set; }
        public List<ArmyRegiment> ArmyRegiments { get; private set; }
        public List<Unit> Units { get; private set; }
        public KnightSystem Knights { get; private set; }
        public CommanderSystem Commander { get; private set; }
        public CommanderTraits Traits { get; private set; }
        public DefensiveSystem Defences { get; private set; }

        public string CommanderID { get; set; }
        public bool isMainArmy { get; private set; }
        bool IsHumanPlayer { get; set; }
        bool IsMainEnemy { get; set; }

        public string RealmName { get; set; }
        public string CombatSide { get; set; }
        public UnitsResults UnitsResults { get; set; }
        public List<UnitCasualitiesReport> CasualitiesReports { get; private set; }

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
        public void AddMergedArmy(Army army) {
            if (MergedArmies == null) { MergedArmies = new List<Army>(); }
            MergedArmies.Add(army); 
        }
        public void IsPlayer(bool u) { IsHumanPlayer = u; }
        public void IsEnemy(bool u) { IsMainEnemy = u; }
        public void SetUnits(List<Unit> l) { Units = l; }
        public void SetCommander(CommanderSystem l) { Commander = l; }
        public void SetCommanderTraits(CommanderTraits l) { Traits = l; }
        public void SetDefences(DefensiveSystem l) { Defences = l; }

        public void SetArmyRegiments(List<ArmyRegiment> list) { ArmyRegiments = list; }
        public void SetKnights(KnightSystem knights){ Knights = knights; }
        public void SetCasualitiesReport(List<UnitCasualitiesReport> reports) { CasualitiesReports = reports; } 
        public void ClearNullArmyRegiments()
        {
            for (int i = 0; i < ArmyRegiments.Count; i++)
            {
                if (ArmyRegiments[i].Regiments is null)
                {
                    ArmyRegiments.Remove(ArmyRegiments[i]);
                }
            }
        }

        public void ClearEmptyRegiments()
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

        public int GetTotalSoldiers()
        {
            return Units.Sum(x => x.GetSoldiers());
        }

        public void ScaleUnits(int ratio)
        {
            if (ratio > 0)
            {
                foreach (var unit in Units)
                {
                    if (unit.GetRegimentType() == RegimentType.Knight || unit.GetRegimentType() == RegimentType.Commander) continue;

                    double porcentage = (double)ratio / 100;
                    double num_ratio = unit.GetSoldiers() * porcentage;
                    num_ratio = Math.Round(num_ratio);
                    unit.ChangeSoldiers((int)num_ratio);
                }

                Console.WriteLine("Army scaled by " + (double)ratio / 100 + '%');
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

            if(Commander != null)
            {
                Console.WriteLine($"## GENERAL | Name: {Commander.Name} | Soldiers: {Commander.GetUnitSoldiers()} | NobleRank: {Commander.Rank} | ArmyXP: +{Commander.GetUnitsExperience()} | Culture: {Commander.GetCultureName()} | Heritage: {Commander.GetHeritageName()}");
            }
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
