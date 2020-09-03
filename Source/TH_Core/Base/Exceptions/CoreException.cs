namespace TH.Core.Base.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class CoreException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreException"/> class.
        /// </summary>
        /// <param name="notifyDevSupport"> Notifies the support staff of this exception. </param>
        public CoreException(bool notifyDevSupport = false, bool notifyGeneralSupport = false)
            : base()
        {
            if (notifyDevSupport || notifyGeneralSupport)
            {
                //NotifySupport(notifyDevSupport: notifyDevSupport, notifyGeneralSupport: notifyGeneralSupport);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="notifyDevSupport"> Notifies the support staff of this exception. </param>
        public CoreException(string message, bool notifyDevSupport = false, bool notifyGeneralSupport = false)
            : base(message)
        {
            if (notifyDevSupport || notifyGeneralSupport)
            {
                //NotifySupport(notifyDevSupport: notifyDevSupport, notifyGeneralSupport: notifyGeneralSupport);
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CoreException"/> class.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public CoreException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreException"/> class.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="innerException"></param>
        /// <param name="args"></param>
        public CoreException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreException"/> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected CoreException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="notifyDevSupport"> Notifies the support staff of this exception. </param>
        public CoreException(string message, Exception innerException, bool notifyDevSupport = false, bool notifyGeneralSupport = false)
            : base(message, innerException)
        {
            if (notifyDevSupport || notifyGeneralSupport)
            {
                //NotifySupport(notifyDevSupport: notifyDevSupport, notifyGeneralSupport: notifyGeneralSupport);
            }
        }


        ////=== Helper

        ///// <summary> Notify support with basic information. </summary>
        ///// <param name="notifyDevSupport"> The development support department will be notified (used for development errors). </param>
        ///// <param name="notifyGeneralSupport"> The general support department will be notified (used for implementation errors). </param> 
        //private void NotifySupport(bool notifyDevSupport = true, bool notifyGeneralSupport = false)
        //{
        //    if (!Config.MODE_PRODUCTION)
        //        return;

        //    MX.Core.Modules.Communication.CommunicationCenter cc = new MX.Core.Modules.Communication.CommunicationCenter();

        //    // Message
        //    StringBuilder d = new StringBuilder();
        //    d.AppendLine("== Message");
        //    d.AppendLine(this.Message ?? string.Empty);

        //    // Exception
        //    d.AppendLine(Environment.NewLine + "== Exception Type");
        //    d.AppendLine(this.GetType().ToString());

        //    string data = d.ToString();

        //    cc.NotifySupport(data: data, exception: this.InnerException, general: notifyGeneralSupport, development: notifyDevSupport);
        //}
    }
}
