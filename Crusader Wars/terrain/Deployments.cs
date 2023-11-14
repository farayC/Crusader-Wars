using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Crusader_Wars.terrain
{
    public static class Deployments
    {
        public static string Direction { get; private set; }

        //Center Positions
        static (string X, string Y) CENTER_NORTH = ("0.00", "450.00");
        static (string X, string Y) CENTER_EAST = ("450.00", "0.00");
        static (string X, string Y) CENTER_WEST = ("-450.00", "0.00");
        static (string X, string Y) CENTER_SOUTH = ("0.00", "-450.00");

        //Radius
        static string ROTATION_0º = "0.00";
        static string ROTATION_90º = "1.57";
        static string ROTATION_180º = "3.14";
        static string ROTATION_270º = "4.71";
        static string ROTATION_360º = "6.28";

        //Sizes
        static (string Width, string Height) HORIZONTAL_SIZE = ("1300", "400");
        static (string Width, string Height) VERTICAL_SIZE = ("400", "1300");

        struct Directions
        {
            static string SetSouth()
            {
                string PR_Deployment = "<deployment_area>\n" +
                          $"<centre x =\"{CENTER_SOUTH.X}\" y =\"{CENTER_SOUTH.Y}\"/>\n" +
                          $"<width metres =\"{HORIZONTAL_SIZE.Width}\"/>\n" +
                          $"<height metres =\"{HORIZONTAL_SIZE.Height}\"/>\n" +
                          $"<orientation radians =\"{ROTATION_0º}\"/>\n" +
                          "</deployment_area>\n\n";
                return PR_Deployment;
            }
            static string SetWest()
            {
                string PR_Deployment = "<deployment_area>\n" +
                          $"<centre x =\"{CENTER_WEST.X}\" y =\"{CENTER_WEST.Y}\"/>\n" +
                          $"<width metres =\"{VERTICAL_SIZE.Width}\"/>\n" +
                          $"<height metres =\"{VERTICAL_SIZE.Height}\"/>\n" +
                          $"<orientation radians =\"{ROTATION_180º}\"/>\n" +
                          "</deployment_area>\n\n";
                return PR_Deployment;
            }
            static string SetEast()
            {
                string PR_Deployment = "<deployment_area>\n" +
                          $"<centre x =\"{CENTER_EAST.X}\" y =\"{CENTER_EAST.Y}\"/>\n" +
                          $"<width metres =\"{VERTICAL_SIZE.Width}\"/>\n" +
                          $"<height metres =\"{VERTICAL_SIZE.Height}\"/>\n" +
                          $"<orientation radians =\"{ROTATION_360º}\"/>\n" +
                          "</deployment_area>\n\n";
                return PR_Deployment;
            }
            static string SetNorth()
            {
                string PR_Deployment = "<deployment_area>\n" +
                          $"<centre x =\"{CENTER_NORTH.X}\" y =\"{CENTER_NORTH.Y}\"/>\n" +
                          $"<width metres =\"{HORIZONTAL_SIZE.Width}\"/>\n" +
                          $"<height metres =\"{HORIZONTAL_SIZE.Height}\"/>\n" +
                          $"<orientation radians =\"{ROTATION_180º}\"/>\n" +
                          "</deployment_area>\n\n";
                return PR_Deployment;
            }

            public static string SetOppositeDirection(string direction)
            {
                switch (direction)
                {
                    case "N":
                        return Directions.SetSouth();
                    case "S":
                        return Directions.SetNorth();
                    case "E":
                        return Directions.SetWest();
                    case "W":
                        return Directions.SetEast();
                }

                return null;
            }

            public static string SetDirection(string direction)
            {
                switch (direction)
                {
                    case "N":
                        return Directions.SetNorth();
                    case "S":
                        return Directions.SetSouth();
                    case "E":
                        return Directions.SetEast();
                    case "W":
                        return Directions.SetWest();
                }

                return null;
            }
        }



        static bool isFirstDirectionSet;
        static string OppositeDirection;
        public static string SetDirection(string combat_side,(string x, string y, string[] attacker_dir, string[] defender_dir) battle_map)
        {
            Random random = new Random();

            if(!isFirstDirectionSet)
            {   

                //All directions battle maps
                if (battle_map.attacker_dir[0] == "All")
                {
                    string[] coords = { "N", "S", "E", "W" };
                    int index = random.Next(0, 4);
                    Direction = coords[index];
                    string data_all = Directions.SetDirection(coords[index]);
                    isFirstDirectionSet = true;
                    OppositeDirection = Directions.SetOppositeDirection(coords[index]);
                    return data_all;
                    
                }
                //Defined directions battle maps
                else
                {
                    switch (combat_side)
                    {

                        case "attacker":
                            int index = random.Next(0, 2);
                            string direction = battle_map.attacker_dir[index];
                            Direction = direction;
                            string data = Directions.SetDirection(direction);
                            isFirstDirectionSet = true;
                            OppositeDirection = Directions.SetOppositeDirection(direction);
                            return data;

                        case "defender":
                            int i = random.Next(0, 2);
                            string dir = battle_map.defender_dir[i];
                            Direction = dir;
                            string text = Directions.SetDirection(dir);
                            isFirstDirectionSet = true;
                            OppositeDirection = Directions.SetOppositeDirection(dir);
                            return text;
                    }
                }
            }
            else
            {
                string t = OppositeDirection;
                OppositeDirection = "";
                isFirstDirectionSet= false;
                return t;
            }

            return "";
        }


    }


}

