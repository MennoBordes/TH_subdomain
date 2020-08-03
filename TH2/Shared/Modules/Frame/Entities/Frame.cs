using System;
using System.Collections.Generic;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Frame.Entities
{
    using Connection.Entities;
    using Glass.Entities;
    using Door.Entities;
    using Window.Entities;

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

        //=== Helper functions

        public decimal GetPrice()
        {
            decimal totalPrice = PriceBase;
            if (Connection != null) totalPrice += Connection.GetPrice();
            if (FrameSill != null) totalPrice += FrameSill.GetPrice();
            if (Glass != null) totalPrice += Glass.GetPrice();
            if (Window != null) totalPrice += Window.GetPrice();
            if (Door != null) totalPrice += Door.GetPrice();

            return totalPrice;
        }
    }
}
