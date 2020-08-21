using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Modules.Glass.Entities
{
    [DbTable("Glass")]
    public class Glass: Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Glass_Ventilation")]
        public int? IdGlassVentilation { get; set; }

        [DbColumn]
        public int Type { get; set; }

        /// <summary> Width in mm. </summary>
        [DbColumn]
        public int Width { get; set; }

        /// <summary> Height in mm. </summary>
        [DbColumn]
        public int Height { get; set; }

        /// <summary> Thickness in mm. </summary>
        [DbColumn]
        public int Thickness { get; set; }

        [DbColumn]
        public decimal Price { get; set; }

        //=== Helpers

        public GlassVentilation GlassVentilation { get; set; }

        //=== Helper functions
        
        /// <summary> Gets the price of Glass and ventilation, when available. </summary>
        public decimal GetPrice()
        {
            if (GlassVentilation != null) return Price + GlassVentilation.GetPrice();
            return Price;
        }

        public enum GlassType
        {
            Gelaagd = 0,
            HRpp = 1
        }
    }
}
