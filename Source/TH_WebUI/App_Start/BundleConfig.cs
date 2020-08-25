using System.Web;
using System.Web.Optimization;

namespace TH.WebUI
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Resources/Libraries/jquery-3.4.1/jquery-{version}.js",
                        "~/Resources/Libraries/jquery-3.4.1/jquery-3.4.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Resources/Libraries/jquery-validate/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Resources/Libraries/modernizr-2.8.3/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Resources/Libraries/bootstrap-3.4.1/js/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/th").Include(
                      "~/Resources/Scripts/th.feedback.js",
                      "~/Resources/Scripts/th.js",
                      "~/Resources/Scripts/th.dialog.js"));

            bundles.Add(new StyleBundle("~/Styles/th/css").Include(
                      "~/Resources/Styles/th.ui.css",
                      "~/Resources/Styles/th.cards.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Resources/Libraries/bootstrap-3.4.1/css/bootstrap.css",
                      "~/Resources/Styles/site.css"));
        }
    }
}
