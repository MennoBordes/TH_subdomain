using System;
using System.Collections.Generic;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Modules.Frame.Entities
{
    using Connection.Entities;
    using Glass.Entities;
    using Door.Entities;
    using Window.Entities;
    using Base.Extensions;

    [DbTable("frame")]
    public class Frame: Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Connection")]
        public int IdConnection { get; set; }

        [DbColumn("Id_Frame_Sill")]
        public int? IdFrameSill { get; set; }

        [DbColumn("Id_Glass")]
        [Obsolete]
        public int? IdGlass { get; set; }

        [DbColumn("Id_Window")]
        public int? IdWindow { get; set; }

        [DbColumn("Id_Door")]
        public int? IdDoor { get; set; }

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

        public Window Window { get; set; }

        public Door Door { get; set; }

        public Connection Connection { get; set; }

        public FrameSill FrameSill { get; set; }

        [Obsolete]
        public Glass Glass { get; set; }

        public List<FrameGlass> FrameGlasses { get; set; }

        public List<FrameWood> FrameWoods { get; set; }

        //=== Helper functions

        public decimal GetPrice()
        {
            decimal totalPrice = PriceBase;

            totalPrice += GetGlassPrice();

            totalPrice += GetWoodPrice();

            if (Connection != null) totalPrice += Connection.GetPrice();

            if (FrameSill != null) totalPrice += FrameSill.GetPrice();

            if (Window != null) totalPrice += Window.GetPrice();

            if (Door != null) totalPrice += Door.GetPrice();

            return totalPrice;
        }

        public decimal GetGlassPrice()
        {
            decimal price = 0m;

            if (FrameWoods.IsNullOrEmpty())
                return price;

            foreach (FrameGlass frameGlass in FrameGlasses)
            {
                // TODO
                // price += frameGlass.GetPrice();
            }

            return price;
        } 
        public decimal GetWoodPrice()
        {
            decimal price = 0m;

            if (FrameWoods.IsNullOrEmpty())
                return price;

            foreach (FrameWood frameWood in FrameWoods)
            {
                // TODO
                // price += frameWood.GetPrice();
            }

            return price;
        }
    }
}
