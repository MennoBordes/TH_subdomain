using System;

namespace TH.Core.Base.Translation.Attributes
{
    /// <summary> The Translation Source Attribute.
    /// <para> Place this attribute on a property (inside the data entity) to indicate that this property is the source identifier. </para>
    /// <para> Placement of this attribute is optional (specifier will be resolved via reflection instead). </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TranslationSourceAttribute : System.Attribute
    {
        /// <summary> Initializes a new instance of the <see cref="TranslationSourceAttribute" /> class. </summary>
        public TranslationSourceAttribute() { }
    }
}
