using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    public class Button : FormElement
    {
        private string name;

        /// <summary> Button Type. </summary>
        public FormButtonType ButtonType { get; set; }

        /// <summary> The JavaScript onClick action. </summary>
        public string OnClick { get; set; }

        /// <summary> Html Name. </summary>
        public new string Name
        {
            get
            {
                if (this.name == null)
                { return string.Format("button_{0}", Id); }
                else
                { return this.name; }
            }
            set { this.name = value; }
        }

        /// <summary> Constructs a new button. </summary>
        public Button()
        {
            Visible = true;
            Type = FormElementType.Button;
        }

        /// <summary> Creates a new instance with the default configuration. </summary>
        public static Button Init(int? id = null, string label = null, int? col = null, FormButtonType type = FormButtonType.Alternate)
        {
            Button obj = new Button();

            obj.Id = id ?? 0;
            obj.Type = FormElementType.Button;
            obj.ButtonType = type;
            obj.Visible = true;
            obj.Index = 0;

            obj.Label = label;

            obj.Column = col ?? 1;

            return obj;
        }
    }
}
