using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TH.WebUI.Modules.People.Controllers
{
    using Base.Controllers;
    using TH.Core.Modules.People;
    using TH.Core.Modules.People.Entities;

    public class PeopleController : CoreController
    {
        private string views = "~/modules/people/views/";
        PeopleManager pMan = new PeopleManager();
        
        [HttpGet]
        public ActionResult Index()        
        {
            //PeopleManager pMan = new PeopleManager();

            List<People> peoples = pMan.GetPeoples();

            ViewData["peoples"] = peoples;
            return View(views + "_Index.cshtml");
        }

        public ActionResult Edit(int Id)
        {
            People person = pMan.GetPerson(Id);
            //PeopleModel pModel = person;

            ViewData["person"] = person;
            return View(views + "_Edit.cshtml", person);
        }

        [HttpPost]
        public ActionResult Edit(People person)
        {
            //pMan.SavePerson(person);

            return RedirectToAction("Index");
        }
    }
}