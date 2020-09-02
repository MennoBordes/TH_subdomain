using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TH.Core.Base.Exceptions;
using TH.Core.Base.Structs;

namespace TH.Core.Base.Extensions
{
    public static class DateTimeExtensions
    {
        private static Dictionary<int, string> _timeTranslations = new Dictionary<int, string>();

        /// <summary> Resolves interval for specified datetime. </summary>
        public static DateTime ResolveInterval(this DateTime input, DateTimeInterval interval)
        {
            DateTime output = input;

            // Resolve
            output = output.AddYears(interval.Years);
            output = output.AddMonths(interval.Months);
            output = output.AddDays(interval.Days);
            output = output.AddHours(interval.Hours);
            output = output.AddMinutes(interval.Minutes);

            return output;
        }

        /// <summary> Extension: Converts a datetime to a raw datetime string. 
        /// <para> Returns a string in the "yyyyMMddHHmmss" format. </para>
        /// <para> Example: "20120416161732". </para>
        /// </summary>
        /// <param name="input"> The input string. </param>
        /// <returns> Returns a string in the "yyyyMMddHHmmss" format. </returns>
        public static string Raw(this DateTime input)
        {
            return string.Format("{0:yyyyMMddHHmmss}", input);
        }

        /// <summary> Extension: Converts a datetime to a raw date string. 
        /// <para> Returns a string in the "yyyyMMdd" format. </para>
        /// <para> Example: "20120416". </para>
        /// </summary>
        /// <param name="input"> The input string. </param>
        /// <returns> Returns a string in the "yyyyMMdd" format. </returns>
        public static string RawDate(this DateTime input)
        {
            return string.Format("{0:yyyyMMdd}", input);
        }

        /// <summary> Extension: Converts a datetime to a raw time string. 
        /// <para> Returns a string in the "HHmmss" format. </para>
        /// <para> Example: "131000". </para>
        /// </summary>
        /// <param name="input"> The input string. </param>
        /// <returns> Returns a string in the "HHmmss" format. </returns>
        public static string RawTime(this DateTime input, bool inclSeconds = true)
        {
            if (inclSeconds)
            {
                return string.Format("{0:HHmmss}", input);
            }
            else
            {
                return string.Format("{0:HHmm}", input);
            }
        }

        /// <summary> Extension: Parse this 'raw' string to a date object. </summary>
        public static DateTime ParseRawDate(this string input)
        {
            return DateTime.ParseExact(input, "yyyyMMdd", null);
        }

        /// <summary> Extension: Try parse this 'raw' string to a date object. </summary>
        public static DateTime? TryParseRawDate(this string input)
        {
            try
            { return input.ParseRawDate(); }
            catch { return null; }
        }

        /// <summary> Extension: Parse this 'raw' string to a datetime object. </summary>
        public static DateTime ParseRawDateTime(this string input)
        {
            return DateTime.ParseExact(input, "yyyyMMddHHmmss", null);
        }

        /// <summary> Extension: Try parse this 'raw' string to a datetime object. </summary>
        public static DateTime? TryParseRawDateTime(this string input)
        {
            try
            { return input.ParseRawDateTime(); }
            catch { return null; }
        }

        /// <summary> Get the next weekday. </summary>
        /// <param name="holidays"> Adds days until the date is not on a holiday and saturday/sunday. </param>
        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day, List<DateTime> holidays = null)
        {
            // So to get the value for "today or in the next 6 days":
            //DateTime nextTuesday = GetNextWeekday(DateTime.Today, DayOfWeek.Tuesday);

            // To get the value for "the next Tuesday excluding today":
            //DateTime nextTuesday = GetNextWeekday(DateTime.Today.AddDays(1), DayOfWeek.Tuesday);

            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;

            DateTime newDate = start.AddDays(daysToAdd);

            // Check holiday
            if (!holidays.IsNullOrEmpty())
            {
                // Check if newDate is on a holiday
                if (holidays.Any(x => x.Date.Date == newDate.Date.Date))
                {
                    bool available = false;
                    while (!available)
                    {
                        newDate = newDate.AddDays(1);

                        if (newDate.DayOfWeek != DayOfWeek.Saturday && newDate.DayOfWeek != DayOfWeek.Sunday && !holidays.Any(x => x.Date.Date == newDate.Date))
                        {
                            available = true;
                            break;
                        }
                    }
                }
            }

            return newDate;
        }

        /// <summary> Get next day</summary>
        public static DateTime GetNextDay(this DateTime value, int increment = 1, DayOfWeek[] skipDays = null, List<DateTime> holidays = null)
        {
            DateTime tempDate = value.AddDays(increment);
            DateTime resultDate = tempDate;

            if (skipDays != null && skipDays.Length > 0)
            {
                List<DayOfWeek> _skip = skipDays.ToList();

                for (DateTime date = value.AddDays(1); date <= tempDate; date = date.AddDays(1))
                {
                    if (_skip.Contains(date.DayOfWeek))
                    {
                        resultDate = resultDate.AddDays(1);
                    }
                }

                while (_skip.Contains(resultDate.DayOfWeek))
                {
                    // remove the occurence to avoid loops
                    _skip.RemoveAll(x => x == resultDate.DayOfWeek);
                    resultDate = resultDate.AddDays(1);
                }
            }

            if (!holidays.IsNullOrEmpty())
            {
                // Check if newDate is on a holiday
                if (holidays.Any(x => x.Date.Date == resultDate.Date.Date))
                {
                    bool available = false;
                    while (!available)
                    {
                        resultDate = resultDate.AddDays(1);

                        if (!skipDays.Contains(resultDate.DayOfWeek) && !holidays.Any(x => x.Date.Date == resultDate.Date))
                        {
                            available = true;
                            break;
                        }
                    }
                }
            }


            return resultDate;
        }

        /// <summary> Get formatted date, based on language id </summary>
        /// <param name="momentRelativeDate"> Used to compare date with other moment than datetime.now </param>
        /// <param name="inclTime"> Includes the time (hours and minutes) </param>
        /// <param name="moment"> Will convert it to: "x hours ago" with a max of "3 days ago" </param>
        /// <param name="written"> If true: "January 1st, 2017", if false: "01/01/2017". Is only effective when moment is false. </param>
        public static string FormatDateTime(this DateTime value, int languageId, string destinationTimeZoneId, bool inclTime = true, bool moment = false, DateTime? momentRelativeDate = null, bool written = true)
        {
            moment = false; // Currently turned off
            string output = "";

            // Get the timezones
            if (string.IsNullOrWhiteSpace(destinationTimeZoneId))
                destinationTimeZoneId = Config.TH_PLATFORM_TIMEZONE_ID;
            TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Config.TH_PLATFORM_TIMEZONE_ID);
            TimeZoneInfo destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId);

            // Convert value to correct time zone
            value = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
            value = TimeZoneInfo.ConvertTime(value, sourceTimeZone, destinationTimeZone);

            // Moment?
            if (moment)
            {                
                string sOneSecondAgo = "one second ago";
                string sSecondsAgo = "{0} seconds ago";
                string sOneMinuteAgo = "one minute ago";
                string sMinutesAgo = "{0} minutes ago";
                string sOneHourAgo = "one hour ago";
                string sHoursAgo = "{0} hours ago";
                string sYesterday = "yesterday";
                string sDaysAgo = "{0} days ago";

                // Current date
                DateTime now = (momentRelativeDate != null) ? momentRelativeDate.Value : DateTime.Now;

                // Get seconds and days between
                TimeSpan ts = new TimeSpan(now.Ticks - value.Ticks);
                double delta = ts.TotalSeconds;

                // Check for yesterday
                if (now.Date - value.Date == TimeSpan.FromDays(0))
                {
                    if (delta < 60)
                        output = (ts.Seconds <= 1) ? sOneSecondAgo : string.Format(sSecondsAgo, ts.Seconds);
                    else if (delta < 120)
                        output = sOneMinuteAgo;
                    else if (delta < 45 * 60)
                        output = string.Format(sMinutesAgo, ts.Minutes);
                    else if (delta < 90 * 60)
                        output = sOneHourAgo;
                    else
                        output = string.Format(sHoursAgo, ts.Hours);
                }
                else if (now.Date - value.Date == TimeSpan.FromDays(1))
                    output = sYesterday;
                else if (now.Date - value.Date == TimeSpan.FromDays(2))
                    output = string.Format(sDaysAgo, 2);
                else if (now.Date - value.Date == TimeSpan.FromDays(3))
                    output = string.Format(sDaysAgo, 3);
                else
                {
                    // More than 3 days, show normal date
                    moment = false;
                }
            }

            // No moment? (different if statement, because moment can be disabled if too long ago)
            if (moment == false)
            {
                CultureInfo ci = null;

                string format = GetFormat(languageId, written);

                // EN-GB
                if (languageId == (int)TH.Core.Base.Database.Entities.Localization.Language.Ref.English_UK)
                {
                    // Culture info
                    ci = new CultureInfo("en-GB");

                    // Format date
                    if (written)
                    {
                        string daySuffix = GetDaySuffix(value.Day);
                        output = String.Format(value.ToString(format, ci), daySuffix);
                    }
                    else
                    {
                        output = value.ToString(format, ci);
                    }
                }
                // EN-US
                else if (languageId == (int)TH.Core.Base.Database.Entities.Localization.Language.Ref.English_US)
                {
                    // Culture info
                    ci = new CultureInfo("en-US");

                    // Format date
                    if (written)
                    {
                        string daySuffix = GetDaySuffix(value.Day);
                        output = String.Format(value.ToString(format, ci), daySuffix);
                    }
                    else
                    {
                        output = value.ToString(format, ci);
                    }
                }

                // Europe (NL, FR, etc)
                else
                {
                    // Culture info => NL
                    ci = new CultureInfo("nl-NL");
                    output = value.ToString(format, ci);
                }


                // Incl time
                if (inclTime)
                { output += value.ToString(" HH:mm", ci); }
            }

            return output;
        }

        /// <summary> Gets the format based on language. </summary>
        public static string GetFormat(int languageId, bool written)
        {
            string format = "dd/MM/yyyy";

            if (languageId == (int)TH.Core.Base.Database.Entities.Localization.Language.Ref.English_UK)
            {
                format = "dd/MM/yyyy";
                if (written)
                    format = "MMMM d{0}, yyyy";
            }
            else if (languageId == (int)TH.Core.Base.Database.Entities.Localization.Language.Ref.English_US)
            {
                format = "MM/dd/yyyy";
                if (written)
                    format = "MMMM d{0}, yyyy";
            }
            else
            {
                format = "dd/MM/yyyy";
                if (written)
                    format = "d MMMM yyyy";
            }


            return format;
        }

        /// <summary> Convert the datetime to the MX timezone (must be used when storing a datetime). </summary>
        public static DateTime ConvertToMxTimeZone(this DateTime value, string timezoneId)
        {
            // Check input
            if (string.IsNullOrWhiteSpace(timezoneId))
                throw new CoreException("No timezone specified.", notifyDevSupport: true);

            // Set Datetime kind
            value = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);

            // Fetch timezones
            TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            if (sourceTimeZone == null)
                throw new CoreException("Invalid timezone specified.", notifyDevSupport: true);

            TimeZoneInfo mxTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Config.TH_PLATFORM_TIMEZONE_ID);

            // Convert value to correct time zone
            return TimeZoneInfo.ConvertTime(value, sourceTimeZone, mxTimeZone);
        }

        /// <summary> Convert the datetime to the users' timezone. </summary>
        public static DateTime ConvertToUserTimeZone(this DateTime value, string timezoneId)
        {
            // Check input
            if (string.IsNullOrWhiteSpace(timezoneId))
                throw new CoreException("No timezone specified.", notifyDevSupport: true);

            // Set Datetime kind
            value = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);

            // Fetch timezones
            TimeZoneInfo destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            if (destinationTimeZone == null)
                throw new CoreException("Invalid timezone specified.", notifyDevSupport: true);

            TimeZoneInfo mxTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Config.TH_PLATFORM_TIMEZONE_ID);

            // Convert value to correct time zone
            return TimeZoneInfo.ConvertTime(value, mxTimeZone, destinationTimeZone);
        }

        /// <summary> Get the day suffix for english notation </summary>
        private static string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        /// <summary> Check if a time is valid. </summary>
        public static bool IsValidTime(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            Regex checktime = new Regex(@"^(([0-1][0-9])|([2][0-3])):([0-5][0-9])$");
            return checktime.IsMatch(input);
        }


        /// <summary> Check if a string is a valid time list (comma separated 'time' strings => example: 09:00,10:15,13:15) and formats it (example: 9:00, 10:00,10:00,13:15 => 09:00,10:00,13:15). </summary>
        /// <returns> error: null, success: formatted string. </returns>
        public static string ValidateTimeList(this string input)
        {
            // Check input
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Check all times
            List<string> validTimeList = new List<string>();

            string[] timeList = input.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string time in timeList)
            {
                // Trim
                string _time = time.Trim();

                // Check time
                if (IsValidTime(_time))
                {
                    if (!validTimeList.Contains(_time))
                        validTimeList.Add(_time);
                }
                else
                {
                    // Try to repair (9:00 => 09:00)                    
                    string hours = _time.GetInnerString("", ":");
                    if (hours != null && hours.Length == 1)
                    {
                        _time = "0" + _time;

                        // Check again
                        if (IsValidTime(_time))
                        {
                            if (!validTimeList.Contains(_time))
                                validTimeList.Add(_time);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

            // Check valid times
            if (validTimeList.Count != 0)
            {
                return string.Join(",", validTimeList);
            }
            else
            {
                return null;
            }
        }


        /// <summary> Check if a string is a valid time block list (comma separated 'time block' strings => example: 09:00-12:30,14:00-17:00) and formats it. </summary>
        /// <returns> error: null, success: formatted string. </returns>
        public static string ValidateTimeBlockList(this string input)
        {
            // Check input
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Check all time blocks
            List<string> validTimeBlockList = new List<string>();

            string[] timeBlockList = input.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string timeBlock in timeBlockList)
            {
                // Trim
                string _timeblock = timeBlock.Trim();

                // Check times
                string[] times = _timeblock.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                if (times.Length != 2)
                {
                    return null;
                }

                // Validate times
                string timeFrom = times[0].ValidateTimeList();
                string timeTo = times[1].ValidateTimeList();
                if (timeFrom == null || timeTo == null)
                    return null;

                DateTime dtFrom = Convert.ToDateTime(timeFrom);
                DateTime dtTo = Convert.ToDateTime(timeTo);
                if (dtTo <= dtFrom)
                    return null;

                // Add time block     
                _timeblock = string.Format("{0}-{1}", timeFrom, timeTo);
                if (!validTimeBlockList.Contains(_timeblock))
                    validTimeBlockList.Add(_timeblock);
            }

            // Check overlap
            //.. todo (v2) => time blocks can't interfere with each other


            // Check valid timeblocks
            if (validTimeBlockList.Count != 0)
            {
                return string.Join(",", validTimeBlockList);
            }
            else
            {
                return null;
            }
        }
    }
}
