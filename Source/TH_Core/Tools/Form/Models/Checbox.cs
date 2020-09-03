using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TH.Core.Base.Database.Enums;
using TH.Core.Base.Extensions;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    public class Checkbox : FormElement
    {
        /// <summary> The posted/current value. 
        /// <para> The Checked checkbox values's. </para>
        /// </summary>
        public string[] Value { get; set; }

        public RepeatDirection Direction { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }

        /// <summary> Determines rendering. </summary>
        public Option? Rendering { get; set; }

        [XmlArray]
        [XmlArrayItem("Item")]
        public List<CheckboxItem> Items { get; set; }

        public Checkbox()
        {
            Type = FormElementType.Checkbox;
            Visible = true;
        }


        /// <summary> Creates a new instance with the default configuration. </summary>
        public static Checkbox Init(int? id = null, string label = null, string description = null, bool? @switch = null)
        {
            Checkbox obj = new Checkbox();

            obj.Id = id ?? 0;
            obj.Type = FormElementType.Checkbox;
            obj.Status = (int)Base.Enums.Status.Active;
            obj.Visible = true;
            obj.Disabled = false;
            obj.Index = 0;

            obj.Label = label;
            obj.Description = description;

            obj.Direction = RepeatDirection.Horizontal;
            obj.Min = null;
            obj.Max = null;
            obj.Items = new List<CheckboxItem>();

            if (@switch == true)
            {
                obj.Items.Add(new CheckboxItem { Value = bool.TrueString, Selected = false });
                obj.Rendering = Checkbox.Option.Switch;
            }

            return obj;
        }

        /// <summary> Switch: set value. </summary>
        public void Switch(bool value)
        {
            if (this.Items != null && this.Items.Count > 0)
            {
                CheckboxItem cbi = this.Items.FirstOrDefault(x => x.Value == bool.TrueString);
                if (cbi != null)
                { cbi.Selected = value; }
            }
            this.Value = new string[] { value.ToString() };
        }

        /// <summary> Switch: get value. </summary>
        public bool Switch()
        {
            if (this.Value != null)
            {
                string v = this.Value.FirstOrDefault();
                return (string.Compare(v, bool.TrueString, true) == 0);
            }
            else
            {
                if (this.Items != null && this.Items.Count > 0)
                {
                    CheckboxItem cbi = this.Items.FirstOrDefault(x => x.Value == bool.TrueString);
                    if (cbi != null)
                    { return cbi.Selected; }
                }
                return false;
            }
        }

        /// <summary> Add Item. </summary>
        public CheckboxItem AddItem(int id)
        {
            if (this.Items == null)
                this.Items = new List<CheckboxItem>();

            CheckboxItem sibling = this.Items.LastOrDefault();

            // Default
            CheckboxItem item = new CheckboxItem
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

        [XmlIgnore]
        public bool HasAnyValue
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

        /// <summary> Checks if this checkbox has specified value. </summary>
        public bool HasValue(string obj)
        {
            return this.Value != null && this.Value.Contains(obj);
        }

        //=== Overrides

        /// <summary> Retrieves FormElement value. </summary>
        public override object GetElementValue()
        {
            return this.Value;
        }

        /// <summary> Sets FormElement value. </summary>
        /// <returns>Success flag</returns>
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
                    foreach (CheckboxItem item in this.Items)
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
            IEnumerable<CheckboxItem> selected = this.Items.Where(x => x.Selected);

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
            this.Min = req ? 1 : 0;
        }

        /// <summary> Gets the required property. </summary>
        public override bool GetRequired()
        {
            return this.Min >= 1;
        }

        //=== Helpers

        private string[] ConvertFrom(object obj)
        {
            string[] a = null;

            if (!(obj is string) && obj.IsEnumerable())
            {
                IEnumerable<object> e = (IEnumerable<object>)obj;

                a = e.Select(x => Convert.ToString(x)).ToArray();
            }
            else
            {
                a = new string[] { Convert.ToString(obj) };
            }

            return a;
        }

        [Flags]
        public enum Option
        {
            Switch = 1
        }
    }
}
