using Crusader_Wars.armies;
using Crusader_Wars.client;
using Crusader_Wars.terrain;
using System;
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
            SetDeploymentArea(Player.CombatSide, battleMap);
            //Write player deployables defenses
            AddDeployablesDefenses(Player);
            //Set unit positions values
            SetPositions();
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
            SetDeploymentArea(Enemy.CombatSide, battleMap);
            //Write enemy deployables defenses
            AddDeployablesDefenses(Enemy);
            //Write unit positions values
            SetPositions();
            //Write all enemy army units
            UnitsFile.ConvertandAddArmyUnits(Enemy);
            //Write essential data
            CloseArmy();
            //Write essential data
            SetVictoryCondition();
            //Write essential data
            CloseAlliance();
            //Write battle description
            SetBattleDescription(Player);
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

        private static void SetDeploymentArea(string combat_side, (string, string, string[], string[]) battlemap)
        {
            string PR_Deployment = Deployments.SetDirection(combat_side, battlemap); ;

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


        static int default_x = -350;
        static int default_y = 0;
        static (int x, int y) Position = (default_x, default_y);
        public static void ResetPositions()
        {
            Position = (default_x, default_y);
        }

        static string west_rotation = "1.57";
        static string east_rotation = "4.71";
        static string south_rotation = "0.00";
        static string north_rotation = "3.14";

        static string Direction = Deployments.Direction;
        static string Rotation;

        public static void SetPositions()
        {
            Direction = Deployments.Direction;

            if (Direction == "N")
            {
                default_x = 0;
                default_y = 350;
                Position = (default_x, default_y);
                Rotation = north_rotation;
            }
            else if (Direction == "S")
            {
                default_x = 0;
                default_y = -350;
                Position = (default_x, default_y);
                Rotation = south_rotation;
            }
            else if (Direction == "E")
            {
                default_x = 350;
                default_y = 0;
                Position = (default_x, default_y);
                Rotation = east_rotation;
            }
            else if (Direction == "W")
            {
                default_x = -350;
                default_y = 0;
                Position = (default_x, default_y);
                Rotation = west_rotation;
            }
        }

        public static void AddUnit(string troopKey, int numSoldiers, int numUnits, int numRest, string unitScript, string unit_experience)
        {
            if(numSoldiers <= 1 || numUnits == 0) return;



            if(Direction == "S" || Direction == "N")
            {
                Position = (default_x, Position.y);
            }
            else if (Direction == "E" || Direction == "W")
            {
                Position = (Position.x, default_y);
            }

            if (Int32.Parse(unit_experience) < 0) unit_experience = "0";
            if (Int32.Parse(unit_experience) > 9) unit_experience = "9";

            for (int i = 0; i < numUnits; i++)
            {
                //Adding the rest soldiers to the first unit
                if (i == 0) numSoldiers += numRest;

                Unit_Script_Name = unitScript + i.ToString();
                string PR_Unit = $"<unit num_soldiers= \"{numSoldiers}\" script_name= \"{Unit_Script_Name}\">\n" +
                 $"<unit_type type=\"{troopKey}\"/>\n" +
                 $"<position x=\"{Position.x}\" y=\"{Position.y}\"/>\n" +
                 $"<orientation radians=\"{Rotation}\"/>\n" +
                 "<width metres=\"21.70\"/>\n" +
                 $"<unit_experience level=\"{unit_experience}\"/>\n" +
                 "</unit>\n\n";

                //Add horizontal spacing between units
                if(Direction is "N" || Direction is "S")
                Position = UnitsFile.AddUnitXSpacing(Direction,Position);
                else
                {
                    Position = UnitsFile.AddUnitYSpacing(Direction, Position);
                }

                //Reset soldiers num to normal
                if (i == 0) numSoldiers -= numRest;

                //Adds Declarations and Locals to the Battle Files
                DeclarationsFile.AddUnitDeclaration("UNIT_" + Unit_Script_Name, Unit_Script_Name);
                BattleScript.SetLocals(Unit_Script_Name, "UNIT_" + Unit_Script_Name);
                
                //Write to file
                File.AppendAllText(battlePath, PR_Unit);
            }

            //Add vertical spacing between units
            if (Direction is "N" || Direction is "S")
            Position = UnitsFile.AddUnitYSpacing(Direction,Position);
            else
            Position = UnitsFile.AddUnitXSpacing(Direction, Position);

        }

        public static void AddGeneralUnit(CommanderSystem Commander, string troopType, string unitScript, int experience)
        {
            if(Commander != null)
            {
                string name = Commander.Name;
                int numberOfSoldiers = Commander.GetUnitSoldiers();

                if (numberOfSoldiers < 1) return;

                if (experience < 0) experience = 0;
                if (experience > 9) experience = 9;

                for (int i = 0; i < 1; i++)
                {
                    Unit_Script_Name = unitScript + i.ToString();

                    string PR_General = $"<unit num_soldiers= \"{numberOfSoldiers}\" script_name= \"{Unit_Script_Name}\">\n" +
                     $"<unit_type type=\"{troopType}\"/>\n" +
                     $"<position x=\"{Position.x}\" y=\"{Position.y}\"/>\n" +
                     $"<orientation radians=\"{Rotation}\"/>\n" +
                     "<width metres=\"21.70\"/>\n" +
                     $"<unit_experience level=\"{experience}\"/>\n" +
                     "<general>\n" +
                     $"<name>{name}</name>\n" +
                     $"<star_rating level=\"{Commander.GetCommanderStarRating()}\"/>\n" +
                     "</general>\n" +
                     "</unit>\n\n";

                    //Add horizontal spacing between units
                    if (Direction is "N" || Direction is "S")
                        Position = UnitsFile.AddUnitXSpacing(Direction, Position);
                    else
                    {
                        Position = UnitsFile.AddUnitYSpacing(Direction, Position);
                    }

                    DeclarationsFile.AddUnitDeclaration("UNIT_" + Unit_Script_Name, Unit_Script_Name);
                    BattleScript.SetLocals(Unit_Script_Name, "UNIT_" + Unit_Script_Name);
                    File.AppendAllText(battlePath, PR_General);
                }

                //Add vertical spacing between units
                if (Direction is "N" || Direction is "S")
                    Position = UnitsFile.AddUnitYSpacing(Direction, Position);
                else
                    Position = UnitsFile.AddUnitXSpacing(Direction, Position);
            }

        }

        public static void AddKnightUnit(KnightSystem Knights, string troopType, string unitScript, int experience)
        {

            Knights.WoundedDebuffs();
            int numberOfSoldiers = Knights.GetKnightsSoldiers();


            if(numberOfSoldiers == 0) return;


            if (experience < 0) experience = 0;
            if (experience > 9) experience = 9;

            for (int i = 0; i < 1; i++)
            {
                Unit_Script_Name = unitScript + i.ToString();

                string PR_Unit = $"<unit num_soldiers= \"{numberOfSoldiers}\" script_name= \"{Unit_Script_Name}\">\n" +
                 $"<unit_type type=\"{troopType}\"/>\n" +
                 $"<position x=\"{Position.x}\" y=\"{Position.y}\"/>\n" +
                 $"<orientation radians=\"{Rotation}\"/>\n" +
                 "<width metres=\"21.70\"/>\n" +
                 $"<unit_experience level=\"{experience}\"/>\n" +
                 "</unit>\n\n";
                //Add vertical spacing between units
                if (Direction is "N" || Direction is "S")
                    Position = UnitsFile.AddUnitYSpacing(Direction, Position);
                else
                    Position = UnitsFile.AddUnitXSpacing(Direction, Position);


                DeclarationsFile.AddUnitDeclaration("UNIT_" + Unit_Script_Name, Unit_Script_Name);
                BattleScript.SetLocals(Unit_Script_Name, "UNIT_" + Unit_Script_Name);
                File.AppendAllText(battlePath, PR_Unit);
            }
            //Add vertical spacing between units
            if (Direction is "N" || Direction is "S")
                Position = UnitsFile.AddUnitYSpacing(Direction, Position);
            else
                Position = UnitsFile.AddUnitXSpacing(Direction, Position);

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

        private static void SetBattleDescription(Player Player)
        {
            switch (Player.CombatSide)
            {
                // 0 = player defender 
                // 1 = enemy defender
                case "defender":
                    SetBattleDescription("0");
                    break;
                case "attacker":
                    SetBattleDescription("1");
                    break;
                default:
                    SetBattleDescription("1");
                    break;
            }
        }

        private static void SetBattleDescription(string combat_side)
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

            string PR_PlayableArea = "<playable_area dimension=\"1500\"/>\n\n";

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
