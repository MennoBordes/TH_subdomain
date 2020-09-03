using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TH.Core.Base.Database.Entities.Localization;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    public class HtmlElement : FormElement
    {
        //=== Properties

        /// <summary> The Html content. </summary>
        public string Value { get; set; }

        /// <summary> The initial field height pertaining the editor (in pixels). </summary>
        public int Height { get; set; }

        /// <summary> Indicator if the edit modus of this field is enabled. </summary>
        public bool Editable { get; set; }

        /// <summary> Indicator if the value is required. </summary>
        public bool Required { get; set; }

        /// <summary> Show character count. </summary>
        public bool CharCounter { get; set; }

        /// <summary> Maximum number of characters. </summary>
        public int? MaxLength { get; set; }

        /// <summary> Personalization. </summary>
        //public List<FieldReference> PersonalizationOptions { get; set; }

        /// <summary> Language ids for spellchecker. </summary>
        public List<Language> Languages { get; set; }

        //=== Custom

        /// <summary> Load complex Html. 
        /// <para> When calling the partial view, a Field model is used. 
        /// </summary>
        public string MvcPartialView { get; set; }


        //=== Editor Configuration

        /// <summary> Disable the superscript </summary>
        public bool? DisableSuperscript { get; set; }

        /// <summary> Disable the subscript </summary>
        public bool? DisableSubscript { get; set; }

        /// <summary> Disable the font color </summary>
        public bool? DisableFontcolor { get; set; }

        /// <summary> Disable the background color </summary>
        public bool? DisableBackgroundcolor { get; set; }

        /// <summary> Enabled the source editor </summary>
        public bool? EnableSource { get; set; }

        /// <summary> Editor Profile (if null then the basic configuration will be used). </summary>
        [XmlIgnore]
        public EEditorProfile? EditorProfile { get; set; }


        //=== Actions

        /// <summary> The constructor. </summary>
        public HtmlElement()
        {
            Type = FormElementType.Html;
            Visible = true;
        }

        ///// <summary> Gets the value of the body node => strips all other html markup. </summary>        
        //public string GetBodyValue()
        //{
        //    // Check input
        //    if (string.IsNullOrWhiteSpace(this.Value))
        //        return null;

        //    // Load html & get body node
        //    HtmlEngine htmlEngine = new HtmlEngine();
        //    HtmlNode bodyNode = htmlEngine.LoadDoc(this.Value).DocumentNode.SelectSingleNode("//body | //BODY");

        //    // Check => body node
        //    if (bodyNode == null)
        //        return this.Value;

        //    // Check => empty or whitespace
        //    if (string.IsNullOrWhiteSpace(bodyNode.InnerHtml))
        //        return null;

        //    // Return body node content
        //    return bodyNode.InnerHtml;
        //}

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

        /// <summary> Creates a new instance with the default configuration. </summary>
        public static HtmlElement Init(int? id = null, string label = null, string description = null, bool charCounter = false)
        {
            HtmlElement obj = new HtmlElement();

            obj.Id = id ?? 0;
            obj.Type = FormElementType.Html;
            obj.Status = (int)Base.Enums.Status.Active;
            obj.Visible = true;
            obj.Disabled = false;
            obj.Required = false;
            obj.Index = 0;

            obj.Label = label;
            obj.Description = description;

            obj.Height = 0;
            obj.Editable = false;

            obj.CharCounter = charCounter;
            obj.MaxLength = null;

            //obj.PersonalizationOptions = null;

            obj.DisableSuperscript = false;
            obj.DisableSubscript = false;
            obj.DisableFontcolor = false;
            obj.DisableBackgroundcolor = false;

            return obj;
        }

        /// <summary> Enum: Editor Profile. </summary>
        public enum EEditorProfile
        {
            Basic,
            Advanced
        }
    }
}
