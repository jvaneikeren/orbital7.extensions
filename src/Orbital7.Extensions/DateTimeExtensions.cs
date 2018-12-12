﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public enum Month
    {
        January = 0,
        February = 1,
        March = 2,
        April = 3,
        May = 4,
        June = 5,
        July = 6,
        August = 7,
        September = 8,
        October = 9,
        November = 10,
        December = 11,
    }

    public static partial class DateTimeExtensions
    {
        public static DateTime UTCToTimeZone(this DateTime date, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(date, DateTimeKind.Utc), timeZone);
        }

        public static DateTime? UTCToTimeZone(this DateTime? date, TimeZoneInfo timeZone)
        {
            if (date.HasValue)
                return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(date.Value, DateTimeKind.Utc), timeZone);
            else
                return null;
        }

        public static string FormatTimeSpan(this TimeSpan ts)
        {
            return String.Format("{0:00}:{1:00}:{2:00}", (int)ts.TotalHours, ts.Minutes, ts.Seconds);
        }

        public static DateTimeSpan CalulateDateTimeSpan(this DateTime date, DateTime dateToCompare)
        {
            return DateTimeSpan.CompareDates(date, dateToCompare);
        }

        public static int CalculateMonthsDifference(this DateTime date, DateTime dateToCompare)
        {
            return ((date.Year - dateToCompare.Year) * 12) + date.Month - dateToCompare.Month;
        }

        public static int CalculateQuartersDifference(this DateTime date, DateTime dateToCompare)
        {
            return ((date.Year - dateToCompare.Year) * 4) + date.ToQuarter() - dateToCompare.ToQuarter();
        }

        public static double CalculateAverageMonthsDifference(this DateTime date, DateTime dateToCompare)
        {
            return date.Subtract(dateToCompare).Days / (365.25 / 12);
        }
        
        public static string FormatAsShortDate(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy");
        }

        public static string FormatAsShortDate(this DateTime? date, string nullValue = "")
        {
            if (date.HasValue)
                return date.Value.FormatAsShortDate();
            else
                return nullValue;
        }
        
        public static string FormatAsShortTime(this DateTime time)
        {
            return time.ToString("h:mm:ss tt");
        }

        public static string FormatAsShortTime(this DateTime? time, string nullValue = "")
        {
            if (time.HasValue)
                return time.Value.FormatAsShortTime();
            else
                return nullValue;
        }

        public static string FormatAsShortDateTime(this DateTime dateTime)
        {
            return dateTime.FormatAsShortDate() + " " + dateTime.FormatAsShortTime();
        }

        public static string FormatAsShortDateTime(this DateTime? dateTime, string nullValue = "")
        {
            if (dateTime.HasValue)
                return dateTime.Value.FormatAsShortDateTime();
            else
                return nullValue;
        }

        public static string FormatAsMonthDate(this DateTime date)
        {
            return date.ToString("MMMM yyyy");
        }

        public static string FormatAsNaiveDateTime(
            this DateTime dateTime)
        {
            return string.Format("{0:yyyy-MM-ddTHH:mm:ss}", dateTime);
        }

        public static string FormatUtcAsISO8601DateTime(
            this DateTime dateTimeUtc)
        {
            return string.Format("{0:yyyy-MM-ddTHH:mm:ss}Z", dateTimeUtc);
        }

        public static string FormatAsISO8601Date(
            this DateTime dateTimeUtc)
        {
            return string.Format("{0:yyyy-MM-dd}", dateTimeUtc);
        }

        public static DateTime RoundToStartOfBusinessDay(this DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday)
                date = date.AddDays(2);
            else if (date.DayOfWeek == DayOfWeek.Sunday)
                date = date.AddDays(1);

            return date.RoundToStartOfDay();
        }

        public static DateTime RoundToStartOfNextBusinessDay(this DateTime date)
        {
            return date.AddDays(1).RoundToStartOfBusinessDay();
        }

        public static DateTime RoundToStartOfHour(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
        }

        public static DateTime RoundToEndOfHour(this DateTime date)
        {
            return date.RoundToStartOfHour().AddHours(1).Subtract(new TimeSpan(0, 0, 0, 1));
        }

        public static DateTime RoundToStartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static DateTime RoundToEndOfDay(this DateTime date)
        {
            return date.RoundToStartOfDay().AddDays(1).Subtract(new TimeSpan(0, 0, 0, 1));
        }

        public static DateTime RoundToStartOfWeek(this DateTime date)
        {
            while (date.DayOfWeek != DayOfWeek.Sunday)
                date = date.AddDays(-1);

            return date.RoundToStartOfDay();
        }

        public static DateTime RoundToEndOfWeek(this DateTime date)
        {
            while (date.DayOfWeek != DayOfWeek.Saturday)
                date = date.AddDays(1);

            return date.RoundToEndOfDay();
        }

        public static DateTime RoundToStartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).RoundToStartOfDay();
        }

        public static int ToQuarter(this DateTime date)
        {
            if (date.Month <= 3)
                return 1;
            if (date.Month <= 6)
                return 2;
            if (date.Month <= 9)
                return 3;
            return 4;
        }

        public static DateTime RoundToStartOfQuarter(this DateTime date)
        {
            if (date.Month <= 3)
                return new DateTime(date.Year, 1, 1);
            if (date.Month <= 6)
                return new DateTime(date.Year, 4, 1);
            if (date.Month <= 9)
                return new DateTime(date.Year, 7, 1);
            return new DateTime(date.Year, 10, 1);
        }

        public static DateTime RoundToEndOfQuarter(this DateTime date)
        {
            if (date.Month <= 3)
                return new DateTime(date.Year, 4, 1).AddDays(-1);
            if (date.Month <= 6)
                return new DateTime(date.Year, 7, 1).AddDays(-1);
            if (date.Month <= 9)
                return new DateTime(date.Year, 10, 1).AddDays(-1);
            return new DateTime(date.Year + 1, 1, 1).AddDays(-1);
        }

        public static DateTime RoundToEndOfMonth(this DateTime date)
        {
            if (date.Month == 12)
                return date.RoundToEndOfYear();
            else
                return new DateTime(date.Year, date.Month + 1, 1).AddDays(-1).RoundToEndOfDay();
        }

        public static DateTime RoundToStartOfYear(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1).RoundToStartOfDay();
        }

        public static DateTime RoundToEndOfYear(this DateTime date)
        {
            return new DateTime(date.Year, 12, 31).RoundToEndOfDay();
        }

        public static string FormatAsFileSystemSafeDate(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string FormatAsFileSystemSafeDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd_HH-mm-ss");
        }
    }
}
