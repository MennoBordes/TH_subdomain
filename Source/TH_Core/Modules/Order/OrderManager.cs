using System;
using System.Collections.Generic;
using System.Text;

namespace TH.Core.Modules.Order
{
    using Base.Database;
    using Base.Exceptions;
    using Entities;
    using Connection;
    using Glass;
    using Window;
    using Door;
    using Frame;
    using THTools.ORM;
    using System.Linq;
    using Base.Extensions;

    /// <summary> A manager for all order related actions. </summary>
    public class OrderManager
    {
        private Repository repository;

        public Repository Repository { get { return this.repository; } }

        public OrderManager()
        {
            this.repository = new Repository();
        }

        WindowManager wMan = new WindowManager();
        DoorManager dMan = new DoorManager();
        FrameManager fMan = new FrameManager();

        //=== Manage: Order

        /// <summary> Get all orders. </summary>
        public List<Order> GetOrders()
        {
            XQuery q = new XQuery()
                .From<Order>()
                .Where()
                    .Column<Order>(x => x.Id).GreaterThan().Value(0);

            List<Order> orders = repository.GetEntities<Order>(q).ToList();

            return orders;
        }

        /// <summary> Get Order. </summary>
        public Order GetOrder(int id)
        {
            if (id < 1)
                throw new CoreException("No Id Specified!");

            Order order = repository.GetEntity<Order>(id);

            return order;
        }

        /// <summary> Adds OrderData into the orders. </summary>
        /// <param name="orders">The orders to add Data into.</param>
        public void MergeOrderDataIntoOrder(List<Order> orders)
        {
            if (orders.IsNullOrEmpty())
                return;

            // Get Data
            List<OrderData> datas = GetOrderData(ids: orders.Select(x => x.Id).ToArray());
            if (datas.IsNullOrEmpty())
                return;

            foreach(Order order in orders)
            {
                if (!datas.Any(x => x.IdOrder == order.Id)) continue;

                order.OrderDatas = datas.Where(x => x.IdOrder == order.Id).ToList();
            }
        }

        /// <summary>Save or update a order. </summary>
        /// <returns>The index in the DB.</returns>
        public int SaveOrder(Order order)
        {
            // Check
            if (order == null)
                throw new CoreException("No Order Specified!");

            // Insert or update
            if (order.Id == 0)
            {
                order.Id = repository.Insert(order).InsertId.Value;
            }
            else
            {
                repository.Update(order);
            }

            return order.Id;
        }


        //=== Manage: Order Data

        /// <summary> Get OrderData. </summary>
        /// <param name="door">Get the Door? Default false.</param>
        /// <param name="window">Get the Window? Default false.</param>
        /// <param name="frame">Get the Frame? Default false.</param>
        public OrderData GetOrderData(int id, bool door = false, bool window = false, bool frame = false)
        {
            if (id < 1)
                throw new CoreException("No Id Specified!");

            OrderData orderData = repository.GetEntity<OrderData>(id);

            if (orderData == null)
                return null;

            if (door && orderData.IdDoor.HasValue)
            {
                orderData.Door = dMan.GetDoor(orderData.IdDoor.Value);
            }

            if (window && orderData.IdWindow.HasValue)
            {
                orderData.Window = wMan.GetWindow(orderData.IdWindow.Value);
            }

            if (frame && orderData.IdFrame.HasValue)
            {
                orderData.Frame = fMan.GetFrame(orderData.IdFrame.Value);
            }

            return orderData;            
        }

        public List<OrderData> GetOrderData(int[] ids)
        {
            if (ids.Length < 1)
                throw new CoreException("No Ids specified!");

            XQuery q = new XQuery()
                .From<OrderData>()
                .Where()
                    .Column<OrderData>(x => x.Id).In(ids);

            return repository.GetEntities<OrderData>(q).ToList();
        }

        /// <summary> Get OrderDatas by orderId. </summary>
        /// <param name="orderId">The id of the order to retrieve data for.</param>
        public List<OrderData> GetOrderData(int orderId)
        {
            if (orderId < 1)
                throw new CoreException("No OrderId Specified!");

            XQuery q = new XQuery()
                .From<OrderData>()
                .Where()
                    .Column<OrderData>(x => x.IdOrder).Equals().Value(orderId);

            return repository.GetEntities<OrderData>(q).ToList();
        }

        /// <summary> Save or update orderData. </summary>
        /// <returns>The index in the DB.</returns>
        public int SaveOrderData(OrderData orderData)
        {
            if (orderData == null)
                throw new CoreException("No OrderData Specified!");

            // Save Order
            orderData.IdOrder = SaveOrder(orderData.Order);

            // Save Window
            if (orderData.Window != null)
            {
                orderData.IdWindow = wMan.SaveWindow(orderData.Window);
            }

            // Save Door
            if (orderData.Door != null)
            {
                orderData.IdDoor = dMan.SaveDoor(orderData.Door);
            }

            // Save Frame
            if (orderData.Frame != null)
            {
                orderData.IdFrame = fMan.SaveFrame(orderData.Frame);
            }

            // Insert or update
            if (orderData.Id == 0)
            {
                orderData.Id = repository.Insert(orderData).InsertId.Value;
            }
            else
            {
                repository.Update(orderData);
            }

            return orderData.Id;
        }
    }
}
