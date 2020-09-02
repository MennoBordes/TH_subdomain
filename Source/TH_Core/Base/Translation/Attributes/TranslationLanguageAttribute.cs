using System;

namespace TH.Core.Base.Translation.Attributes
{
    /// <summary> The Translation Language Attribute.
    /// <para> Place this attribute on a property (inside the data entity) to indicate that this property is the language identifier. </para>
    /// <para> Placement of this attribute is optional (specifier will be resolved via reflection instead). </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TranslationLanguageAttribute : System.Attribute
    {
        /// <summary> Initializes a new instance of the <see cref="TranslationLanguageAttribute" /> class. </summary>
        public TranslationLanguageAttribute() { }
    }
}
