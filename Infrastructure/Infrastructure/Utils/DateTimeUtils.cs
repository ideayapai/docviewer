using System;

namespace Infrasturcture.Utils
{
    public enum DateTimeConstants
    {
        Today = 0,
        Yesterday,
        LastWeek,
        LastMonth,
        ThisYear,
        LastYear,
        
    }

    public static class DateTimeUtils
    {
         public static string GetBegin(string duration)
         {
            string begin = string.Empty;
            string end = string.Empty;
            GetDuration(duration, ref begin, ref end);
            return begin;
         }

         public static string GetEnd(string duration)
         {
             string begin = string.Empty;
             string end = string.Empty;
             GetDuration(duration, ref begin, ref end);
             return end;
         }

        private static void GetDuration(string duration, ref string begin, ref string end)
        {
            if (string.IsNullOrWhiteSpace(duration))
            {
                begin = string.Empty;
                end = string.Empty;
            }
            else
            {
                DateTimeConstants constants = (DateTimeConstants)Enum.Parse(typeof(DateTimeConstants), duration);
                GetDuratioin(constants, ref begin, ref end);
            }
            
        }

        public static void GetDuratioin(DateTimeConstants constants, ref string begin, ref string end)
         {
             switch(constants)
             {
                 case DateTimeConstants.Today:
                     begin = DateTime.Today.ToString("yyyyMMdd");
                     end = DateTime.Today.AddDays(1).ToString("yyyyMMdd");
                     break;

                 case DateTimeConstants.Yesterday:
                     begin = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
                     end = DateTime.Today.ToString("yyyyMMdd");
                     break;

                 case DateTimeConstants.LastWeek:
                     begin = DateTime.Today.AddDays(-7).ToString("yyyyMMdd");
                     end = DateTime.Today.ToString("yyyyMMdd");
                     break;

                 case DateTimeConstants.LastMonth:
                     begin = DateTime.Today.AddMonths(-1).ToString("yyyyMMdd");
                     end = DateTime.Today.ToString("yyyyMMdd");
                     break;

                 case DateTimeConstants.ThisYear:
                     begin = DateTime.Today.AddYears(-1).ToString("yyyyMMdd");
                     end = DateTime.Today.ToString("yyyyMMdd");
                     break;

                 case DateTimeConstants.LastYear:
                     begin = DateTime.Today.AddYears(-2).ToString("yyyyMMdd");
                     end = DateTime.Today.ToString("yyyyMMdd");
                     break;
             }
         }
    }
   
}