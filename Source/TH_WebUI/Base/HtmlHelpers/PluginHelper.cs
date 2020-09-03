using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TH.WebUI.Base.HtmlHelpers
{
    using System.Web.Mvc;
    public class PluginHelper
    {

        private HtmlHelper htmlHelper;
        private UrlHelper urlHelper;

        public PluginHelper(HtmlHelper helper)
        {
            this.htmlHelper = helper;
            this.urlHelper = new UrlHelper(this.htmlHelper.ViewContext.RequestContext);
        }

        /// <summary> Renders 'Moment' script reference(s). 
        /// <para> Version: 2.12.0 </para>
        /// <para> http://momentjs.com/ </para>
        /// </summary>
        public MvcHtmlString Moment()
        {
            string ref1 = "<script type=\"text/javascript\" src=\"" + urlHelper.Content("~/Resources/Plugins/momentjs-2.12.0/moment-with-locales.min.js") + "\"></script>";

            return new MvcHtmlString(ref1);
        }

        public MvcHtmlString Select2()
        {
            string ref1 = "<link rel=\"stylesheet\" href=\"" + urlHelper.Content("~/resources/Plugins/Select2/Select2.css") + "\" />";
            string ref2 = "<script type=\"text/javascript\" src=\"" + urlHelper.Content("~/resources/Plugins/Select2/select2.min.js") + "\"></script>";

            return new MvcHtmlString(ref1 + ref2);
        }
    }
}