using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TH.Core.Tools.Form.Models
{
    /// <summary> Describes the result of a validation returned to the client in JSON format. </summary>
    public class ValidationError
    {
        /// <summary> Gets or sets the name of the HTML element. </summary>
        public string HtmlName { get; set; }

        /// <summary> Gets or sets the message. </summary>
        public string Message { get; set; }

        /// <summary> Initializes a new instance of the <see cref="ValidationError" /> class. </summary>
        /// <param name="htmlName"> Name of the HTML element. </param>
        /// <param name="message"> The message. </param>
        public ValidationError(string htmlName, string message)
        {
            this.HtmlName = htmlName;
            this.Message = message;
        }
    }
}
