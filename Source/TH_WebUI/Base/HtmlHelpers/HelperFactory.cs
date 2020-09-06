namespace TH.WebUI.Base.HtmlHelpers
{
    using System.Web.Mvc;

    /// <summary> Html Helper factory. </summary>
    public static class HelperFactory
    {
        /// <summary> Collection of available (javascript/css) plugins. </summary>
        public static PluginHelper Plugins(this HtmlHelper htmlHelper)
        {
            return new PluginHelper(htmlHelper);
        }

        public static LibraryHelper Libraries(this HtmlHelper htmlHelper)
        {
            return new LibraryHelper(htmlHelper);
        }
    }
}