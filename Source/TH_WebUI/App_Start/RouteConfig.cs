using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TH.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region Modules

            routes.MapRoute(
                name: "modules/people",
                url: "modules/people/{action}",
                defaults: new { controller = "People" },
                namespaces: new string[] { "TH.WebUI.Modules.People.Controllers" }
            );

            routes.MapRoute(
                name: "modules/order",
                url: "modules/order/{action}",
                defaults: new { controller = "Order" },
                namespaces: new string[] { "TH.WebUI.Modules.Order.Controllers" }
            );

            #endregion


            #region Info

            routes.MapRoute(
                name: "pages",
                url: "pages/{pageName}",
                defaults: new { controller = "Page", action = "ShowPage" },
                namespaces: new string[] { "TH.WebUI.Base.Controllers" }
            );

            #endregion


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
