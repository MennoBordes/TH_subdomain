using System.Collections.Generic;
using System.Xml.Serialization;

namespace TH.Core.Tools.Form.Models
{
    using TH.Core.Tools.Form.Enums;
    public class FormElement
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> fieldAttributes;
        private string name;

        //=== Settings

        [XmlAttribute]
        public int Id { get; set; }

        [XmlIgnore]
        public int FormId { get; set; }

        [XmlIgnore]
        public int FormElementId { get; set; }

        /// <summary> Element type. </summary>
        [XmlAttribute]
        public FormElementType Type { get; set; }

        /// <summary> Key used for mapping purposes. </summary>
        [XmlAttribute]
        public string Key { get; set; }

        /// <summary> Display Label. </summary>
        public string Label { get; set; }

        /// <summary> Display Description. </summary>
        public string Description { get; set; }

        [XmlAttribute]
        public int Status { get; set; }

        [XmlAttribute]
        public bool Visible { get; set; }

        [XmlAttribute]
        public bool Disabled { get; set; }

        /// <summary> Validation errors. </summary>
        [XmlIgnore]
        public string[] Errors { get; set; }

        //=== Formatting

        /// <summary> Used to specify an extra css class for the field element. </summary>
        [XmlIgnore]
        public string FieldCssClass { get; set; }

        /// <summary> Used to specify an extra css class to the form row. </summary>
        public string CssClass { get; set; }

        /// <summary> InlineMode: used to render the form element in the inline style.</summary>
        [XmlAttribute]
        public bool InlineMode { get; set; }

        /// <summary> IconClass: used to specify an icon class to the form element.</summary>
        public string IconClass { get; set; }

        /// <summary> FontAwesome: used in combination with IconClass </summary>
        public bool FontAwesome { get; set; }

        /// <summary> The column number where the form element should be placed. </summary>
        [XmlAttribute]
        public int Column { get; set; }

        [XmlAttribute]
        public int Index { get; set; }

        /// <summary> Get or set the elements name. </summary>
        public string Name
        {
            get
            {
                if (this.name == null)
                {
                    return string.Format("form_{0}", Id);
                }
                else
                {
                    return this.name;
                }
            }
            set
            {
                this.name = value;
            }
        }

        public string InlineCss
        {
            get { return !Visible ? " display: none; " : string.Empty; }
        }


        /// <summary> Collection of Field Attributes. </summary>
        [XmlIgnore]
        public Dictionary<string, object> FieldAttributes
        {
            get
            {
                if (this.fieldAttributes == null)
                { this.fieldAttributes = new Dictionary<string, object>(); }

                return this.fieldAttributes;
            }
            set
            {
                if (this.fieldAttributes == null)
                { this.fieldAttributes = new Dictionary<string, object>(); }

                this.fieldAttributes = value;
            }
        }

        /// <summary> Collection of Element Attributes. </summary>
        [XmlIgnore]
        public Dictionary<string, object> Attributes
        {
            get
            {
                if (this.attributes == null)
                { this.attributes = new Dictionary<string, object>(); }

                return this.attributes;
            }
            set
            {
                if (this.attributes == null)
                { this.attributes = new Dictionary<string, object>(); }

                this.attributes = value;
            }
        }

        /// <summary> Relative element ratio (percentage). </summary>
        [XmlIgnore]
        public int Ratio { get; set; }

        /// <summary> Field meta data. </summary>
        [XmlIgnore]
        public Metadata FieldMeta { get; set; }

        //=== Overrides

        /// <summary> Retrieves FormElement value. Must be overriden in child classes. </summary>
        public virtual object GetElementValue()
        {
            return null;
        }

        /// <summary> Sets FormElement value. Must be overriden in child classes. </summary>
        /// <returns>Success flag</returns>
        public virtual bool SetElementValue(object value)
        {
            return false;
        }

        /// <summary> Returns a simple display value for presentation purposes. </summary>
        public virtual string GetDisplayValue()
        {
            return null;
        }

        /// <summary> Returns a simple display value for presentation purposes.
        /// <para> Uses the field's configuration without modifying its data. </para>
        /// </summary>
        public virtual string GetDisplayValue(object value)
        {
            return null;
        }

        /// <summary> Sets the element required value </summary>
        public virtual void SetRequired(bool req)
        {
            return;
        }

        public virtual bool GetRequired()
        {
            return false;
        }
    }
}
