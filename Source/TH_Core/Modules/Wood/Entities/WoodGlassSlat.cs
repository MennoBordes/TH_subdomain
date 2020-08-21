using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Modules.Wood.Entities
{
    [DbTable("wood_glass_slat")]
    public class WoodGlassSlat : Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Wood_Kind")]
        public int? IdWoodKind { get; set; }

        /// <summary> Length in mm. </summary>
        [DbColumn]
        public int Length { get; set; }

        [DbColumn("Price_m")]
        public decimal Price { get; set; }

        //=== Helpers
        public WoodKind WoodKind { get; set; }

        //=== Helper functions

        public decimal GetPrice()
        {
            decimal totalPrice = Price;

            if (WoodKind != null) totalPrice += WoodKind.GetPrice();

            return totalPrice;
        }
    }
}
