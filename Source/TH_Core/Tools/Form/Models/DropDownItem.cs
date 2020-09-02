using System.Xml.Serialization;

namespace TH.Core.Tools.Form.Models
{
    public class DropdownItem
    {
        /// <summary> Item handle. </summary>
        public int Id { get; set; }

        /// <summary> This item's index. </summary>
        public int Index { get; set; }

        /// <summary> Indicator if the item is selected. </summary>
        public bool Selected { get; set; }

        /// <summary> The displayed label. </summary>
        public string Label { get; set; }

        /// <summary> The value. </summary>
        public string Value { get; set; }

        /// <summary> Indicator if the selection of this item can be altered. </summary>
        public bool Locked { get; set; }

        /// <summary> Indicator if this item is disabled. </summary>
        public bool Disabled { get; set; }

        /// <summary> Option Group Index. </summary>
        public int OptionGroupIndex { get; set; }

        /// <summary> Option Group Name. </summary>
        public string OptionGroupName { get; set; }

        /// <summary> The reference object which is used as source.
        /// <para> Not used in actual form rendering. </para>
        /// </summary>
        [XmlIgnore]
        public object ReferenceObject { get; set; }
    }
}
