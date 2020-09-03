using System;
using System.Collections.Generic;
using System.Linq;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Modules.Order.Entities
{
    [DbTable("order")]
    public class Order: Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Project_Name")]
        public string ProjectName { get; set; }

        [DbColumn("Description")]
        public string Description { get; set; }

        [DbColumn("Created_Date")]
        public DateTime CreationDate { get; set; }

        // Helpers

        public List<OrderData> OrderDatas { get; set; }

        /// <summary> Get Date without time. </summary>
        public string GetDate() 
        {
            return CreationDate.ToShortDateString();    
        }

        /// <summary> Get the amount of doors in this order. </summary>
        public int GetDoorsCount()
        {
            if (OrderDatas == null || OrderDatas.Count < 1)
                return 0;

            int doors = OrderDatas.Where(x => x.IdDoor != null && x.IdDoor > 0).Count();

            return doors;
        }
        /// <summary> Get the amount of Windows in this order. </summary>
        public int GetWindowsCount()
        {
            if (OrderDatas == null || OrderDatas.Count < 1)
                return 0;

            int windows = OrderDatas.Where(x => x.IdWindow != null && x.IdWindow > 0).Count();

            return windows;
        }
        /// <summary> Get the amount of doors in this order. </summary>
        public int GetFrameCount()
        {
            if (OrderDatas == null || OrderDatas.Count < 1)
                return 0;

            int frames = OrderDatas.Where(x => x.IdFrame != null && x.IdFrame > 0).Count();

            return frames;
        }
    }
}
