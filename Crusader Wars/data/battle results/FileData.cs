using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crusader_Wars
{
    public static class FileData
    {
        public static void SetRemainingSoldiers(List<(string Name, string Remaining)> Attila_RemainingSoldiers,
                                           ref List<(string ID, string StartingNum, string CurrentNum)> Side_RegimentsList,
                                           List<(string ID, string Type, string[] ChunksIDs, string Full)> ArmyRegimentsList)
        {
            //Get Attacker Army Regiments
            List<(string ID, string Type, string[] ChunksIDs, string Full)> Attacker_ArmyRegimentsData = new List<(string ID, string Type, string[] ChunksID, string Full)>();
            foreach (var regiment in Side_RegimentsList)
            {
                foreach (var army_regiment in ArmyRegimentsList)
                {
                    (string ID, string Type, string[] ChunksIDs, string Full) data;
                    if (regiment.ID == army_regiment.ID)
                    {
                        data.ID = army_regiment.ID;
                        data.Type = army_regiment.Type;
                        data.ChunksIDs = army_regiment.ChunksIDs;
                        data.Full = army_regiment.Full;

                        Attacker_ArmyRegimentsData.Add(data);
                        break;
                    }
                }
            }


            //Get Total Remaining Levies
            double total_remaining_levies = 0;
            foreach (var unit in Attila_RemainingSoldiers)
            {
                if (unit.Name.Contains("levy"))
                {
                    double num = FileData.ConvertDouble(unit.Remaining);
                    total_remaining_levies += num;
                }
            }


            //Distribute Attila remaining soldiers
            for (int i = 0; i < Side_RegimentsList.Count; i++)
            {
                for (int x = 0; x < Attacker_ArmyRegimentsData.Count; x++)
                {
                    double total_remaining_num = 0;
                    if (Side_RegimentsList[i].ID == Attacker_ArmyRegimentsData[x].ID)
                    {
                        //Men at Arms Distribution
                        for (int y = 0; y < Attila_RemainingSoldiers.Count; y++)
                        {
                            string type = Attacker_ArmyRegimentsData[x].Type.Trim('"');
                            if (Attila_RemainingSoldiers[y].Name.Contains(type) && type != "levy")
                            {

                                double num = FileData.ConvertDouble(Attila_RemainingSoldiers[y].Remaining);
                                total_remaining_num += num;
                                double value = FileData.DistributeValues(total_remaining_num, FileData.ConvertDouble(Side_RegimentsList[i].StartingNum));
                                Side_RegimentsList[i] = (Side_RegimentsList[i].ID, Side_RegimentsList[i].StartingNum, value.ToString());

                            }
                        }

                        //Levies Distribution
                        if (Attacker_ArmyRegimentsData[x].Type == "\"levy\"")
                        {
                            double value = FileData.DistributeValues(total_remaining_levies, FileData.ConvertDouble(Side_RegimentsList[i].StartingNum));
                            total_remaining_levies -= value;
                            Side_RegimentsList[i] = (Side_RegimentsList[i].ID, Side_RegimentsList[i].StartingNum, value.ToString());
                            
                        }


                        break;
                    }
                }
            }

        }
        
        public static void SetChunks(string ArmyRegiments, ref string Regiments,
                                List<(string ID, string StartingNum, string CurrentNum)> Side_RegimentsList,
                                List<(string ID, string Type, string[] ChunksIDs, string Full)> ArmyRegimentsList,
                                List<(string ID, string Type, string Max, string Chunks, string Full)> RegimentsList)
        {

            double currentNum;
            for (int i = 0; i < Side_RegimentsList.Count; i++)
            {
                for (int x = 0; x < ArmyRegimentsList.Count; x++)
                {
                    if (Side_RegimentsList[i].ID == ArmyRegimentsList[x].ID)
                    {
                        //If is not a knight
                        if (FileData.ConvertDouble(Side_RegimentsList[i].StartingNum) > 1)
                        {
                            currentNum = FileData.ConvertDouble(Side_RegimentsList[i].CurrentNum);
                            //Regiments Search
                            for (int p = 0; p < RegimentsList.Count; p++)
                            {
                                //Chunk ID Search
                                for (int y = 0; y < ArmyRegimentsList[x].ChunksIDs.Length; y++)
                                {
                                    if (ArmyRegimentsList[x].ChunksIDs[y] == RegimentsList[p].ID)
                                    {

                                        string Edited_Chunks;
                                        //Check if has "current="
                                        Match match = Regex.Match(RegimentsList[p].Full, @"current=(?<Number>[\d.]+)");
                                        switch (match.Success)
                                        {
                                            case true:
                                                Edited_Chunks = RegimentsList[p].Full;
                                                int index = match.Groups["Number"].Index;
                                                int length = match.Groups["Number"].Length;

                                                //string currentNum = Side_RegimentsList[i].CurrentNum;
                                                double value = DistributeValues(currentNum, FileData.ConvertDouble(RegimentsList[p].Max));
                                                currentNum -= value;


                                                Edited_Chunks = Edited_Chunks.Remove(index, length)
                                                                             .Insert(index, value.ToString());

                                                RegimentsList[p] = (RegimentsList[p].ID, RegimentsList[p].Type, RegimentsList[p].Max, RegimentsList[p].Chunks, Edited_Chunks);

                                                break;

                                            case false:
                                                Edited_Chunks = RegimentsList[p].Full;


                                                //Siege Weapons Fix
                                                if (RegimentsList[p].Type == "\"onager\"" ||
                                                    RegimentsList[p].Type == "\"mangonel\"" ||
                                                    RegimentsList[p].Type == "\"trebuchet\"" ||
                                                    RegimentsList[p].Type == "\"bombard\"")
                                                {
                                                    continue;
                                                }

                                                //If it is Men At Arms
                                                if (RegimentsList[p].Type != "levies")
                                                {

                                                    Console.WriteLine("Before- " + Edited_Chunks);
                                                    Match match_data = Regex.Match(Edited_Chunks, @"size=(?<Size>\d+)\s+owner=(?<OwnerNum>\d+)");
                                                    string ownerNum = match_data.Groups["OwnerNum"].Value;
                                                    string sizeNum = match_data.Groups["Size"].Value;

                                                    double current = DistributeValues(currentNum, FileData.ConvertDouble(RegimentsList[p].Max));
                                                    currentNum -= current;

                                                    string chunks_data = $"owner={ownerNum}\n" +
                                                                          "\t\t\tchunks={\n" +
                                                                          "\t\t\t\t{\n" +
                                                                         $"\t\t\t\t\tmax={sizeNum}\n" +
                                                                         $"\t\t\t\t\tcurrent={current}\n" +
                                                                          "\t\t\t\t}\n" +
                                                                          "\t\t\t}\n";

                                                    Edited_Chunks = Regex.Replace(Edited_Chunks, @"size=", "max=");
                                                    Edited_Chunks = Regex.Replace(Edited_Chunks, @"owner=\d+", chunks_data);


                                                    Console.WriteLine("After - " + Edited_Chunks);
                                                    RegimentsList[p] = (RegimentsList[p].ID, RegimentsList[p].Type, RegimentsList[p].Max, RegimentsList[p].Chunks, Edited_Chunks);
                                                }
                                                else // If it is levies
                                                {
                                                    MatchCollection size_matches = Regex.Matches(Edited_Chunks, @"size=(?<Size>\d+)");
                                                    foreach (Match size in size_matches)
                                                    {
                                                        if (FileData.ConvertDouble(size.Groups["Size"].Value) != 0)
                                                        {
                                                            string sizeNum = size.Groups["Size"].Value;
                                                            int sizeIndex = size.Index;
                                                            int sizeLength = size.Length;

                                                            double current = DistributeValues(currentNum, FileData.ConvertDouble(RegimentsList[p].Max));
                                                            currentNum -= current;

                                                            string data = $"max={sizeNum}\n" +
                                                                          $"current={current}";

                                                            Edited_Chunks = Edited_Chunks.Remove(sizeIndex, sizeLength)
                                                                                         .Insert(sizeIndex, data);

                                                            break;
                                                        }
                                                    }
                                                }

                                                break;
                                        }
                                        break;
                                    }
                                }

                            }
                        }


                    }
                }

            }

            //Change Regiments
            StringBuilder sb = new StringBuilder();
            foreach (var regiment in RegimentsList)
            {
                sb.Append(regiment.Full);
            }
            string Edited_Regiments = sb.ToString();
            //Regiments = Regex.Replace(Regiments, @"\d+={[\s\S]*?\z", Edited_Regiments);
            Data.String_Regiments = Regex.Replace(Data.String_Regiments, @"\d+={[\s\S]*?\z", Edited_Regiments);
        }

        public static double ConvertDouble(string value)
        {
            double num;
            return num = Double.Parse(value, CultureInfo.InvariantCulture);
        }


        static double DistributeValues(double value, double limit)
        {
            double distributed = 0;
            double remainingValue = value;

            double addedValue = Math.Min(remainingValue, limit - distributed);
            distributed += addedValue;
            remainingValue -= addedValue;



            return distributed;
        }

        public static void SetCache(ref string ArmyRegiments,
                                List<(string ID, string StartingNum, string CurrentNum)> Side_RegimentsList,
                                List<(string ID, string Type,string[] ChunksIDs, string Full)> ArmyRegimentsList)
        {
            for (int i = 0; i < Side_RegimentsList.Count; i++)
            {
                for (int x = 0; x < ArmyRegimentsList.Count; x++)
                {

                    if (Side_RegimentsList[i].ID == ArmyRegimentsList[x].ID)
                    {
                        //If is not a knight
                        if (FileData.ConvertDouble(Side_RegimentsList[i].StartingNum) > 1)
                        {
                            //Check if has "current="
                            Match match = Regex.Match(ArmyRegimentsList[x].Full, @"current=(?<Number>[\d.]+)");
                            if (match.Success)
                            {
                                string Edited_Data = ArmyRegimentsList[x].Full;
                                int index = match.Groups["Number"].Index;
                                int length = match.Groups["Number"].Length;

                                string currentNum = Side_RegimentsList[i].CurrentNum;
                                currentNum = string.Format("{0:0.00}", currentNum);
                                currentNum = currentNum.Replace(',', '.');

                                Edited_Data = Edited_Data.Remove(index, length)
                                                         .Insert(index, currentNum);

                                ArmyRegimentsList[x] = (ArmyRegimentsList[x].ID, ArmyRegimentsList[x].Type,ArmyRegimentsList[x].ChunksIDs, Edited_Data);
                                //Console.WriteLine(RegimentsList[x].Full);
                                break;
                            }
                            else
                            {
                                Match match1 = Regex.Match(ArmyRegimentsList[x].Full, @"size=");
                                string Edited_Data = ArmyRegimentsList[x].Full;
                                int index = match1.Index;

                                string currentNum = "current=" + Side_RegimentsList[i].CurrentNum;
                                currentNum = string.Format("{0:0.00}", currentNum);
                                currentNum = currentNum.Replace(',', '.');

                                Edited_Data = Edited_Data.Insert(index, currentNum + "\n\t\t\t\t");

                                ArmyRegimentsList[x] = (ArmyRegimentsList[x].ID, ArmyRegimentsList[x].Type, ArmyRegimentsList[x].ChunksIDs, Edited_Data);
                                //Console.WriteLine(RegimentsList[x].Full);
                                break;

                            }
                        }

                    }
                }
            }
            //Change Army Regiments
            StringBuilder sb = new StringBuilder();
            foreach (var army_regiment in ArmyRegimentsList)
            {
                sb.Append(army_regiment.Full);
            }
            string Edited_Regiments = sb.ToString();
            //ArmyRegiments = Regex.Replace(ArmyRegiments, @"\d+={[\s\S]*?\z", Edited_Regiments);
            Data.String_ArmyRegiments = Regex.Replace(Data.String_ArmyRegiments, @"\d+={[\s\S]*?\z", Edited_Regiments);
        }
    }
}
