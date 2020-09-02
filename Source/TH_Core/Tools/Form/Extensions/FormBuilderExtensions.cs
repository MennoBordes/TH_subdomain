namespace TH.Core.Tools.Form.Extensions
{
    using System.Reflection;
    using TH.Core.Tools.Form.Models;
    public static class FormBuilderExtensions
    {
        /// <summary> Set Label. </summary>
        public static T Label<T>(this T element, string value) where T : FormElement
        {
            element.Label = value;
            return element;
        }

        /// <summary> Set Description. </summary>
        public static T Description<T>(this T element, string value) where T : FormElement
        {
            element.Description = value;
            return element;
        }

        /// <summary> Set Visible. </summary>
        public static T Visible<T>(this T element, bool value) where T : FormElement
        {
            element.Visible = value;
            return element;
        }

        /// <summary> Set Disabled. </summary>
        public static T Disabled<T>(this T element, bool value) where T : FormElement
        {
            element.Disabled = value;
            return element;
        }

        /// <summary> Set Required. </summary>
        public static T Required<T>(this T element, bool value) where T : FormElement
        {
            if (element != null)
            {
                PropertyInfo property = element.GetType().GetProperty("Required");

                if (property != null)
                {
                    property.SetValue(element, value, null);
                }
            }

            return element;
        }

        /// <summary> Set MaxLength. </summary>
        public static Input MaxLength(this Input element, int value)
        {
            element.MaxLength = value;
            return element;
        }
    }
}
