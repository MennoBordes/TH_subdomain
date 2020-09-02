namespace TH.Core.Tools.Form.Models
{
    public class TaggingItem
    {
        /// <summary> Identifier. </summary>
        public string Id { get; set; }

        /// <summary> Text. </summary>
        public string Text { get; set; }

        /// <summary> Index. </summary>
        public int Index { get; set; }

        /// <summary> Checks if this item has an identifier set that differs from it's text value. </summary>
        public bool HasIdentifier()
        {
            return string.Compare(this.Id, this.Text) != 0;
        }
    }
}
