using Crusader_Wars.armies;
using Crusader_Wars.client;
using Crusader_Wars.data.save_file;
using Crusader_Wars.terrain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using static Crusader_Wars.terrain.Lands;


namespace Crusader_Wars
{
    public static class BattleFile
    {

        public static string Unit_Script_Name { get; set; }

        //Get User Path
        static string battlePath = Directory.GetFiles("data\\battle files\\script", "tut_battle.xml", SearchOption.AllDirectories)[0];

        public static void ClearFile()
        {
            
            bool isCreated = false;
            if (isCreated == false)
            {
                using (FileStream logFile = File.Open(battlePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    //File.Create(path_log); 
                    isCreated = true;
                }

            }
        }

        public static void SetArmiesSides(List<Army> attacker_armies, List<Army> defender_armies)
        {
            bool isAttackerEnemy = false,
                 isAttackerPlayer = false;

            bool isDefenderEnemy = false,
                 isDefenderPlayer = false;

            foreach (var army in attacker_armies)
            {
                if (army.CommanderID == CK3LogData.LeftSide.GetCommander().id)
                {
                    isAttackerPlayer = true;
                    isDefenderEnemy = true;
                }
                else if (army.CommanderID == CK3LogData.RightSide.GetCommander().id)
                {
                    isAttackerEnemy = true;
                    isDefenderPlayer = true;
                }
            }

            foreach(var army in attacker_armies)
            {
                if (isAttackerPlayer) army.IsPlayer(true);
                else army.IsEnemy(true);
            }
            foreach(var army in defender_armies)
            {
                if(isDefenderPlayer) army.IsPlayer(true);
                else army.IsEnemy(true);
            }

        }

        // Merge armies until there are only three
        static void MergeArmiesUntilThree(List<Army> armies)
        {
            while (armies.Count > 3)
            {
                for (int i = 3; i < armies.Count; i++)
                {
                    armies[0].AddMergedArmy(armies[i]);
                    //armies[0].Units.AddRange(armies[i].Units);
                    armies.RemoveAt(i);
                }
            }
        }

        // Merge armies until there are only four
        static void MergeArmiesUntilFour(List<Army> armies)
        {
            while (armies.Count > 4)
            {
                for (int i = 4; i < armies.Count; i++)
                {
                    armies[0].AddMergedArmy(armies[i]);
                    //armies[0].Units.AddRange(armies[i].Units);
                    armies.RemoveAt(i);
                }
            }
        }
        static void MergeIntoOneArmy(List<Army> armies)
        {
            if (armies.Count > 1)
            {
                for (int i = 1; i < armies.Count; i++)
                {
                    armies[0].AddMergedArmy(armies[i]);
                    //armies[0].Units.AddRange(armies[i].Units);
                }
                armies.RemoveRange(1, armies.Count - 1); // Remove all armies except the first one
            }
        }

        static Army MergeFriendlies(List<Army> armies, Army main_army)
        {
            string main_owner = main_army.Owner.GetID();
            armies.Remove(main_army);
            
            for (int i = 0; i < armies.Count; i++)
            {
                if (armies[i].Owner.GetID() == main_owner)
                {
                    main_army.AddMergedArmy(armies[i]);
                    //main_army.Units.AddRange(armies[i].Units);
                    //armies.RemoveAt(i);
                }
            }

            if(main_army.MergedArmies != null)
            {
                foreach (Army merged_army in main_army.MergedArmies)
                {
                    armies.Remove(merged_army);
                }
            }
            


            return main_army;
        }

        static void AllControledArmies(List<Army> temp_attacker_armies, List<Army> temp_defender_armies, Army player_army, Army enemy_main_army, int total_soldiers, (string X, string Y, string[] attPositions, string[] defPositions) battleMap)
        {
            //----------------------------------------------
            //  Merge armies until there are only one      
            //----------------------------------------------
            MergeIntoOneArmy(temp_attacker_armies);
            MergeIntoOneArmy(temp_defender_armies);

            // WRITE DECLARATIONS
            DeclarationsFile.CreateAlliances(temp_attacker_armies, temp_defender_armies);

            //Write essential data
            OpenBattle();
            //Write essential data
            OpenPlayerAlliance();


            if (player_army.CombatSide == "attacker")
            {
                //#### WRITE HUMAN PLAYER ARMY
                WriteArmy(temp_attacker_armies[0], battleMap, total_soldiers, false, "stark");
            }
            else if (player_army.CombatSide == "defender")
            {
                //#### WRITE HUMAN PLAYER ARMY
                WriteArmy(temp_defender_armies[0], battleMap, total_soldiers, false, "stark");
            }

            //Write essential data
            SetVictoryCondition();
            //Write essential data
            CloseAlliance();
            //Write essential data
            OpenEnemyAlliance();

            if (enemy_main_army.CombatSide == "attacker")
            {
                //#### WRITE HUMAN PLAYER ARMY
                WriteArmy(temp_attacker_armies[0], battleMap, total_soldiers, false, "bolton");
            }
            else if (enemy_main_army.CombatSide == "defender")
            {
                //#### WRITE HUMAN PLAYER ARMY
                WriteArmy(temp_defender_armies[0], battleMap, total_soldiers, false, "bolton");
            }

            //Write essential data
            SetVictoryCondition();
            //Write essential data
            CloseAlliance();
            //Write battle description
            SetBattleDescription(player_army, total_soldiers);
            //Write battle map
            SetBattleTerrain(battleMap.X, battleMap.Y, Weather.GetWeather(), GetAttilaMap());
            //Write essential data
            CloseBattle();
        }

        static void FriendliesOnlyArmies(List<Army> temp_attacker_armies, List<Army> temp_defender_armies, Army player_army, Army enemy_main_army, int total_soldiers, (string X, string Y, string[] attPositions, string[] defPositions) battleMap)
        {
            //----------------------------------------------
            //  Merge friendly armies to main army     
            //----------------------------------------------
            if (player_army.CombatSide == "attacker")
            {
                player_army = MergeFriendlies(temp_attacker_armies, player_army);
            }
            else if (player_army.CombatSide == "defender")
            {
                player_army = MergeFriendlies(temp_defender_armies, player_army);
            }

            if (enemy_main_army.CombatSide == "attacker")
            {
                enemy_main_army = MergeFriendlies(temp_attacker_armies, enemy_main_army);
            }
            else if (enemy_main_army.CombatSide == "defender")
            {
                enemy_main_army = MergeFriendlies(temp_defender_armies, enemy_main_army);
            }
            //----------------------------------------------
            //  Merge armies until there are only four      
            //----------------------------------------------
            MergeArmiesUntilThree(temp_attacker_armies);
            MergeArmiesUntilThree(temp_defender_armies);

            // WRITE DECLARATIONS
            DeclarationsFile.CreateAlliances(temp_attacker_armies, temp_defender_armies, player_army, enemy_main_army); //<----- include main armies here

            //Write essential data
            OpenBattle();
            //Write essential data
            OpenPlayerAlliance();

            //#### WRITE HUMAN PLAYER ARMY
            WriteArmy(player_army, battleMap, total_soldiers, false, "stark");

            //#### WRITE AI ALLIED ARMIES
            if (player_army.CombatSide == "attacker")
            {
                foreach (var army in temp_attacker_armies)
                {
                    WriteArmy(army, battleMap, total_soldiers, false, "stark");
                }
            }
            else if (player_army.CombatSide == "defender")
            {
                foreach (var army in temp_defender_armies)
                {
                    WriteArmy(army, battleMap, total_soldiers, false, "stark");
                }
            }

            //Write essential data
            SetVictoryCondition();
            //Write essential data
            CloseAlliance();
            //Write essential data
            OpenEnemyAlliance();



            //#### WRITE ENEMY MAIN ARMY
            WriteArmy(enemy_main_army, battleMap, total_soldiers, false, "bolton");

            //#### WRITE ENEMY ALLIED ARMIES
            if (enemy_main_army.CombatSide == "attacker")
            {
                foreach (var army in temp_attacker_armies)
                {
                    WriteArmy(army, battleMap, total_soldiers, false, "bolton");
                }
            }
            else if (enemy_main_army.CombatSide == "defender")
            {
                foreach (var army in temp_defender_armies)
                {
                    WriteArmy(army, battleMap, total_soldiers, false, "bolton");
                }
            }

            //Write essential data
            SetVictoryCondition();
            //Write essential data
            CloseAlliance();
            //Write battle description
            SetBattleDescription(player_army, total_soldiers);
            //Write battle map
            SetBattleTerrain(battleMap.X, battleMap.Y, Weather.GetWeather(), GetAttilaMap());
            //Write essential data
            CloseBattle();
        }

        static void AllSeparateArmies(List<Army> temp_attacker_armies, List<Army> temp_defender_armies, Army player_army, Army enemy_main_army, int total_soldiers, (string X, string Y, string[] attPositions, string[] defPositions) battleMap)
        {
            //----------------------------------------------
            //  Merge armies until there are only four      
            //----------------------------------------------
            MergeArmiesUntilFour(temp_attacker_armies);
            MergeArmiesUntilFour(temp_defender_armies);

            // WRITE DECLARATIONS
            DeclarationsFile.CreateAlliances(temp_attacker_armies, temp_defender_armies);

            //Write essential data
            OpenBattle();
            //Write essential data
            OpenPlayerAlliance();

            //#### WRITE HUMAN PLAYER ARMY
            WriteArmy(player_army, battleMap, total_soldiers, false, "stark");

            //#### WRITE AI ALLIED ARMIES
            if (player_army.CombatSide == "attacker")
            {
                temp_attacker_armies.Remove(player_army);
                foreach (var army in temp_attacker_armies)
                {
                    WriteArmy(army, battleMap, total_soldiers, false, "stark");

                }
            }
            else if (player_army.CombatSide == "defender")
            {
                temp_defender_armies.Remove(player_army);
                foreach (var army in temp_defender_armies)
                {
                    WriteArmy(army, battleMap, total_soldiers, false, "stark");
                }
            }

            //Write essential data
            SetVictoryCondition();
            //Write essential data
            CloseAlliance();
            //Write essential data
            OpenEnemyAlliance();



            //#### WRITE ENEMY MAIN ARMY
            WriteArmy(enemy_main_army, battleMap, total_soldiers, false, "bolton");

            //#### WRITE ENEMY ALLIED ARMIES
            if (enemy_main_army.CombatSide == "attacker")
            {
                temp_attacker_armies.Remove(enemy_main_army);
                foreach (var army in temp_attacker_armies)
                {
                    WriteArmy(army, battleMap, total_soldiers, false, "bolton");
                }
            }
            else if (enemy_main_army.CombatSide == "defender")
            {
                temp_defender_armies.Remove(enemy_main_army);
                foreach (var army in temp_defender_armies)
                {
                    WriteArmy(army, battleMap, total_soldiers, false, "bolton");
                }
            }

            //Write essential data
            SetVictoryCondition();
            //Write essential data
            CloseAlliance();
            //Write battle description
            SetBattleDescription(player_army, total_soldiers);
            //Write battle map
            SetBattleTerrain(battleMap.X, battleMap.Y, Weather.GetWeather(), GetAttilaMap());
            //Write essential data
            CloseBattle();
        }

        public static void BETA_CreateBattle(List<Army> attacker_armies, List<Army> defender_armies)
        {
            //  TEMP OBJETS TO USE HERE
            List<Army> temp_attacker_armies = new List<Army>(),
                       temp_defender_armies = new List<Army>();

            temp_attacker_armies.AddRange(attacker_armies);
            temp_defender_armies.AddRange(defender_armies);

            // SIDES MAIN ARMIES
            Army player_army = null;
            Army enemy_main_army = null;
            player_army = temp_attacker_armies.FirstOrDefault(x => x.IsPlayer() && x.isMainArmy) ?? temp_defender_armies.FirstOrDefault(x => x.IsPlayer() && x.isMainArmy);
            enemy_main_army = temp_attacker_armies.FirstOrDefault(x => x.IsEnemy() && x.isMainArmy) ?? temp_defender_armies.FirstOrDefault(x => x.IsEnemy() && x.isMainArmy);

            //
            if(player_army.Commander!=null)  {
                UnitsFile.PlayerCommanderTraits = player_army.Commander.CommanderTraits;
            }
            else { 
                UnitsFile.PlayerCommanderTraits = null;
            }
            if (enemy_main_army.Commander != null) {
                UnitsFile.EnemyCommanderTraits = enemy_main_army.Commander.CommanderTraits;
            }
            else {
                UnitsFile.EnemyCommanderTraits = null;
            }



            // TOTAL SOLDIERS
            int total_soldiers = 0;
            total_soldiers = temp_attacker_armies.SelectMany(army => army.Units).Sum(unit => unit.GetSoldiers()) +
                             temp_defender_armies.SelectMany(army => army.Units).Sum(unit => unit.GetSoldiers());

            //  BATTLE MAP
            var battleMap = TerrainGenerator.GetBattleMap();
            Deployments.beta_SetSidesDirections(total_soldiers, battleMap);


            //  ALL CONTROLED ARMIES
            //
            if (ModOptions.SeparateArmies() == ModOptions.ArmiesSetup.All_Controled)
            {
                AllControledArmies(temp_attacker_armies, temp_defender_armies, player_army, enemy_main_army, total_soldiers, battleMap);
            }
            //  FRIENDLIES ONLY ARMIES
            //
            else if (ModOptions.SeparateArmies() == ModOptions.ArmiesSetup.Friendly_Only)
            {
                FriendliesOnlyArmies(temp_attacker_armies, temp_defender_armies, player_army, enemy_main_army, total_soldiers, battleMap);
            }
            //  ALL SEPARATE ARMIES
            //
            else if (ModOptions.SeparateArmies() == ModOptions.ArmiesSetup.All_Separate)
            {
                AllSeparateArmies(temp_attacker_armies, temp_defender_armies, player_army, enemy_main_army, total_soldiers, battleMap);
            }
 
        }

        private static void WriteArmy(Army army, (string X, string Y, string[] attPositions, string[] defPositions) battleMap, int total_soldiers, bool isReinforcement, string x)
        {
            //Write essential data
            if (isReinforcement)
                //OpenReinforcementArmy();
                OpenArmy();
            else
                OpenArmy();

            //Write army faction name
            if(army.RealmName is null) {
                AddArmyName("Army");
            }
            else {
                AddArmyName(army.RealmName);
            }
            


            //Write essential data
            if (x == "stark")
                SetPlayerFaction();
            else
                SetEnemyFaction();

            //Write deployment area
            SetDeploymentArea(army.CombatSide, battleMap);
            //Write deployables defenses
            AddDeployablesDefenses(army);
            //Set unit positions values
            SetPositions(total_soldiers, Deployments.beta_GeDirection(army.CombatSide));
            //Write all player army units
            UnitsFile.BETA_ConvertandAddArmyUnits(army);
            //Write essential data
            if (isReinforcement)
                //CloseReinforcementArmy();
                CloseArmy();
            else
                CloseArmy();
        }


        private static string GetAttilaMap()
        {
            string default_attila_map = "Terrain/battles/main_attila_map/";
            return default_attila_map;
            /*
            if (UnitMapper.Attila_Map == null) return default_attila_map;
            else return UnitMapper.Attila_Map;
            */
        }

        private static void OpenBattle()
        {
            string PR_CreateBattle = "<?xml version=\"1.0\"?>\n" +
                                     "<battle>\n";

            File.AppendAllText(battlePath, PR_CreateBattle);

        }

        private static void OpenPlayerAlliance()
        {
            string PR_OpenAlliance = "<alliance id=\"0\">\n";

            File.AppendAllText(battlePath, PR_OpenAlliance);

        }

        private static void OpenArmy()
        {
            string PR_OpenArmy = "<army>\n\n";

            File.AppendAllText(battlePath, PR_OpenArmy);
        }

        private static void OpenReinforcementArmy()
        {
            string PR_OpenArmy = "<reinforcement_army>\n\n";

            File.AppendAllText(battlePath, PR_OpenArmy);
        }

        private static void AddArmyName(string name)
        {
            if(name != String.Empty)
            {
                string PR_ArmyName = $"<army_name>{name}</army_name>\n\n";

                File.AppendAllText(battlePath, PR_ArmyName);
            }
            
        }

        private static void SetPlayerFaction()
        {
            string PR_PlayerFaction = "<faction>historical_house_stark</faction>\n\n";

            File.AppendAllText(battlePath, PR_PlayerFaction);
        }

        private static void SetDeploymentArea(string combat_side, (string, string, string[], string[]) battlemap)
        {
            string PR_Deployment = Deployments.beta_GetDeployment(combat_side); 

            File.AppendAllText(battlePath, PR_Deployment);

        }

        private static void AddDeployablesDefenses(Army army)
        {
            if (army.CombatSide == "defender" && ModOptions.DefensiveDeployables() is true && army.Commander != null)
            {
                int army_soldiers = army.Units.Sum(unit => unit.GetSoldiers());
                army.SetDefences(new DefensiveSystem(army_soldiers, army.Commander.Martial));
                string PR_DefensiveDeployments = army.Defences.GetText();
                File.AppendAllText(battlePath, PR_DefensiveDeployments);
            }
        }


        static int default_x = -350;
        static int default_y = 0;
        static UnitsDeploymentsPosition Position;
        public static void ResetPositions()
        {
            Position = new UnitsDeploymentsPosition(default_x, default_y);
            isFirstDirection = false;
        }

        static string west_rotation = "1.57";
        static string east_rotation = "4.71";
        static string south_rotation = "0.00";
        static string north_rotation = "3.14";


        static string Rotation;

        static bool isFirstDirection = false;
        public static void SetPositions(int total_soldiers, string direction)
        {

            UnitsDeploymentsPosition UnitsPosition = new UnitsDeploymentsPosition(direction, ModOptions.DeploymentsZones(), total_soldiers) ;

            if (!isFirstDirection) { isFirstDirection = true; }
            else
            {
                UnitsPosition.InverseDirection();
            }

            if (UnitsPosition.Direction == "N")
            {
                default_x = 0;
                default_y = 350;
                Position = UnitsPosition;
                Rotation = north_rotation;
            }
            else if (UnitsPosition.Direction == "S")
            {
                default_x = 0;
                default_y = -350;
                Position = UnitsPosition;
                Rotation = south_rotation;
            }
            else if (UnitsPosition.Direction == "E")
            {
                default_x = 350;
                default_y = 0;
                Position = UnitsPosition;
                Rotation = east_rotation;
            }
            else if (UnitsPosition.Direction == "W")
            {
                default_x = -350;
                default_y = 0;
                Position = UnitsPosition;
                Rotation = west_rotation;
            }
        }


        public static void AddUnit(string troopKey, int numSoldiers, int numUnits, int numRest, string unitScript, string unit_experience, string direction)
        {
            if(numSoldiers <= 1 || numUnits == 0) return;


            if (Int32.Parse(unit_experience) < 0) unit_experience = "0";
            if (Int32.Parse(unit_experience) > 9) unit_experience = "9";

            for (int i = 0; i < numUnits; i++)
            {
                //Adding the rest soldiers to the first unit
                if (i == 0) numSoldiers += numRest;

                Unit_Script_Name = unitScript + i.ToString();
                string PR_Unit = $"<unit num_soldiers= \"{numSoldiers}\" script_name= \"{Unit_Script_Name}\">\n" +
                 $"<unit_type type=\"{troopKey}\"/>\n" +
                 $"<position x=\"{Position.X}\" y=\"{Position.Y}\"/>\n" +
                 $"<orientation radians=\"{Rotation}\"/>\n" +
                 "<width metres=\"21.70\"/>\n" +
                 $"<unit_experience level=\"{unit_experience}\"/>\n" +
                 "</unit>\n\n";

                //Add horizontal spacing between units
                if(direction is "N" || direction is "S")
                    Position.AddUnitXSpacing(direction);
                else
                {
                    Position.AddUnitYSpacing(direction);
                }

                //Reset soldiers num to normal
                if (i == 0) numSoldiers -= numRest;

                //Adds Declarations and Locals to the Battle Files
                DeclarationsFile.AddUnitDeclaration("UNIT" + Unit_Script_Name, Unit_Script_Name);
                BattleScript.SetLocals(Unit_Script_Name, "UNIT" + Unit_Script_Name);
                Data.units_scripts.Add((Unit_Script_Name, "UNIT" + Unit_Script_Name));
                
                //Write to file
                File.AppendAllText(battlePath, PR_Unit);
            }

            //Add vertical spacing between units
            if (direction is "N" || direction is "S")
            Position.AddUnitYSpacing(direction);
            else
            Position.AddUnitXSpacing(direction);

        }

        public static void AddGeneralUnit(CommanderSystem Commander, string troopType, string unitScript, int experience, string direction)
        {
            if(Commander != null)
            {
                string name = Commander.Name;
                int numberOfSoldiers = Commander.GetUnitSoldiers();
                var accolade = Commander.GetAccolade();

                if (numberOfSoldiers < 1) return;

                if (experience < 0) experience = 0;
                if (experience > 9) experience = 9;

                for (int i = 0; i < 1; i++)
                {
                    Unit_Script_Name = unitScript + i.ToString();

                    string PR_General = $"<unit num_soldiers= \"{numberOfSoldiers}\" script_name= \"{Unit_Script_Name}\">\n" +
                     $"<unit_type type=\"{troopType}\"/>\n" +
                     $"<position x=\"{Position.Y}\" y=\"{Position.X}\"/>\n" +
                     $"<orientation radians=\"{Rotation}\"/>\n" +
                     "<width metres=\"21.70\"/>\n" +
                     $"<unit_experience level=\"{experience}\"/>\n" +
                     "<unit_capabilities>\n" +
                     "<special_ability></special_ability>\n";


                    if(accolade != null)
                    {
                        var special_ability = AccoladesAbilities.ReturnAbilitiesKeys(accolade);
                        if (special_ability.primaryKey != "null")
                        {
                            PR_General += $"<special_ability>{special_ability.primaryKey}</special_ability>\n";
                        }
                        if (special_ability.secundaryKey != "null")
                        {
                            PR_General += $"<special_ability>{special_ability.secundaryKey}</special_ability>\n";
                        }

                    }

                     PR_General += 
                     "</unit_capabilities>\n" +
                     "<general>\n" +
                     $"<name>{name}</name>\n" +
                     $"<star_rating level=\"{Commander.GetCommanderStarRating()}\"/>\n" +
                     "</general>\n" +
                     "</unit>\n\n";

                    //Add horizontal spacing between units
                    if (direction is "N" || direction is "S")
                        Position.AddUnitXSpacing(direction);
                    else
                    {
                        Position.AddUnitYSpacing(direction);
                    }

                    DeclarationsFile.AddUnitDeclaration("UNIT" + Unit_Script_Name, Unit_Script_Name);
                    BattleScript.SetLocals(Unit_Script_Name, "UNIT" + Unit_Script_Name);
                    Data.units_scripts.Add((Unit_Script_Name, "UNIT" + Unit_Script_Name));
                    File.AppendAllText(battlePath, PR_General);
                }

                //Add vertical spacing between units
                if (direction is "N" || direction is "S")
                    Position.AddUnitYSpacing(direction);
                else
                    Position.AddUnitXSpacing(direction);
            }

        }

        public static void AddKnightUnit(KnightSystem Knights, string troopType, string unitScript, int experience, string direction)
        {
            int numberOfSoldiers = Knights.GetKnightsSoldiers();

            Knights.SetAccolades();
            var accoladesList = Knights.GetAccolades();


            if (numberOfSoldiers == 0) return;


            if (experience < 0) experience = 0;
            if (experience > 9) experience = 9;

            for (int i = 0; i < 1; i++)
            {
                Unit_Script_Name = unitScript + i.ToString();

                string PR_Unit = $"<unit num_soldiers= \"{numberOfSoldiers}\" script_name= \"{Unit_Script_Name}\">\n" +
                 $"<unit_type type=\"{troopType}\"/>\n" +
                 $"<position x=\"{Position.X}\" y=\"{Position.Y}\"/>\n" +
                 $"<orientation radians=\"{Rotation}\"/>\n" +
                 "<width metres=\"21.70\"/>\n" +
                 $"<unit_experience level=\"{experience}\"/>\n";
                
                PR_Unit += "<unit_capabilities>\n";
                PR_Unit += "<special_ability></special_ability>\n"; //dummy ability to remove all abilities from this unit

                //accolades special abilities
                if(accoladesList != null)
                {
                    foreach (var accolade in accoladesList)
                    {
                        var accoladeAbilites = AccoladesAbilities.ReturnAbilitiesKeys(accolade);
                        if (accoladeAbilites.primaryKey != "null")
                        {
                            PR_Unit += $"<special_ability>{accoladeAbilites.primaryKey}</special_ability>\n";
                        }
                        if (accoladeAbilites.secundaryKey != "null")
                        {
                            PR_Unit += $"<special_ability>{accoladeAbilites.secundaryKey}</special_ability>\n";
                        }

                    }
                }


                PR_Unit += "</unit_capabilities>\n";
                PR_Unit += "</unit>\n\n";

                //Add vertical spacing between units
                if (direction is "N" || direction is "S")
                    Position.AddUnitYSpacing(direction);
                else
                    Position.AddUnitXSpacing(direction);


                DeclarationsFile.AddUnitDeclaration("UNIT" + Unit_Script_Name, Unit_Script_Name);
                BattleScript.SetLocals(Unit_Script_Name, "UNIT" + Unit_Script_Name);
                Data.units_scripts.Add((Unit_Script_Name, "UNIT" + Unit_Script_Name));
                File.AppendAllText(battlePath, PR_Unit);
            }

            //Add vertical spacing between units
            if (direction is "N" || direction is "S")
                Position.AddUnitYSpacing(direction);
            else
                Position.AddUnitXSpacing(direction);

        }



        private static void SetVictoryCondition()
        {
            string PR_Victory = "<victory_condition>\n" +
                                "<kill_or_rout_enemy></kill_or_rout_enemy>\n" +
                                "</victory_condition>\n" +
                                "<rout_position x=\"0.00\" y=\"0.00\"/>\n\n";

            File.AppendAllText(battlePath, PR_Victory);


        }

        private static void CloseArmy()
        {
            string PR_CloseArmy = "</army>\n\n";

            File.AppendAllText(battlePath, PR_CloseArmy);

        }
        private static void CloseReinforcementArmy()
        {
            string PR_CloseArmy = "</reinforcement_army>\n\n";

            File.AppendAllText(battlePath, PR_CloseArmy);

        }

        private static void CloseAlliance()
        {
            string PR_CloseAlliance = "</alliance>\n\n";

            File.AppendAllText(battlePath, PR_CloseAlliance);


        }

        private static void OpenEnemyAlliance()
        {
            string PR_OpenAlliance = "<alliance id=\"1\">\n";

            File.AppendAllText(battlePath, PR_OpenAlliance);
        }

        private static void SetEnemyFaction()
        {
            string PR_EnemyFaction = "<faction>historical_house_bolton</faction>\n\n";

            File.AppendAllText(battlePath, PR_EnemyFaction);
        }

        private static void SetBattleDescription(Army army, int total_soldiers)
        {
            switch (army.CombatSide)
            {
                // 0 = player defender 
                // 1 = enemy defender
                case "defender":
                    SetBattleDescription("0", total_soldiers);
                    break;
                case "attacker":
                    SetBattleDescription("1", total_soldiers);
                    break;
                default:
                    SetBattleDescription("1", total_soldiers);
                    break;
            }
        }

        private static void SetBattleDescription(string combat_side, int total_soldiers)
        {
            // 0 = player defender 
            // 1 = enemy defender

            string PR_BattleDescription = "<battle_description>\n" +
                                          "<battle_script prepare_for_fade_in=\"false\">tut_start.lua</battle_script>\n" +
                                          "<time_of_day>day</time_of_day>\n" +
                                          "<season>Summer</season>\n" +
                                          "<precipitation_type>snow</precipitation_type>\n" +
                                          "<type>land_normal</type>\n" +
                                          ModOptions.TimeLimit() +
                                          $"<timeout_winning_alliance_index>{combat_side}</timeout_winning_alliance_index>\n" +
                                          "<boiling_oil></boiling_oil>\n" +
                                          "</battle_description>\n\n";

            string PR_PlayableArea = $"<playable_area dimension=\"{ModOptions.SetMapSize(total_soldiers)}\"/>\n\n";

            File.AppendAllText(battlePath, PR_BattleDescription + PR_PlayableArea);
        }

        private static void SetBattleTerrain(string X, string Y, string weather_key, string attila_map)
        {
        

            string PR_BattleTerrain =   "<weather>\n" +
                                        $"<environment_key>{weather_key}</environment_key>\n" +
                                        "<prevailing_wind x=\"1.00\" y=\"0.00\"/>\n" +
                                        "</weather>\n\n" +

                                        "<sea_surface_name>wind_level_4</sea_surface_name>\n\n" +

                                        "<battle_map_definition>\n" +
                                        $"<name>{attila_map}</name>\n" +
                                        $"<tile_map_position x=\"{X}\" y=\"{Y}\">/</tile_map_position>\n" +
                                        "</battle_map_definition>\n\n";

            File.AppendAllText(battlePath, PR_BattleTerrain);
        }

        private static void CloseBattle()
        {
            string PR_CloseBattle = "</battle>\n";

            File.AppendAllText(battlePath, PR_CloseBattle);
        }


    }
}
