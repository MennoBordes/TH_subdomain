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
        
        public ActionResult Content()        
        {
            //PeopleManager pMan = new PeopleManager();

            List<People> peoples = pMan.GetPeoples();

            ViewData["peoples"] = peoples;
            return View(views + "_Content.cshtml");
        }

        [ActionName("create-person")]
        public ActionResult Create()
        {
            return View(views + "/Components/_CreateNew.cshtml");
        }

        [ActionName("save-create-person")]
        [HttpPost]
        public ActionResult Create(string firstName, string lastName, string email, DateTime birthDate)
        {
            People people = new People()
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = email,
                DateOfBirth = birthDate
            };
            pMan.SavePerson(people);
            //return RedirectToAction("Index");
            return Json(new { message = "Saved new person" });
        }

        public ActionResult Edit(int Id)
        {
            People person = pMan.GetPerson(Id);

            return View(views + "/Components/_Edit.cshtml", person);
        }

        [ActionName("edit-person")]
        public ActionResult EditPerson(int Id)
        {
            People person = pMan.GetPerson(Id);

            return View(views + "/Components/_Edit.cshtml", person);
        }

        [ActionName("save-edit-person")]
        [HttpPost]
        public ActionResult SaveEditPerson(People people)
        {
            pMan.SavePerson(people);

            return Ok();
        }

        [HttpPost]
        public ActionResult Edit(People person)
        {
            //pMan.SavePerson(person);

            return RedirectToAction("Index");
        }

        [ActionName("get-details")]
        public ActionResult Details(int Id)
        {
            People person = pMan.GetPerson(Id);

            return View(views + "/Components/_Details.cshtml", person);
        }

        [ActionName("delete-person")]
        [HttpPost]
        public ActionResult DeletePerson(int Id)
        {
            pMan.DeletePerson(Id);

            return Json(new { message = "Succesfully deleted" });
        }
    }
}