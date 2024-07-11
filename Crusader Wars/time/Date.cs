using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Crusader_Wars
{
    public static class Date
    {
        enum Months
        {
            START,
            January, 
            February, 
            March, 
            April, 
            May, 
            June, 
            July, 
            August, 
            September, 
            October, 
            November, 
            December
        };

        public static int GetMonthInteger()
        {

            switch(Month)
            {
                case "January":
                case "enero":
                case "janvier":
                case "Januar":
                case "Январь":
                case "1":
                    return (int)Months.January;
                case "February":
                case "febrero":
                case "février":
                case "Februar":
                case "Февраль":
                case "2":
                    return (int)Months.February;
                case "March":
                case "marzo":
                case "mars":
                case "März":
                case "Март":
                case "3":
                    return (int)Months.March;
                case "April":
                case "abril":
                case "avril":
                //case "April":
                case "Апрель":
                case "4":
                    return (int)Months.April;
                case "May":
                case "mayo":
                case "mai":
                case "Mai":
                case "Май":
                case "5":
                    return (int)Months.May;
                case "June":
                case "junio":
                case "juin":
                case "Juni":
                case "Июнь":
                case "6":
                    return (int)Months.June;
                case "July":
                case "julio":
                case "juillet":
                case "Juli":
                case "Июль":
                case "7":
                    return (int)Months.July;
                case "August":
                case "agosto":
                case "août":
                //case "August":
                case "Август":
                case "8":
                    return (int)Months.August;
                case "September":
                case "septiembre":
                case "septembre":
                //case "Januar":
                case "Сентябрь":
                case "9":
                    return (int)Months.September;
                case "Octuber":
                case "octubre":
                case "octobre":
                case "Oktober":
                case "Октябрь":
                case "10":
                    return (int)Months.October;
                case "November":
                case "noviembre":
                case "novembre":
                //case "November":
                case "Ноябрь":
                case "11":
                    return (int)Months.November;
                case "December":
                case "diciembre":
                case "décembre":
                case "Dezember":
                case "Декабрь":
                case "12":
                    return (int)Months.December;
                default:
                    return (int)Months.January;
            }
        }

        public static string Month { get; set; }
        public static int Year { get; set; }
        
        public static string GetSeason()
        {
            int month = GetMonthInteger();
            string season = "summer";
            //Winter
            if ((month >= 1 && month <= 2) || month == 12)
            {
                season = "winter";
                return season;
            }
            //Spring
            else if (month >= 3 && month <= 5)
            {
                season = "spring";
                return season;
            }
            //Summer
            else if (month >= 6 && month <= 8)
            {
                season = "summer";
                return season;
            }
            //Autumn
            else if (month >= 9 && month <= 11)
            {
                season = "fall";
                return season;
            }

            return season;
        }

    }

    
}
