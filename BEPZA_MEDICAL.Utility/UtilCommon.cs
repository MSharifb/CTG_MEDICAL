using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.Utility
{
    public class YearMonth
    {
        public string Year { get; set; }
        public string Month { get; set; }
    }

    public class UtilCommon
    {
        #region Methods
        public static int GetMonthNo(string strmonth)
        {
            int intMonthNo = 0;

            switch (strmonth)
            {
                case "January":
                    intMonthNo = 1;
                    break;
                case "February":
                    intMonthNo = 2;
                    break;
                case "March":
                    intMonthNo = 3;
                    break;
                case "April":
                    intMonthNo = 4;
                    break;
                case "May":
                    intMonthNo = 5;
                    break;
                case "June":
                    intMonthNo = 6;
                    break;
                case "July":
                    intMonthNo = 7;
                    break;
                case "August":
                    intMonthNo = 8;
                    break;
                case "September":
                    intMonthNo = 9;
                    break;
                case "October":
                    intMonthNo = 10;
                    break;
                case "November":
                    intMonthNo = 11;
                    break;
                case "December":
                    intMonthNo = 12;
                    break;
            }

            return intMonthNo;
        }

        public static string GetMonthName(int monthNo)
        {
            string MonthName = "";

            switch (monthNo)
            {
                case 1:
                    MonthName = "January";
                    break;
                case 2:
                    MonthName = "February";
                    break;
                case 3:
                    MonthName = "March";
                    break;
                case 4:
                    MonthName = "April";
                    break;
                case 5:
                    MonthName = "May";
                    break;
                case 6:
                    MonthName = "June";
                    break;
                case 7:
                    MonthName = "July";
                    break;
                case 8:
                    MonthName = "August";
                    break;
                case 9:
                    MonthName = "September";
                    break;
                case 10:
                    MonthName = "October";
                    break;
                case 11:
                    MonthName = "November";
                    break;
                case 12:
                    MonthName = "December";
                    break;
            }

            return MonthName;
        }

        public static int GetPreviousMonthLastDay(DateTime date)
        {
            date = date.AddMonths(-1);

            return DateTime.DaysInMonth(date.Year, date.Month);
        }

        public static int GetMonthLastDay(string year, string month)
        {
            int intYear = Convert.ToInt32(year);
            int intMonth = GetMonthNo(month);

            return DateTime.DaysInMonth(intYear, intMonth);
        }

        public static DateTime GetMonthLastDate(string year, string month)
        {
            int intYear = Convert.ToInt32(year);
            int intMonth = GetMonthNo(month);

            return new DateTime(intYear, intMonth, DateTime.DaysInMonth(intYear, intMonth));
        }

        public static DateTime GetPreviousMonthLastDate(DateTime date)
        {
            date = date.AddMonths(-1);

            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        
        #endregion
    }
}
