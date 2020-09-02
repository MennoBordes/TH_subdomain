using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TH.Core.Base.Database;
using TH.Core.Base.Enums;
using TH.Core.Base.Extensions;
using TH.Core.Tools.Form.Enums;
using TH.Core.Tools.Form.Models;

using Global = System.Globalization;

namespace TH.Core.Tools.Form
{
    public partial class FormManager
    {
        private readonly Repository repository;
        private readonly string namespaceEntityFormat = "TH.Core.Tools.Form{0}";

        /// <summary> Default date format </summary>
        private const string DefaultDateFormat = "dd/MM/yyyy";

        public FormManager(Repository repository = null)
        {
            this.repository = repository ?? new Repository();
        }

        #region Validation => Logic
        /// <summary> Validates the specified form element. </summary>
        public IEnumerable<ValidationInfo> Validate(FormElement formElement)
        {
            IEnumerable<ValidationInfo> errorTypes = Enumerable.Empty<ValidationInfo>();

            if (formElement.Status == (int)Status.Inactive || formElement.Status == (int)Status.Removed)
            {
                return errorTypes;
            }

            switch (formElement.Type)
            {
                case TH.Core.Tools.Form.Enums.FormElementType.Input:
                    errorTypes = Validate((Input)formElement);
                    break;

                case TH.Core.Tools.Form.Enums.FormElementType.Textarea:
                    errorTypes = Validate((Textarea)formElement);
                    break;

                case TH.Core.Tools.Form.Enums.FormElementType.Dropdown:
                    errorTypes = Validate((Dropdown)formElement);
                    break;

                case TH.Core.Tools.Form.Enums.FormElementType.Radiobutton:
                    errorTypes = Validate((Radiobutton)formElement);
                    break;

                case TH.Core.Tools.Form.Enums.FormElementType.Checkbox:
                    errorTypes = Validate((Checkbox)formElement);
                    break;

                case TH.Core.Tools.Form.Enums.FormElementType.Calendar:
                    errorTypes = Validate((TH.Core.Tools.Form.Models.Calendar)formElement);
                    break;

                case TH.Core.Tools.Form.Enums.FormElementType.Hidden:
                    errorTypes = Validate((Hidden)formElement);
                    break;

                //case TH.Core.Tools.Form.Enums.FormElementType.FileUpload:
                //    errorTypes = Validate((FileUpload)formElement);
                //    break;

                case TH.Core.Tools.Form.Enums.FormElementType.Tagging:
                    errorTypes = Validate((Tagging)formElement);
                    break;

                case TH.Core.Tools.Form.Enums.FormElementType.Link:
                    errorTypes = Validate((Link)formElement);
                    break;

                case TH.Core.Tools.Form.Enums.FormElementType.Html:
                    errorTypes = Validate((HtmlElement)formElement);
                    break;

                case Enums.FormElementType.Range:
                    errorTypes = Validate((RangeElement)formElement);
                    break;
            }
            return errorTypes;
        }

        /// <summary> Validates the dropdown. </summary>
        private IEnumerable<ValidationInfo> Validate(Dropdown dropdown)
        {
            List<ValidationInfo> errors = new List<ValidationInfo>();

            if (dropdown.Required && (dropdown.Value.IsNullOrEmpty() || string.IsNullOrEmpty(dropdown.Value.FirstOrDefault())))
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(dropdown);
                validationInfo.ErrorType = ValidationErrorType.Required;
                errors.Add(validationInfo);
            }

            return errors;
        }

        /// <summary> Validates the checkbox. </summary>
        private IEnumerable<ValidationInfo> Validate(Checkbox checkbox)
        {
            List<ValidationInfo> errors = new List<ValidationInfo>();

            if (checkbox.Min != 0 && checkbox.Items.Count(item => item.Selected) < checkbox.Min)
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(checkbox);
                validationInfo.ErrorType = (checkbox.Min == 1 && checkbox.Max == 1 ? ValidationErrorType.SelectSingle : ValidationErrorType.Min);
                validationInfo.ProperValue = checkbox.Min.Value.ToString(Global.CultureInfo.InvariantCulture);
                errors.Add(validationInfo);
            }

            if (checkbox.Max != 0 && checkbox.Items.Count(item => item.Selected) > checkbox.Max)
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(checkbox);
                validationInfo.ErrorType = ValidationErrorType.Max;
                validationInfo.ProperValue = checkbox.Max.Value.ToString(Global.CultureInfo.InvariantCulture);
                errors.Add(validationInfo);
            }

            return errors;
        }

        /// <summary> Validates the calendar. </summary>
        /// <param name="minStartDate"> Additional minimum start date validation. </param>
        /// <param name="maxEndDate"> Additional maximum end date validation. </param>
        public IEnumerable<ValidationInfo> Validate(Calendar calendar, string minStartDate = null, string maxEndDate = null)
        {
            List<ValidationInfo> errors = new List<ValidationInfo>();
            string customDateFormat = DefaultDateFormat;

            if (calendar.AdvancedMode)
            {
                Calendar.AdvancedValue v = (calendar.ObjValue is Calendar.AdvancedValue) ? (Calendar.AdvancedValue)calendar.ObjValue : null;

                // Required
                if ((v == null || v.StartDate == null || v.EndDate == null) && calendar.Required)
                {
                    ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(calendar);
                    validationInfo.ErrorType = ValidationErrorType.Required;
                    errors.Add(validationInfo);
                    return errors;
                }

                if (v.StartDate != null)
                {
                    // Check min date
                    if (calendar.MinDate != null)
                    {
                        DateTime dateTime = DateTime.ParseExact(calendar.MinDate, calendar.DisplayFormat, Global.CultureInfo.InvariantCulture);
                        if (!string.IsNullOrEmpty(calendar.MinDate) && v.StartDate.Value < dateTime)
                        {
                            ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(calendar);
                            validationInfo.ErrorType = ValidationErrorType.CalendarMinDate;
                            validationInfo.ProperValue = calendar.MinDate;
                            errors.Add(validationInfo);
                        }
                    }

                    // Check additional min date
                    if (minStartDate != null)
                    {
                        DateTime dateTime = DateTime.ParseExact(minStartDate, calendar.DisplayFormat, Global.CultureInfo.InvariantCulture);
                        if (!string.IsNullOrEmpty(minStartDate) && v.StartDate.Value < dateTime)
                        {
                            ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(calendar);
                            validationInfo.ErrorType = ValidationErrorType.CalendarMinStartDate;
                            validationInfo.ProperValue = minStartDate;
                            errors.Add(validationInfo);
                        }
                    }
                }

                if (v.EndDate != null)
                {
                    // Check max date
                    if (calendar.MaxDate != null)
                    {
                        DateTime dateTime = DateTime.ParseExact(calendar.MaxDate, calendar.DisplayFormat, Global.CultureInfo.InvariantCulture);
                        if (!string.IsNullOrEmpty(calendar.MaxDate) && v.EndDate.Value > dateTime)
                        {
                            ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(calendar);
                            validationInfo.ErrorType = ValidationErrorType.CalendarMaxDate;
                            validationInfo.ProperValue = calendar.MaxDate;
                            errors.Add(validationInfo);
                        }
                    }

                    // Check additional max date
                    if (maxEndDate != null)
                    {
                        DateTime dateTime = DateTime.ParseExact(maxEndDate, calendar.DisplayFormat, Global.CultureInfo.InvariantCulture);
                        if (!string.IsNullOrEmpty(maxEndDate) && v.EndDate.Value > dateTime)
                        {
                            ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(calendar);
                            validationInfo.ErrorType = ValidationErrorType.CalendarMaxEndDate;
                            validationInfo.ProperValue = maxEndDate;
                            errors.Add(validationInfo);
                        }
                    }
                }

                return errors;
            }

            // Required
            if (string.IsNullOrEmpty(calendar.Value) && calendar.Required)
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(calendar);
                validationInfo.ErrorType = ValidationErrorType.Required;
                errors.Add(validationInfo);
                return errors;
            }

            // Check date format and value
            if (!string.IsNullOrEmpty(calendar.Value))
            {
                DateTime defaultDate;
                if (!DateTime.TryParseExact(calendar.Value, calendar.DisplayFormat, Global.CultureInfo.InvariantCulture, Global.DateTimeStyles.None, out defaultDate))
                {
                    ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(calendar);
                    validationInfo.ErrorType = ValidationErrorType.InvalidFormat;
                    errors.Add(validationInfo);
                    return errors;
                }

                // Check min date
                if (calendar.MinDate != null)
                {
                    DateTime dateTime = DateTime.ParseExact(calendar.MinDate, calendar.DisplayFormat, Global.CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(calendar.MinDate) && defaultDate < dateTime)
                    {
                        ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(calendar);
                        validationInfo.ErrorType = ValidationErrorType.CalendarMinDate;
                        validationInfo.ProperValue = calendar.MinDate;
                        errors.Add(validationInfo);
                    }
                }

                // Check max date
                if (calendar.MaxDate != null)
                {
                    DateTime dateTime = DateTime.ParseExact(calendar.MaxDate, calendar.DisplayFormat, Global.CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(calendar.MaxDate) && defaultDate > dateTime)
                    {
                        ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(calendar);
                        validationInfo.ErrorType = ValidationErrorType.CalendarMaxDate;
                        validationInfo.ProperValue = calendar.MaxDate;
                        errors.Add(validationInfo);
                    }
                }

                // Check additional min date
                if (minStartDate != null)
                {
                    DateTime dateTime = DateTime.ParseExact(minStartDate, calendar.DisplayFormat, Global.CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(minStartDate) && defaultDate < dateTime)
                    {
                        ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(calendar);
                        validationInfo.ErrorType = ValidationErrorType.CalendarMinStartDate;
                        validationInfo.ProperValue = minStartDate;
                        errors.Add(validationInfo);
                    }
                }

                // Check additional max date
                if (maxEndDate != null)
                {
                    DateTime dateTime = DateTime.ParseExact(maxEndDate, calendar.DisplayFormat, Global.CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(maxEndDate) && defaultDate > dateTime)
                    {
                        ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(calendar);
                        validationInfo.ErrorType = ValidationErrorType.CalendarMaxEndDate;
                        validationInfo.ProperValue = maxEndDate;
                        errors.Add(validationInfo);
                    }
                }
            }

            return errors;
        }

        /// <summary> Validates the input. </summary>
        private IEnumerable<ValidationInfo> Validate(Input input)
        {
            List<ValidationInfo> errors = new List<ValidationInfo>();

            if (input.Required && string.IsNullOrEmpty(input.Value))
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(input);
                validationInfo.ErrorType = ValidationErrorType.Required;
                errors.Add(validationInfo);
            }

            if (!string.IsNullOrWhiteSpace(input.Value) && input.Value.Length > input.MaxLength)
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(input);
                validationInfo.ErrorType = ValidationErrorType.MaxLength;
                validationInfo.ProperValue = input.MaxLength.ToString(Global.CultureInfo.InvariantCulture);
                errors.Add(validationInfo);
            }

            if (!string.IsNullOrWhiteSpace(input.Value) && input.Value.Length < input.MinLength)
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(input);
                validationInfo.ErrorType = ValidationErrorType.MinLength;
                validationInfo.ProperValue = input.MinLength.ToString(Global.CultureInfo.InvariantCulture);
                errors.Add(validationInfo);
            }

            if (!string.IsNullOrWhiteSpace(input.Value) && !string.IsNullOrWhiteSpace(input.MaskPattern) && Regex.IsMatch(input.MaskPattern, @"\d") && !Regex.IsMatch(input.Value, @"\d"))
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(input);
                validationInfo.ErrorType = ValidationErrorType.InvalidFormat;
                validationInfo.ProperValue = input.MaskPattern.ToString(Global.CultureInfo.InvariantCulture);
                errors.Add(validationInfo);
            }

            return errors;
        }

        /// <summary> Validates the radiobutton. </summary>
        private IEnumerable<ValidationInfo> Validate(Radiobutton radiobutton)
        {
            List<ValidationInfo> errors = new List<ValidationInfo>();

            if (radiobutton.Required && string.IsNullOrEmpty(radiobutton.Value))
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(radiobutton);
                validationInfo.ErrorType = ValidationErrorType.Required;
                errors.Add(validationInfo);
            }

            if (!string.IsNullOrWhiteSpace(radiobutton.Value) && !radiobutton.Items.IsNullOrEmpty())
            {
                if (!radiobutton.Items.Any(x => x.Value == radiobutton.Value))
                {
                    ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(radiobutton);
                    validationInfo.ErrorType = ValidationErrorType.InvalidFormat;
                    errors.Add(validationInfo);
                }
            }

            return errors;
        }

        /// <summary> Validates the textarea. </summary>
        private IEnumerable<ValidationInfo> Validate(Textarea textarea)
        {
            List<ValidationInfo> errors = new List<ValidationInfo>();

            if (textarea.Required && string.IsNullOrEmpty(textarea.Value))
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(textarea);
                validationInfo.ErrorType = ValidationErrorType.Required;
                errors.Add(validationInfo);
            }

            if (textarea.MaxLength.HasValue && textarea.Value != null && textarea.Value.Length > textarea.MaxLength)
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(textarea);
                validationInfo.ErrorType = ValidationErrorType.MaxLength;
                validationInfo.ProperValue = textarea.MaxLength.Value.ToString(Global.CultureInfo.InvariantCulture);
                errors.Add(validationInfo);
            }

            return errors;
        }

        /// <summary> Validates the hidden input. </summary>
        private IEnumerable<ValidationInfo> Validate(Hidden hidden)
        {
            List<ValidationInfo> errors = new List<ValidationInfo>();

            if (hidden.Required && string.IsNullOrEmpty(hidden.Value))
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(hidden);
                validationInfo.ErrorType = ValidationErrorType.Required;
                errors.Add(validationInfo);
            }

            return errors;
        }

        ///// <summary> Validates the file upload. </summary>
        //private IEnumerable<ValidationInfo> Validate(FileUpload fileUpload)
        //{
        //    List<ValidationInfo> errors = new List<ValidationInfo>();

        //    // Get uploaded files
        //    IEnumerable<FileReference> fileReferences = fileUpload.Values.Where(x => x is FileReference).Cast<FileReference>();
        //    IEnumerable<ContentReference> contentReferences = fileUpload.Values.Where(x => x is ContentReference).Cast<ContentReference>();

        //    // Check Required
        //    if (fileUpload.Required)
        //    {
        //        if (fileReferences.Count() == 0 && contentReferences.Count() == 0)
        //        {
        //            errors.Add(new ValidationInfo
        //            {
        //                HtmlName = fileUpload.Name,
        //                ErrorType = ValidationErrorType.Required,
        //                Label = fileUpload.Label
        //            });
        //        }
        //    }

        //    // Check Min
        //    if (fileReferences.Count() != 0)
        //    {
        //        if (fileUpload.Min > fileReferences.Count())
        //        {
        //            errors.Add(new ValidationInfo
        //            {
        //                HtmlName = fileUpload.Name,
        //                ErrorType = ValidationErrorType.FileUploadMin,
        //                Label = fileUpload.Label,
        //                ProperValue = fileUpload.Min.ToString()
        //            });
        //        }
        //    }

        //    // Check Max
        //    if (fileReferences.Count() != 0)
        //    {
        //        if (fileUpload.Max < fileReferences.Count())
        //        {
        //            errors.Add(new ValidationInfo
        //            {
        //                HtmlName = fileUpload.Name,
        //                ErrorType = ValidationErrorType.FileUploadMax,
        //                Label = fileUpload.Label,
        //                ProperValue = fileUpload.Max.ToString()
        //            });
        //        }
        //    }

        //    // Check Extensions
        //    if (fileReferences.Count() != 0)
        //    {
        //        foreach (FileReference reference in fileReferences)
        //        {
        //            bool validExtension = ExtensionPool.CheckExtension(reference.FileExtension, allowed: fileUpload.AllowedExtensions);

        //            if (!validExtension)
        //            {
        //                errors.Add(new ValidationInfo
        //                {
        //                    HtmlName = fileUpload.Name,
        //                    ErrorType = ValidationErrorType.FileUploadExtension,
        //                    Label = fileUpload.Label,
        //                    ProperValue = (fileUpload.AllowedExtensions != null ? string.Join(",", fileUpload.AllowedExtensions) : String.Empty)
        //                });
        //            }
        //        }
        //    }

        //    return errors;
        //}

        /// <summary> Validates the tagging. </summary>
        private IEnumerable<ValidationInfo> Validate(Tagging tagging)
        {
            List<ValidationInfo> errors = new List<ValidationInfo>();

            if (tagging.Required && (tagging.Value == null || tagging.Value.Count == 0))
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(tagging);
                validationInfo.ErrorType = ValidationErrorType.Required;
                errors.Add(validationInfo);
            }

            return errors;
        }

        /// <summary> Validates the link. </summary>
        private IEnumerable<ValidationInfo> Validate(Link link)
        {
            List<ValidationInfo> errors = new List<ValidationInfo>();

            if (link.Required && string.IsNullOrWhiteSpace(link.Url))
            {
                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(link);
                validationInfo.ErrorType = ValidationErrorType.Required;
                errors.Add(validationInfo);
            }

            return errors;
        }

        ///// <summary> Validates the link. </summary>
        //private IEnumerable<ValidationInfo> Validate(HtmlElement htmlElement)
        //{
        //    List<ValidationInfo> errors = new List<ValidationInfo>();

        //    if (htmlElement.Editable)
        //    {
        //        // Required
        //        if (htmlElement.Required && string.IsNullOrWhiteSpace(htmlElement.Value))
        //        {
        //            ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(htmlElement);
        //            validationInfo.ErrorType = ValidationErrorType.Required;
        //            errors.Add(validationInfo);
        //        }

        //        // Max Length
        //        if (htmlElement.MaxLength != null)
        //        {
        //            int length = 0;
        //            if (!string.IsNullOrEmpty(htmlElement.Value))
        //            {
        //                HtmlEngine engine = new HtmlEngine();
        //                HtmlAgilityPack.HtmlDocument doc = engine.LoadDoc(htmlElement.Value);

        //                if (doc != null)
        //                {
        //                    string text = engine.ExtractVisibleText(doc);

        //                    // Trim (same behaviour as in the editor)
        //                    text = text.Trim();

        //                    // Count breaks as 1 character
        //                    text = text.Replace("\r\n", " ");

        //                    length = text.Length;
        //                }
        //            }

        //            if (length > htmlElement.MaxLength.Value)
        //            {
        //                ValidationInfo validationInfo = Mapper.Map<ValidationInfo>(htmlElement);
        //                validationInfo.ErrorType = ValidationErrorType.MaxLength;
        //                errors.Add(validationInfo);
        //            }
        //        }
        //    }

        //    return errors;
        //}

        /// <summary> Validates the ranges. </summary>
        private IEnumerable<ValidationInfo> Validate(RangeElement range)
        {
            List<ValidationInfo> errors = new List<ValidationInfo>();

            if (range.Required && range.Items.IsNullOrEmpty())
            {
                errors.Add(new ValidationInfo
                {
                    HtmlName = range.Name,
                    ErrorType = ValidationErrorType.Required,
                    Label = range.Label
                });
            }

            if (!range.Items.IsNullOrEmpty())
            {
                foreach (RangeElement.RangeElementItem item in range.Items)
                {
                    if (string.IsNullOrWhiteSpace(item.Label))
                    {
                        errors.Add(new ValidationInfo
                        {
                            HtmlName = range.Name,
                            ErrorType = ValidationErrorType.MissingRangelabel,
                            Label = range.Label
                        });
                    }

                    if (range.Items.Any(x => x.MinimumValue == item.MinimumValue && x.MaximumValue == item.MaximumValue && x.Id != item.Id))
                    {
                        errors.Add(new ValidationInfo
                        {
                            HtmlName = range.Name,
                            ErrorType = ValidationErrorType.DuplicateRanges,
                            Label = string.IsNullOrWhiteSpace(item.Label) ? item.Label : range.Label
                        });
                    }
                }
            }

            return errors;
        }

        #endregion

        #region Validation => Result

        ///// <summary> Gets the translated validation results for the ValidationInfo. </summary>
        //public List<ValidationError> GetTranslatedValidationResults(IEnumerable<ValidationInfo> validationInfos, int languageId)
        //{
        //    List<ValidationError> result = new List<ValidationError>();

        //    Translation tm = new Translation();
        //    Dictionary<int, string> translations = tm.Get(new int[] { 1034, 1035, 1036, 1037, 1038, 1039, 1040, 1041, 1042, 1043, 1044, 1863, 4487, 4488 }, languageId);

        //    foreach (ValidationInfo valInfo in validationInfos)
        //    {
        //        string messageFormat = this.GetValidationMessageFormat(valInfo.ErrorType, translations);
        //        string message = string.Format(messageFormat, valInfo.Label, valInfo.ProperValue);

        //        result.Add(new ValidationError(valInfo.HtmlName, message));
        //    }

        //    return result;
        //}

        /// <summary> Gets the validation message format by error type and language id. </summary>
        private string GetValidationMessageFormat(ValidationErrorType errorType, Dictionary<int, string> translations)
        {
            string format = null;

            //switch (errorType)
            //{
            //    case ValidationErrorType.InvalidFormat:
            //        format = (translations.ContainsKey(1034) ? translations[1034] : null);
            //        break;
            //    case ValidationErrorType.Required:
            //        format = (translations.ContainsKey(1035) ? translations[1035] : null);
            //        break;
            //    case ValidationErrorType.MinLength:
            //        format = (translations.ContainsKey(1036) ? translations[1036] : null);
            //        break;
            //    case ValidationErrorType.MaxLength:
            //        format = (translations.ContainsKey(1037) ? translations[1037] : null);
            //        break;
            //    case ValidationErrorType.Min:
            //        format = (translations.ContainsKey(1038) ? translations[1038] : null);
            //        break;
            //    case ValidationErrorType.Max:
            //        format = (translations.ContainsKey(1039) ? translations[1039] : null);
            //        break;
            //    case ValidationErrorType.SelectSingle:
            //        format = (translations.ContainsKey(1863) ? translations[1863] : null);
            //        break;
            //    case ValidationErrorType.SelectMultiple:
            //        format = (translations.ContainsKey(1864) ? translations[1864] : null);
            //        break;
            //    case ValidationErrorType.CalendarMinDate:
            //        format = (translations.ContainsKey(1040) ? translations[1040] : null);
            //        break;
            //    case ValidationErrorType.CalendarMaxDate:
            //        format = (translations.ContainsKey(1041) ? translations[1041] : null);
            //        break;
            //    case ValidationErrorType.CalendarMinStartDate:
            //        format = (translations.ContainsKey(1861) ? translations[1861] : null);
            //        break;
            //    case ValidationErrorType.CalendarMaxEndDate:
            //        format = (translations.ContainsKey(1862) ? translations[1862] : null);
            //        break;
            //    case ValidationErrorType.FileUploadMin:
            //        format = (translations.ContainsKey(1042) ? translations[1042] : null);
            //        break;
            //    case ValidationErrorType.FileUploadMax:
            //        format = (translations.ContainsKey(1043) ? translations[1043] : null);
            //        break;
            //    case ValidationErrorType.FileUploadExtension:
            //        format = (translations.ContainsKey(1044) ? translations[1044] : null);
            //        break;
            //    case ValidationErrorType.MissingRangelabel:
            //        format = (translations.ContainsKey(4487) ? translations[4487] : null);
            //        break;
            //    case ValidationErrorType.DuplicateRanges:
            //        format = (translations.ContainsKey(4488) ? translations[4488] : null);
            //        break;
            //}

            if (string.IsNullOrEmpty(format))
            {
                format = this.GetDefaultValidationMessageFormat(errorType);
            }

            return format;
        }

        /// <summary> Gets the default validation message format by error type. </summary>
        private string GetDefaultValidationMessageFormat(ValidationErrorType errorType)
        {
            switch (errorType)
            {
                case ValidationErrorType.InvalidFormat:
                    return "Invalid '{0}' format.";

                case ValidationErrorType.Required:
                    return "The field '{0}' is required.";

                case ValidationErrorType.MinLength:
                    return "Too short '{0}'. Min length: {1}.";

                case ValidationErrorType.MaxLength:
                    return "Too long '{0}'. Max length: {1}.";

                case ValidationErrorType.Min:
                    return "Please select at least {1} '{0}'.";

                case ValidationErrorType.Max:
                    return "Please select less than {1} '{0}'.";

                case ValidationErrorType.SelectSingle:
                    return "Please select {1} '{0}'.";

                case ValidationErrorType.SelectMultiple:
                    return "Please select {1} '{0}'.";

                case ValidationErrorType.CalendarMinDate:
                    return "Min value of '{0}' is {1}.";

                case ValidationErrorType.CalendarMaxDate:
                    return "Max value of '{0}' is {1}.";

                case ValidationErrorType.CalendarMinStartDate:
                    return "The value of '{0}' can not be earlier than the start date '{1}'.";

                case ValidationErrorType.CalendarMaxEndDate:
                    return "The value of '{0}' can not exceed the end date '{1}'.";

                case ValidationErrorType.FileUploadMin:
                    return "Minimum files for '{0}' is {1}.";

                case ValidationErrorType.FileUploadMax:
                    return "Maximum files for '{0}' is {1}.";

                case ValidationErrorType.FileUploadExtension:
                    return "Invalid extension for '{0}' ({1}).";
                case ValidationErrorType.MissingRangelabel:
                    return "Missing range label for '{0}'";
                case ValidationErrorType.DuplicateRanges:
                    return "Duplicate range '{0}'";
                default:
                    throw new ArgumentOutOfRangeException("errorType");
            }
        }

        #endregion


        //=== Initialization

        /// <summary> Inits a new field for the specified type. </summary>
        /// <param name="languageId"> This is needed for the calender field. </param>
        public static FormElement InitField(FormElementType Type, int? languageId = null, int? id = null, string label = null, string description = null, bool? required = null, bool applyBasicSettings = false)
        {
            switch (Type)
            {
                case Enums.FormElementType.Calendar:
                    Calendar c = Calendar.Init(languageId: languageId, id: id, label: label, description: description, required: required);
                    ;

                    if (applyBasicSettings)
                    {
                        c.AdvancedMode = false;
                        c.DefaultDate = "*|DATETIME:NOW|*";
                    }

                    return c;

                case Enums.FormElementType.Checkbox:
                    Checkbox cb = Checkbox.Init(id: id, label: label, description: description);

                    if (applyBasicSettings)
                    {
                        cb.Min = required.GetValueOrDefault() ? 1 : 0;
                        cb.Max = 1;
                    }

                    return cb;

                case Enums.FormElementType.Dropdown:
                    Dropdown dd = Dropdown.Init(id: id, label: label, description: description, required: required);

                    if (applyBasicSettings)
                    {
                        dd.Multiple = false;
                    }

                    return dd;

                //case Enums.FormElementType.FileUpload:
                //    return FileUpload.Init(id: id, label: label, description: description, required: required);

                case Enums.FormElementType.Html:
                    HtmlElement html = HtmlElement.Init(id: id, label: label, description: description);

                    if (applyBasicSettings)
                    {
                        html.Editable = true;
                        html.EditorProfile = HtmlElement.EEditorProfile.Basic;
                        html.Required = required.GetValueOrDefault();
                    }

                    return html;

                case Enums.FormElementType.Input:
                    Input input = Input.Init(id: id, label: label, description: description, required: required);

                    if (applyBasicSettings)
                    {
                        input.MaxLength = 255;
                        input.MinLength = 0;
                    }

                    return input;

                case Enums.FormElementType.Radiobutton:
                    Radiobutton rb = Radiobutton.Init(id: id, label: label, description: description, required: required);

                    //if (applyBasicSettings)
                    //{
                    //    None available
                    //}

                    return rb;

                case Enums.FormElementType.Textarea:
                    Textarea ta = Textarea.Init(id: id, label: label, description: description, required: required);

                    //if (applyBasicSettings)
                    //{
                    //    None available
                    //}

                    return ta;

                case Enums.FormElementType.Link:
                    Link link = Link.Init(id: id, label: label, description: description, required: required);

                    //if (applyBasicSettings)
                    //{
                    //    None available
                    //}

                    return link;

                case Enums.FormElementType.Label:
                    Label l = Label.Init(id: id, label: label, description: description);

                    //if (applyBasicSettings)
                    //{
                    //    None available
                    //}

                    return l;


                case Enums.FormElementType.List:
                    ListElement list = ListElement.Init(id: id, label: label, description: description);

                    //if (applyBasicSettings)
                    //{
                    //    None available
                    //}

                    return list;

                default:
                    throw new Exception("Init: Field type is not supported.");
                    // Hidden, Tagging, etc.
            }
        }


        //=== Conversion

        /// <summary> Converts a field to another type. </summary>        
        public static FormElement ConvertField(FormElement field, Enums.FormElementType Type)
        {
            // Create new field
            FormElement newField = InitField(Type);

            // Merge properties
            newField.Merge(field);
            newField.Type = Type;

            return newField;
        }


        ////=== Serialization

        ///// <summary> Serializes fields to an xml based format. </summary>
        //public static string Serialize(IEnumerable<FormElement> fields)
        //{
        //    string xml = null;

        //    if (fields != null)
        //    {
        //        xml = fields.XmlSerialize(indent: false,
        //            extraTypes: new Type[] { typeof(ContentReference), typeof(FileReference), typeof(List<object>), typeof(Link.LinkData) });
        //    }

        //    return xml;
        //}

        ///// <summary> De-serializes fields from an xml based format. </summary>
        //public static List<FormElement> Deserialize(string xml)
        //{
        //    List<FormElement> fields = null;

        //    if (xml != null)
        //    {
        //        fields = xml.DeserializeObject<List<FormElement>>(extraTypes:
        //                new Type[] { typeof(ContentReference), typeof(FileReference), typeof(List<object>), typeof(Link.LinkData) });
        //    }

        //    return fields;
        //}

    }
}
