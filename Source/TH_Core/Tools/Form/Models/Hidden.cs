using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    /// <summary> A Hidden Form element. 
    /// <para> Usefull for setting important references. </para>
    /// </summary>
    public class Hidden : FormElement
    {
        /// <summary> The posted/current value. </summary>
        public string Value { get; set; }

        public string DefaultValue { get; set; }

        public bool Required { get; set; }

        public Hidden()
        {
            Type = FormElementType.Hidden;
            Visible = true;
        }

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
                this.Value = value.ToString();

                return true;
            }
            catch
            {
                return false;
            }
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

        /// <summary> Creates a new instance with the default configuration. </summary>
        public static Hidden Init(int? id = null, string label = null, string description = null, bool? required = null)
        {
            Hidden obj = new Hidden();

            obj.Id = id ?? 0;
            obj.Type = FormElementType.Hidden;
            obj.Status = (int)Base.Enums.Status.Active;
            obj.Visible = true;
            obj.Disabled = false;
            obj.Index = 0;

            obj.Label = label;
            obj.Description = description;

            if (required != null)
                obj.Required = required.Value;

            return obj;
        }
    }
}
