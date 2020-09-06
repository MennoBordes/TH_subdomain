using System.Web.Mvc;

namespace TH.WebUI.Base.HtmlHelpers
{
    public class LibraryHelper
    {
        private const string FORMAT_SCRIPT = "<script type=\"text/javascript\" src=\"{0}\"></script>";
        private const string FORMAT_STYLE = "<link rel=\"stylesheet\" href=\"{0}\" />";

        private HtmlHelper htmlHelper;
        private UrlHelper urlHelper;

        public LibraryHelper(HtmlHelper helper)
        {
            this.htmlHelper = helper;
            this.urlHelper = new UrlHelper(this.htmlHelper.ViewContext.RequestContext);
        }

        public MvcHtmlString Bootstrap()
        {
            string ref1 = Style("~/resources/Libraries/bootstrap-3.4.1/css/bootstrap.min.css");
            string ref2 = Script("~/Resources/Libraries/bootstrap-3.4.1/js/bootstrap.js");

            return new MvcHtmlString(ref1 + ref2);
        }

        public MvcHtmlString JQuery()
        {
            string ref1 = Script("~/Resources/Libraries/jquery-3.4.1/jquery-3.4.1.js");

            return new MvcHtmlString(ref1);
        }

        public MvcHtmlString Modernizr()
        {
            string ref1 = Script("~/Resources/Libraries/modernizr-2.8.3/modernizr-2.8.3.js");

            return new MvcHtmlString(ref1);
        }

        /// <summary> Script reference. </summary>
        private string Script(string url)
        {
            return string.Format(FORMAT_SCRIPT, urlHelper.Content(url));
        }

        /// <summary> Style reference. </summary>
        private string Style(string url)
        {
            return string.Format(FORMAT_STYLE, urlHelper.Content(url));
        }
    }
}