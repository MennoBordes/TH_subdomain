using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Modules.Order.Entities
{
    using Door.Entities;
    using Window.Entities;
    using Frame.Entities;

    [DbTable("order_data")]
    public class OrderData : Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Order")]
        public int IdOrder { get; set; }

        [DbColumn("Id_Door")]
        public int? IdDoor { get; set; }

        [DbColumn("Id_Window")]
        public int? IdWindow { get; set; }

        [DbColumn("Id_Frame")]
        public int? IdFrame { get; set; }

        [DbColumn("Description")]
        public string Description { get; set; }

        //=== Helpers
        public Order Order { get; set; }

        public Door Door { get; set; }

        public Window Window { get; set; }

        public Frame Frame { get; set; }

        //=== Helper functions
        public decimal GetPrice()
        {
            decimal totalPrice = 0m;

            if (Door != null) totalPrice += Door.GetPrice();
            if (Window != null) totalPrice += Window.GetPrice();
            if (Frame != null) totalPrice += Frame.GetPrice();

            return totalPrice;
        }
    }
}
