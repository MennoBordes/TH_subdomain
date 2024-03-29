﻿
namespace TH.WebUI
{
    using TH.Core;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Register Syncfusion license
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzA2ODAyQDMxMzgyZTMyMmUzMGxPYktJNTIrTlhmZVFoNEhpZG9vTDNHV2kydkY0RDN6aHFtUGs2VXlWY289");

            // Code on application startup
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinderConfig.RegisterBinders(ModelBinders.Binders);

            AutoMapperConfiguration.Configure();
        }
    }
}
