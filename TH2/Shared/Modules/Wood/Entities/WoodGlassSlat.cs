using System;
using System.Collections.Generic;
using System.Text;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Wood.Entities
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
