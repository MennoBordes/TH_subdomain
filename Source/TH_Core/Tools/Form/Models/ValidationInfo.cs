using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    /// <summary> Describes full validation info. </summary>
    public class ValidationInfo
    {
        /// <summary> Gets or sets the name of the HTML element. </summary>
        public string HtmlName { get; set; }

        /// <summary> Gets or sets the type. </summary>
        public ValidationErrorType ErrorType { get; set; }

        /// <summary> Gets or sets the label. </summary>
        public string Label { get; set; }

        /// <summary> Gets or sets the proper value. </summary>
        public string ProperValue { get; set; }
    }
}
