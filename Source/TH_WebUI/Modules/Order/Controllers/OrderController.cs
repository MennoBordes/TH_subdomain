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
            List<Order> orders = oMan.GetOrders(orderByDecending: true);

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
            oMan.MergeAllIntoOrderData(new List<Order>() { order });

            ViewData["order"] = order;
            return View(views + "_OrderDetails.cshtml");
        }

        [ActionName("new-order")]
        public ActionResult CreateNewOrder()
        {
            Order order = new Order();
            return View(views + "Components/_CreateNewOrder.cshtml", order);
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

    }
}