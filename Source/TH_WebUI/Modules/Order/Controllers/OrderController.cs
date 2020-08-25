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

        [ActionName("get-orders")]
        public ActionResult GetOrders()
        {
            List<Order> orders = oMan.GetOrders();

            oMan.MergeOrderDataIntoOrder(orders);

            return Json(new { orders = orders});
        }

    }
}