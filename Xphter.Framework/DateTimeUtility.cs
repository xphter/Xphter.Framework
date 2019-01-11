using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Provides utilities to process DateTime.
    /// </summary>
    public static class DateTimeUtility {
        private static DateTime g_19700101 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// The month short names dictionary, key is the short name, value is the month value.
        /// </summary>
        private static IDictionary<string, int> g_monthShortNames = new Dictionary<string, int> {
            {"jan", 1},
            {"feb", 2},
            {"mar", 3},
            {"apr", 4},
            {"may", 5},
            {"jun", 6},
            {"jul", 7},
            {"aug", 8},
            {"sep", 9},
            {"oct", 10},
            {"nov", 11},
            {"dec", 12},
        };

        /// <summary>
        /// Gets the maximum DateTime.
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static DateTime Max(DateTime time1, DateTime time2) {
            return time1 >= time2 ? time1 : time2;
        }

        /// <summary>
        /// Gets the minimum DateTime.
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static DateTime Min(DateTime time1, DateTime time2) {
            return time1 <= time2 ? time1 : time2;
        }

        /// <summary>
        /// Gets the corresponding month value of the specified month short name.
        /// </summary>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public static int GetMonthFromShortName(string shortName) {
            if(string.IsNullOrWhiteSpace(shortName)) {
                throw new ArgumentException("The short name string is null or empty.", "shortName");
            }
            if(!g_monthShortNames.ContainsKey(shortName = shortName.ToLower())) {
                throw new ArgumentException("The short name string is not a valid month short name.", "shortName");
            }

            return g_monthShortNames[shortName];
        }

        /// <summary>
        /// Gets the number of months cross of the specified time1 and time2.
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static int GetCrossedMonthsCount(DateTime time1, DateTime time2) {
            if(time2 < time1) {
                DateTime temp = time1;
                time1 = time2;
                time2 = temp;
            }

            if(time1.Year == time2.Year) {
                return time2.Month - time1.Month + 1;
            } else {
                return (12 - time1.Month + 1) + (time2.Year - time1.Year - 1) * 12 + time2.Month;
            }
        }

        /// <summary>
        /// Gets the first day of current month.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(this DateTime time) {
            return new DateTime(time.Year, time.Month, 1);
        }

        /// <summary>
        /// Gets the last day of current month.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(this DateTime time) {
            return new DateTime(time.Year, time.Month, 1).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Gets the first day of current year.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfYear(this DateTime time) {
            return new DateTime(time.Year, 1, 1);
        }

        /// <summary>
        /// Gets the last day of current year.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfYear(this DateTime time) {
            return new DateTime(time.Year, 1, 1).AddYears(1).AddDays(-1);
        }

        /// <summary>
        /// Gets the week of the year represented by a DateTime.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetWeekOfYear(this DateTime time) {
            int offset = 0;
            DayOfWeek firstDayOfWeek = new DateTime(time.Year, 1, 1).DayOfWeek;

            switch(firstDayOfWeek) {
                case DayOfWeek.Sunday:
                    offset = 1;
                    break;
                case DayOfWeek.Monday:
                    offset = 0;
                    break;
                default:
                    offset = 8 - (int) firstDayOfWeek;
                    break;
            }

            int day = time.DayOfYear - offset;

            return day > 0 ? day / 7 + (day % 7 == 0 ? 0 : 1) + (offset > 0 ? 1 : 0) : 1;
        }

        /// <summary>
        /// Gets the day of the week represented by this instance.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>1, 2, 3, 4, 5, 6, 7</returns>
        public static int GetDayOfWeek(this DateTime dateTime) {
            DayOfWeek dow = dateTime.DayOfWeek;

            switch(dow) {
                case DayOfWeek.Sunday:
                    return 7;
                default:
                    return (int) dow;
            }
        }
        /// <summary>
        /// Gets a value to indicate whether <paramref name="time1"/> and <paramref name="time2"/> is in the same week.
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static bool IsInSameWeek(this DateTime time1, DateTime time2) {
            if(time1.Year == time2.Year) {
                return time1.GetWeekOfYear() == time2.GetWeekOfYear();
            } else {
                return Math.Abs((time1.Date - time2.Date).TotalDays) <= 6;
            }
        }

        /// <summary>
        /// Gets the specified day of current week.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="weekDay">1, 2, 3, 4, 5, 6, 7</param>
        /// <returns></returns>
        public static DateTime GetWeekDay(this DateTime dateTime, int weekDay) {
            if(weekDay < 1 || weekDay > 7) {
                throw new ArgumentOutOfRangeException("weekDay");
            }

            return dateTime.Date.AddDays(weekDay - dateTime.GetDayOfWeek());
        }

        /// <summary>
        /// Gets monday of current week.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetMonday(this DateTime dateTime) {
            return dateTime.GetWeekDay(1);
        }

        /// <summary>
        /// Gets sunday of current week.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetSunday(this DateTime dateTime) {
            return dateTime.GetWeekDay(7);
        }

        /// <summary>
        /// Get a value to indicate whether the DateTime instance only contains date component.
        /// </summary>
        /// <param name="dataTime"></param>
        /// <returns></returns>
        public static bool IsDate(this DateTime dataTime) {
            return dataTime.Hour == 0 && dataTime.Minute == 0 && dataTime.Second == 0 && dataTime.Millisecond == 0;
        }

        /// <summary>
        /// Gets timestamp of the specified DateTime.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTimestamp(this DateTime dateTime) {
            return (long) (dateTime.ToUniversalTime() - g_19700101).TotalMilliseconds;
        }

        /// <summary>
        /// Creates a DateTime from the specified timestamp.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime FromTimestamp(long timestamp) {
            return g_19700101.AddMilliseconds(timestamp).ToLocalTime();
        }

        /// <summary>
        /// Parses <paramref name="text"/> to DateTime and return <paramref name="defaultValue"/> if <paramref name="text"/> not represents a DateTime.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime TryParse(string text, DateTime defaultValue) {
            if(string.IsNullOrWhiteSpace(text)) {
                return defaultValue;
            }

            DateTime value = DateTime.Now;
            if(!DateTime.TryParse(text, out value)) {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Parses <paramref name="text"/> to DateTime and return <paramref name="defaultValue"/> if <paramref name="text"/> not represents a DateTime.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? TryParse(string text, DateTime? defaultValue) {
            if(string.IsNullOrWhiteSpace(text)) {
                return defaultValue;
            }

            DateTime value = DateTime.Now;
            if(DateTime.TryParse(text, out value)) {
                return value;
            } else {
                return defaultValue;
            }
        }
    }
}
