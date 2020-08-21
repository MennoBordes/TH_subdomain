using System;
using System.Collections.Generic;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Modules.Window.Entities
{
    using Base.Extensions;
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
        [Obsolete]
        public int? IdGlass { get; set; }

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

        [Obsolete]
        public Glass Glass { get; set; }

        public List<WindowGlass> WindowGlasses { get; set; }

        public List<WindowWood> WindowWoods { get; set; }

        //=== Helper functions

        public decimal GetPrice()
        {
            decimal totalPrice = PriceBase;

            totalPrice += GetWoodPrice();

            totalPrice += GetGlassPrice();

            if (Connection != null) totalPrice += Connection.GetPrice();

            if (WindowKind != null) totalPrice += WindowKind.GetPrice();

            return totalPrice;
        }

        public decimal GetWoodPrice()
        {
            decimal price = 0m;

            if (WindowWoods.IsNullOrEmpty())
                return price;

            foreach (WindowWood windowWood in WindowWoods)
            {
                // TODO
                // price += windowWood.GetPrice();
            }

            return price;
        }

        public decimal GetGlassPrice()
        {
            decimal price = 0m;

            if (WindowGlasses.IsNullOrEmpty())
                return price;

            foreach (WindowGlass windowGlass in WindowGlasses)
            {
                // TODO
                // price += windowGlass.GetPrice();
            }

            return price;
        }
    }
}
