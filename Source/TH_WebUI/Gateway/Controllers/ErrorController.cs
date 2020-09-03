using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TH.WebUI.Gateway.Controllers
{
    public class ErrorController : Controller
    {
        /// <summary> Returns the global/default 404 page.  </summary>
        [ActionName("404")]
        public ActionResult Error_404()
        {
            if (Response != null)
            {
                Response.StatusCode = 404;
            }

            return View("~/Base/Views/Shared/404.cshtml");
        }

        /// <summary> Returns the global/default 404 page. </summary>
        [NonAction]
        public ActionResult Error_404(HttpResponseBase response)
        {
            if (response != null)
            {
                response.StatusCode = 404;
            }

            return View("~/Base/Views/Shared/404.cshtml");
        }

        public ActionResult Generic()
        {
            return View("~/Base/Views/Shared/Error.cshtml");
        }
    }
}