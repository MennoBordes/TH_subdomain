using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TH.WebUI.Gateway.Controllers;

namespace TH.WebUI.Base.Controllers
{
    public class PageController : Controller
    {
        // GET: Page
        public ActionResult ShowPage(string pageName)
        {
            try
            {
                return View("~/Base/views/shared/" + pageName + ".cshtml");
            }
            catch
            {
                return new ErrorController().Error_404(this.Response);
            }
        }
    }
}