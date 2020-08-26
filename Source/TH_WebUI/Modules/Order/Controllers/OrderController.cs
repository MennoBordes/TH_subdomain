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

        [ActionName("order-details")]
        public ActionResult OrderDetails(int id)
        {
            Order order = oMan.GetOrder(id);

            oMan.MergeOrderDataIntoOrder(new List<Order>() { order });
            oMan.MergeAllIntoOrderData(new List<Order>() { order });

            ViewData["order"] = order;
            return View(views + "_OrderDetails.cshtml");
        }

    }
}