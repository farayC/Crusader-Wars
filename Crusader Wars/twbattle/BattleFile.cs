using Crusader_Wars.armies;
using Crusader_Wars.client;
using Crusader_Wars.terrain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;


namespace Crusader_Wars
{
    public static class BattleFile
    {

        public static string Unit_Script_Name { get; set; }

        //Get User Path
        static string battlePath = Directory.GetFiles("Battle Files\\script", "tut_battle.xml", SearchOption.AllDirectories)[0];

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

        
        public static void CreateBattle(Player Player, Enemy Enemy)
        {
            var battleMap = TerrainGenerator.GetBattleMap();
            int total_soldiers = Player.TotalNumber + Enemy.TotalNumber;

            //Write essential data
            OpenBattle();
            //Write essential data
            OpenPlayerAlliance();
            //Write essential data
            OpenArmy();
            //Write player army name
            AddArmyName(Player.RealmName);
            //Write essential data
            SetPlayerFaction();
            //Write player deployment area
            SetDeploymentArea(total_soldiers, Player.CombatSide, battleMap);
            //Write player deployables defenses
            AddDeployablesDefenses(Player);
            //Set unit positions values
            SetPositions(total_soldiers);
            //Write all player army units
            UnitsFile.ConvertandAddArmyUnits(Player);
            //Write essential data
            CloseArmy();
            //Write essential data
            SetVictoryCondition();
            //Write essential data
            CloseAlliance();
            //Write essential data
            OpenEnemyAlliance();
            //Write essential data
            OpenArmy();
            //Write enemy army name
            AddArmyName(Enemy.RealmName);
            //Write essential data
            SetEnemyFaction();
            //Write enemy deployment area
            SetDeploymentArea(total_soldiers, Enemy.CombatSide, battleMap);
            //Write enemy deployables defenses
            AddDeployablesDefenses(Enemy);
            //Write unit positions values
            SetPositions(total_soldiers);
            //Write all enemy army units
            UnitsFile.ConvertandAddArmyUnits(Enemy);
            //Write essential data
            CloseArmy();
            //Write essential data
            SetVictoryCondition();
            //Write essential data
            CloseAlliance();
            //Write battle description
            SetBattleDescription(Player, total_soldiers);
            //Write battle map
            SetBattleTerrain(battleMap.X, battleMap.Y, Weather.GetWeather(), GetAttilaMap());
            //Write essential data
            CloseBattle();

        }

        public static void BETA_CreateBattle(List<Army> attacker_armies, List<Army> defender_armies, Player Player, Enemy Enemy)
        {


            var battleMap = TerrainGenerator.GetBattleMap();
            int total_soldiers = 0;

            Army player_army = null;
            Army enemy_main_army = null;
            // TOTAL SOLDIERS
            foreach (var army in attacker_armies) 
            {
                if(army.CommanderID == Player.ID.ToString())
                {
                    army.IsPlayer(true);
                    player_army = army;
                }
                else if(army.CommanderID == Enemy.ID.ToString())
                {
                    enemy_main_army = army;
                }
                foreach(var unit in army.Units)
                {
                    total_soldiers += unit.GetSoldiers();
                }
            }
            foreach(var army in defender_armies)
            {
                if (army.CommanderID == Player.ID.ToString())
                {
                    army.IsPlayer(true);
                    player_army = army;
                }
                else if (army.CommanderID == Enemy.ID.ToString())
                {
                    enemy_main_army = army;
                }
                foreach (var unit in army.Units)
                {
                    total_soldiers += unit.GetSoldiers();
                }
            }

            //----------------------------------------------
            //  TODO: Merge armies until there are only four
            //----------------------------------------------
            if(attacker_armies.Count > 4)
            {
            }
            if(defender_armies.Count > 4)
            {
            }



            

            //Write essential data
            OpenBattle();
            //Write essential data
            OpenPlayerAlliance();

            //#### WRITE HUMAN PLAYER ARMY
            //Write essential data
            OpenArmy();
            //Write player army name
            AddArmyName(player_army.RealmName);
            //Write essential data
            SetPlayerFaction();
            //Write player deployment area
            SetDeploymentArea(total_soldiers, player_army.CombatSide, battleMap);
            //Write player deployables defenses
            AddDeployablesDefenses(player_army);
            //Set unit positions values
            SetPositions(total_soldiers);
            //Write all player army units
            UnitsFile.BETA_ConvertandAddArmyUnits(player_army);
            //Write essential data
            CloseArmy();

            //#### WRITE AI ALLIED ARMIES
            if(player_army.CombatSide == "attacker")
            {
                attacker_armies.Remove(player_army);
                foreach(var army in attacker_armies)
                {
                    //Write essential data
                    OpenArmy();
                    //Write player army name
                    AddArmyName(army.RealmName);
                    //Write essential data
                    SetPlayerFaction();
                    //Write player deployment area
                    SetDeploymentArea(total_soldiers, army.CombatSide, battleMap);
                    //Write player deployables defenses
                    AddDeployablesDefenses(army);
                    //Set unit positions values
                    SetPositions(total_soldiers);
                    //Write all player army units
                    UnitsFile.BETA_ConvertandAddArmyUnits(army);
                    //Write essential data
                    CloseArmy();
                }
            }
            else if(player_army.CombatSide == "defender")
            {
                defender_armies.Remove(player_army);
                foreach (var army in defender_armies)
                {
                    //Write essential data
                    OpenArmy();
                    //Write player army name
                    AddArmyName(army.RealmName);
                    //Write essential data
                    SetPlayerFaction();
                    //Write player deployment area
                    SetDeploymentArea(total_soldiers, army.CombatSide, battleMap);
                    //Write player deployables defenses
                    AddDeployablesDefenses(army);
                    //Set unit positions values
                    SetPositions(total_soldiers);
                    //Write all player army units
                    UnitsFile.BETA_ConvertandAddArmyUnits(army);
                    //Write essential data
                    CloseArmy();
                }
            }

            //Write essential data
            SetVictoryCondition();
            //Write essential data
            CloseAlliance();
            //Write essential data
            OpenEnemyAlliance();



            //#### WRITE ENEMY MAIN ARMY
            //Write essential data
            OpenArmy();
            //Write player army name
            AddArmyName(enemy_main_army.RealmName);
            //Write essential data
            SetPlayerFaction();
            //Write player deployment area
            SetDeploymentArea(total_soldiers, enemy_main_army.CombatSide, battleMap);
            //Write player deployables defenses
            AddDeployablesDefenses(enemy_main_army);
            //Set unit positions values
            SetPositions(total_soldiers);
            //Write all player army units
           UnitsFile.BETA_ConvertandAddArmyUnits(enemy_main_army);
            //Write essential data
            CloseArmy();

            //#### WRITE ENEMY ALLIED ARMIES
            if (enemy_main_army.CombatSide == "attacker")
            {
                attacker_armies.Remove(enemy_main_army);
                foreach (var army in attacker_armies)
                {
                    //Write essential data
                    OpenArmy();
                    //Write player army name
                    AddArmyName(army.RealmName);
                    //Write essential data
                    SetPlayerFaction();
                    //Write player deployment area
                    SetDeploymentArea(total_soldiers, army.CombatSide, battleMap);
                    //Write player deployables defenses
                    AddDeployablesDefenses(army);
                    //Set unit positions values
                    SetPositions(total_soldiers);
                    //Write all player army units
                    UnitsFile.BETA_ConvertandAddArmyUnits(army);
                    //Write essential data
                    CloseArmy();
                }
            }
            else if (enemy_main_army.CombatSide == "defender")
            {
                defender_armies.Remove(enemy_main_army);
                foreach (var army in defender_armies)
                {
                    //Write essential data
                    OpenArmy();
                    //Write player army name
                    AddArmyName(army.RealmName);
                    //Write essential data
                    SetPlayerFaction();
                    //Write player deployment area
                    SetDeploymentArea(total_soldiers, army.CombatSide, battleMap);
                    //Write player deployables defenses
                    AddDeployablesDefenses(army);
                    //Set unit positions values
                    SetPositions(total_soldiers);
                    //Write all player army units
                   UnitsFile.BETA_ConvertandAddArmyUnits(army);
                    //Write essential data
                    CloseArmy();
                }
            }




            //Write essential data
            SetVictoryCondition();
            //Write essential data
            CloseAlliance();
            //Write battle description
            SetBattleDescription(Player, total_soldiers);
            //Write battle map
            SetBattleTerrain(battleMap.X, battleMap.Y, Weather.GetWeather(), GetAttilaMap());
            //Write essential data
            CloseBattle();
        }

        private static string GetAttilaMap()
        {
            string default_attila_map = "Terrain/battles/main_attila_map/";
            if (UnitMapper.Attila_Map == null) return default_attila_map;
            else return UnitMapper.Attila_Map;
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

        private static void SetDeploymentArea(int total_soldiers,string combat_side, (string, string, string[], string[]) battlemap)
        {
            string PR_Deployment = Deployments.SetDirection(total_soldiers, combat_side, battlemap); ;

            File.AppendAllText(battlePath, PR_Deployment);

        }

        private static void AddDeployablesDefenses(ICharacter Side)
        {
            if(Side.CombatSide == "defender" && ModOptions.DefensiveDeployables() is true)
            {
                Side.Defences = new DefensiveSystem();
                string PR_DefensiveDeployments = Side.Defences.SetDefenses(Side.TotalNumber, Side.Commander.Martial);
                File.AppendAllText(battlePath, PR_DefensiveDeployments);
            }
            
        }

        private static void AddDeployablesDefenses(Army army)
        {
            //NEED TO DO THIS!

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

        static string Direction = Deployments.Direction;
        static string Rotation;

        static bool isFirstDirection = false;
        public static void SetPositions(int total_soldiers)
        {
            Direction = Deployments.Direction;
            UnitsDeploymentsPosition UnitsPosition = new UnitsDeploymentsPosition(Direction, ModOptions.DeploymentsZones(), total_soldiers) ;

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


        public static void AddUnit(string troopKey, int numSoldiers, int numUnits, int numRest, string unitScript, string unit_experience)
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
                if(Direction is "N" || Direction is "S")
                    Position.AddUnitXSpacing(Direction);
                else
                {
                    Position.AddUnitYSpacing(Direction);
                }

                //Reset soldiers num to normal
                if (i == 0) numSoldiers -= numRest;

                //Adds Declarations and Locals to the Battle Files
                DeclarationsFile.AddUnitDeclaration("UNIT_" + Unit_Script_Name, Unit_Script_Name);
                BattleScript.SetLocals(Unit_Script_Name, "UNIT_" + Unit_Script_Name);
                Data.units_scripts.Add((Unit_Script_Name, "UNIT_" + Unit_Script_Name));
                
                //Write to file
                File.AppendAllText(battlePath, PR_Unit);
            }

            //Add vertical spacing between units
            if (Direction is "N" || Direction is "S")
            Position.AddUnitYSpacing(Direction);
            else
            Position.AddUnitXSpacing(Direction);

        }

        public static void AddGeneralUnit(CommanderSystem Commander, string troopType, string unitScript, int experience)
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


                    if(!string.IsNullOrEmpty(accolade.Item1))
                    {
                        var special_ability = Accolades.ReturnAbilitiesKeys(accolade);
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
                    if (Direction is "N" || Direction is "S")
                        Position.AddUnitXSpacing(Direction);
                    else
                    {
                        Position.AddUnitYSpacing(Direction);
                    }

                    DeclarationsFile.AddUnitDeclaration("UNIT_" + Unit_Script_Name, Unit_Script_Name);
                    BattleScript.SetLocals(Unit_Script_Name, "UNIT_" + Unit_Script_Name);
                    Data.units_scripts.Add((Unit_Script_Name, "UNIT_" + Unit_Script_Name));
                    File.AppendAllText(battlePath, PR_General);
                }

                //Add vertical spacing between units
                if (Direction is "N" || Direction is "S")
                    Position.AddUnitYSpacing(Direction);
                else
                    Position.AddUnitXSpacing(Direction);
            }

        }

        public static void AddKnightUnit(KnightSystem Knights, string troopType, string unitScript, int experience)
        {

            Knights.WoundedDebuffs();
            int numberOfSoldiers = Knights.GetKnightsSoldiers();
            
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
                        var accoladeAbilites = Accolades.ReturnAbilitiesKeys(accolade);
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
                if (Direction is "N" || Direction is "S")
                    Position.AddUnitYSpacing(Direction);
                else
                    Position.AddUnitXSpacing(Direction);


                DeclarationsFile.AddUnitDeclaration("UNIT_" + Unit_Script_Name, Unit_Script_Name);
                BattleScript.SetLocals(Unit_Script_Name, "UNIT_" + Unit_Script_Name);
                Data.units_scripts.Add((Unit_Script_Name, "UNIT_" + Unit_Script_Name));
                File.AppendAllText(battlePath, PR_Unit);
            }

            //Add vertical spacing between units
            if (Direction is "N" || Direction is "S")
                Position.AddUnitYSpacing(Direction);
            else
                Position.AddUnitXSpacing(Direction);

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

        private static void SetBattleDescription(Player Player, int total_soldiers)
        {
            switch (Player.CombatSide)
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
