//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TH.Server.Base.Database;
//using TH.Server.Base.Database.DataRetrieval;
//using TH.Server.Modules.Orders;
//using TH.Shared.Models;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace TH.Server.Modules.Orders.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class OrderController : ControllerBase
//    {
//        private readonly ThDbEntities _context;

//        public OrderController(ThDbEntities context)
//        {
//            _context = context;
//            oMan = new OrderManager(_context);
//        }

//        OrderManager oMan;

//        #region HttpGet's

//        // GET: api/<OrderController>
//        [HttpPost("GetLatestOrders")]
//        public async Task<ActionResult<List<Order>>> GetOrders([FromBody] int amount)
//        {
//            // Check
//            if (amount < 1) return null;

//            return await oMan.GetLatestOrders(amount);
//        }

//        // GET api/<OrderController>/5
//        [HttpGet("GetOrder/{id}")]
//        public async Task<ActionResult<Order>> GetOrder(int id)
//        {
//            Order order = await oMan.GetOrder(id);

//            if (order == null)
//            {
//                return NotFound();
//            }

//            return order;
//        }

//        [HttpGet("GetFullOrder/{id}")]
//        public async Task<ActionResult<Order>> GetFullOrder(int id)
//        {
//            // Get the order
//            Order order = await oMan.GetOrder(id, data: true);            

//            if (order == null)
//            {
//                return NotFound();
//            }

//            //order.OrderFrame = null;

//            //// Merge kozijnen into order
//            //oMan.MergeDataIntoOrders(new List<Order>() { order });            

//            return order;
//        }

//        #endregion
//        #region HttpPost's
//        // Post: api/<OrderController>
//        [HttpPost("GetOrders")]
//        public async Task<ActionResult<List<Order>>> GetOrders([FromBody] PaginationSpecifier p)
//        {
//            return await _context.Order.Skip(p.PageSize * p.CurrentPage).Take(p.PageSize).ToListAsync();
//        }

//        // POST api/<OrderController>
//        [HttpPost("AddOrder")]
//        public async Task<ActionResult<Order>> AddOrder([FromBody] Order order)
//        {
//            // Ensure Id is 0
//            order.Id = 0;
//            // Set creationDate to current time
//            order.CreatedDate = DateTime.Now;
//            _context.Order.Add(order);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
//        }

//        #endregion
//    }
//}
