using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TH.WebUI.Tools.Form.Models
{
    using TH.Core.Tools.Form.Models;
    public class Field
    {
        /// <summary> Field's Parent Form. </summary>
        public Form Form { get; set; }

        /// <summary> Field's Element. </summary>
        public FormElement Element { get; set; }

        /// <summary> Translator. </summary>
        //public Translator Translator { get; set; }

        /// <summary> Context. </summary>
        //public SessionContext Context { get; set; }

        /// <summary> Indicates if the field should be displayed in a pop-up. </summary>
        public bool RenderInPopUp { get; set; }

        /// <summary> Create unique field identifier. </summary>
        public string CreateUniqueId()
        {
            if (this.Element == null)
                return null;

            string prime_id = (this.Form != null ? this.Form.Name : null) + this.Element.Name;
            return prime_id;
        }
    }
}