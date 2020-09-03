using System.Collections.Generic;
using System.Xml.Serialization;

namespace TH.Core.Tools.Form.Models
{
    public class Column
    {
        /// <summary> The column's index. </summary>
        [XmlAttribute]
        public int Number { get; set; }

        /// <summary> The elements inside this column. </summary>
        public List<FormElement> Elements { get; set; }

        public Column()
        {
            this.Elements = new List<FormElement>();
        }

        public string ColumnCssClass
        {
            get
            {
                return string.Format("col{0}", Number);
            }
        }
    }
}
