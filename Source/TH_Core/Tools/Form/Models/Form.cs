using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;
using TH.Core.Base.Extensions;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    /// <summary> The Form. </summary>
    public class Form
    {
        private Dictionary<string, object> attributes;
        private string formName = null;

        /// <summary> Gets or sets the Form ID. </summary>
        public int Id { get; set; }

        //public SessionContext Context { get; set; }

        /// <summary> Indicates the content type of the form. </summary>
        public int? ContentTypeId { get; set; }

        /// <summary> Gets or sets the language ID. </summary>
        public int LanguageId { get; set; }

        /// <summary> Gets or sets the ACTION attribute of the FORM element. </summary>
        /// <value> The html action. </value>
        public string Action { get; set; }

        /// <summary> The form's Metadata. </summary>
        /// <value> Contains data related to the form. </value>
        public Metadata Metadata { get; set; }

        /// <summary> Gets or sets the blocks of the form. </summary>
        public List<Block> Blocks { get; set; }

        /// <summary> The link buttons of the form. </summary>
        [XmlIgnore]
        public IEnumerable<Button> LinkButtons { get; set; }

        /// <summary> The save button. </summary>
        [XmlIgnore]
        public Button SaveButton { get; set; }

        /// <summary> The cancel button. </summary>
        [XmlIgnore]
        public Button CancelButton { get; set; }

        /// <summary> Collection of Attributes. </summary>
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

        /// <summary> The form's name. 
        /// <para> Is used the specify html id/name attributes. </para>
        /// </summary>
        public string Name
        {
            get
            {
                if (this.formName == null)
                {
                    // Guid.NewGuid().ToString() -> maybe a good solution?
                    return string.Format("formschema_{0}", Id);
                }
                else
                {
                    return this.formName;
                }
            }
            set
            {
                this.formName = value;
            }
        }

        /// <summary> Gets a value indicating whether the form contains a SaveButton, a CancelButton or any LinkButtons. </summary>
        public bool HasButtons
        {
            get { return SaveButton != null || CancelButton != null || LinkButtons.Any(); }
        }

        /// <summary> Gets a value indicating whether the form will be posted via ajax. </summary>
        public bool AjaxPost { get; set; }

        /// <summary> Initializes a new form. </summary>
        public Form()
        {
            LinkButtons = new List<Button>();
            Blocks = new List<Block>();
            this.Metadata = new Metadata();
        }

        /// <summary> Initializes a new form. </summary>
        public Form(IEnumerable<FormElement> elements)
        {
            LinkButtons = new List<Button>();
            Blocks = new List<Block>();
            this.Metadata = new Metadata();

            if (!elements.IsNullOrEmpty())
            {
                // Prepare form
                Block b1 = this.AddBlock();
                elements.ForEach(x => { b1.AddFormElement(x); });
            }
        }

        /// <summary> Adds a name (format) for this form and it's elements. </summary>
        public void ApplyNameFormat(string name)
        {
            // Set Form name
            this.Name = name;

            // Set element name
            IEnumerable<FormElement> elements = this.GetAllElements();

            foreach (FormElement element in elements)
            {
                element.Name = "form_" + name + "_" + element.Id;
            }
        }

        #region Elements

        /// <summary> Gets a flattened list of all form elements. </summary>
        public List<FormElement> GetAllElements()
        {
            return Blocks.SelectMany(x => x.Columns.SelectMany(e => e.Elements)).ToList();
        }

        /// <summary> Get form element. </summary>
        /// <param name="id"> The form layout id. </param>
        public FormElement GetFormElement(int id)
        {
            return this.GetAllElements().Where(x => x.Id == id).FirstOrDefault();
        }

        /// <summary> Get form element. </summary>
        public FormElement Element(int id)
        {
            return this.GetFormElement(id);
        }

        /// <summary> Get form element. </summary>
        public T Element<T>(int id) where T : FormElement
        {
            return (T)this.GetFormElement(id);
        }

        /// <summary> Get Fields. </summary>
        public List<FormElement> GetFields(Func<FormElement, bool> predicate)
        {
            return this.GetAllElements().Where(predicate).ToList();
        }

        /// <summary> Check if form element is present. </summary>
        /// <param name="id"> The form layout id. </param>
        public bool HasFormElement(int id)
        {
            return this.GetAllElements().Where(x => x.Id == id).Any();
        }

        /// <summary> Gets a maximum existing element ID among all form elements. </summary>
        public int GetMaxExistingElementId()
        {
            FormElement last = this.GetAllElements().OrderBy(x => x.Id).LastOrDefault();

            return last != null ? last.Id : 0;
        }

        /// <summary> Gets a maximum existing element Index among all form elements. </summary>
        public int GetMaxExistingElementIndex()
        {
            FormElement last = this.GetAllElements().OrderBy(x => x.Id).LastOrDefault();

            return last != null ? last.Index : 0;
        }

        #endregion

        #region Values

        /// <summary> Set Form Value. </summary>
        /// <param name="elementId"> The form element id. </param>
        /// <param name="typeId"> The form element type id. </param>
        /// <param name="value"> The form value. </param>
        public void SetValue(int formLayoutId, object value)
        {
            IEnumerable<FormElement> elements = GetAllElements();

            FormElement element = elements.Where(x => x.Id == formLayoutId).FirstOrDefault();

            if (element == null)
                return;

            if (element.Type == FormElementType.Block)
            { }
            else if (element.Type == FormElementType.Html)
            {
                HtmlElement html = element as HtmlElement;

                html.Value = Convert.ToString(value);
            }
            else if (element.Type == FormElementType.Input)
            {
                Input input = element as Input;

                input.Value = Convert.ToString(value);
            }
            else if (element.Type == FormElementType.Textarea)
            {
                Textarea textarea = element as Textarea;

                textarea.Value = Convert.ToString(value);
            }
            else if (element.Type == FormElementType.Dropdown)
            {
                Dropdown dropdown = element as Dropdown;

                // Vast value to string array
                string[] valueArray = null;

                if (value != null)
                {
                    Type valueType = value.GetType();
                    Type genType = valueType.IsGenericType ? valueType.GetGenericTypeDefinition() : null;
                    bool valueIsEnumerable = valueType.IsArray || (genType != null && (genType == typeof(List<>) || genType == typeof(IEnumerable<>)));

                    if (valueIsEnumerable)
                    {
                        valueArray = ((IEnumerable)value).Cast<object>().Select(x => x.ToString()).ToArray();
                    }
                    else
                    {
                        string sValue = Convert.ToString(value);
                        if (sValue != null)
                        {
                            valueArray = new string[] { sValue };
                        }
                    }
                }

                // Dropdown with items, or ajax source
                if (dropdown.Items != null && dropdown.Items.Count > 0)
                {
                    // Deselect every item
                    foreach (DropdownItem itemX in dropdown.Items)
                    {
                        itemX.Selected = false;
                    }

                    // Values set?
                    if (valueArray != null)
                    {
                        // Valid item placeholder
                        List<string> validItems = new List<string>();

                        // Loop over every value in the value array
                        foreach (string val in valueArray)
                        {
                            // Select item
                            DropdownItem item = dropdown.Items.Where(x => x.Value != null && x.Value.Equals(val, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                            // Check if selected item exists
                            if (item != null)
                            {
                                item.Selected = true;
                                validItems.Add(item.Value);
                            }
                        }

                        // Set dropdown value
                        dropdown.Value = validItems.ToArray();
                    }
                }
                else
                {
                    // No items, maybe it's an ajax source
                    if (dropdown.AjaxSource != null)
                    {
                        dropdown.Value = valueArray;
                    }
                }

            }
            else if (element.Type == FormElementType.Radiobutton)
            {
                Radiobutton radio = element as Radiobutton;

                string sValue = Convert.ToString(value);

                RadiobuttonItem item = radio.Items.Where(x => x.Value.Equals(sValue, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (item != null)
                {
                    foreach (RadiobuttonItem itemX in radio.Items)
                    {
                        itemX.Selected = false;
                    }

                    item.Selected = true;

                    radio.Value = sValue;
                }
            }
            else if (element.Type == FormElementType.Checkbox)
            {
                Checkbox checkbox = element as Checkbox;

                // Vast value to string array
                string[] valueArray = null;
                Type valueType = value.GetType();
                if (valueType.IsArray)
                {
                    valueArray = ((IEnumerable)value).Cast<object>().Select(x => x.ToString()).ToArray();
                }
                else
                {
                    string sValue = Convert.ToString(value);
                    valueArray = new string[] { sValue };
                }

                // Deselect every item
                foreach (CheckboxItem itemX in checkbox.Items)
                {
                    itemX.Selected = false;
                }

                // Values set?
                if (valueArray != null)
                {
                    // Valid item placeholder
                    List<string> validItems = new List<string>();

                    // Loop over every value in the value array
                    foreach (string val in valueArray)
                    {
                        CheckboxItem item = checkbox.Items.Where(x => x.Value.Equals(val, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                        if (item != null)
                        {
                            item.Selected = true;
                            validItems.Add(item.Value);
                        }
                    }

                    // Set checkbox value
                    checkbox.Value = validItems.ToArray();
                }
            }
            else if (element.Type == FormElementType.Calendar)
            {
                Calendar calendar = element as Calendar;
                calendar.SetElementValue(value);
            }
            else if (element.Type == FormElementType.Hidden)
            {
                Hidden hidden = element as Hidden;

                hidden.Value = Convert.ToString(value);
            }
            else if (element.Type == FormElementType.Tagging)
            {
                Tagging tagging = element as Tagging;

                tagging.Value = (List<TaggingItem>)value;
            }
            else if (element.Type == FormElementType.Link)
            {
                Link obj = element as Link;

                obj.SetElementValue(value);
            }
            else if (element.Type == FormElementType.Label)
            {
                Label label = element as Label;
                label.SetElementValue(value);
            }
        }

        /// <summary> Get Form Value. </summary>
        /// <param name="formLayoutId"> The form layout id. </param>
        public object GetValue(int formLayoutId)
        {
            IEnumerable<FormElement> elements = GetAllElements();

            FormElement element = elements.Where(x => x.Id == formLayoutId).FirstOrDefault();

            if (element == null)
                return null;

            if (element.Type == FormElementType.Block)
            { }
            else if (element.Type == FormElementType.Html)
            {
                HtmlElement html = element as HtmlElement;

                return html.Value;
            }
            else if (element.Type == FormElementType.Input)
            {
                Input input = element as Input;

                return input.Value;
            }
            else if (element.Type == FormElementType.Textarea)
            {
                Textarea textarea = element as Textarea;

                return textarea.Value;
            }
            else if (element.Type == FormElementType.Dropdown)
            {
                Dropdown dropdown = element as Dropdown;

                return dropdown.Value;
            }
            else if (element.Type == FormElementType.Radiobutton)
            {
                Radiobutton radio = element as Radiobutton;

                return radio.Value;
            }
            else if (element.Type == FormElementType.Checkbox)
            {
                Checkbox checkbox = element as Checkbox;

                return checkbox.Value;
            }
            else if (element.Type == FormElementType.Calendar)
            {
                Calendar calendar = element as Calendar;

                return calendar.Value;
            }
            else if (element.Type == FormElementType.Hidden)
            {
                Hidden hidden = element as Hidden;

                return hidden.Value;
            }
            else if (element.Type == FormElementType.Link)
            {
                return (element as Link).GetElementValue();
            }

            return null;
        }

        /// <summary> Convert specific objects to it's string representation. </summary>
        public string GetStringValue(object value)
        {
            // Check NULL
            if (value == null)
                return null;

            if (value is string)
            {
                return (string)value;
            }
            else if (value is DtpReference)
            {
                DtpReference reference = (DtpReference)value;

                return "dtp|" + reference.DtpRequestId;
            }
            else
            {
                return Convert.ToString(value);
            }
        }


        /// <summary> Injects posted form values into the form. </summary>
        public void InjectFormData(NameValueCollection data)
        {
            Form form = this;

            IDictionary<string, string> formValues = data.AllKeys.ToDictionary(k => k, v => data[v]);

            var elements = form.GetAllElements();

            foreach (FormElement formElement in elements)
            {
                // skip non-input elements
                if (formElement.Type == FormElementType.Block)
                {
                    continue;
                }

                Form.InjectFormData(formValues, formElement);
            }
        }

        /// <summary> Injects posted form values into the form. </summary>
        public void InjectFormData(Dictionary<string, string> data)
        {
            Form form = this;

            NameValueCollection collection = new NameValueCollection();

            foreach (string key in data.Keys)
            {
                collection.Add(key, data[key]);
            }

            this.InjectFormData(collection);
        }

        /// <summary> Inject posted form values into the form. (field id, value) </summary>
        public void InjectFormData(Dictionary<int, object> data)
        {
            Form form = this;
            NameValueCollection collection = new NameValueCollection();
            foreach (KeyValuePair<int, object> pair in data)
            {
                FormElement element = form.GetFormElement(pair.Key);
                if (element == null)
                    continue;

                collection.Add(element.Key, pair.Value.ToString());
            }

            this.InjectFormData(collection);
        }

        /// <summary> Injects posted form values into the form element. </summary>
        public static void InjectFormData(NameValueCollection data, FormElement element)
        {
            // Check
            if (element == null)
                return;

            IDictionary<string, string> formValues = data.AllKeys.ToDictionary(k => k, v => data[v]);

            Form.InjectFormData(formValues, element);
        }

        /// <summary> Injects posted form values into the form element. </summary>
        public static void InjectFormData(string data, FormElement element)
        {
            // Check
            if (element == null)
                return;

            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(element.Name, data);

            InjectFormData(nvc, element);
        }

        /// <summary> Injects posted form values into the form element. </summary>
        public static void InjectFormData(Dictionary<string, string> data, FormElement element)
        {
            // Check
            if (element == null)
                return;

            NameValueCollection collection = new NameValueCollection();

            foreach (string key in data.Keys)
            {
                collection.Add(key, data[key]);
            }

            InjectFormData(collection, element);
        }


        /// <summary> Injects form values into the form element. </summary>
        private static void InjectFormData(IDictionary<string, string> formValues, FormElement formElement)
        {
            // Check
            if (formElement == null)
                return;

            #region Checkbox
            if (formElement.Type == FormElementType.Checkbox)
            {
                // possible multiple selection
                Checkbox checkbox = (Checkbox)formElement;
                List<string> selectedCheckboxValues = new List<string>();
                foreach (var item in checkbox.Items)
                {
                    string key = string.Format("{0}_{1}", checkbox.Name, item.Id);
                    item.Selected = formValues.ContainsKey(key);

                    if (item.Selected)
                    {
                        selectedCheckboxValues.Add(item.Value);
                    }
                }
                checkbox.Value = selectedCheckboxValues.ToArray();
            }
            #endregion
            //#region FileUpload
            //else if (formElement.Type == FormElementType.FileUpload)
            //{
            //    List<string> keys = new List<string>();

            //    foreach (string key in formValues.Keys)
            //    {
            //        if (key.Equals(formElement.Name) || key.StartsWith(formElement.Name + "["))
            //        {
            //            keys.Add(key);
            //        }
            //    }

            //    foreach (string key in keys)
            //    {
            //        string value = formValues[key];

            //        if (!string.IsNullOrWhiteSpace(value))
            //        {
            //            FileUpload fileUpload = (FileUpload)formElement;

            //            // Parse values
            //            List<string> parsedValues = new List<string>();

            //            if (value.StartsWith("["))
            //            {
            //                parsedValues = JArray.Parse(value).Values<string>().ToList();
            //            }
            //            else
            //            {
            //                parsedValues.Add(value);
            //            }

            //            foreach (string parsedValue in parsedValues)
            //            {
            //                bool isParsed = false;

            //                // Try int parsing
            //                if (!isParsed)
            //                {
            //                    int intValue = 0;
            //                    bool isInt = Int32.TryParse(parsedValue, out intValue);

            //                    if (isInt)
            //                    {
            //                        fileUpload.AddValue(new ContentReference { ReferenceId = intValue });
            //                        isParsed = true;
            //                    }
            //                }

            //                // Try dtp request
            //                if (!isParsed)
            //                {
            //                    string[] info = parsedValue.Split(new char[] { '|' });

            //                    if (info.Length == 2)
            //                    {
            //                        string tag = info[0]; // 'dtp'
            //                        string id = info[1];

            //                        fileUpload.AddValue(new DtpReference { DtpRequestId = Convert.ToInt32(id) });
            //                        isParsed = true;
            //                    }
            //                }

            //                // Try temp upload
            //                if (!isParsed)
            //                {
            //                    object parsed = FileUpload.ParseTag(parsedValue);

            //                    if (parsed is FileReference)
            //                    {
            //                        fileUpload.AddValue((FileReference)parsed);
            //                        isParsed = true;
            //                    }
            //                    else if (parsed is ContentReference)
            //                    {
            //                        fileUpload.AddValue((ContentReference)parsed);
            //                        isParsed = true;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //#endregion
            #region Dropdown
            else if (formElement.Type == FormElementType.Dropdown)
            {
                Dropdown dropdown = (Dropdown)formElement;
                string value = formValues.ContainsKey(formElement.Name) ? formValues[formElement.Name] : null;

                string[] multipleValues = null;

                if (dropdown.Multiple)
                {
                    bool parsed = false;

                    // Check Array
                    if (!parsed)
                    {
                        try
                        {
                            JArray ja = JArray.Parse(value);
                            multipleValues = ja.Values<string>().ToArray();
                            parsed = true;
                        }
                        catch { }
                    }

                    // Check CSV
                    if (!parsed)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(value))
                            {
                                multipleValues = new string[0];
                            }
                            else
                            {
                                multipleValues = value.Split(',');
                            }
                            parsed = true;
                        }
                        catch { }
                    }
                }

                // Assign
                dropdown.Value = multipleValues ?? new string[] { value };

                // Mark Selection
                dropdown.Items.ForEach(x => x.Selected = dropdown.Value.Contains(x.Value));
            }
            #endregion
            #region Tagging
            else if (formElement.Type == FormElementType.Tagging)
            {
                Tagging tagging = (Tagging)formElement;
                string value = formValues.ContainsKey(formElement.Name) ? formValues[formElement.Name] : null;

                if (string.IsNullOrEmpty(value))
                {
                    tagging.Value = new List<TaggingItem>();
                }
                else
                {
                    if ((value.StartsWith("{") && value.EndsWith("}")) || //For object
                        (value.StartsWith("[") && value.EndsWith("]"))) //For array
                    {
                        JToken jValue = JToken.Parse(value);

                        if (jValue is JObject)
                        {
                            tagging.Value = new List<TaggingItem> { JsonConvert.DeserializeObject<TaggingItem>(value) };
                        }
                        else if (jValue is JArray)
                        {
                            tagging.Value = JsonConvert.DeserializeObject<List<TaggingItem>>(value);
                        }
                        else
                        {
                            tagging.Value = new List<TaggingItem>();
                        }
                    }
                    else
                    {
                        tagging.Value = new List<TaggingItem>();
                    }

                }
            }
            #endregion
            #region Link
            else if (formElement.Type == FormElementType.Link)
            {
                Link obj = formElement as Link;

                if (formValues.ContainsKey(formElement.Name))
                { obj.Url = formValues[formElement.Name]; }
                if (formValues.ContainsKey(formElement.Name + "_display"))
                { obj.DisplayText = formValues[formElement.Name + "_display"]; }
                if (formValues.ContainsKey(formElement.Name + "_title"))
                { obj.Title = formValues[formElement.Name + "_title"]; }
                if (formValues.ContainsKey(formElement.Name + "_target"))
                {
                    int targetId = Convert.ToInt32(formValues[formElement.Name + "_target"]);
                    obj.Target = (Link.LinkTarget)targetId;
                }
            }
            #endregion
            #region Radiobutton
            else if (formElement.Type == FormElementType.Radiobutton)
            {
                if (formValues.ContainsKey(formElement.Name))
                {
                    string value = formValues[formElement.Name];

                    ((Radiobutton)formElement).Value = value;
                    ((Radiobutton)formElement).Items.ForEach(x => x.Selected = x.Value == value);
                }
            }
            #endregion
            #region Html
            else if (formElement.Type == FormElementType.Html)
            {
                HtmlElement htmlElement = (HtmlElement)formElement;

                if (htmlElement.Editable)
                {
                    if (formValues.ContainsKey(formElement.Name))
                    {
                        string value = formValues[formElement.Name];
                        htmlElement.Value = value;
                    }
                }
            }
            #endregion
            #region Calendar
            else if (formElement.Type == FormElementType.Calendar)
            {
                Calendar element = (Calendar)formElement;

                // Simple Value
                element.Value = formValues.ContainsKey(formElement.Name) ? formValues[formElement.Name] : null;

                // Advanced Value
                string json = formValues.ContainsKey(formElement.Name + "_data") ? formValues[formElement.Name + "_data"] : null;

                element.ObjValue = Calendar.AdvancedValue.ParseJson(json);
            }
            #endregion
            #region List
            else if (formElement.Type == FormElementType.List)
            {
                ListElement obj = formElement as ListElement;

                string value = formValues.ContainsKey(formElement.Name) ? formValues[formElement.Name] : null;

                obj.RawValue = value;

                if (string.IsNullOrEmpty(value))
                {
                    obj.Value = null;
                }
                else
                {
                    JsonSerializerSettings jser = new JsonSerializerSettings();
                    jser.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    obj.Value = JsonConvert.DeserializeObject<ListElement.ValueItem[]>(value, jser);
                }
            }
            #endregion
            #region Ranges
            else if (formElement.Type == FormElementType.Range)
            {
                RangeElement range = formElement as RangeElement;

                if (formValues.ContainsKey(formElement.Name))
                {
                    string value = formValues[formElement.Name];

                    // Only change something when it has a value
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        // Overwrite current items
                        range.Items = new List<RangeElement.RangeElementItem>();
                        JArray array = JArray.Parse(value);

                        foreach (JObject jo in array)
                        {
                            RangeElement.RangeElementItem rangeItem = new RangeElement.RangeElementItem();
                            rangeItem.Id = Convert.ToInt32(jo["Id"].ToString());
                            rangeItem.ColorCode = jo["ColorCode"].ToString();
                            rangeItem.Label = jo["Label"].ToString();
                            rangeItem.MinimumValue = 0;
                            rangeItem.MaximumValue = 0;

                            string minVal = jo["MinimumValue"].ToString();
                            string maxVal = jo["MaximumValue"].ToString();
                            if (minVal.IsInteger())
                                rangeItem.MinimumValue = Convert.ToInt32(minVal);
                            if (maxVal.IsInteger())
                                rangeItem.MaximumValue = Convert.ToInt32(maxVal);
                            range.Items.Add(rangeItem);
                        }
                    }
                }
            }
            #endregion
            #region Other: Input / Textarea / Calendar / Hidden / Lookup / Library Input
            else
            {
                string value = formValues.ContainsKey(formElement.Name) ? formValues[formElement.Name] : null;

                switch (formElement.Type)
                {
                    case FormElementType.Input:
                        ((Input)formElement).Value = value;
                        break;

                    case FormElementType.Textarea:
                        ((Textarea)formElement).Value = value;
                        break;

                    case FormElementType.Hidden:
                        ((Hidden)formElement).Value = value;
                        break;
                }
            }
            #endregion
        }

        /// <summary> Sets the defaults values as active values. </summary>
        public void PersistDefaultValues()
        {
            List<FormElement> fields = this.GetAllElements();
            foreach (FormElement field in fields)
            {
                if (field is Input)
                {
                    Input f = (field as Input);
                    if (string.IsNullOrEmpty(f.Value))
                    { f.Value = f.DefaultValue; }
                }
                else if (field is Textarea)
                {
                    Textarea f = (field as Textarea);
                    if (string.IsNullOrEmpty(f.Value))
                    { f.Value = f.DefaultValue; }
                }
                else if (field is Calendar)
                {
                    Calendar f = (field as Calendar);
                    if (string.IsNullOrEmpty(f.Value))
                    { f.Value = f.DefaultDate; }
                }
                else if (field is Hidden)
                {
                    Hidden f = (field as Hidden);
                    if (string.IsNullOrEmpty(f.Value))
                    { f.Value = f.DefaultValue; }
                }
                else if (field is Radiobutton)
                {
                    Radiobutton f = (field as Radiobutton);
                    //if (string.IsNullOrEmpty(f.Value)) { f.Value = f.DefaultValue; }
                    if (f.Value.IsNullOrEmpty() && !f.Items.IsNullOrEmpty() && f.Items.Any(x => x.Selected))
                    { f.Value = f.Items.FirstOrDefault(x => x.Selected).Value; }
                }
                else if (field is Checkbox)
                {
                    Checkbox f = (field as Checkbox);
                    if (f.Value.IsNullOrEmpty() && !f.Items.IsNullOrEmpty())
                    { f.Value = f.Items.Where(x => x.Selected).Select(x => x.Value).ToArray(); }
                }
                else if (field is Dropdown)
                {
                    Dropdown f = (field as Dropdown);
                    if (f.Value.IsNullOrEmpty() && !f.Items.IsNullOrEmpty())
                    { f.Value = f.Items.Where(x => x.Selected).Select(x => x.Value).ToArray(); }
                }
            }
        }

        #endregion

        #region Form Building

        /// <summary> Adds a new block to this form. </summary>
        /// <returns> Returns the instance. </returns>
        public Block AddBlock()
        {
            return this.AddBlock(FormColumnLayout.NoSplit);
        }

        /// <summary> Adds a new block to this form. </summary>
        /// <returns> Returns the instance. </returns>
        public Block AddBlock(FormColumnLayout layout)
        {
            Block block = new Block(layout);
            this.Blocks.Add(block);
            return block;
        }

        /// <summary> Add Input. </summary>
        public Input AddInput(int id)
        {
            Block block = this.Blocks.FirstOrDefault();
            if (block == null)
                block = this.AddBlock();

            return block.AddInput(id);
        }

        /// <summary> Add Dropdown. </summary>
        public Dropdown AddDropDown(int id)
        {
            Block block = this.Blocks.FirstOrDefault();
            if (block == null)
                block = this.AddBlock();

            return block.AddDropDown(id);
        }

        #endregion

        #region Layout

        /// <summary> Rearrange elements according to limits. </summary>
        public void RearrangeElements(int maxColumns, int minElementsPerColumn)
        {
            // Adjust input
            if (maxColumns > 3)
                maxColumns = 3;
            if (maxColumns < 1)
                maxColumns = 1;
            if (minElementsPerColumn < 1)
                minElementsPerColumn = 1;

            // Loop blocks
            foreach (Block block in this.Blocks)
            {
                // Gather elements
                IEnumerable<FormElement> elements = block.Columns.SelectMany(e => e.Elements);
                int cElements = elements.Count();

                // Calculate layout
                while (maxColumns > 1)
                {
                    double epc = (Convert.ToDouble(cElements) / Convert.ToDouble(maxColumns));

                    if (epc >= Convert.ToDouble(minElementsPerColumn))
                    {
                        break;
                    }
                    //else if (cElements > ((maxColumns - 1) * minElementsPerColumn))
                    //{
                    //    break;
                    //}

                    maxColumns--;
                }

                block.SetLayout(this.ColumnToLayout(maxColumns));
            }
        }

        /// <summary> Converts a column count to a layout format. </summary>
        private FormColumnLayout ColumnToLayout(int columns)
        {
            // Adjust input
            if (columns > 3)
                columns = 3;
            if (columns < 1)
                columns = 1;

            switch (columns)
            {
                case 3:
                    return FormColumnLayout.Split3;
                case 2:
                    return FormColumnLayout.Split2;
                case 1:
                    return FormColumnLayout.NoSplit;
                default:
                    return FormColumnLayout.NoSplit;
            }
        }

        #endregion

        //=== ?

        /// <summary> Creates a Form Guid. </summary>
        public static string Guid()
        {
            System.Guid g = System.Guid.NewGuid();
            string guid = Convert.ToBase64String(g.ToByteArray()).Replace("=", "").Replace("+", "").Replace("/", "");
            return guid;
        }
    }
}
