using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TH.WebUI.Tools.UIComponents
{
    /// <summary> UI Component: Tabs. </summary>
    public class TabsDisplay
    {
        /// <summary> The skin of this component (eg:default). </summary>
        public string Skin { get; set; }

        /// <summary> Attributes (html). </summary>
        public Dictionary<string, object> Attributes { get; set; }

        /// <summary> Tabs / Items. </summary>
        public List<Item> Items { get; set; }

        //=== Item
        public class Item
        {
            /// <summary> Item key. </summary>
            public string Key { get; set; }

            /// <summary> Item label. </summary>
            public string Label { get; set; }

            /// <summary> Item state active. </summary>
            public bool Active { get; set; }

            /// <summary> Attributes (html). </summary>
            public Dictionary<string, object> Attributes { get; set; }
        }
    }
}