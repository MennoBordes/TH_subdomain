using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace TH.WebUI.Tools.UIComponents
{
    public static class UIComponentHelper
    {
        /// <summary> Renders Tabs. </summary>
        public static void RenderTabs(this HtmlHelper helper, TabsDisplay model)
        {
            helper.RenderPartial("~/Tools/UIComponents/Views/_Tabs.cshtml", model);
        }

        public static MvcHtmlString RenderHtmlAttributes(this HtmlHelper helper, Dictionary<string, object> attributes)
        {
            if (attributes != null && attributes.Count > 0)
            {
                string s = string.Empty;
                foreach (KeyValuePair<string, object> attr in attributes)
                {
                    s += attr.Key + "=\"" + attr.Value + "\" ";
                }
                return new MvcHtmlString(s);
            }

            return null;
        }
    }
}