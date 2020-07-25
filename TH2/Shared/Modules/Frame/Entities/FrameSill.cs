using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Frame.Entities
{
    [DbTable("frame_sill")]
    public class FrameSill: Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn]
        public int Type { get; set; }

        [DbColumn("Price_Base")]
        public decimal PriceBase { get; set; }

        [DbColumn("Price_m")]
        public decimal Price { get; set; }

        //=== Helper functions
        public decimal GetPrice()
        {
            return Price + PriceBase;
        }


        public enum FrameSillType
        {
            Paneel_Plaat = 0,
            Paneel_Glas = 1,
            StapelDorpel_Glas = 2
        }
    }
}
