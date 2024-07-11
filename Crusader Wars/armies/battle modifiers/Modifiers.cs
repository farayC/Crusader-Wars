using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crusader_Wars.armies
{
    public class Modifiers
    {
        bool IsDefendingStrait { get; set; }
        bool IsDefendingRiver { get; set; }
        bool isLiegeLeadingArmy { get; set; }
        bool isRealmInDebt { get; set; }
        bool isArmyGathering { get; set; }
        bool isArmyLowOnSupplies { get;set; }
        bool isArmyOutOfSupplies { get; set; }
        bool isArmyAttackingAcrossSea { get; set; }

        public int GetXP()
        {
            int xp = 0;
            if (isLiegeLeadingArmy) xp = xp + 1;
            if (isRealmInDebt) xp = xp - 1;
            if(isArmyGathering) xp = xp - 1;
            if (isArmyLowOnSupplies) xp = xp - 1;
            if (isArmyOutOfSupplies) xp -= 2;

            return xp;
        }


        /*
         * COMPLETLY REWORK THIS
         */
        /*
        public void ReadModifiers(string log, ICharacter Side)
        {
            if(Side is Player)
            {
                string modifiers_text = Regex.Match(log, @"(Our Advantage[\s\S]*)\n\n").Groups[1].Value;

                if (modifiers_text.Contains("Defending a Strait Crossing")) { IsDefendingStrait = true; TerrainGenerator.isStraitBattle(true); }
                if (modifiers_text.Contains("Defending a River Crossing")) { IsDefendingRiver = true; TerrainGenerator.isRiverBattle(true); }
                if(modifiers_text.Contains("Defending a Major River Crossing")) { IsDefendingRiver = true; TerrainGenerator.isRiverBattle(true); }
                if(modifiers_text.Contains("Leading Own Soldiers")) { isLiegeLeadingArmy = true; }
                if (modifiers_text.Contains("Debt")) { isRealmInDebt = true; }
                if (modifiers_text.Contains("Gathering Army")) { isArmyGathering = true; }
                if (modifiers_text.Contains("Army Supply is Running Low")) { isArmyLowOnSupplies = true; }
                if (modifiers_text.Contains("Army is Starving")) { isArmyOutOfSupplies = true; }
            }
            else
            {
                string modifiers_text = Regex.Match(log, @"(Their Advantage[\s\S]*)Keyword").Groups[1].Value;

                if (modifiers_text.Contains("Defending a Strait Crossing")) { IsDefendingStrait = true; TerrainGenerator.isStraitBattle(true); }
                if (modifiers_text.Contains("Defending a River Crossing")) { IsDefendingRiver = true; TerrainGenerator.isRiverBattle(true); }
                if (modifiers_text.Contains("Defending a Major River Crossing")) { IsDefendingRiver = true; TerrainGenerator.isRiverBattle(true); }
                if (modifiers_text.Contains("Leading Own Soldiers")) { isLiegeLeadingArmy = true; }
                if (modifiers_text.Contains("Debt")) { isRealmInDebt = true; }
                if (modifiers_text.Contains("Gathering Army")) { isArmyGathering = true; }
                if (modifiers_text.Contains("Army Supply is Running Low")) { isArmyLowOnSupplies = true; }
                if (modifiers_text.Contains("Army is Starving")) { isArmyOutOfSupplies = true; }
            }
            


        }
        */
    }
}
