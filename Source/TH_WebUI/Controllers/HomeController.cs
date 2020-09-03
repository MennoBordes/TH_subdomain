using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TH.WebUI.Controllers
{
    using TH.Core.Modules.People;
    using TH.Core.Modules.People.Entities;
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            PeopleManager pman = new PeopleManager();
            List<People> peoples = pman.GetPeoples();
            ViewData["peoples"] = peoples;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}