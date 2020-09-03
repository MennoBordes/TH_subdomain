using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Door.Entities
{
    [DbTable("Door_Kind")]
    public class DoorKind : Entity
    {
        [DbPrimaryKey, DbColumn("Id")]
        public int Id { get; set; }

        [DbColumn("Type")]
        public int Type { get; set; }

        [DbColumn("Turn_Direction")]
        public DoorTurnDir TurnDirection { get; set; }

        [DbColumn("Price")]
        public decimal Price { get; set; }

        /// <summary> Returns the price. </summary>
        public decimal GetPrice()
        {
            return Price;
        }


        public enum DoorTurnDir
        {
            [DbEnumValue("In")]
            Inside,

            [DbEnumValue("Out")]
            Outside
        }
    }
}
