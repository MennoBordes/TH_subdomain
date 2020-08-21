using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Modules.Wood.Entities
{
    [DbTable("wood")]
    public class Wood: Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Wood_Kind")]
        public int? IdWoodKind { get; set; }

        [DbColumn("Id_Wood_Glass_Slat")]
        public int? IdWoodGlassSlat { get; set; }

        [DbColumn("Id_Wood_Paint_Color")]
        public int? IdWoodPaintColor { get; set; }

        //=== Helpers
        public WoodKind WoodKind { get; set; }

        public WoodGlassSlat WoodGlassSlat { get; set; }

        public WoodPaintColor WoodPaintColor { get; set; }

        //=== Helper functions
        public decimal GetPrice()
        {
            decimal totalPrice = 0m;

            if (WoodKind != null) totalPrice += WoodKind.GetPrice();
            if (WoodGlassSlat != null) totalPrice += WoodGlassSlat.GetPrice();
            if (WoodPaintColor != null) totalPrice += WoodPaintColor.GetPrice();

            return totalPrice;
        }
    }
}
