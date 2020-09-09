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
        public List<Order> GetOrders(bool orderByDecending = true)
        {
            XQuery q = new XQuery()
                .From<Order>()
                .Where()
                    .Column<Order>(x => x.Id).GreaterThan().Value(0);

            if (orderByDecending) q.OrderByDescending<Order>(x => x.Id);

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

        public int SaveNewOrder(Order order)
        {
            // Check
            if (order == null)
                throw new CoreException("No Order Specified!");

            // Insert
            order.Id = SaveOrder(order);  // repository.Insert(order).InsertId.Value;

            foreach (OrderData data in order.OrderDatas)
            {
                this.SaveOrderData(data);
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

        public List<OrderData> GetOrderData(int[] ids = null, int[] orderIds = null)
        {
            if (ids != null && orderIds != null)
                throw new CoreException("No Ids specified!");

            XQuery q = new XQuery()
                .From<OrderData>()
                .Where();

            bool whereset = false;
            if (ids != null)
            {
                q.Column<OrderData>(x => x.Id).In(ids);
                whereset = true;
            }

            if (orderIds != null)
            {
                if (whereset) q.And();
                q.Column<OrderData>(x => x.IdOrder).In(orderIds);
            }

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
            //orderData.IdOrder = SaveOrder(orderData.Order);

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

        //=== Mergers

        /// <summary> Adds OrderData into the orders. </summary>
        /// <param name="orders">The orders to add Data into.</param>
        public void MergeOrderDataIntoOrder(List<Order> orders)
        {
            if (orders.IsNullOrEmpty())
                return;

            // Get Data
            List<OrderData> datas = GetOrderData(orderIds: orders.Select(x => x.Id).ToArray());
            if (datas.IsNullOrEmpty())
                return;

            foreach (Order order in orders)
            {
                if (!datas.Any(x => x.IdOrder == order.Id))
                    continue;

                order.OrderDatas = datas.Where(x => x.IdOrder == order.Id).ToList();
            }
        }

        /// <summary> Merge orderData into orders. </summary>
        public void MergeAllIntoOrder(List<Order> orders)
        {
            if (orders.IsNullOrEmpty())
                return;

            MergeOrderDataIntoOrder(orders);

            List<OrderData> orderDatas = orders.Where(x => x.OrderDatas != null).SelectMany(x => x.OrderDatas).ToList();

            if (orderDatas.IsNullOrEmpty())
                return;

            MergeAllIntoOrderData(orderDatas);
        }

        /// <summary> Merge childs into orderdata. </summary>
        public void MergeAllIntoOrderData(List<OrderData> orderDatas)
        {
            if (orderDatas.IsNullOrEmpty())
                return;

            MergeDoorsIntoOrderData(orderDatas);
            MergeWindowsIntoOrderData(orderDatas);
            MergeFramesIntoOrderData(orderDatas);
        }

        /// <summary> Merge door into orderdata. </summary>
        public void MergeDoorsIntoOrderData(List<OrderData> orderDatas)
        {
            if (orderDatas.IsNullOrEmpty())
                return;

            DoorManager dMan = new DoorManager();
            
            // Single DB query
            //// Get Doors for orders
            //int[] doorIds = orderDatas.Select(x => x.IdDoor).Where(x => x.HasValue).Select(x => x.Value).ToArray();
            //List<Door> doors = dMan.GetDoor(doorIds);

            //// Merge data into doors
            //dMan.MergeAllIntoDoors(doors);

            //// Merge doors into orderdata
            //foreach (OrderData data in orderDatas)
            //{
            //    if (!doors.Any(x => x.Id == data.IdDoor))
            //        continue;

            //    data.Door = doors.FirstOrDefault(x => x.Id == data.IdDoor);
            //}

            // Multiple DB queries
            foreach (OrderData data in orderDatas)
            {
                if (!data.IdDoor.HasValue || data.IdDoor.Value <= 0)
                    continue;

                data.Door = dMan.GetDoor(data.IdDoor.Value, kind: true, connection: true, glass: true);
            }
        }

        /// <summary> Merge window into orderdata. </summary>
        public void MergeWindowsIntoOrderData(List<OrderData> orderDatas)
        {
            if (orderDatas.IsNullOrEmpty())
                return;

            WindowManager wMan = new WindowManager();

            foreach (OrderData data in orderDatas)
            {
                if (!data.IdWindow.HasValue || data.IdWindow.Value <= 0)
                    continue;

                data.Window = wMan.GetWindow(data.IdWindow.Value, kind: true, connection: true, glass: true);
            }
        }

        /// <summary> Merge frame into orderdata. </summary>
        public void MergeFramesIntoOrderData(List<OrderData> orderDatas)
        {
            if (orderDatas.IsNullOrEmpty())
                return;

            FrameManager fMan = new FrameManager();

            foreach (OrderData data in orderDatas)
            {
                if (!data.IdFrame.HasValue || data.IdFrame.Value <= 0)
                    continue;

                data.Frame = fMan.GetFrame(data.IdFrame.Value, sill: true, connection: true, glass: true);
            }
        }
    }
}
