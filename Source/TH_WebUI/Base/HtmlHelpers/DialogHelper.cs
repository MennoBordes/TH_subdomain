namespace TH.WebUI.Base.HtmlHelpers
{
    using System.Text;
    using System.Web.Mvc;

    /// <summary> Html helper for dialog system. </summary>
    public static class DialogHelper
    {
        /// <summary> Renders the dialog settings segment. </summary>
        public static MvcHtmlString DialogSettings(this HtmlHelper helper, string title)
        {
            StringBuilder s = new StringBuilder();

            s.Append("<div data-ds-key=\"\"></div>");

            s.AppendFormat("<div class=\"hidden\" data-ds-title=\"{0}\"></div>", title);

            return new MvcHtmlString(s.ToString());
        }
    }
}