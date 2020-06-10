using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Calculator.Base.HtmlHelpers
{
    public static class ResourceFactory
    {
        public static MvcHtmlString THScript(this HtmlHelper htmlHelper, string name)
        {
            // Check Null
            if (string.IsNullOrWhiteSpace(name)) return new MvcHtmlString(string.Empty);

            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            string uri = urlHelper.Content("~/resources/scripts/th." + name + ".js");

            string reference = "<script type=\"text/javascript\" src=\"" + uri + "\"></script>";

            return new MvcHtmlString(reference);
        }
    }
}