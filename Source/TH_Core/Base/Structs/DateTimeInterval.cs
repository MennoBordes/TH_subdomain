using System;
using TH.Core.Base.Exceptions;
using TH.Core.Base.Extensions;
using THTools.ORM;

namespace TH.Core.Base.Structs
{
    /// <summary> Represents the relative value for a datetime. 
    /// <para> Format: 'years:months:days:hours:minutes'. </para>
    /// </summary>
    public struct DateTimeInterval
    {
        private int years;
        private int months;
        private int days;
        private int hours;
        private int minutes;

        public int Years
        {
            get { return this.years; }
        }

        public int Months
        {
            get { return this.months; }
        }

        public int Days
        {
            get { return this.days; }
        }

        public int Hours
        {
            get { return this.hours; }
        }

        public int Minutes
        {
            get { return this.minutes; }
        }

        /// <summary> Constructs a new interval. </summary>
        public DateTimeInterval(int years, int months, int days, int hours, int minutes)
        {
            this.years = years;
            this.months = months;
            this.days = days;
            this.hours = hours;
            this.minutes = minutes;
        }

        /// <summary> Constructs a new interval from string. </summary>
        public DateTimeInterval(string interval)
        {
            try
            {
                string[] values = interval.Split(':');

                years = Convert.ToInt32(values[0]);
                months = Convert.ToInt32(values[1]);
                days = Convert.ToInt32(values[2]);
                hours = Convert.ToInt32(values[3]);
                minutes = Convert.ToInt32(values[4]);
            }
            catch (Exception ex)
            {
                throw new CoreException("Invalid interval specified (" + (interval == null ? "NULL" : "'" + interval + "'") + ")", ex);
            }
        }

        /// <summary> Constructs a new interval from to dates. </summary>
        public DateTimeInterval(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                TimeSpan timeSpan = dateTo - dateFrom;
                this.years = 0;
                this.months = 0;
                this.days = 0;
                this.hours = 0;
                this.minutes = Convert.ToInt32(timeSpan.TotalMinutes);
            }
            catch (Exception ex)
            {
                throw new CoreException("Can't resolve interval.", ex);
            }
        }

        /// <summary> Checks whether the interval has actual values set (interval > 0). </summary>
        public bool HasValue()
        {
            return (this.years > 0 || this.months > 0 || this.days > 0 || this.hours > 0 || this.minutes > 0);
        }

        /// <summary> Converts the interval values to negatives. </summary>
        public void ConvertToNegatives()
        {
            this.years = this.years * -1;
            this.months = this.months * -1;
            this.days = this.days * -1;
            this.hours = this.hours * -1;
            this.minutes = this.minutes * -1;
        }

        /// <summary> Converts the interval to database interval (for usage in date functions). </summary>        
        public Tuple<int, DbTimeUnit> ConvertToDbInterval()
        {
            // Check lowest unit & fetch value            
            if (this.minutes != 0)
            {
                TimeSpan timeSpan = DateTime.Now.ResolveInterval(this) - DateTime.Now;
                return new Tuple<int, DbTimeUnit>(Convert.ToInt32(timeSpan.TotalMinutes), DbTimeUnit.MINUTE);
            }
            else if (this.hours != 0)
            {
                TimeSpan timeSpan = DateTime.Now.ResolveInterval(this) - DateTime.Now;
                return new Tuple<int, DbTimeUnit>(Convert.ToInt32(timeSpan.TotalHours), DbTimeUnit.HOUR);
            }
            else if (this.days != 0)
            {
                TimeSpan timeSpan = DateTime.Now.ResolveInterval(this) - DateTime.Now;
                return new Tuple<int, DbTimeUnit>(Convert.ToInt32(timeSpan.TotalDays), DbTimeUnit.DAY);
            }
            else if (this.months != 0)
            {
                int totalMonths = this.months;
                if (this.years > 0)
                    totalMonths += (this.years * 12);
                return new Tuple<int, DbTimeUnit>(totalMonths, DbTimeUnit.MONTH);
            }
            else if (this.years != 0)
            {
                return new Tuple<int, DbTimeUnit>(this.years, DbTimeUnit.YEAR);
            }
            else
            {
                return null;
            }
        }

        /// <summary> Check whether the string is an interval </summary>
        public static bool IsInterval(string interval)
        {
            // Check => empty
            if (string.IsNullOrWhiteSpace(interval))
                return false;

            // Check => contains 5 values
            string[] values = interval.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != 5)
                return false;

            // Check => values are numeric
            foreach (string v in values)
            {
                if (!v.IsInteger())
                    return false;
            }

            // All checks passed
            return true;
        }

        /// <summary> Returns the string value of the relative datetime. </summary>
        public override string ToString()
        {
            return years + ":" + months + ":" + days + ":" + hours + ":" + minutes;
        }
    }
}
