using Crusader_Wars.data.save_file;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crusader_Wars
{
    public class UnitCasualitiesReport
    {
        RegimentType UnitType {  get; set; }
        string Type {  get; set; }
        Culture Culture { get; set; }
        int StartingSoldiers {  get; set; }
        int RemainingSoldiers { get; set; }
        int Killed { get; set; }
        
        public UnitCasualitiesReport(RegimentType unit_type, string type,Culture culture, int startingSoldiers,int remaingSoldiers)
        {
            UnitType = unit_type;
            Type = type;
            Culture = culture;
            StartingSoldiers = startingSoldiers;
            RemainingSoldiers = remaingSoldiers;
            Killed = StartingSoldiers - RemainingSoldiers;
        }

        public void PrintReport()
        {
            Console.WriteLine("CASUALITIES REPORT: " + $"{Type}\n" + 
                             $"Culture: {Culture.GetCultureName()}\n" + 
                             $"Starting: {StartingSoldiers}\n" + 
                             $"Alive: {RemainingSoldiers}");
            Console.Write("\n\n");
        }

        public void SetKilled(int i)
        {
            Killed = i;
        }
        
        public int GetStarting() {  return StartingSoldiers; }
        public int GetAlive() {  return RemainingSoldiers; }
        public RegimentType GetUnitType() { return UnitType; }
        public string GetTypeName() { return Type; }
        public Culture GetCulture() { return Culture; }
        public int GetKilled()
        {
            return Killed;
        }
    }

    public class UnitsResults
    {
        //SOLDIERS ALIVE
        public List<(string Script, string Type, string CultureID, string Remaining)> Alive_MainPhase { get; private  set; }
        public List<(string Script, string Type, string CultureID, string Remaining)> Alive_PursuitPhase { get; private set; }


        //SOLDIERS KILLS
        public List<(string Script, string Type, string CultureID, string Kills)> Kills_MainPhase { get; private set; }
        public List<(string Script, string Type, string CultureID, string Kills)> Kills_PursuitPhase { get; private set; }



        //DATA SETTERS

        /// <summary>
        /// Fills the alive soldiers of the Main Phase.
        /// </summary>
        /// <param name="list">List of Main Phase Alive soldiers</param>

        public void SetAliveMainPhase(List<(string, string, string, string)> list)
        {
            Alive_MainPhase = list;
        }

        /// <summary>
        /// Fills the alive soldiers of the Pursuit Phase.
        /// </summary>
        /// <param name="list">List of Pursuit Phase Alive soldiers</param>
        public void SetAlivePursuitPhase(List<(string, string, string, string)> list)
        {
            Alive_PursuitPhase = list;
        }

        /// <summary>
        /// Fills the soldiers kills of the Main Phase.
        /// </summary>
        /// <param name="list">List of Main Phase Kills of soldiers</param>
        public void SetKillsMainPhase(List<(string, string, string, string)> list)
        {
            Kills_MainPhase = list;
        }

        /// <summary>
        /// Fills the soldiers kills of the Pursuit Phase.
        /// </summary>
        /// <param name="list">List of Pursuit Phase Kills of soldiers</param>
        public void SetKillsPursuitPhase(List<(string, string, string, string)> list)
        {
            Kills_PursuitPhase= list;
        }

    }
}
