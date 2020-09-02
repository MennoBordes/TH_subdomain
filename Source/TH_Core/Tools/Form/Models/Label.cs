using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    public class Label : FormElement
    {
        public Label()
        {
            Type = FormElementType.Label;
            Visible = true;
        }

        public string Value { get; set; }


        /// <summary> Retrieves FormElement value. </summary>
        public override object GetElementValue()
        {
            return this.Value;
        }

        /// <summary> Sets FormElement value. </summary>
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

        /// <summary> Creates a new instance with the default configuration. </summary>
        public static Label Init(int? id = null, string label = null, string description = null, bool? inlineMode = false, int? col = null)
        {
            Label obj = new Label();

            obj.Id = id ?? 0;
            obj.Type = FormElementType.Label;
            obj.Status = (int)Base.Enums.Status.Active;
            obj.Visible = true;
            obj.Disabled = false;
            obj.Index = 0;

            obj.Label = label;
            obj.Description = description;
            obj.InlineMode = inlineMode ?? false;

            obj.Column = col ?? 1;

            return obj;
        }
    }
}
