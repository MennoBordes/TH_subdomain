using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Glass.Entities
{
    [DbTable("glass_ventilation")]
    public class GlassVentilation: Entity
    {
        /// <summary> The width for the base price. </summary>
        public const int BaseWidth = 400;
        /// <summary> The division for the width </summary>
        public const int BaseDivision = 100;

        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        /// <summary> Width in mm. </summary>
        [DbColumn]
        public int Width { get; set; }

        /// <summary> Height in mm. </summary>
        [DbColumn]
        public int Height { get; set; }

        [DbColumn]
        public string Color { get; set; }

        /// <summary> The default price. </summary>
        [DbColumn("Price_Base")]
        public decimal PriceBase { get; set; }

        /// <summary> Price for each 10 cm Width over 40cm.  </summary>
        [DbColumn("Price_m")]
        public decimal Price { get; set; }


        //=== Helper functions
        /// <summary> Calculates the price for the ventilation. </summary>
        /// <returns></returns>
        public decimal GetPrice()
        {
            // Calculate width without base width
            int withoutBase = Width - BaseWidth;


            if (withoutBase > 0)
            {
                int timesGreater = withoutBase / BaseDivision;
                int remainder = withoutBase % BaseDivision;

                if (remainder > 0)
                    timesGreater++;

                return PriceBase + (Price * timesGreater);
            }
            else
            {
                return PriceBase;
            }
        }
    }
}
