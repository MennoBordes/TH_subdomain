using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace TH.Core.Base.Extensions
{
    public static class JsonExtensions
    {
        /// <summary> Checks if this jtoken contains child key. </summary>
        public static bool ContainsKey(this JToken token, string key)
        {
            if (token == null)
                return false;
            return token.SelectToken(key) != null;

            // Example:
            //string distance = jObject.SelectToken("routes[0].legs[0].distance.text").ToString();
        }

        /// <summary> Read json property value as string. </summary>
        public static string ReadString(this JToken jObj, string name)
        {
            if (jObj == null)
                return null;

            string s = jObj[name] != null ? jObj.Value<string>(name) : null;
            if (string.IsNullOrWhiteSpace(s))
            { s = null; }
            return s;
        }

        /// <summary> Read json property value as target type. 
        /// <para> Provides fallback mechanism. </para>
        /// </summary>
        /// <param name="name"> Property name (set null to refer to the object itself). </param>
        /// <param name="defaultValue"> Default value. </param>
        /// <param name="format"> Format (eg: decimals). </param>
        public static T ReadValue<T>(this JToken jObj, string name, T defaultValue, object format)
        {
            if (jObj == null)
                return defaultValue;

            JToken o = null;

            if (name == null)
            {
                // convert whole object
                o = jObj;
            }
            else
            {
                o = jObj[name];
            }

            if (o != null)
            {
                Type t = typeof(T);
                object v = null;
                Newtonsoft.Json.JsonSerializer jser = null;


                if (format != null)
                {
                    if (format is CultureInfo)
                    {
                        jser = new Newtonsoft.Json.JsonSerializer();
                        jser.Culture = (format as CultureInfo);
                    }
                }

                if (jser != null)
                {
                    v = o.ToObject(t, jser);
                }
                else
                {
                    try
                    { v = o.ToObject(t); }
                    catch { v = defaultValue; }
                }

                // requested type JObject must have a value of JObject
                if (t == typeof(JObject))
                {
                    if (!(v is JObject))
                    { v = defaultValue; }
                }
                else if (t == typeof(JArray))
                {
                    if (!(v is JArray))
                    { v = defaultValue; }
                }

                return (T)v;
            }

            return defaultValue;
        }

        /// <summary> Read json property value as target type. 
        /// <para> Provides fallback mechanism. </para>
        /// </summary>
        /// <param name="name"> Property name (set null to refer to the object itself). </param>
        /// <param name="defaultValue"> Default value. </param>
        public static T ReadValue<T>(this JToken jObj, string name, T defaultValue)
        {
            return jObj.ReadValue<T>(name: name, defaultValue: defaultValue, format: null);
        }
    }
}
