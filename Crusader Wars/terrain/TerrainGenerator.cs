﻿using Crusader_Wars.terrain;
using System;
using System.Drawing;
using System.Globalization;

namespace Crusader_Wars
{
    //Desert
    //Desert Mountain
    //Dryland
    //Farmlands
    //Floodplains -- X
    //Forest
    //Hills
    //Jungle -- X
    //Mountains
    //Oasis -- X
    //---------------------
    //Plains
    //Steppe
    //Taiga
    //Wetlands


    public static class TerrainGenerator
    {
   
        public static string TerrainType { get; set; }
        public static string Region { get; private set; }

        public static bool isRiver { get; private set; }
        public static bool isStrait { get; private set; }
        public static bool isUnique { get; private set; }

        public static void SetRegion(string a)
        {
            Region = a;
        }
        public static void isUniqueBattle(bool yn)
        {
            switch (yn)
            {
                case true:
                    isUnique = true;
                    return;
                case false:
                    isUnique = false;
                    return;
            }
        }

        public static void isRiverBattle(bool yn)
        {
            switch(yn) 
            {
                case true:
                    isRiver = true;
                    return;
                case false:
                    isRiver = false;
                    return;
            }
        }

        public static void isStraitBattle(bool yn)
        {
            switch (yn)
            {
                case true:
                    isStrait = true;
                    return;
                case false:
                    isStrait = false;
                    return;
            }
        }
        static void ClearData()
        {
            TerrainType = String.Empty;
            isRiver = false;
            isStrait = false;
            isUnique = false;
        }



        public static (string X, string Y, string[] attPositions, string[] defPositions) GetBattleMap()
       {

            //Special Battle Maps           
            if(isUnique)
            {
                var battlemap = UniqueMaps.GetBattleMap();
                //ClearData();
                return battlemap;
            }

            //Straits Battle Maps
            if(isStrait)
            {
                var battlemap = Straits.GetBattleMap(Region ,TerrainType);
                //ClearData();
                return battlemap;   
            }

            //River Battle Maps
            if(isRiver) 
            {
                var battlemap = Lands.GetBattleMap(TerrainType);
                //ClearData();
                return battlemap;
            }

            //Land Battle Maps
            bool isLand = (!isStrait && !isRiver && !isUnique);
            if (isLand) 
            {
                var battlemap = Lands.GetBattleMap(TerrainType);
                //ClearData();
                return battlemap;
            }


            return ("0.146", "0.177", new string[] { "All", "All" }, new string[] { "All", "All" });
       }
    }
}
