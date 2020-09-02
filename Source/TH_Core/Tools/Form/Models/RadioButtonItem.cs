using System.Collections.Generic;
using System.Xml.Serialization;

namespace TH.Core.Tools.Form.Models
{
    public class RadiobuttonItem
    {
        /// <summary> Item's Id. </summary>
        public int Id { get; set; }

        /// <summary> Item's Index. </summary>
        public int Index { get; set; }

        /// <summary> Indicator if this item is selected. </summary>
        public bool Selected { get; set; }

        /// <summary> Item's Label. </summary>
        public string Label { get; set; }

        /// <summary> Item's Value. </summary>
        public string Value { get; set; }

        /// <summary> Indicator if this item is disabled. </summary>
        public bool Disabled { get; set; }

        /// <summary> Collection of Attributes. </summary>
        [XmlIgnore]
        public Dictionary<string, object> ItemAttributes { get; set; }
    }
}
