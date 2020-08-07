using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TH2.Server.Modules.Order
{
    using Shared.Modules.Order;
    using Shared.Modules.Order.Entities;
    using Shared.Modules.Order.Getter;

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        OrderManager oMan;
        public OrderController()
        {
            oMan = new OrderManager();
        }

        // GET: api/<OrderController>
        [HttpGet]
        public Order GetOrder([FromBody] JToken json)
        {
            /*  Request 
                {
                    id: 1,
                    data: false
                }
             */

            if (json["id"] == null)
                return null;

            int id = json.Value<int>("id");
            bool data = json.Value<bool>("data");

            Order order = oMan.GetOrder(id);
            if (order == null) return order;

            if (data)
                oMan.MergeOrderDataIntoOrder(orders: new List<Order>() { order });

            return order;
        }

        /// <summary> Get OrderDatas. </summary>
        public List<OrderData> GetOrderDatas([FromBody] GetOrderDatas getDatas)
        {
            if (getDatas.UseOrderId && getDatas.OrderId.HasValue)
            {
                List<OrderData> orderIds = oMan.GetOrderData(getDatas.OrderId.Value);
                return orderIds;
            }

            if (getDatas.Ids.Length > 1)
            {
                List<OrderData> ids = oMan.GetOrderData(ids: getDatas.Ids);
                return ids;
            }

            return null;
        }
    }
}
