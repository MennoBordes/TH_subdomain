using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TH.Core.Base.Exceptions;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    public class ListElement : FormElement
    {
        /// <summary> The general item name. </summary>
        public string Subject { get; set; }

        /// <summary> Feature: Add. </summary>
        public bool Add { get; set; }

        /// <summary> Feature: Sort. </summary>
        public bool Sort { get; set; }

        /// <summary> Feature: Search. </summary>
        public bool Search { get; set; }

        /// <summary> Restriction: Minimum items. </summary>
        public int Min { get; set; }

        /// <summary> Restriction: Maximum items. </summary>
        public int Max { get; set; }

        /// <summary> Indicates if each template may only be used once. </summary>
        public bool UniqueTemplates { get; set; }

        /// <summary> Templates. </summary>
        public List<ListElementItem> Templates { get; set; }

        /// <summary> Items. </summary>
        //public List<AppUsage.ListItem> Items { get; set; }

        /// <summary> Value (raw). </summary>
        [XmlIgnore]
        public string RawValue { get; set; }

        /// <summary> Value (parsed). </summary>
        [XmlIgnore]
        public ValueItem[] Value { get; set; }


        /// <summary> Construct a new list element. </summary>
        public ListElement()
        {
            Type = FormElementType.List;
            Visible = true;
            Templates = new List<ListElementItem>();
            //Items = new List<AppUsage.ListItem>();
        }

        /// <summary> Creates a new instance with the default configuration. </summary>
        public static ListElement Init(int? id = null, string label = null, string description = null)
        {
            ListElement obj = new ListElement();

            obj.Id = id ?? 0;
            obj.Type = FormElementType.List;
            obj.Status = (int)Base.Enums.Status.Active;
            obj.Visible = true;
            obj.Disabled = false;
            obj.Min = 0;
            obj.Max = 0;
            obj.Index = 0;

            obj.Label = label;
            obj.Description = description;

            obj.Subject = "item";
            obj.Add = true;
            obj.Sort = true;
            obj.Search = false;

            obj.Templates = new List<ListElementItem>();
            //obj.Items = new List<AppUsage.ListItem>();

            return obj;
        }

        /// <summary> Set custom sort action for custom processing. </summary>
        [XmlIgnore]
        public string CustomSortAction { get; set; }

        /// <summary> Set custom delete action for custom processing. </summary>
        public string CustomDeleteAction { get; set; }

        /// <summary> Set custom render action for custom processing. </summary>
        public string CustomRenderAction { get; set; }

        /// <summary> Custom identifier for the list. </summary>
        public int? CustomListIdentifier { get; set; }

        /// <summary> Adds a template to this list. </summary>
        public void AddTemplate(ListElementItem template, int? index = null)
        {
            // Check
            if (template == null)
                throw new CoreException("Parameter 'template' can not be null.");

            if (this.Templates == null)
                this.Templates = new List<ListElementItem>();

            // Set index
            if (template.Index == 0)
            {
                template.Index = (index != null) ? index.Value : this.Templates.Count + 1;
            }

            this.Templates.Add(template);
        }

        /// <summary> Finds the template by id. </summary>
        public ListElementItem FindTemplate(int id)
        {
            if (Templates == null || !Templates.Any())
                return null;
            return Templates.FirstOrDefault(x => x.Id == id);
        }

        /// <summary> Removes the template. </summary>
        public void RemoveTemplate(int id)
        {
            if (this.Templates == null || !this.Templates.Any())
                return;

            this.Templates.RemoveAll(x => x.Id == id);
        }

        /// <summary> Retrieves FormElement value. </summary>
        //public override object GetElementValue()
        //{
        //    return !string.IsNullOrWhiteSpace(this.Value) ? this.Value.Trim() : this.Value;
        //}

        /// <summary> Sets FormElement value. </summary>
        /// <returns>Success flag</returns>
        //public override bool SetElementValue(object value)
        //{
        //    try
        //    {
        //        this.Value = value.ToString();

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

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

        /// <summary> Represents a value item. </summary>
        public class ValueItem
        {
            public string Id { get; set; }

            public string Label { get; set; }

            public int? Index { get; set; }

            public object Data { get; set; }
        }
    }
}
