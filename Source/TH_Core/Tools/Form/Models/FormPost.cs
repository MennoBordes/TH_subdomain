using System;
using System.Collections.Generic;

namespace TH.Core.Tools.Form.Models
{
    /// <summary> FormPost, contains all parsed values after a form post. </summary>
    public class FormPost
    {
        /// <summary> The form's RequestVerificationToken. 
        /// <para> Usefull for checking against XSS attacks. </para>
        /// </summary>
        public string RequestVerificationToken { get; set; }

        /// <summary> The form's Metadata. 
        /// <para> Contains information related to the form. </para>
        /// </summary>
        public Metadata Metadata { get; set; }

        /// <summary> The form's data.
        /// <para> Contains the values. </para>
        /// </summary>
        public Dictionary<string, string> Data { get; set; }



        /// <summary> Get FormElement's data by ID. Throws an exception if element is not found. </summary>
        public string GetDataOfElement(int elementId)
        {
            string key = string.Format("form_{0}", elementId);

            return this.Data[key];
        }

        /// <summary> Get FormElement's data by ID and convert to specified type. Throws an exception if element is not found. </summary>
        public T GetDataOfElement<T>(int elementId)
        {
            string key = string.Format("form_{0}", elementId);

            return (T)Convert.ChangeType(this.Data[key], typeof(T));
        }

        /// <summary> Get FormElement's data by ID </summary>
        public string GetDataOfElementOrDefault(int elementId)
        {
            string key = string.Format("form_{0}", elementId);

            return this.Data.ContainsKey(key) ? this.Data[key] : string.Empty;
        }

        /// <summary> Get FormElement's data by ID and convert to specified type </summary>
        public T GetDataOfElementOrDefault<T>(int elementId)
        {
            string key = string.Format("form_{0}", elementId);

            return this.Data.ContainsKey(key) ? (T)Convert.ChangeType(this.Data[key], typeof(T)) : default(T);
        }
    }
}
