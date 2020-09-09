using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TH.WebUI.Modules.Order.Controllers
{
    using Base.Controllers;
    using TH.Core.Modules.Order;
    using TH.Core.Modules.Order.Entities;
    using TH.Core.Tools.Form.Models;

    public class OrderController : CoreController
    {
        private string views = "~/modules/order/views/";
        OrderManager oMan = new OrderManager();

        public ActionResult Content()
        {
            List<Order> orders = oMan.GetOrders();

            oMan.MergeOrderDataIntoOrder(orders);

            ViewData["orders"] = orders;
            return View(views + "_Content.cshtml");
        }

        [ActionName("order-details")]
        public ActionResult OrderDetails(int id)
        {
            Order order = oMan.GetOrder(id);
            if (order == null)
                return BadRequest();

            oMan.MergeOrderDataIntoOrder(new List<Order>() { order });
            oMan.MergeAllIntoOrder(new List<Order>() { order });

            ViewData["order"] = order;
            return View(views + "_OrderDetails.cshtml");
        }

        [ActionName("save-new-order")]
        public ActionResult SaveCreateNewOrder(Order order)
        {
            if (order == null || order.ProjectName == null)
                return Json(new { success = false, message = "Invalid order!" });

            // Set date to today
            order.CreationDate = DateTime.Now;

            oMan.SaveNewOrder(order);

            return Json(new { success = true, message = "Saved succesfully!" });
        }

        //[ActionName("create-order-form")]
        //public ActionResult CreateOrderForm()
        //{
        //    Form form = GetCreateOrderForm();

        //    ViewData["Form"] = form;
        //    return View(views + "/components/_CreateNewOrderForm.cshtml");
        //}

        /// <summary> Renders the page for creating or editing order. </summary>
        /// <param name="id">Optional id for editing order.</param>
        [ActionName("create-or-update-order")]
        public ActionResult CreateOrder(int? id = null)
        {
            ViewData["order"] = null;
            if (id.HasValue && id.Value > 0)
            {
                Order order = oMan.GetOrder(id.Value);

                oMan.MergeAllIntoOrder(new List<Order>() { order });
                ViewData["order"] = order;
            }
            ViewData["orderId"] = id;

            return View(views + "_CreateOrUpdateOrder.cshtml");
        }

        [ActionName("create-order-form")]
        public ActionResult CreateOrderForm()
        {
            Form form = GetCreateOrderFormNew();

            ViewData["Form"] = form;
            return View(views + "/components/_CreateOrderForm.cshtml");
        }

        [ActionName("save-order")]
        public ActionResult SaveOrder(FormPost postData)
        {
            Form form = this.GetCreateOrderFormNew();
            form.InjectFormData(postData.Data);

            FormValidator fv = new FormValidator();
            if (!fv.Validate(form))
            {
                return Json(new { success = false, message = string.Join("<br/>", fv.Results.Select(x => x.Message)) });
            }

            Order order = new Order();
            order.CreationDate = DateTime.Now;
            order.ProjectName = form.Element<Input>(1).Value;
            order.Description = form.Element<Input>(2).Value;

            order.Id = oMan.SaveOrder(order);

            return Json(new { success = true, order = order, message = "Saved Order" });
        }

        private Form GetCreateOrderFormNew()
        {
            Form form = new Form();
            form.Name = "create-order-form";
            form.Blocks = new List<Block>();

            Block b1 = new Block(Core.Tools.Form.Enums.FormColumnLayout.NoSplit);
            form.Blocks.Add(b1);

            Input f1 = Input.Init(id: 1, label: "Project naam", description: "De naam van het project.", required: true);
            b1.AddFormElement(f1);

            Input f2 = Input.Init(id: 2, label: "Project beschrijving", description: "De beschrijving van het project.", required: true);
            b1.AddFormElement(f2);

            return form;
        }

        private Form GetCreateOrderForm()
        {
            Form form = new Form();
            form.Name = "create-order-form";
            form.Blocks = new List<Block>();

            Block b1 = new Block(Core.Tools.Form.Enums.FormColumnLayout.NoSplit);
            form.Blocks.Add(b1);

            Input f1 = Input.Init(id: 1, label: "Project naam", description: "De naam van het project.", required: true);
            b1.AddFormElement(f1);

            Input f2 = Input.Init(id: 2, label: "Project beschrijving", description: "De beschrijving van het project.", required: true);
            b1.AddFormElement(f2);

            Block b2 = new Block(Core.Tools.Form.Enums.FormColumnLayout.Split3);
            form.Blocks.Add(b2);

            // Button for creating a new Door
            Button f6 = Button.Init(id: 1, label: "Deur", col: 1, type: Core.Tools.Form.Enums.FormButtonType.Alternate);
            b2.AddFormElement(f6);
            
            // Button for creating a new Frame
            Button f7 = Button.Init(id: 2, label: "Kozijn", col: 2, type: Core.Tools.Form.Enums.FormButtonType.Cancel);
            b2.AddFormElement(f7);
            
            // Button for creating a new Window
            Button f8 = Button.Init(id: 3, label: "Raam", col: 3, type: Core.Tools.Form.Enums.FormButtonType.Save);
            b2.AddFormElement(f8);

            //form.SaveButton = Button.Init(id: 11, label: "Save", type: Core.Tools.Form.Enums.FormButtonType.Save);
            return form;
        }

    }
}