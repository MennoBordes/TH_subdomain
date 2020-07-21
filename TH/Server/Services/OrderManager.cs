using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.DocumentEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH.Server.Base.Database;
using TH.Server.Base.Extensions;
using TH.Shared.Models;

namespace TH.Server.Services
{
    public class OrderManager
    {
        private readonly ThDbEntities _context;

        public OrderManager(ThDbEntities context)
        {
            _context = context;
        }

        /// <summary> Gets a order. </summary>
        /// <param name="id">The id of the order to get.</param>
        /// <param name="data">Whether additional data should be included.</param>
        /// <returns>Returns a order.</returns>
        public async Task<Order> GetOrder(int id, bool data = false)
        {
            // Check
            if (id < 1) return null;

            // Get 
            Order order = await (from o in _context.Set<Order>().Where(x => x.Id == id) select o).FirstOrDefaultAsync();

            if (order == null) return null;

            if (data)
            {
                this.MergeDataIntoOrders(orders: new List<Order>() { order });
                //order.OrderKozijn = await _context.OrderKozijn.Where(x => x.OrderId == order.Id).ToListAsync();
            }

            return order;
        }

        /// <summary> Gets all orders from the specified ids. </summary>
        /// <param name="ids">The orders to retrieve (when left empty, returns all orders).</param>
        /// <returns>Returns a list of orders</returns>
        public async Task<List<Order>> GetOrders(int[] ids = null)
        {
            var query = from o in _context.Set<Order>() select o;

            if (ids != null)
            {
                query = query.Where(t => ids.Contains(t.Id));
            }

            List<Order> orders = await query.ToListAsync();

            return orders;
        }

        /// <summary> Get the latest orders present. </summary>
        /// <param name="amount">The amount of orders to retrieve.</param>
        /// <returns>Returns a list of orders.</returns>
        public async Task<List<Order>> GetLatestOrders(int amount)
        {
            List<Order> Orders = await (from o in _context.Set<Order>().OrderByDescending(x => x.Id).Take(amount) select o).ToListAsync();// _context.Order.TakeLast(amount).ToListAsync();

            return Orders;
        }

        /// <summary> Merges the order kozijn into the order. </summary>
        /// <param name="orders">The orders to add data into</param>
        public async void MergeKozijnIntoOrders(List<Order> orders)
        {
            if (orders.IsNullOrEmpty()) return;

            // Create list of ids
            List<int> ids = orders.Select(x => x.Id).ToList();

            // Fetch data
            List<OrderKozijn> orderKozijnen = await _context.OrderKozijn
                .Where(x => ids.Contains(x.OrderId))
                .ToListAsync();

            // Merge data
            foreach (Order order in orders)
            {
                order.OrderKozijn = orderKozijnen.Where(x => x.OrderId == order.Id).ToList();
            }
        }

        /// <summary> Merges the order data into the order. </summary>
        /// <param name="orders">The orders to add data into.</param>
        public void MergeDataIntoOrders(List<Order> orders)
        {
            MergeKozijnIntoOrders(orders);
        }
    }
}
