﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TH.WebUI.Base.HtmlHelpers
{
    using System.Web.Mvc;
    public class PluginHelper
    {
        private const string FORMAT_SCRIPT = "<script type=\"text/javascript\" src=\"{0}\"></script>";
        private const string FORMAT_STYLE = "<link rel=\"stylesheet\" href=\"{0}\" />";

        private HtmlHelper htmlHelper;
        private UrlHelper urlHelper;

        public PluginHelper(HtmlHelper helper)
        {
            this.htmlHelper = helper;
            this.urlHelper = new UrlHelper(this.htmlHelper.ViewContext.RequestContext);
        }

        public MvcHtmlString MomentJS()
        {
            string ref1 = Script("~/Resources/Plugins/momentjs-2.12.0/moment.min.js");
            string ref2 = Script("~/Resources/Plugins/momentjs-2.12.0/moment-with-locales.min.js");

            return new MvcHtmlString(ref1 + ref2);
        }

        public MvcHtmlString Select2()
        {
            string ref1 = Style("~/resources/Plugins/Select2/Select2.css");
            string ref2 = Script("~/resources/Plugins/Select2/select2.min.js");

            return new MvcHtmlString(ref1 + ref2);
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