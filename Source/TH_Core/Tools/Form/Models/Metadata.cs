using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace TH.Core.Tools.Form.Models
{
    /// <summary> Metadata (collection). </summary>
    public class Metadata
    {
        /// <summary> The (meta-) data. </summary>
        private NameValueCollection data;

        public Metadata()
        {
            this.data = new NameValueCollection();
        }

        /// <summary> Gets all the keys in this Metadata collection. </summary>
        public string[] Keys
        {
            get
            {
                return this.data.AllKeys;
            }
        }

        /// <summary> Gets the number of Metadata items. </summary>
        public int Count
        {
            get
            {
                return this.data.Count;
            }
        }

        /// <summary> Indicator if this Metadata contains a specific key. </summary>
        public bool HasKey(string key)
        {
            return this.data.AllKeys.Contains(key);
        }

        /// <summary> Removes a specific key. </summary>
        public void RemoveKey(string key)
        {
            this.data.Remove(key);
        }

        /// <summary> Get typed value. </summary>
        public T Value<T>(string key)
        {
            string value = this.data[key];

            Type type = typeof(T);

            object convertedValue = TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);

            return (T)convertedValue;

            //return default(T);
        }

        /// <summary> The Metadata item. </summary>
        /// <value> The value (string). </value>
        public object this[string key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = (value == null ? (string)null : value.ToString());
            }
        }
    }
}
