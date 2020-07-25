using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Door.Entities
{
    using Connection.Entities;
    using Glass.Entities;

    [DbTable("Door")]
    public class Door : Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Connection")]
        public int IdConnection { get; set; }

        [DbColumn("Id_Door_Kind")]
        public int IdDoorKind { get; set; }

        [DbColumn("Id_Glass")]
        public int? IdGlass { get; set; }

        /// <summary> Width in mm. </summary>
        [DbColumn]
        public int Width { get; set; }

        /// <summary> Height in mm. </summary>
        [DbColumn]
        public int Height { get; set; }

        /// <summary> Thickness in mm. </summary>
        [DbColumn]
        public int Thickness { get; set; }

        [DbColumn("Price_Base")]
        public decimal Price { get; set; }

        //=== Helpers
        public DoorKind DoorKind { get; set; }

        public Connection Connection { get; set; }

        public Glass Glass { get; set; }

        //=== Helper functions
        public decimal GetPrice()
        {
            decimal totalPrice = 0m;
            if (Glass != null) totalPrice += Glass.GetPrice();

            if (Connection != null) totalPrice += Connection.GetPrice();

            if (DoorKind != null) totalPrice += DoorKind.GetPrice();

            return totalPrice;
        }
    }
}
