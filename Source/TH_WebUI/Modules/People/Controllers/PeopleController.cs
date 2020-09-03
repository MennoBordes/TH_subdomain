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
    using TH.Core.Tools.Form.Models;

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

        [ActionName("create-person-form")]
        public ActionResult CreateForm()
        {
            Form form = GetCreatePeopleForm();

            ViewData["Form"] = form;
            return View(views + "/Components/_CreateNewForm.cshtml");
        }

        [ActionName("save-person-form")]
        public ActionResult SavePersonForm(FormPost postData)
        {
            Form form = this.GetCreatePeopleForm();
            form.InjectFormData(postData.Data);

            FormValidator fv = new FormValidator();
            if (!fv.Validate(form))
            {
                return Json(new { success = false, message = string.Join("<br/>", fv.Results.Select(x => x.Message)) });
            }

            People person = new People();
            person.FirstName = form.Element<Input>(1).Value;
            person.LastName = form.Element<Input>(2).Value;
            var res = form.Element<Calendar>(3).Value;
            person.EmailAddress = form.Element<Input>(4).Value;


            return Json(new { success = true, message = "saved person" });
        }

        private Form GetCreatePeopleForm()
        {
            Form form = new Form();
            form.Name = "create-person-form";
            form.Blocks = new List<Block>();

            Block block = new Block(Core.Tools.Form.Enums.FormColumnLayout.Split2);
            form.Blocks.Add(block);

            Input i1 = Input.Init(id: 1, label: "First Name", required: true, col: 1);
            block.AddFormElement(i1);

            Input i2 = Input.Init(id: 2, label: "Last Name", required: false, col: 2);
            block.AddFormElement(i2);

            Calendar c1 = Calendar.Init(id: 3, label: "Birth Date", required: true, col: 1);
            //block.AddFormElement(c1);

            Input i3 = Input.Init(id: 4, label: "Email Address", required: true, col: 2);
            block.AddFormElement(i3);

            RadiobuttonItem ri1 = new RadiobuttonItem() { Id = 1, Label = "r1 label", Value = "r1", Selected = true };
            RadiobuttonItem ri2 = new RadiobuttonItem() { Id = 2, Label = "r2 label", Value = "r2", Selected = false };
            List<RadiobuttonItem> rbiList = new List<RadiobuttonItem>();
            rbiList.Add(ri1);
            rbiList.Add(ri2);

            Radiobutton r1 = Radiobutton.Init(id: 5, label: "radio button test", items: rbiList, col: 1);
            block.AddFormElement(r1);

            CheckboxItem ch1 = new CheckboxItem() { Id = 1, Label = "ch1 label", Value = "ch1", Selected = true };
            CheckboxItem ch2 = new CheckboxItem() { Id = 2, Label = "ch2 label", Value = "ch2", Selected = false };

            Checkbox cb1 = Checkbox.Init(id: 6, label: "checkbox switch test", @switch: true);
            block.AddFormElement(cb1);

            Checkbox cb2 = Checkbox.Init(id: 7, label: "checkbox test");
            block.AddFormElement(cb2);

            cb2.Items.Add(new CheckboxItem { Id = 1, Value = bool.TrueString });
            cb2.Items.Add(new CheckboxItem { Id = 2, Value = bool.FalseString });

            Dropdown d1 = Dropdown.Init(id: 8, label: "Dropdown test", col: 2);
            block.AddFormElement(d1);
            d1.Items.Add(new DropdownItem()
            {
                Id = 1,
                Value = "d1",
                Label = "dropdown 1"
            });
            d1.Items.Add(new DropdownItem()
            {
                Id = 2,
                Value = "d2",
                Label = "dropdown 2"
            });
            d1.Items.Add(new DropdownItem()
            {
                Id = 3,
                Value = "d3",
                Label = "dropdown 3"
            });

            return form;
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

            return Json(new { message = "Saved new person." });
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

            return Json(new { success = true, message = "Successfully saved." });
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
                return Json(new { success = false, message = "Invalid person!" });

            pMan.DeletePerson(Id);

            return Json(new { success = true, message = "Succesfully deleted" });
        }
    }
}