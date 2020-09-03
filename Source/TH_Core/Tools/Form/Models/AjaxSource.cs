using System.Collections.Generic;

namespace TH.Core.Tools.Form.Models
{
    /// <summary> General Ajax Source. </summary>
    public class AjaxSource
    {
        public string Url { get; set; }

        public Dictionary<string, object> Meta { get; set; }

        public AjaxSource()
        {
            this.Meta = new Dictionary<string, object>();
        }

        public AjaxSource(string url)
        {
            this.Url = url;
            this.Meta = new Dictionary<string, object>();
        }
    }
}
