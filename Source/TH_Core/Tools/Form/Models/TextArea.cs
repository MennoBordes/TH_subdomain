using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    public class Textarea : FormElement
    {
        /// <summary> The posted/current value. </summary>
        public string Value { get; set; }

        public int? MaxLength { get; set; }
        public bool CharCounter { get; set; }
        public bool Required { get; set; }
        public int? FieldSize { get; set; }
        public int? FieldHeight { get; set; }
        public string Placeholder { get; set; }
        public string DefaultValue { get; set; }

        //=== Ace editor

        public bool AceEditor { get; set; }

        /// <summary> When AceEditor is true this is required. </summary>
        public int? AceEditorHeight { get; set; }

        // Personalization
        //public List<FieldReference> PersonalizationOptions { get; set; }

        public Textarea()
        {
            Type = FormElementType.Textarea;
            Visible = true;
        }


        /// <summary> Creates a new instance with the default configuration. </summary>
        public static Textarea Init(int? id = null, string label = null, string description = null, bool? required = null, bool? inlineMode = null, int? col = null)
        {
            Textarea obj = new Textarea();

            obj.Id = id ?? 0;
            obj.Type = FormElementType.Textarea;
            obj.Status = (int)Base.Enums.Status.Active;
            obj.Visible = true;
            obj.Disabled = false;
            obj.Index = 0;

            obj.Label = label;
            obj.Description = description;

            obj.MaxLength = null;
            obj.CharCounter = false;
            obj.Required = required ?? false;
            obj.FieldSize = null;
            obj.FieldHeight = null;
            obj.InlineMode = inlineMode ?? false;

            //obj.PersonalizationOptions = null;

            obj.Column = col ?? 1;


            return obj;
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
                this.Value = value.ToString();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Display: (trimmed-) value. </summary>
        public override string GetDisplayValue()
        {
            return !string.IsNullOrWhiteSpace(this.Value) ? this.Value.Trim() : this.Value;
        }

        /// <summary> Display: (trimmed-) value. </summary>
        public override string GetDisplayValue(object value)
        {
            string s = null;

            if (value is string)
            {
                s = (string)value;
            }
            else
            {
                s = Convert.ToString(value);
            }

            return !string.IsNullOrWhiteSpace(s) ? s.Trim() : s;
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
    }
}
