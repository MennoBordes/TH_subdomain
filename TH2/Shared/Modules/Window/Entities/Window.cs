using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Window.Entities
{
    using Connection.Entities;
    using Glass.Entities;

    [DbTable("window")]
    public class Window : Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Connection")]
        public int IdConnection { get; set; }

        [DbColumn("Id_Window_Kind")]
        public int IdWindowKind { get; set; }

        [DbColumn("Id_Glass")]
        public int IdGlass { get; set; }

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
        public decimal PriceBase { get; set; }

        //=== Helpers
        public Connection Connection { get; set; }

        public WindowKind WindowKind { get; set; }

        public Glass Glass { get; set; }

        //=== Helper functions

        public decimal GetPrice()
        {
            decimal totalPrice = PriceBase;
            if (Connection != null) totalPrice += Connection.GetPrice();
            if (WindowKind != null) totalPrice += WindowKind.GetPrice();
            if (Glass != null) totalPrice += Glass.GetPrice();

            return totalPrice;
        }
    }
}
