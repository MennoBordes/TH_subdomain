using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Wood.Entities
{
    [DbTable("Wood_Kind")]
    public class WoodKind : Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Wood_Paint_Color")]
        public int? IdWoodPaintColor { get; set; }

        [DbColumn]
        public int Type { get; set; }

        [DbColumn]
        public string Name { get; set; }

        /// <summary> Length of the wood in mm. </summary>
        [DbColumn]
        public int Length { get; set; }

        [DbColumn]
        public decimal Price { get; set; }

        //=== Helpers
        public WoodPaintColor WoodPaintColor { get; set; }

        //=== Helper functions

        public decimal GetPrice()
        {
            decimal totalPrice = Price;

            if(WoodPaintColor != null) totalPrice += WoodPaintColor.GetPrice();

            return totalPrice;
        }
    }
}
