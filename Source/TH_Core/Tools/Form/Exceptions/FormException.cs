using System;

namespace TH.Core.Tools.Form.Exceptions
{
    using TH.Core.Base.Exceptions;
    using TH.Core.Tools.Form.Enums;

    class FormException : CoreException
    {
        public ExceptionType Type { get; set; }

        public FormException() : base()
        {
            this.Type = ExceptionType.Undefined;
        }

        public FormException(string message) : base(message)
        {
            this.Type = ExceptionType.Undefined;
        }

        public FormException(string message, Exception innerException) : base(message, innerException)
        {
            this.Type = ExceptionType.Undefined;
        }

        public FormException(ExceptionType type) : base()
        {
            this.Type = type;
        }

        public FormException(ExceptionType type, Exception innerException) : base(string.Empty, innerException)
        {
            this.Type = type;
        }
    }
}
