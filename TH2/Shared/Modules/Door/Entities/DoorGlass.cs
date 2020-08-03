using System.Collections.Generic;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Door.Entities
{
    using Glass.Entities;

    [DbTable("door_glass")]
    public class DoorGlass: Entity
    {
        [DbPrimaryKey, DbColumn("Id")]
        public int Id { get; set; }

        [DbColumn("Id_Door")]
        public int IdDoor { get; set; }

        [DbColumn("Id_Glass")]
        public int IdGlass { get; set; }

        //=== Helpers
        public List<Door> Doors { get; set; }

        public List<Glass> Glasses { get; set; }
    }
}
