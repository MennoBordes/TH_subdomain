using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Window.Entities
{
    [DbTable("window_kind")]
    public class WindowKind : Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn]
        public int Type { get; set; }

        [DbColumn("Turn_Direction")]
        public WindowTurnDir TurnDirection { get; set; }

        [DbColumn]
        public decimal Price { get; set; }


        //=== Helper functions

        public decimal GetPrice()
        {
            return Price;
        }

        public enum WindowTurnDir
        {
            [DbEnumValue("In")]
            Inside,

            [DbEnumValue("Out")]
            Outside
        }
    }
}
