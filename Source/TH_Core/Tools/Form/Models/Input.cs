using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    public class Input : FormElement
    {
        /// <summary> This supports both negative and positive numbers. </summary>
        /// <remarks> The n gets translated to a optional -. </remarks>
        public const string MASK_PRESET_NEGATIVE_NUMBER = "N9999999999";

        /// <summary> This supports both negative and positive numbers. </summary>
        /// <remarks> The n gets translated to a optional -. </remarks>
        public const string MASK_PRESET_NEGATIVE_VALUTA_EU = "N9999999999,00";

        public const string MASK_PRESET_NUMBER = "9999999999";
        public const string MASK_PRESET_DECIMAL = "9999999999,00";
        public const string MASK_PRESET_VALUTA_EU = "9999999999,00";
        public const string MASK_PRESET_TIME = "00:00"; // 00:00:00 >> removed the seconds
        public const string MASK_PRESET_DATE_TIME = "00/00/0000 00:00:00";
        public const string MASK_PRESET_INTERVAL = "00:00:00:00:00";

        /// <summary> The posted/current value. </summary>
        public string Value { get; set; }

        //==== Calculated value

        /// <summary> The calculated value switch (text/integer). Use the ECalculatedValue enum </summary>
        public string CalculatedValueType { get; set; }

        /// <summary> The calculated value formula. </summary>
        public string CalculatedValueFormula { get; set; }

        /// <summary> The calculed value (end result). </summary>
        public string CalculatedValue { get; set; }


        //=== Properties

        public int MaxLength { get; set; }
        public int MinLength { get; set; }
        public bool CharCounter { get; set; }
        public bool Required { get; set; }
        public int? FieldSize { get; set; }
        public int? TabIndex { get; set; }

        public string Placeholder { get; set; }
        public string DefaultValue { get; set; }

        // Mask settings
        public string MaskPattern { get; set; }
        public bool MaskReversed { get; set; }

        // Colorpicker
        public bool ColorPicker { get; set; }

        // Personalization
        //public List<FieldReference> PersonalizationOptions { get; set; }

        /// <summary> Determines the rendering of the input field. </summary>
        public ERenderType? Rendering { get; set; }

        /// <summary> Helper: enum render type. </summary>
        public enum ERenderType
        {
            Input = 1,
            Password = 2,
            Email = 3
        }

        /// <summary> Helper: calculated value type. </summary>
        public enum ECalculatedValueType
        {
            Text = 1,
            Integer = 2
        }


        public Input()
        {
            Type = FormElementType.Input;
            Visible = true;
        }


        /// <summary> Creates a new instance with the default configuration. </summary>
        public static Input Init(int? id = null, string label = null, string description = null, bool? required = null, bool? inlineMode = false, int? col = null, int? tabIndex = null)
        {
            Input obj = new Input();

            obj.Id = id ?? 0;
            obj.Type = FormElementType.Input;
            obj.Status = (int)Base.Enums.Status.Active;
            obj.Visible = true;
            obj.Disabled = false;
            obj.Index = 0;

            obj.Label = label;
            obj.Description = description;

            obj.MinLength = 0;
            obj.MaxLength = 255;
            obj.CharCounter = false;
            obj.Required = required ?? false;
            obj.MaskPattern = null;
            obj.MaskReversed = false;
            obj.InlineMode = inlineMode ?? false;

            //obj.PersonalizationOptions = null;

            obj.Column = col ?? 1;

            obj.TabIndex = tabIndex;

            return obj;
        }


        //=== Overrides

        /// <summary> Get string value. </summary>
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
            return GetDisplayValue(this.Value);
        }

        /// <summary> Display: (trimmed-) value. </summary>
        public override string GetDisplayValue(object value)
        {
            string s = null;

            if (value is string)
            {
                s = (string)value;
            }
            else if (value is DateTime)
            {
                s = TH.Core.Base.Extensions.DateTimeExtensions.FormatDateTime((DateTime)value,
                    TH.Core.Base.Translation.Translation.DEFAULT_LANGUAGE,
                    Config.TH_PLATFORM_TIMEZONE_ID,
                    inclTime: true,
                    written: false);
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
