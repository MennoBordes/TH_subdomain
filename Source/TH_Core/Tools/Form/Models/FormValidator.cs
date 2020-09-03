using System.Collections.Generic;
using System.Linq;
using TH.Core.Base.Enums;

namespace TH.Core.Tools.Form.Models
{
    /// <summary> Form Validator. </summary>
    public class FormValidator
    {
        private ValidationError[] results;
        private List<FormElement> invalidFields;
        private bool isValid;


        /// <summary> The Validation results. </summary>
        public ValidationError[] Results
        {
            get
            {
                return this.results;
            }
        }

        /// <summary> The fields marked as invalid. </summary>
        public List<FormElement> InvalidFields { get { return this.invalidFields; } }

        /// <summary> Indicator if form/data is valid. </summary>
        public bool IsValid { get { return this.isValid; } }

        /// <summary> Indicator if the errors should be injected into the fields. </summary>
        public bool InjectErrors { get; set; }


        /// <summary> Initializes a new form validator. </summary>
        public FormValidator()
        {
            this.results = new ValidationError[0];
            this.isValid = false;
            this.invalidFields = new List<FormElement>();
            this.InjectErrors = false;
        }


        /// <summary> Validates the data. </summary>
        public bool Validate(Form form)
        {
            return this.Validate(form, TH.Core.Base.Translation.Translation.DEFAULT_LANGUAGE);
        }

        /// <summary> Validates the data. </summary>
        public bool Validate(Form form, int languageId)
        {
            List<FormElement> fields = form.GetAllElements();

            return this.Validate(fields, languageId);
        }

        /// <summary> Validates the data. </summary>
        public bool Validate(List<FormElement> fields)
        {
            return this.Validate(fields, TH.Core.Base.Translation.Translation.DEFAULT_LANGUAGE);
        }

        /// <summary> Validates the data. </summary>
        public bool Validate(List<FormElement> fields, int languageId)
        {
            FormManager fm = new FormManager();

            bool _isValid = true;
            List<ValidationError> valResults = new List<ValidationError>();
            this.invalidFields.Clear();

            List<Calendar> calendarFields = fields.Where(x => x is Calendar).Select(x => x as Calendar).ToList();

            foreach (FormElement formElement in fields)
            {
                bool _validate = true;

                if (_validate)
                {
                    IEnumerable<Tools.Form.Models.ValidationInfo> validations = null;
                    bool processed = false;

                    // 0 is inactive, 2 is deleted
                    if (formElement.Status == (int)Status.Inactive || formElement.Status == (int)Status.Removed)
                    {
                        validations = Enumerable.Empty<ValidationInfo>();
                        processed = true;
                    }

                    // Validation: Calendar
                    if (formElement is Calendar && !processed)
                    {
                        // Validate: Start/End Date
                        Calendar fCalendar = formElement as Calendar;
                        if (fCalendar.IsStartDate && calendarFields.Count(x => x.IsEndDate) == 1) // Must be exact
                        {
                            // Find Matching End Date
                            Calendar opposition = calendarFields.FirstOrDefault(x => x.IsEndDate);

                            string maxDate = string.IsNullOrEmpty(opposition.Value) ? null : opposition.Value;

                            // Validate: Element
                            validations = fm.Validate(fCalendar, maxEndDate: maxDate);

                            processed = true;
                        }
                        else if (fCalendar.IsEndDate && calendarFields.Count(x => x.IsStartDate) == 1) // Must be exact
                        {
                            // Find Matching Start Date
                            Calendar opposition = calendarFields.FirstOrDefault(x => x.IsStartDate);

                            string minDate = string.IsNullOrEmpty(opposition.Value) ? null : opposition.Value;

                            // Validate: Element
                            validations = fm.Validate(fCalendar, minStartDate: minDate);

                            processed = true;
                        }
                    }

                    // Validate: Element
                    if (!processed)
                    {
                        validations = fm.Validate(formElement);
                    }

                    if (validations.Any())
                    {
                        List<ValidationError> validationErrors = new List<ValidationError>();
                        foreach(ValidationInfo info in validations)
                        {
                            validationErrors.Add(new ValidationError(htmlName: info.HtmlName, message: "Invalid " + info.Label));
                        }
                        valResults.AddRange(validationErrors);
                        //List<ValidationError> translatedErrors = fm.GetTranslatedValidationResults(validations, languageId);
                        //valResults.AddRange(translatedErrors);
                        _isValid = false;

                        if (this.InjectErrors)
                        {
                            //formElement.Errors = translatedErrors.Select(x => x.Message).ToArray();
                        }

                        this.invalidFields.Add(formElement);
                    }
                }
            }

            this.results = valResults.ToArray();
            this.isValid = _isValid;

            return _isValid;
        }
    }
}
