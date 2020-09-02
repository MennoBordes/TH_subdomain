using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TH.Core.Tools.Form.Models
{
    public class ValidationResult
    {
        public IEnumerable<ValidationError> Errors { get; set; }

        public bool Valid
        {
            get
            {
                return this.Errors == null || !this.Errors.Any();
            }
        }
    }
}
