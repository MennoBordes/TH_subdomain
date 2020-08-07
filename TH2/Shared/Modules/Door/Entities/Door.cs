using System;
using System.Collections.Generic;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Door.Entities
{
    using Connection.Entities;
    using Glass.Entities;
    using TH2.Shared.Base.Extensions;

    [DbTable("Door")]
    public class Door : Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Connection")]
        public int IdConnection { get; set; }

        [DbColumn("Id_Door_Kind")]
        public int IdDoorKind { get; set; }

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
        public decimal Price { get; set; }

        //=== Helpers

        public DoorKind DoorKind { get; set; }

        public Connection Connection { get; set; }

        [Obsolete]
        public Glass Glass { get; set; }

        public List<DoorWood> DoorWoods { get; set; }

        public List<DoorGlass> DoorGlasses { get; set; }


        //=== Helper functions
        public decimal GetPrice()
        {
            decimal totalPrice = 0m;

            totalPrice += GetWoodPrice();

            totalPrice += GetGlassPrice();

            if (Connection != null) totalPrice += Connection.GetPrice();

            if (DoorKind != null) totalPrice += DoorKind.GetPrice();

            return totalPrice;
        }

        public decimal GetWoodPrice()
        {
            decimal price = 0m;

            if (DoorWoods.IsNullOrEmpty())
                return price;

            foreach (DoorWood doorWood in DoorWoods)
            {
                // TODO
                // price += doorWood.GetPrice();
            }

            return price;
        }

        public decimal GetGlassPrice()
        {
            decimal price = 0m;

            if (DoorGlasses.IsNullOrEmpty())
                return price;

            foreach (DoorGlass doorGlass in DoorGlasses)
            {
                // TODO
                // price += doorGlass.GetPrice();
            }

            return price;
        }
    }
}
