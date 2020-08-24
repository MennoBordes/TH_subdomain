using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TH.WebUI.Base.HtmlHelpers
{
    /// <summary> Html Resource factory. </summary>
    public static class ResourceFactory
    {
        public static MvcHtmlString THScript(this HtmlHelper htmlHelper, string name)
        {
            // Check null
            if (string.IsNullOrWhiteSpace(name)) return new MvcHtmlString(string.Empty);

            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            string uri = urlHelper.Content("~/resources/scripts/" + name + ".js");

            string reference = "<script type=\"text/javascript\" scr=\"" + uri + "\"></script>";

            return new MvcHtmlString(reference);
        }

        public static MvcHtmlString THStyle(this HtmlHelper htmlHelper, string name)
        {
            // Check null
            if (string.IsNullOrWhiteSpace(name))
                return new MvcHtmlString(string.Empty);

            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            string uri = urlHelper.Content("~/resources/styles/" + name + ".css");

            string reference = "<link rel=\"stylesheet\" href=\"" + uri + "\" />";

            return new MvcHtmlString(reference);
        }
    }
}