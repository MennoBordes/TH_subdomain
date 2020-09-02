using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TH.Core.Base.Database.Enums;
using TH.Core.Base.Extensions;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    public class Radiobutton : FormElement
    {
        //=== Properties

        /// <summary> The posted/current value. 
        /// <para> The value of the radiobutton item. </para>
        /// </summary>
        public string Value { get; set; }

        public RepeatDirection Direction { get; set; }

        public bool Required { get; set; }

        [XmlArray]
        [XmlArrayItem("Item")]
        public List<RadiobuttonItem> Items { get; set; } // Set IList >> List for serialization support

        public string DefaultValue { get; set; }

        /// <summary> Renders the radiobutton in slider mode. </summary>
        public bool SliderMode { get; set; }

        /// <summary> Show the min and max slider values? </summary>
        [XmlIgnore]
        public bool SliderHideMinMax { get; set; }

        /// <summary> Show the current value of the slider? </summary>
        [XmlIgnore]
        public bool SliderHideValue { get; set; }

        /// <summary> The custom min value, will work if HideMinMax is enabled. </summary>
        [XmlIgnore]
        public string SliderCustomMinValue { get; set; }

        /// <summary> The custom max value, will work if HideMinMax is enabled. </summary>
        [XmlIgnore]
        public string SliderCustomMaxValue { get; set; }

        /// <summary> Whether to show dots on the line or not (dots represent the intervals) </summary>
        [XmlIgnore]
        public bool SliderDisplayIntervalDots { get; set; }

        /// <summary> Whether to show the handle of the slider (false when it's not editable) </summary>
        [XmlIgnore]
        public bool SliderDisplayHandleDot { get; set; }


        //=== Functions

        /// <summary> Constructor. </summary>
        public Radiobutton()
        {
            Type = FormElementType.Radiobutton;
            Visible = true;
            InlineMode = false;
            SliderMode = false;
        }


        /// <summary> Add Item. </summary>
        public RadiobuttonItem AddItem(int id)
        {
            if (this.Items == null)
                this.Items = new List<RadiobuttonItem>();

            RadiobuttonItem sibling = this.Items.LastOrDefault();

            // Default
            RadiobuttonItem item = new RadiobuttonItem
            {
                Id = id,
                Label = null,
                Value = null,
                Index = (sibling != null ? sibling.Index + 1 : 0),
                Selected = false
            };

            this.Items.Add(item);

            return item;
        }

        /// <summary> Removes the item. </summary>
        public void RemoveItem(int id)
        {
            if (this.Items == null || !this.Items.Any())
                return;

            this.Items.RemoveAll(x => x.Id == id);
        }

        /// <summary> Creates a new instance with the default configuration. </summary>
        public static Radiobutton Init(int? id = null, string label = null, string description = null, bool? required = null, List<RadiobuttonItem> items = null, bool? inlineMode = false, int? col = null)
        {
            Radiobutton obj = new Radiobutton();

            obj.Id = id ?? 0;
            obj.Type = FormElementType.Radiobutton;
            obj.Status = (int)Base.Enums.Status.Active;
            obj.Visible = true;
            obj.Disabled = false;
            obj.Index = 0;

            obj.Label = label;
            obj.Description = description;

            obj.Direction = RepeatDirection.Horizontal;
            obj.Required = required ?? false;
            obj.Items = new List<RadiobuttonItem>();
            if (items != null && items.Any())
            { obj.Items = items; }
            obj.InlineMode = inlineMode ?? false;
            obj.SliderDisplayHandleDot = true;

            obj.Column = col ?? 1;

            return obj;
        }

        /// <summary> Creates the radiobutton items for a true/ false switch.</summary>        
        public static List<RadiobuttonItem> GetBoolItems(string labelTrue = null, string labelFalse = null, bool selected = true)
        {
            return new List<RadiobuttonItem>
                   {
                    new RadiobuttonItem() { Id = 1, Index = 0, Label = string.IsNullOrWhiteSpace(labelTrue) ? "Yes" : labelTrue, Value = "true", Selected = (selected) },
                    new RadiobuttonItem() { Id = 2, Index = 1, Label = string.IsNullOrWhiteSpace(labelFalse) ? "No" : labelFalse, Value = "false", Selected = (!selected) }
                   };
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
                if (this.Items != null)
                {
                    foreach (var item in this.Items)
                    {
                        item.Selected = false;
                    }
                }
                return true;
            }

            // set
            try
            {
                this.Value = this.ConvertFrom(value);

                if (this.Items != null)
                {
                    foreach (var item in this.Items)
                    {
                        item.Selected = item.Value.Equals(value.ToString(), StringComparison.Ordinal);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Display: comma separated selection. </summary>
        public override string GetDisplayValue()
        {
            IEnumerable<RadiobuttonItem> selected = this.Items.Where(x => x.Selected);

            string value = string.Join(", ", selected.Select(x => x.Label));

            return value;
        }

        /// <summary> Display: comma separated selection. </summary>
        public override string GetDisplayValue(object value)
        {
            string s = this.ConvertFrom(value);

            if (!string.IsNullOrEmpty(s) && !this.Items.IsNullOrEmpty())
            {
                IEnumerable<string> selected = this.Items.Where(x => x.Value == s).Select(x => x.Label);

                if (selected.Any())
                {
                    return string.Join(", ", selected);
                }
            }

            return null;
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


        //=== Helpers

        private string ConvertFrom(object obj)
        {
            return Convert.ToString(obj);
        }
    }
}
