using System;

namespace TH.Core.Base.Translation.Attributes
{
    /// <summary> The Translation Attribute.
    /// <para> Place this attribute on a 'normal' entity to indicate that this entity has translation data. 
    /// The class type specification is optional. 
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TranslationAttribute : System.Attribute
    {
        /// <summary> The Data Entity. </summary>
        private Type dataType = null;

        /// <summary> Initializes a new instance of the <see cref="TranslationAttribute" /> class. 
        /// <para> Data Entity will be resolved via reflection. </para>
        /// </summary>
        public TranslationAttribute()
        {
            this.dataType = null;
        }

        /// <summary> Initializes a new instance of the <see cref="TranslationAttribute" /> class. </summary>
        /// <param name="dataType"> The data entity type. </param>
        public TranslationAttribute(Type dataType)
        {
            this.dataType = dataType;
        }

        /// <summary> Gets the data entity type. </summary>
        public Type DataType
        {
            get
            {
                return this.dataType;
            }
        }
    }
}
