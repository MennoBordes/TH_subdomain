using System.Collections.Generic;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    // Case 1: No Ajax
    // Use 'Value' for list of preselected items
    // Use 'Items' for list of selectable items
    // Use AllowNew = false if new items should not be created

    // Case 2: Ajax
    // Use 'AjaxSource' for URL which points to json (e.g.: [{Id: 'test', Text: 'test'}, {Id: 12, Text: 'Test Existing'}])
    // Optionally use AjaxData to send extra parameters to the json request

    public class Tagging : FormElement
    {
        /// <summary> Select2: Ajax Source. </summary>
        public string AjaxSource { get; set; }

        /// <summary> Select2: Ajax Data. </summary>
        public Dictionary<string, string> AjaxData { get; set; }

        /// <summary> Select2: Indicates the separation of tags. </summary>
        public char[] TokenSeperator { get; set; }

        /// <summary> Select2: Minimum amount of tag characters to search for. </summary>
        public int MinimumInputLength { get; set; }

        /// <summary> Required. </summary>
        public bool Required { get; set; }

        /// <summary> Maximum selectable items. </summary>
        public int MaximumSelectionSize { get; set; }

        /// <summary> List of preselected items. </summary>
        public IList<TaggingItem> Value { get; set; }

        /// <summary> List of selectable items. </summary>
        public IList<TaggingItem> Items { get; set; }

        /// <summary> Indicator if new items should can be created. </summary>
        public bool AllowNew { get; set; }

        /// <summary> Indicator if selected items can be sorted. </summary>
        public bool AllowSorting { get; set; }

        /// <summary> The placeholder for the field. </summary>
        public string Placeholder { get; set; }

        public Tagging()
        {
            Type = FormElementType.Tagging;
            Visible = true;
        }

        /// <summary> Retrieves FormElement value. </summary>
        public override object GetElementValue()
        {
            return this.Value;
        }

        /// <summary> Sets FormElement value. </summary>
        /// <returns> Success flag. </returns>
        public override bool SetElementValue(object value)
        {
            // clear
            if (value == null)
            {
                if (this.Value != null)
                { this.Value.Clear(); }
                return true;
            }

            // set
            try
            {
                this.Value = (IList<TaggingItem>)value;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Creates a new instance with the default configuration. </summary>
        public static Tagging Init(int? id = null, string label = null, string description = null, bool? required = null, bool? inlineMode = null)
        {
            Tagging obj = new Tagging();

            obj.Id = id ?? 0;
            obj.Type = FormElementType.Tagging;
            obj.Status = (int)Base.Enums.Status.Active;
            obj.Visible = true;
            obj.Disabled = false;
            obj.Index = 0;

            obj.Label = label;
            obj.Description = description;

            obj.TokenSeperator = new char[] { ',' };
            obj.AjaxData = new Dictionary<string, string>();
            obj.Required = required ?? false;
            obj.MaximumSelectionSize = 0;
            obj.Value = new List<TaggingItem>();
            obj.Items = new List<TaggingItem>();

            obj.AllowNew = true;
            obj.AllowSorting = false;
            obj.InlineMode = inlineMode ?? false;

            return obj;
        }
    }
}
