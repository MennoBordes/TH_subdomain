using System;
using System.Collections.Generic;
using System.Linq;

namespace TH.WebUI.Tools.Form.Extensions
{
    using TH.Core.Base.Extensions;
    using TH.Core.Tools.Form.Models;
    using TH.Core.Tools.Form.Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class FormExtensions
    {
        /// <summary> Parses the form object to a json object (for client-side code). </summary>
        public static string JsonRegistration(this Form form)
        {
            JObject jForm = new JObject();
            jForm.Add(new JProperty("id", form.Name));
            jForm.Add(new JProperty("action", form.Action));

            JObject jMeta = new JObject();
            jForm.Add(new JProperty("metadata", jMeta));
            foreach (string key in form.Metadata.Keys)
            {
                jMeta.Add(new JProperty(key, form.Metadata[key]));
            }

            #region Fields
            JArray jFields = new JArray();
            jForm.Add(new JProperty("fields", jFields));

            IEnumerable<FormElement> elements = form.GetAllElements();
            List<Calendar> calendarFields = elements.Where(x => x is Calendar).Select(x => x as Calendar).ToList();

            foreach (FormElement element in elements)
            {
                if (element.Type == FormElementType.Block)
                {
                    // Nothing
                }
                else if (element.Type == FormElementType.Html)
                {
                    if ((element as HtmlElement).Editable)
                    {
                        jFields.Add(new JObject(
                            new JProperty("id", element.Name),
                            new JProperty("type", "html"),
                            new JProperty("value", null),
                            new JProperty("initialvalue", null)));
                    }
                }
                else if (element.Type == FormElementType.Input)
                {
                    jFields.Add(new JObject(
                        new JProperty("id", element.Name),
                        new JProperty("type", "input"),
                        new JProperty("value", null),
                        new JProperty("isValid", true),
                        new JProperty("initialValue", null)));
                }
                else if (element.Type == FormElementType.Textarea)
                {
                    jFields.Add(new JObject(
                        new JProperty("id", element.Name),
                        new JProperty("type", "textarea"),
                        new JProperty("value", null),
                        new JProperty("initialValue", null)));
                }
                else if (element.Type == FormElementType.Dropdown)
                {
                    jFields.Add(new JObject(
                        new JProperty("id", element.Name),
                        new JProperty("type", "dropdown"),
                        new JProperty("value", null),
                        new JProperty("initialValue", null)));
                }
                else if (element.Type == FormElementType.Radiobutton)
                {
                    Radiobutton radioGroup = element as Radiobutton;

                    if (radioGroup.Items != null && radioGroup.Items.Any())
                    {
                        foreach (RadiobuttonItem item in radioGroup.Items)
                        {
                            string id = string.Format("{0}_{1}", radioGroup.Name, item.Id);

                            jFields.Add(new JObject(
                                new JProperty("id", id),
                                new JProperty("type", "radiobutton"),
                                new JProperty("value", null),
                                new JProperty("initialValue", null)));
                        }
                    }
                }
                else if (element.Type == FormElementType.Checkbox)
                {
                    Checkbox checkboxGroup = element as Checkbox;

                    if (checkboxGroup.Items != null && checkboxGroup.Items.Any())
                    {
                        foreach (CheckboxItem item in checkboxGroup.Items)
                        {
                            string id = string.Format("{0}_{1}", checkboxGroup.Name, item.Id);

                            jFields.Add(new JObject(
                                new JProperty("id", id),
                                new JProperty("type", "checkbox"),
                                new JProperty("value", null),
                                new JProperty("initialValue", null)));
                        }
                    }
                }
                else if (element.Type == FormElementType.Calendar)
                {
                    Calendar calendar = element as Calendar;

                    string refStart = null;
                    string refEnd = null;

                    if (calendar.IsStartDate && calendarFields.Count(x => x.IsEndDate) == 1) // Must be exact
                    {
                        // Find Matching End Date
                        Calendar opposition = calendarFields.FirstOrDefault(x => x.IsEndDate);
                        refEnd = opposition.Name;
                    }
                    else if (calendar.IsEndDate && calendarFields.Count(x => x.IsStartDate) == 1) // Must be exact
                    {
                        // Find Matching Start Date
                        Calendar opposition = calendarFields.FirstOrDefault(x => x.IsStartDate);
                        refStart = opposition.Name;
                    }

                    jFields.Add(new JObject(
                        new JProperty("id", element.Name),
                        new JProperty("type", "calendar"),
                        new JProperty("value", null),
                        new JProperty("initialValue", null),
                        new JProperty("isStartDate", calendar.IsStartDate),
                        new JProperty("isEndDate", calendar.IsEndDate),
                        new JProperty("refStart", refStart),
                        new JProperty("refDate", refEnd)));
                }
                else if (element.Type == FormElementType.Hidden)
                {
                    jFields.Add(new JObject(
                        new JProperty("id", element.Name),
                        new JProperty("type", "hidden"),
                        new JProperty("value", null),
                        new JProperty("initialValue", null)));
                }
                else if (element.Type == FormElementType.Tagging)
                {
                    jFields.Add(new JObject(
                        new JProperty("id", element.Name),
                        new JProperty("type", "tagging"),
                        new JProperty("value", null),
                        new JProperty("initialValue", null)));
                }
                else if (element.Type == FormElementType.Link)
                {
                    jFields.Add(new JObject(
                        new JProperty("id", element.Name),
                        new JProperty("type", "link"),
                        new JProperty("value", null),
                        new JProperty("initialValue", null)));
                }

                else if (element.Type == FormElementType.List)
                {
                    jFields.Add(new JObject(
                        new JProperty("id", element.Name),
                        new JProperty("type", "list"),
                        new JProperty("value", null),
                        new JProperty("initialValue", null)));
                }
            }
            #endregion

            #region Buttons
            JArray jButtons = new JArray();
            jForm.Add(new JProperty("buttons", jButtons));

            if (form.CancelButton != null)
            {
                jButtons.Add(
                    new JObject(
                        new JProperty("id", form.CancelButton.Name),
                        new JProperty("type", "cancel")));
            }

            if (form.SaveButton != null)
            {
                jButtons.Add(
                    new JObject(
                        new JProperty("id", form.SaveButton.Name),
                        new JProperty("type", "save")));
            }

            if (form.LinkButtons != null && form.LinkButtons.Any())
            {
                foreach (Button button in form.LinkButtons)
                {
                    jButtons.Add(
                       new JObject(
                           new JProperty("id", button.Name),
                           new JProperty("type", "link")));
                }
            }
            #endregion

            return jForm.ToString();
        }

        /// <summary> Converts the whole form object to json. </summary>
        public static string JsonFull(this Form form)
        {
            JsonSerializer js = new JsonSerializer();
            js.TypeNameHandling = TypeNameHandling.Auto;

            JObject jForm = JObject.FromObject(form, js);

            return jForm.ToString();
        }

        /// <summary> Parses the json back to a  full form object. </summary>
        public static Form JsonToForm(JToken json)
        {
            JsonSerializer jser = new JsonSerializer();
            jser.TypeNameHandling = TypeNameHandling.Auto;

            Form form = json.ToObject<Form>(jser);

            return form;
        }

        /// <summary> Deserialize Json Field. </summary>
        public static FormElement DeserializeJsonField(JObject json)
        {
            if (json == null)
            { return null; }

            // Serialization
            JsonSerializer jser = new JsonSerializer();
            jser.TypeNameHandling = TypeNameHandling.Auto;

            FormElement _field = null;

            FormElementType type = (FormElementType)json.Value<int>("Type");
            Type objectType = null;
            switch (type)
            {
                case FormElementType.Calendar:
                    objectType = typeof(Calendar);
                    break;
                case FormElementType.Checkbox:
                    objectType = typeof(Checkbox);
                    break;
                case FormElementType.Dropdown:
                    objectType = typeof(Dropdown);
                    break;
                case FormElementType.Html:
                    objectType = typeof(HtmlElement);
                    break;
                case FormElementType.Input:
                    objectType = typeof(Input);
                    break;
                case FormElementType.Radiobutton:
                    objectType = typeof(Radiobutton);
                    break;
                case FormElementType.Textarea:
                    objectType = typeof(Textarea);
                    break;
                case FormElementType.Link:
                    objectType = typeof(Link);
                    break;
                case FormElementType.Label:
                    objectType = typeof(Label);
                    break;
                case FormElementType.List:
                    objectType = typeof(ListElement);
                    break;
                default:
                    throw new Exception("Init: Field type is not supported.");
            }

            _field = (FormElement)json.ToObject(objectType, jser);

            return _field;
        }

        /// <summary> Serialize Json Field. </summary>
        public static JObject SerializeJsonField(FormElement field)
        {
            if (field == null)
            { return null; }

            // Serialization
            JsonSerializer jser = new JsonSerializer();
            jser.TypeNameHandling = TypeNameHandling.Auto;

            return JObject.FromObject(field, jser);
        }

        /// <summary> Deserialize Json Fields. </summary>
        public static List<FormElement> DeserializeJsonFields(JArray json)
        {
            if (json == null)
            { return null; }

            List<FormElement> fields = new List<FormElement>();

            foreach (JObject obj in json)
            {
                FormElement field = DeserializeJsonField(obj);

                fields.Add(field);
            }

            return fields;
        }

        /// <summary> Serialize Json Fields. </summary>
        public static JArray SerializeJsonFields(List<FormElement> fields)
        {
            if (fields == null)
            { return null; }

            JArray array = new JArray();

            foreach (FormElement field in fields)
            {
                JObject obj = SerializeJsonField(field);

                array.Add(obj);
            }

            return array;
        }

        /// <summary> Serialize ajax source request for dropdown field. </summary>
        /// <param name="items"> The items that will be returned. </param>
        /// <param name="more"> Indicates if there are more items available. </param>
        public static JObject DropdownAjaxResponse(List<DropdownItem> items, bool more)
        {
            JArray ja = new JArray();

            if (!items.IsNullOrEmpty())
            {
                Dictionary<string, int> groups = new Dictionary<string, int>();
                Dictionary<string, List<DropdownItem>> groupItems = new Dictionary<string, List<DropdownItem>>();

                List<DropdownItem> groupless = new List<DropdownItem>();

                // Apply indexing
                foreach (DropdownItem ddi in items)
                {
                    if (!string.IsNullOrEmpty(ddi.OptionGroupName))
                    {
                        if (!groups.ContainsKey(ddi.OptionGroupName))
                        {
                            groups[ddi.OptionGroupName] = ddi.OptionGroupIndex;
                            groupItems[ddi.OptionGroupName] = new List<DropdownItem>();
                        }

                        groupItems[ddi.OptionGroupName].Add(ddi);
                    }
                    else
                    {
                        groupless.Add(ddi);
                    }
                }

                // Convert groups
                foreach (KeyValuePair<string, int> pairGroup in groups.OrderBy(x => x.Value))
                {
                    string group = pairGroup.Key;
                    List<DropdownItem> gitems = groupItems[group];
                    if (!gitems.IsNullOrEmpty())
                    {
                        JArray jag = new JArray();
                        foreach (DropdownItem ddi in gitems)
                        {
                            jag.Add(new JObject(
                                new JProperty("id", ddi.Value),
                                new JProperty("text", ddi.Label),
                                new JProperty("selected", ddi.Selected))); //, new JProperty("group", group)
                        }
                        ja.Add(new JObject(
                            new JProperty("text", group),
                            new JProperty("children", jag)));
                    }
                }

                // Convert items
                foreach (DropdownItem ddi in groupless)
                {
                    ja.Add(new JObject(
                        new JProperty("id", ddi.Value), 
                        new JProperty("text", ddi.Label), 
                        new JProperty("selected", ddi.Selected)));
                }
            }

            JObject response = new JObject();

            response.Add(new JProperty("results", ja));
            response.Add(new JProperty("items", ja));
            response.Add(new JProperty("more", more));

            return response;
        }
    }
}