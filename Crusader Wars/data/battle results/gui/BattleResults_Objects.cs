using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crusader_Wars
{
    public class Units
    {
        //SOLDIERS ALIVE
        public List<(string Name, string Remaining)> Alive_MainPhase { get; private  set; }
        public List<(string Name, string Remaining)> Alive_PursuitPhase { get; private set; }


        //SOLDIERS KILLS
        public List<(string Name, string Kills)> Kills_MainPhase { get; private set; }
        public List<(string Name, string Kills)> Kills_PursuitPhase { get; private set; }



        //DATA SETTERS

        /// <summary>
        /// Fills the alive soldiers of the Main Phase.
        /// </summary>
        /// <param name="list">List of Main Phase Alive soldiers</param>

        public void SetAliveMainPhase(List<(string, string)> list)
        {
            Alive_MainPhase = list;
        }

        /// <summary>
        /// Fills the alive soldiers of the Pursuit Phase.
        /// </summary>
        /// <param name="list">List of Pursuit Phase Alive soldiers</param>
        public void SetAlivePursuitPhase(List<(string, string)> list)
        {
            Alive_PursuitPhase = list;
        }

        /// <summary>
        /// Fills the soldiers kills of the Main Phase.
        /// </summary>
        /// <param name="list">List of Main Phase Kills of soldiers</param>
        public void SetKillsMainPhase(List<(string, string)> list)
        {
            Kills_MainPhase = list;
        }

        /// <summary>
        /// Fills the soldiers kills of the Pursuit Phase.
        /// </summary>
        /// <param name="list">List of Pursuit Phase Kills of soldiers</param>
        public void SetKillsPursuitPhase(List<(string, string)> list)
        {
            Kills_PursuitPhase= list;
        }

    }
}
