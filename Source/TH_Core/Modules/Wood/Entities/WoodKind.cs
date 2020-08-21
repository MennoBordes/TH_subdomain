using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Modules.Wood.Entities
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

        [DbColumn("Price_m")]
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

        public enum WoodType
        {
            Meranti = 0,
            Mahonie = 1,
            Other = 2
        }
    }
}
