using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Wood.Entities
{
    [DbTable("wood_paint_color")]
    public class WoodPaintColor : Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn]
        public string Name { get; set; }

        [DbColumn("Price_m")]
        public decimal Price { get; set; }

        //=== Helper functions

        public decimal GetPrice()
        {
            return Price;
        }
    }
}
