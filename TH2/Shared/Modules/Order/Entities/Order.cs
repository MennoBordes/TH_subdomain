using System;
using System.Collections.Generic;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Order.Entities
{
    [DbTable("order")]
    public class Order: Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Project_Name")]
        public string ProjectName { get; set; }

        [DbColumn("Created_Date")]
        public DateTime CreationDate { get; set; }

        // Helpers

        public List<OrderData> OrderDatas { get; set; }
    }
}
