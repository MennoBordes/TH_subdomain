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
        
        /// <summary> View: render content. </summary>
        public ActionResult Content()
        {
            List<People> peoples = pMan.GetPeoples();

            ViewData["peoples"] = peoples;
            return View(views + "_Content.cshtml");
        }

        /// <summary> View: used to render the create new person view. </summary>
        [ActionName("create-person")]
        public ActionResult Create()
        {
            return View(views + "/Components/_CreateNew.cshtml");
        }

        /// <summary> Save new person. </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="email">The email address.</param>
        /// <param name="birthDate">The birth date.</param>
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

        /// <summary> Render edit view. </summary>
        /// <param name="Id">The id of the person to render the view for.</param>
        [ActionName("edit-person")]
        public ActionResult EditPerson(int Id)
        {
            People person = pMan.GetPerson(Id);

            return View(views + "/Components/_Edit.cshtml", person);
        }

        /// <summary> Save the editted person. </summary>
        /// <param name="people">The updated person.</param>
        [ActionName("save-edit-person")]
        [HttpPost]
        public ActionResult SaveEditPerson(People people)
        {
            pMan.SavePerson(people);

            return Ok();
        }

        /// <summary> Render details view. </summary>
        [ActionName("get-details")]
        public ActionResult Details(int Id)
        {
            People person = pMan.GetPerson(Id);

            return View(views + "/Components/_Details.cshtml", person);
        }

        /// <summary> Delete person </summary>
        [ActionName("delete-person")]
        [HttpPost]
        public ActionResult DeletePerson(int Id)
        {
            if (Id < 1)
                return Json(new { message = "Invalid person!" });

            pMan.DeletePerson(Id);

            return Json(new { message = "Succesfully deleted" });
        }
    }
}