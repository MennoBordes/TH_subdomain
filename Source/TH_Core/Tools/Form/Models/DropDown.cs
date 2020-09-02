using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TH.Core.Base.Extensions;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    public class Dropdown : FormElement
    {
        /// <summary> The selected dropdown values (after post). </summary>
        public string[] Value { get; set; }

        public string DefaultValue { get; set; }

        public bool Required { get; set; }

        public string Placeholder { get; set; }

        /// <summary> Indicator if multiple options can be selected. </summary>
        public bool Multiple { get; set; }

        /// <summary> If set true: the formbuilder will not store the items.</summary>
        public bool DynamicItems { get; set; }

        [XmlArray]
        [XmlArrayItem("Item")]
        public List<DropdownItem> Items { get; set; }

        [Obsolete("Needs review...")]
        [XmlIgnore]
        public Dictionary<string, string> ClientSideEvents { get; set; }

        [XmlIgnore]
        public AjaxSource AjaxSource { get; set; }

        [XmlIgnore]
        public Button AddButton { get; set; }

        [XmlIgnore]
        public bool PluginSelect2 { get; set; }

        /// <summary> Indicator if new items should can be created. </summary>
        [XmlIgnore]
        public bool AllowNew { get; set; }

        [XmlIgnore]
        public bool HasValue
        {
            get
            {
                if (this.Value != null)
                {
                    return this.Value.Any(x => !string.IsNullOrEmpty(x));
                }

                return false;
            }
        }

        public Dropdown()
        {
            Type = FormElementType.Dropdown;
            Visible = true;
            PluginSelect2 = true;
        }

        [XmlIgnore]
        private bool _defaultAllowClear = true;

        [XmlIgnore]
        public bool AllowClear
        {
            get
            {
                return this._defaultAllowClear;
            }
            set
            {
                this._defaultAllowClear = value;
            }
        }

        /// <summary> Add Item. </summary>
        public DropdownItem AddItem(int id)
        {
            if (this.Items == null)
                this.Items = new List<DropdownItem>();

            DropdownItem sibling = this.Items.LastOrDefault();

            // Default
            DropdownItem item = new DropdownItem
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
        public static Dropdown Init(int? id = null, string label = null, string description = null, bool? required = null, List<DropdownItem> items = null, bool? inlineMode = false, int? col = null)
        {
            Dropdown obj = new Dropdown();

            obj.Id = id ?? 0;
            obj.Type = FormElementType.Dropdown;
            obj.Status = (int)Base.Enums.Status.Active;
            obj.Visible = true;
            obj.Disabled = false;
            obj.Index = 0;

            obj.Label = label;
            obj.Description = description;

            obj.Required = required ?? false;
            obj.Items = new List<DropdownItem>();
            if (items != null && items.Any())
            { obj.Items = items; }
            obj.Multiple = false;
            obj.ClientSideEvents = new Dictionary<string, string>();
            obj.PluginSelect2 = true;
            obj.AllowNew = false;
            obj.InlineMode = inlineMode ?? false;

            obj.Column = col ?? 1;

            return obj;
        }


        //=== Overrides

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
                this.Value = null;
                if (this.Items != null)
                {
                    foreach (DropdownItem item in this.Items)
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
                    foreach (DropdownItem item in this.Items)
                    {
                        if (this.Value != null)
                        {
                            bool selected = false;
                            for (int i = 0; i < this.Value.Length; i++)
                            {
                                if (item.Value.Equals(this.Value[i], StringComparison.Ordinal))
                                {
                                    selected = true;
                                }
                            }

                            item.Selected = selected;
                        }
                        else
                        {
                            item.Selected = false;
                        }
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
            IEnumerable<DropdownItem> selected = this.Items.Where(x => x.Selected);

            string value = string.Join(", ", selected.Select(x => x.Label));

            return value;
        }

        /// <summary> Display: comma separated selection. </summary>
        public override string GetDisplayValue(object value)
        {
            string[] a = this.ConvertFrom(value);

            if (!a.IsNullOrEmpty() && !this.Items.IsNullOrEmpty())
            {
                IEnumerable<string> selected = this.Items.Where(x => a.Contains(x.Value)).Select(x => x.Label);

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

        private string[] ConvertFrom(object obj)
        {
            string[] a = null;

            if (obj is string[])
            {
                a = (string[])obj;
            }
            else
            {
                a = new string[] { Convert.ToString(obj) };
            }

            return a;
        }
    }
}
