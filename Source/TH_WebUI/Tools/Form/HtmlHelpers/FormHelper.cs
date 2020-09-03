using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TH.Core.Base.Database.Enums;
using TH.Core.Base.Extensions;
using TH.Core.Tools.Form.Enums;

namespace TH.WebUI.Tools.Form.HtmlHelpers
{
    using TH.Core.Tools.Form.Models;
    using TH.WebUI.Tools.Form.Models;

    public static class FormHelper
    {
        private static readonly KeyValuePair<string, object> RequiredHtmlAttribute = new KeyValuePair<string, object>("required", "required");
        private static readonly KeyValuePair<string, object> DisabledHtmlAttribute = new KeyValuePair<string, object>("disabled", "disabled");
        private static readonly KeyValuePair<string, string> DisplayNoneHtmlAttribute = new KeyValuePair<string, string>("style", "display: none;");
        private static string formViews = "~/Tools/Form/Views/";

        /// <summary> Renders the form. </summary>
        public static void RenderForm(this HtmlHelper helper, Form form)
        {
            helper.RenderPartial(formViews + "_Form.cshtml", form);
        }

        /// <summary> Renders the field. </summary>
        public static void RenderField(this HtmlHelper helper, Field field)
        {
            if (field == null || field.Element == null)
                return;

            FormElement element = field.Element;


            bool hasRatio = element.Ratio > 0;
            string style = (hasRatio ? "width:" + element.Ratio + "%;" : null) + element.InlineCss;
            string fieldCssClass = "form-field";
            if (hasRatio)
            { fieldCssClass += " ratio"; }
            if (element.FieldCssClass != null)
            { fieldCssClass += " " + element.FieldCssClass; }

            // start: field
            string sFieldAttrs = null;
            if (element.FieldAttributes != null)
            {
                foreach (KeyValuePair<string, object> fieldAttr in element.FieldAttributes)
                {
                    sFieldAttrs += " " + fieldAttr.Key + "=\"" + fieldAttr.Value + "\"";
                }
            }

            helper.ViewContext.Writer.Write("<div class=\"" + fieldCssClass + "\" style=\"" + style + "\" " + sFieldAttrs + ">");

            if (element.Type == FormElementType.Input)
            {
                helper.RenderPartial(formViews + "_Input.cshtml", field);
            }
            else if (element.Type == FormElementType.Textarea)
            {
                helper.RenderPartial(formViews + "_Textarea.cshtml", field);
            }
            else if (element.Type == FormElementType.Html)
            {
                helper.RenderPartial(formViews + "_Html.cshtml", field);
            }
            else if (element.Type == FormElementType.Dropdown)
            {
                helper.RenderPartial(formViews + "_Dropdown.cshtml", field);
            }
            else if (element.Type == FormElementType.Radiobutton)
            {
                helper.RenderPartial(formViews + "_Radiobutton.cshtml", field);
            }
            else if (element.Type == FormElementType.Checkbox)
            {
                helper.RenderPartial(formViews + "_Checkbox.cshtml", field);
            }
            else if (element.Type == FormElementType.Calendar)
            {
                helper.RenderPartial(formViews + "_Calendar.cshtml", field);
            }
            else if (element.Type == FormElementType.Hidden)
            {
                helper.RenderPartial(formViews + "_Hidden.cshtml", field);
            }
            else if (element.Type == FormElementType.Label)
            {
                helper.RenderPartial(formViews + "_Label.cshtml", field);
            }
            else if (element.Type == FormElementType.Tagging)
            {
                helper.RenderPartial(formViews + "_Tagging.cshtml", field);
            }
            else if (element.Type == FormElementType.Link)
            {
                helper.RenderPartial(formViews + "_Link.cshtml", field);
            }
            else if (element.Type == FormElementType.List)
            {
                helper.RenderPartial(formViews + "_List.cshtml", field);
            }
            else if (element.Type == FormElementType.Button)
            {
                MvcHtmlString hs = helper.FormSchemaButton(element as Button);
                helper.ViewContext.Writer.Write(hs.ToString());
            }
            else if (element.Type == FormElementType.Range)
            {
                helper.RenderPartial(formViews + "_Range.cshtml", field);
            }

            // end: field
            helper.ViewContext.Writer.Write("</div>");
        }

        /// <summary> Renders the field's 'label' html. </summary>
        public static MvcHtmlString FormFieldLabel(this HtmlHelper helper, FormElement formElement, bool isRequired = false)
        {
            if (string.IsNullOrEmpty(formElement.Label))
                return MvcHtmlString.Empty;

            TagBuilder tb = new TagBuilder("label");

            tb.Attributes.Add("for", formElement.Name);

            // Inline?
            if (formElement.InlineMode)
            {
                // Depends on element type
                switch (formElement.Type)
                {
                    case FormElementType.Radiobutton:
                    case FormElementType.Checkbox:
                        tb.AddCssClass(formElement.IconClass);
                        tb.AddCssClass("label-icon");
                        tb.InnerHtml = "<span>" + formElement.Label + "</span>";
                        if (!string.IsNullOrWhiteSpace(formElement.Label))
                            tb.Attributes.Add("title", formElement.Label);
                        break;
                    default:
                        tb.AddCssClass(formElement.IconClass);
                        tb.AddCssClass("label-icon");
                        if (!string.IsNullOrWhiteSpace(formElement.Label))
                            tb.Attributes.Add("title", formElement.Label);
                        break;
                }
            }
            else
            {
                string innerHtml = string.Empty;
                if (formElement.FontAwesome)
                {
                    innerHtml += "<i class='" + formElement.IconClass + "'></i> ";
                }

                innerHtml += isRequired ? string.Format("{0} *", formElement.Label) : formElement.Label;
                tb.InnerHtml = innerHtml;
            }

            return new MvcHtmlString(tb.ToString());
        }

        /// <summary> Renders the field's 'description' html. </summary>
        public static MvcHtmlString FormFieldDescription(this HtmlHelper helper, string description)
        {
            if (string.IsNullOrEmpty(description))
                return MvcHtmlString.Empty;

            string html = string.Format("<div class=\"help\"><a class=\"fas fa-info-circle\" data-tooltip=\"{0}\"></a></div>", helper.Raw(description.Replace(Environment.NewLine, "<br />")));

            return new MvcHtmlString(html);
        }

        /// <summary> Renders the field's 'error' html. </summary>
        public static MvcHtmlString FormFieldError(this HtmlHelper helper, FormElement formElement, bool visible = true)
        {
            // Check
            if (formElement == null || formElement.Errors.IsNullOrEmpty())
                return null;

            // Tag
            TagBuilder tag = new TagBuilder("div");
            tag.AddCssClass("error");
            tag.SetInnerText(formElement.Errors.FirstOrDefault());

            if (formElement.Type == FormElementType.Input)
            {
                Input input = (Input)formElement;
                if (!string.IsNullOrEmpty(input.MaskPattern))
                {
                    if (input.MaskPattern.ToLower() == "iban")
                    {
                        tag.MergeAttribute("data-iban-error", input.Name);
                    }
                }
            }
            if (!visible)
            {
                tag.AddCssClass("hidden");
            }

            // inline mode (not implemented)
            if (formElement.InlineMode)
            {
                // ?
            }

            return new MvcHtmlString(tag.ToString());
        }

        /// <summary> Renders the form 'character count' html. </summary>
        public static MvcHtmlString FormCharCount(this HtmlHelper helper, string elementName, string elementValue, int maxLength)
        {
            string s = string.Format("<div class=\"charcounter\">{0}: <span data-form-label=\"{1}_charcount_cur\">{2}</span>/<span data-form-label=\"{1}_charcount_max\">{3}</span></div>",
                "Characters",
                elementName,
                string.IsNullOrEmpty(elementValue) ? 0 : elementValue.Length,
                maxLength);

            return new MvcHtmlString(s);
        }

        /// <summary> Renders an html 'input' node. </summary>
        public static MvcHtmlString SimpleHtmlInput(this HtmlHelper helper, string type, string name, string id, string value, bool isChecked, bool isDisabled = false)
        {
            TagBuilder tb = new TagBuilder("input");

            tb.Attributes.Add("type", type);
            tb.Attributes.Add("name", name);
            tb.Attributes.Add("id", id);
            tb.Attributes.Add("value", value);
            if (isChecked)
                tb.Attributes.Add("checked", "checked");
            if (isDisabled)
                tb.Attributes.Add("disabled", "disabled");

            return new MvcHtmlString(tb.ToString());
        }

        ///// <summary> Renders a personalization button. </summary>
        //public static MvcHtmlString Personalization(this HtmlHelper helper, string elementName, IEnumerable<FieldReference> options, Translator translator)
        //{
        //    ViewDataDictionary viewData = new ViewDataDictionary();
        //    viewData["elementName"] = elementName;
        //    viewData["options"] = options;
        //    viewData["translator"] = translator;

        //    return helper.Partial("~/tools/form/views/_personalization.cshtml", viewData);
        //}


        //=== Dropdown

        /// <summary> Renders the forms the schema dropdown html. </summary>
        [Obsolete("Not used anymore")]
        public static MvcHtmlString FormSchemaDropdown(this HtmlHelper helper, Dropdown dropdown, string elementName)
        {
            IDictionary<string, object> attributes = dropdown.Attributes;

            if (dropdown.PluginSelect2)
            {
                attributes.Add("data-plugin", "select2");
            }

            if (dropdown.Required)
            {
                attributes.Add(RequiredHtmlAttribute);
            }

            if (dropdown.Multiple)
            {
                attributes.Add("multiple", "multiple");
            }

            if (dropdown.Disabled)
            {
                attributes.Add(DisabledHtmlAttribute);
            }

            if (dropdown.ClientSideEvents != null)
            {
                dropdown.ClientSideEvents.ForEach(x => attributes.Add(x.Key, x.Value));
            }

            IEnumerable<SelectListItem> items = dropdown.Items.OrderBy(gs => gs.OptionGroupIndex).ThenBy(gs => gs.Index)
                .Select(gs => new SelectListItem
                {
                    Selected = gs.Selected,
                    Text = gs.Label,
                    Value = gs.Value,
                });

            bool emptyItem = false;

            if (!string.IsNullOrEmpty(dropdown.Placeholder))
            {
                attributes.Add("data-placeholder", dropdown.Placeholder);
            }

            // select2 require empty item if placeholder is displayed
            if (attributes.ContainsKey("data-placeholder"))
            {
                emptyItem = true;
            }

            if (!dropdown.Required)
            {
                emptyItem = true;
            }

            if (emptyItem && !dropdown.Multiple)
            {
                items = Enumerable.Repeat(new SelectListItem { Selected = true, Text = string.Empty, Value = string.Empty }, 1)
                    .Concat(items);
            }

            IEnumerable<string> optGroups = dropdown.Items.Where(x => !string.IsNullOrWhiteSpace(x.OptionGroupName)).Select(x => x.OptionGroupName).Distinct();
            if (optGroups == null || optGroups.Count() < 1)
            {
                if (dropdown.PluginSelect2)
                {
                    attributes["style"] = "display:none;";
                }

                //-- Use standard MVC DropDownList rendering
                return helper.DropDownList(elementName, items, attributes);
            }
            else
            {
                #region Use custom DropDownList rendering with OptGroup option support

                // Select element
                TagBuilder selectTAG = new TagBuilder("select");

                // Add default attributes
                selectTAG.Attributes.Add("id", elementName);
                selectTAG.Attributes.Add("name", elementName);

                // Add custom attributes
                if (attributes != null)
                    attributes.ForEach(a => selectTAG.Attributes.Add(a.Key, a.Value.ToString()));


                // Add empty item if placeholder is set ("select2" requires empty item if placeholder is displayed)
                if (emptyItem && !dropdown.Multiple)
                {
                    TagBuilder emptyoptionTAG = new TagBuilder("option");
                    emptyoptionTAG.Attributes.Add("selected", "selected");
                    emptyoptionTAG.Attributes.Add("value", string.Empty);

                    selectTAG.InnerHtml += emptyoptionTAG.ToString();
                }

                // Add Items without optGroup
                foreach (DropdownItem item in dropdown.Items.Where(x => string.IsNullOrWhiteSpace(x.OptionGroupName)))
                {
                    TagBuilder optionTAG = new TagBuilder("option");
                    if (item.Selected)
                        optionTAG.Attributes.Add("selected", "selected");
                    if (item.Locked)
                        optionTAG.Attributes.Add("locked", "locked");
                    optionTAG.Attributes.Add("value", item.Value.Trim());
                    optionTAG.SetInnerText(item.Label.Trim());

                    selectTAG.InnerHtml += optionTAG.ToString();
                }

                // Add items with optGroup
                foreach (string optGroup in optGroups)
                {
                    TagBuilder optgroupTAG = new TagBuilder("optgroup");
                    optgroupTAG.Attributes.Add("label", optGroup);

                    foreach (DropdownItem item in dropdown.Items.Where(x => !string.IsNullOrWhiteSpace(x.OptionGroupName) && x.OptionGroupName.Equals(optGroup, StringComparison.OrdinalIgnoreCase)))
                    {
                        TagBuilder optionTAG = new TagBuilder("option");
                        if (item.Selected)
                            optionTAG.Attributes.Add("selected", "selected");
                        if (item.Locked)
                            optionTAG.Attributes.Add("locked", "locked");
                        optionTAG.Attributes.Add("value", item.Value.Trim());
                        optionTAG.SetInnerText(item.Label.Trim());

                        optgroupTAG.InnerHtml += optionTAG.ToString();
                    }

                    selectTAG.InnerHtml += optgroupTAG.ToString();
                }

                #endregion

                if (dropdown.PluginSelect2)
                {
                    selectTAG.Attributes["style"] = "display:none;";
                }

                return new MvcHtmlString(selectTAG.ToString());
            }
        }

        //=== Buttons

        /// <summary> Renders the forms the schema button. </summary>
        public static MvcHtmlString FormSchemaButton(this HtmlHelper helper, Button input)
        {
            if (input == null)
                return new MvcHtmlString("&nbsp");

            if (input.ButtonType == FormButtonType.Link)
                return FormSchemaLinkButton(input);

            TagBuilder tb = new TagBuilder("input");

            foreach (string key in input.Attributes.Keys)
            {
                tb.Attributes.Add(key, Convert.ToString(input.Attributes[key]));
            }

            tb.Attributes.Add("type", "button");
            tb.Attributes.Add("value", input.Label);
            tb.Attributes.Add("id", input.Name);
            tb.AddCssClass("button");

            if (!input.Visible)
            { tb.Attributes.Add(DisplayNoneHtmlAttribute); }
            if (input.Disabled)
            { tb.Attributes.Add("disabled", string.Empty); }

            if (input.ButtonType == FormButtonType.Cancel)
            { tb.AddCssClass("cancel"); }
            else if (input.ButtonType == FormButtonType.Alternate)
            { tb.AddCssClass("alternate"); }

            return new MvcHtmlString(tb.ToString());
        }

        /// <summary> Renders the forms the schema link button. </summary>
        private static MvcHtmlString FormSchemaLinkButton(Button input)
        {
            TagBuilder tb = new TagBuilder("a");

            foreach (string key in input.Attributes.Keys)
            {
                tb.Attributes.Add(key, Convert.ToString(input.Attributes[key]));
            }

            tb.AddCssClass("linkbutton");
            tb.Attributes.Add("href", "#");
            tb.Attributes.Add("id", input.Name);
            if (!string.IsNullOrEmpty(input.OnClick))
                tb.Attributes.Add("onclick", input.OnClick);
            if (!input.Visible)
                tb.Attributes.Add(DisplayNoneHtmlAttribute);

            tb.SetInnerText(input.Label);

            return new MvcHtmlString(tb.ToString());
        }

        //=== Radiobutton

        /// <summary> Renders the forms the schema radiobutton. </summary>
        public static MvcHtmlString FormSchemaRadiobutton(this HtmlHelper helper, Radiobutton radiobutton)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in radiobutton.Items)
            {
                sb.Append(FormSchemaRadiobuttonItem(helper, radiobutton, item));
            }

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary> Renders the forms the schema single radiobutton item. </summary>
        public static string FormSchemaRadiobuttonItem(HtmlHelper helper, Radiobutton radiobutton, RadiobuttonItem buttonItem)
        {
            string id = string.Format("{0}_{1}", radiobutton.Name, buttonItem.Id);
            string name = string.Format("{0}", radiobutton.Name);

            //Dictionary<string, object> attributes = radiobutton.Attributes;
            //attributes.Add("class", "radioLabel");

            bool isDisabled = (radiobutton.Disabled == true);

            MvcHtmlString radio = helper.SimpleHtmlInput("radio", name, id, buttonItem.Value, buttonItem.Selected, isDisabled: isDisabled);
            MvcHtmlString label = helper.Label(id, buttonItem.Label, new Dictionary<string, object> { { "class", "radioLabel" } });

            return radiobutton.Direction == RepeatDirection.Vertical
                ? string.Concat("<div>", radio, label, "</div>")
                : string.Concat(radio, label);
        }


        //=== Hidden

        /// <summary> Renders the forms the schema hidden. </summary>
        public static MvcHtmlString FormSchemaHidden(this HtmlHelper helper, Hidden hidden)
        {
            IDictionary<string, object> attributes = hidden.Attributes;

            // DevNote: Currently no specific attributes available

            string hiddenValue = (string.IsNullOrEmpty(hidden.Value)) ? hidden.DefaultValue : hidden.Value;

            return helper.Hidden(hidden.Name, hiddenValue, attributes);
        }
    }
}