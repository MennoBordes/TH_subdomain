using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using TH.Core.Base.Database.Entities.Localization;
using TH.Core.Base.Extensions;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    public class Calendar : FormElement
    {
        //=== Vars

        /// <summary> Date Format.
        /// <para> This format is used in the logic. </para>
        /// </summary>
        [XmlIgnore]
        public static readonly string format = "dd/MM/yyyy";

        /// <summary> The posted/current value. </summary>
        public string Value { get; set; }

        /// <summary> The posted/current value for more complex values. </summary>
        [XmlIgnore]
        public object ObjValue { get; set; }

        /// <summary> Indicator if a value is required. </summary>
        public bool Required { get; set; }


        /// <summary> The minimum date value. 
        /// <para> Format: 'dd/MM/yyyy'. </para>
        /// </summary>
        public string MinDate { get; set; }

        /// <summary> The maximum date value. 
        /// <para> Format: 'dd/MM/yyyy'. </para>
        /// </summary>
        public string MaxDate { get; set; }

        /// <summary> The default date value. 
        /// <para> Format: 'dd/MM/yyyy'. </para>
        /// </summary>
        public string DefaultDate { get; set; }

        /// <summary> The number of months to display at the same time. </summary>
        public int NumberOfMonths { get; set; }

        /// <summary> Renders the month as a dropdown instead of text. </summary>
        public bool ChangeMonth { get; set; }

        /// <summary> Renders the year as a dropdown instead of text. </summary>
        public bool ChangeYear { get; set; }

        /// <summary> Placeholder Text. </summary>
        public string Placeholder { get; set; }

        /// <summary> Indicator if this field is a start date. </summary>
        public bool IsStartDate { get; set; }

        /// <summary> Indicator if this field is an end date. </summary>
        public bool IsEndDate { get; set; }

        ///<summary> Indicator if advanced mode should be enabled. </summary>
        [XmlIgnore]
        public bool AdvancedMode { get; set; }

        /// <summary> Url to load advanced configuration. </summary>
        [XmlIgnore]
        public string AdvancedUrl { get; set; }

        /// <summary> The display format. </summary>
        [XmlIgnore]
        public string DisplayFormat { get; set; }

        //=== Actions:

        /// <summary> The constructor. </summary>
        public Calendar()
        {
            Type = FormElementType.Calendar;
            Visible = true;
            InlineMode = false;
            DisplayFormat = DateTimeExtensions.GetFormat(Language.DEFAULT, false);
        }

        /// <summary> Returns the value as a datetime object. </summary>
        public DateTime? ValueAsDateTime()
        {
            return Calendar.ValueAsDateTime(this.Value, DisplayFormat);
        }

        /// <summary> Returns the value as a datetime object. </summary>
        public static DateTime? ValueAsDateTime(string value, string format)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            else
            {
                DateTime datetime;
                if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out datetime))
                {
                    return datetime;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary> Creates a new instance with the default configuration. </summary>
        public static Calendar Init(int? languageId = null, int? id = null, string label = null, string description = null, bool? required = null, bool? inlineMode = false, int? col = null)
        {
            Calendar obj = new Calendar();

            obj.Id = id ?? 0;
            obj.Type = FormElementType.Calendar;
            obj.Status = (int)Base.Enums.Status.Active;
            obj.Visible = true;
            obj.Disabled = false;
            obj.Index = 0;

            obj.Label = label;
            obj.Description = description;

            obj.Required = required ?? false;
            obj.MinDate = null;
            obj.MaxDate = null;
            obj.DefaultDate = null;
            obj.NumberOfMonths = 1;
            obj.ChangeMonth = true;
            obj.ChangeYear = true;
            obj.InlineMode = inlineMode ?? false;
            obj.Placeholder = null;
            obj.IsStartDate = false;
            obj.IsEndDate = false;

            obj.Column = col ?? 1;

            obj.DisplayFormat = DateTimeExtensions.GetFormat(languageId.GetValueOrDefault() > 0 ? languageId.Value : Language.DEFAULT, false);

            return obj;
        }

        /// <summary> Checks if specified string matches the current date format. </summary>
        public bool IsValidDate(string value)
        {
            DateTime dt;
            return DateTime.TryParseExact(value, format, null, System.Globalization.DateTimeStyles.None, out dt);
        }


        //=== Overrides

        /// <summary> Retrieves FormElement value. </summary>
        public override object GetElementValue()
        {
            return !string.IsNullOrWhiteSpace(this.Value) ? this.Value.Trim() : this.Value;
        }

        /// <summary> Sets FormElement value. </summary>
        /// <returns> Success flag. </returns>
        public override bool SetElementValue(object value)
        {
            // clear
            if (value == null)
            {
                this.Value = null;
                return true;
            }

            // set
            try
            {
                if (value is DateTime)
                {
                    this.Value = ((DateTime)value).ToString(format);
                }
                else if (value is DateTime?)
                {
                    DateTime? dt = (DateTime?)value;

                    this.Value = dt != null ? dt.Value.ToString(format) : null;
                }
                else if (value is string)
                {
                    this.Value = (value as string);
                }
                else
                {
                    this.Value = Convert.ToString(value);
                }

                if (this.Value != null)
                { this.Value = this.Value.Trim(); }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get display value. </summary>
        public override string GetDisplayValue()
        {
            if (!string.IsNullOrEmpty(this.Value))
            {
                DateTime? dt = ValueAsDateTime();
                if (dt != null)
                {
                    return dt.Value.ToString(format);
                }
            }

            return this.Value;
        }

        /// <summary> Get display value. </summary>
        public override string GetDisplayValue(object value)
        {
            DateTime? dt = null;

            if (value is string)
            {
                string s = (string)value;
                dt = ValueAsDateTime(s, DisplayFormat);
            }
            else if (value is DateTime?)
            {
                dt = (DateTime?)value;
            }
            else if (value is DateTime)
            {
                dt = (DateTime)value;
            }

            if (dt != null)
            {
                return dt.Value.ToString(format);
            }

            return Convert.ToString(value);
        }

        /// <summary> Sets the required property. </summary>
        public override void SetRequired(bool req)
        {
            this.Required = req;
        }

        /// <summary> Gets the required property. </summary>
        public override bool GetRequired()
        {
            return this.Required;
        }


        //=== Helper classes:

        /// <summary> Date config class: used for extended date configurations. </summary>
        public class DateConfig
        {
            //=== Vars

            /// <summary> Range: total available range that can be selected via the calendar. Null = no starting range. </summary>
            public DateTime? RangeStart { get; set; }

            /// <summary> Range: total available range that can be selected via the calendar. Null = no ending range. </summary>
            public DateTime? RangeEnd { get; set; }

            /// <summary> Scope: the scope this configuration is applied to, if calendar date selection is outer scope => new configuration needs to be fetched. Null = No starting scope. </summary>
            public DateTime? ScopeStart { get; set; }

            /// <summary> Scope: the scope this configuration is applied to, if calendar date selection is outer scope => new configuration needs to be fetched. Null = No ending scope. </summary>
            public DateTime? ScopeEnd { get; set; }

            /// <summary> Configuration of the days. </summary>
            public List<DateDayConfig> Days { get; set; }


            //=== Actions

            /// <summary> The constructor. </summary>
            public DateConfig()
            {
                Days = new List<DateDayConfig>();
            }

            public string ToJson()
            {
                string formatFull = "yyyyMMddHHmmss";
                string formatDate = "yyyyMMdd";

                JObject jo = new JObject();

                jo.Add(new JProperty("rangeStart", RangeStart != null ? RangeStart.Value.ToString(formatFull) : null));
                jo.Add(new JProperty("rangeEnd", RangeEnd != null ? RangeEnd.Value.ToString(formatFull) : null));
                jo.Add(new JProperty("scopeStart", ScopeStart != null ? ScopeStart.Value.ToString(formatFull) : null));
                jo.Add(new JProperty("scopeEnd", ScopeEnd != null ? ScopeEnd.Value.ToString(formatFull) : null));

                JArray jDays = null;
                if (this.Days != null)
                {
                    jDays = new JArray();

                    foreach (DateDayConfig day in this.Days)
                    {
                        JObject jd = new JObject();
                        jd.Add(new JProperty("date", day.Date != null ? day.Date.Value.ToString(formatDate) : null));
                        jd["info"] = day.Info;
                        jd["state"] = day.State.ToString().ToLower();

                        JArray jaTime = null;
                        if (day.Times != null)
                        {
                            jaTime = new JArray();
                            foreach (DateDayTimeConfig time in day.Times)
                            {
                                JObject jt = new JObject();
                                jt["label"] = time.Label;
                                jt["end"] = time.End;
                                jt["start"] = time.Start;
                                jaTime.Add(jt);
                            }
                        }
                        jd["time"] = jaTime;

                        jDays.Add(jd);
                    }

                }
                jo["days"] = jDays;

                return JsonConvert.SerializeObject(jo, Formatting.None);
            }
        }


        /// <summary> Date day config class: used for day config IN the date config class. </summary>
        public class DateDayConfig
        {
            //=== Vars

            /// <summary> The date this configuration applies to, or Null for default. (Date config - Scope range needs to be set when default is used). </summary>
            public DateTime? Date { get; set; }

            /// <summary> The state (available, disabled, blocked). </summary>
            public EState State { get; set; }

            /// <summary> Additional information (e.g. 15 units available). </summary>
            public string Info { get; set; }

            /// <summary> Configuration of the times. </summary>
            public List<DateDayTimeConfig> Times { get; set; }


            //=== Actions

            /// <summary> The constructor. </summary>
            public DateDayConfig()
            {
                Times = new List<DateDayTimeConfig>();
            }


            //=== Helpers

            /// <summary> State enum. </summary>
            public enum EState
            {
                Available = 0,
                Disabled = 1,
                Blocked = 2
            }
        }

        /// <summary> Date day time config: used for time config IN the date day config class. </summary>
        public class DateDayTimeConfig
        {
            //=== Vars

            /// <summary> The start time, format: HHmm </summary>
            public string Start { get; set; }

            /// <summary> The end time, format: HHmm. (Optional, if applied then this time is a time range) </summary>
            public string End { get; set; }

            /// <summary> The label (optional) </summary>
            public string Label { get; set; }


            //=== Actions

            /// <summary> The constructor. </summary>
            public DateDayTimeConfig()
            {
                //...
            }
        }

        /// <summary> The Advanced value. 
        /// <para> Contains data for date range and time range. </para>
        /// </summary>
        public class AdvancedValue
        {
            // JSON: {"start":{"date":"20170217"},"end":{"date":"20170217"},"time":{"start":"130000","end":"140000"}}

            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public DateTime? TimeStart { get; set; }
            public DateTime? TimeEnd { get; set; }

            public static AdvancedValue ParseJson(string json)
            {
                // Check
                if (string.IsNullOrEmpty(json))
                    return null;

                JObject jo = JObject.Parse(json);

                AdvancedValue v = new AdvancedValue();
                string formatDate = "yyyyMMdd";
                string formatTime = "HHmmss";
                string formatTimeShort = "HHmm";

                v.StartDate = AdvancedValue.ParseDateTime(jo, new string[] { "start", "date" }, formatDate);
                v.EndDate = AdvancedValue.ParseDateTime(jo, new string[] { "end", "date" }, formatDate);
                v.TimeStart = AdvancedValue.ParseDateTime(jo, new string[] { "time", "start" }, formatTimeShort);
                v.TimeEnd = AdvancedValue.ParseDateTime(jo, new string[] { "time", "end" }, formatTimeShort);

                return v;
            }

            public string ToJson()
            {
                JObject jo = new JObject();

                string formatDate = "yyyyMMdd";
                string formatTime = "HHmmss";

                AdvancedValue.SetDateTime(jo, new string[] { "start", "date" }, StartDate, formatDate);
                AdvancedValue.SetDateTime(jo, new string[] { "end", "date" }, EndDate, formatDate);
                AdvancedValue.SetDateTime(jo, new string[] { "time", "start" }, TimeStart, formatTime);
                AdvancedValue.SetDateTime(jo, new string[] { "time", "end" }, TimeEnd, formatTime);

                return JsonConvert.SerializeObject(jo, Formatting.None);
            }

            private static DateTime? ParseDateTime(JObject jo, string[] path, string format)
            {
                JToken jToken = jo;
                for (int i = 0; i < path.Length; i++)
                {
                    if (jToken != null && jToken.Type != JTokenType.Null)
                    {
                        if (jToken[path[i]] != null)
                        {
                            jToken = jToken[path[i]];
                        }
                    }
                }

                if (jToken != null)
                {
                    //JProperty jProp = (JProperty)jToken;
                    //string sValue = jProp.Value<string>();
                    string sValue = (string)jToken;
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        DateTime? dt = DateTime.ParseExact(sValue, format, null);

                        return dt;
                    }
                }

                return null;
            }

            private static void SetDateTime(JObject jo, string[] path, DateTime? value, string format)
            {
                JToken jToken = jo;
                for (int i = 0; i < path.Length; i++)
                {
                    string prop = path[i];

                    if (jToken[prop] == null)
                    {
                        if (i == path.Length - 1)
                        {
                            // prop
                            jToken[prop] = value != null ? value.Value.ToString(format) : (string)null;
                        }
                        else
                        {
                            JObject _jo = (JObject)jToken;
                            _jo.Add(new JProperty(prop, new JObject()));
                        }

                        jToken = jToken[path[i]];
                    }
                    else
                    {
                        if (i == path.Length - 1)
                        {
                            // prop
                            jToken[prop] = value != null ? value.Value.ToString(format) : (string)null;
                        }

                        jToken = jToken[path[i]];
                    }
                }
            }
        }
    }
}
