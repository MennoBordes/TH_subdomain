using System;
using TH.Core.Base.Translation.Enums;

namespace TH.Core.Base.Translation.Attributes
{
    /// <summary> The Translation Field Attribute.
    /// <para> Place this attribute on a property (inside the data entity) to indicate that this property is translateable field. </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TranslationField : System.Attribute
    {
        /// <summary> The visual name of the field. </summary>
        private string fieldName = null;

        /// <summary> The type of the field. </summary>
        private TranslationFieldType fieldType = TranslationFieldType.Input;

        /// <summary> Indicates whether this field is required or not. </summary>
        private bool required = false;

        /// <summary> Initializes a new instance of the <see cref="TranslationField" /> class. </summary>
        /// <param name="fieldName"> The name of the field. </param>
        /// <param name="fieldType"> The types of the field. </param>
        /// <param name="required"> Indicates whether this field is required or not. </param>
        public TranslationField(string fieldName, TranslationFieldType fieldType, bool required)
        {
            this.fieldName = fieldName;
            this.fieldType = fieldType;
            this.required = required;
        }

        /// <summary> Gets the name of the field. </summary>
        public string FieldName
        {
            get
            {
                return this.fieldName;
            }
        }

        /// <summary> Gets the type of the field. </summary>
        public TranslationFieldType FieldType
        {
            get
            {
                return this.fieldType;
            }
        }

        /// <summary> Gets the required status of the field. </summary>
        public bool Required
        {
            get
            {
                return this.required;
            }
        }
    }
}
