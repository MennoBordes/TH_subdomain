using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TH.Server.Database;
using TH.Server.Database.DataRetrieval;
using TH.Shared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TH.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ThDbEntities _context;

        public OrderController(ThDbEntities context)
        {
            _context = context;
        }

        // GET: api/<OrderController>
        [HttpGet("GetOrders")]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            return await _context.Order.ToListAsync();
        }

        // Post: api/<OrderController>
        [HttpPost("GetOrders")]
        public async Task<ActionResult<List<Order>>> GetOrders([FromBody] PaginationSpecifier p)
        {
            return await _context.Order.Skip(p.PageSize * p.CurrentPage).Take(p.PageSize).ToListAsync();
        }

        // GET api/<OrderController>/5
        [HttpGet("GetOrder/{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            Order order = await _context.Order.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST api/<OrderController>
        [HttpPost("AddOrder")]
        public async Task<ActionResult<Order>> AddOrder([FromBody] Order order)
        {
            order.CreatedDate = DateTime.Now;
            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }
    }
}
