using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crusader_Wars.data.battle_results
{
    internal static class GUI_BattleResultTab
    {

        public static string Levies(string line, ICharacter Side)
        {
            //
            //MAIN KILLS
            if (line.Contains("\tmain_kills="))
            {
                int kills = 0;
                foreach (var unit in Side.UnitsResults.Kills_MainPhase)
                {
                    if (unit.Script.Contains("levy_"))
                    {
                        kills += Int32.Parse(unit.Kills);
                    }
                }

                string newline = Regex.Replace(line, "=(.+)", $"={kills}");
                return newline;
            }
            //
            //PURSUIT KILLS
            else if (line.Contains("\tpursuit_kills="))
            {
                int kills = 0;
                if (Side.UnitsResults.Kills_PursuitPhase == null)
                {
                    string noPursuitLine = Regex.Replace(line, "=(.+)", $"={kills}");
                    return noPursuitLine;
                }
                else
                {
                    foreach (var unit in Side.UnitsResults.Kills_PursuitPhase)
                    {
                        if (unit.Script.Contains("levy_"))
                        {
                            kills += Int32.Parse(unit.Kills);
                        }
                    }

                    int main_kills = 0;
                    foreach(var main_unit in Side.UnitsResults.Kills_MainPhase)
                    {
                        if (main_unit.Script.Contains("levy_"))
                        {
                            main_kills += Int32.Parse(main_unit.Kills);
                        }
                    }
                    kills = kills - main_kills;
                }

                string newline = Regex.Replace(line, "=(.+)", $"={kills}");
                return newline;
            }
            //
            //MAIN LOSSES
            else if (line.Contains("\tmain_losses="))
            {
                int initial = 0;
                foreach (var u in Side.Army)
                {
                    if (u.Type.Contains("Levy "))
                    {
                        initial = u.SoldiersNum;
                        break;
                    }
                }
                int alive = 0;
                int losses = 0;
                foreach (var unit in Side.UnitsResults.Alive_MainPhase)
                {
                    if (unit.Script.Contains("levy_"))
                    {
                        alive += Int32.Parse(unit.Remaining);
                    }
                }

                losses = initial - alive;
                string newline = Regex.Replace(line, "=(.+)", $"={losses}");
                return newline;
            }
            //
            //PURSUIT LOSSES
            else if (line.Contains("\tpursuit_losses_maa="))
            {
                int losses = 0;
                if (Side.UnitsResults.Alive_PursuitPhase == null)
                {
                    string noPursuitLine = Regex.Replace(line, "=(.+)", $"={losses}");
                    return noPursuitLine;
                }
                else
                {
                    int initial = 0;
                    foreach (var i in Side.UnitsResults.Alive_MainPhase)
                    {
                        if (i.Script.Contains("levy_"))
                        {
                            initial += Int32.Parse(i.Remaining);
                        }

                    }

                    int alive = 0;
                    foreach (var unit in Side.UnitsResults.Alive_PursuitPhase)
                    {
                        if (unit.Script.Contains("levy_"))
                        {
                            //Main Phase alive soldiers
                            alive += Int32.Parse(unit.Remaining);
                        }
                    }

                    losses = initial - alive;
                    string newline = Regex.Replace(line, "=(.+)", $"={losses}");
                    return newline;
                }


            }
            //
            //ALIVE
            else if (line.Contains("\tfinal_count="))
            {
                return line;
            }

            return line;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static string MenAtArms (string line, ICharacter Side, string MAAtype)
        {
            //
            //MAIN KILLS
            if (line.Contains("\tmain_kills="))
            {
                int kills = 0;
                foreach (var unit in Side.UnitsResults.Kills_MainPhase)
                {
                    if (unit.Script.Contains(MAAtype))
                    {
                        kills += Int32.Parse(unit.Kills);
                    }
                }

                string newline = Regex.Replace(line, "=(.+)", $"={kills}");
                return newline;
            }
            //
            //PURSUIT KILLS
            else if (line.Contains("\tpursuit_kills="))
            {
                int kills = 0;
                if (Side.UnitsResults.Kills_PursuitPhase == null)
                {
                    string noPursuitLine = Regex.Replace(line, "=(.+)", $"={kills}");
                    return noPursuitLine;       
                }
                else
                {
                    
                    foreach (var unit in Side.UnitsResults.Kills_PursuitPhase)
                    {
                        if (unit.Script.Contains(MAAtype))
                        {
                            kills += Int32.Parse(unit.Kills);
                        }
                    }


                    int main_kills = 0;
                    foreach (var main_unit in Side.UnitsResults.Kills_MainPhase)
                    {
                        if (main_unit.Script.Contains(MAAtype))
                        {
                            main_kills += Int32.Parse(main_unit.Kills);
                        }
                    }
                    kills = kills - main_kills;
                    string newline = Regex.Replace(line, "=(.+)", $"={kills}");
                    return newline;
                }


            }
            //
            //MAIN LOSSES
            else if (line.Contains("\tmain_losses="))
            {
                int initial = 0;
                foreach (var i in Side.Army)
                {
                    if (i.Script.Contains(MAAtype))
                    {
                        initial += i.SoldiersNum;
                    }
                }

                int alive = 0;
                int losses = 0;
                foreach (var unit in Side.UnitsResults.Alive_MainPhase)
                {
                    if (unit.Script.Contains(MAAtype))
                    {
                        alive += Int32.Parse(unit.Remaining);
                    }
                }

                losses = initial - alive;
                string newline = Regex.Replace(line, "=(.+)", $"={losses}");
                return newline;
            }
            //
            //PURSUIT LOSSES
            else if (line.Contains("\tpursuit_losses_maa="))
            {
                int losses = 0;
                if (Side.UnitsResults.Alive_PursuitPhase == null)
                {
                    string noPursuitLine = Regex.Replace(line, "=(.+)", $"={losses}");
                    return noPursuitLine;
                }
                else
                {
                    int initial = 0;

                    foreach (var i in Side.UnitsResults.Alive_MainPhase)
                    {
                        if (i.Script.Contains(MAAtype))
                            initial += Int32.Parse(i.Remaining);
                    }

                    int alive = 0;
                    foreach (var unit in Side.UnitsResults.Alive_PursuitPhase)
                    {
                        if (unit.Script.Contains(MAAtype))
                        {
                            //Main Phase alive soldiers
                            alive += Int32.Parse(unit.Remaining);
                        }
                    }

                    losses = initial - alive;
                    string newline = Regex.Replace(line, "=(.+)", $"={losses}");
                    return newline;
                }


            }
            //
            //ALIVE
            else if (line.Contains("\tfinal_count="))
            {
                return line;
            }

            return line;
        }
    }
}
