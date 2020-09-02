using System.Collections.Generic;
using TH.Core.Base.Extensions;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    public class RangeElement : FormElement
    {
        //=== Properties

        public List<RangeElementItem> Items { get; set; }

        public bool Required { get; set; }

        public EType RangeType { get; set; }


        //=== Init

        public static RangeElement Init(int? id = null, string label = null, string description = null, bool? required = null, bool? inlineMode = false, EType? rangeType = null)
        {
            RangeElement obj = new RangeElement();
            obj.Id = id ?? 0;
            obj.Type = FormElementType.Range;
            obj.Status = (int)Base.Enums.Status.Active;
            obj.Visible = true;
            obj.Disabled = false;
            obj.Index = 0;
            obj.RangeType = rangeType.GetValueOrDefault();

            obj.Label = label;
            obj.Description = description;

            obj.InlineMode = inlineMode ?? false;

            obj.Items = new List<RangeElementItem>();
            obj.Required = required ?? false;

            return obj;
        }


        //=== Helpers

        public enum EType
        {
            Number = 0,
            Percentage = 1,
        }

        public override object GetElementValue()
        {
            if (this.Items.IsNullOrEmpty())
            {
                this.Items = new List<RangeElementItem>();
            }

            return this.Items;
        }

        public override bool SetElementValue(object value)
        {
            if (value == null)
            {
                if (this.Items != null)
                { this.Items = new List<RangeElementItem>(); }
                return true;
            }

            // set
            try
            {
                if (this.Items == null)
                {
                    this.Items = new List<RangeElementItem>();
                }

                if (value == null)
                {
                    this.Items = new List<RangeElementItem>();
                }
                else
                {
                    this.Items = (List<RangeElementItem>)value;
                }

                return true;
            }
            catch
            {
                return false;
            }

            return true;
        }

        public override bool GetRequired()
        {
            return this.Required;
        }

        public override void SetRequired(bool req)
        {
            this.Required = req;
        }


        //=== Childs

        public class RangeElementItem
        {
            /// <summary> The range identifier. </summary>
            public int Id { get; set; }

            /// <summary> The color code for this range. </summary>
            public string ColorCode { get; set; }

            /// <summary> The name/label for this range. </summary>
            public string Label { get; set; }

            /// <summary> The minimum value of this range </summary>
            public int MinimumValue { get; set; }

            /// <summary> The maximum value of this range. </summary>
            public int MaximumValue { get; set; }
        }
    }
}
