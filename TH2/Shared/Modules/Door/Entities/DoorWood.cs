using System.Collections.Generic;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Door.Entities
{
    using Wood.Entities;

    [DbTable("door_wood")]
    public class DoorWood : Entity
    {
        [DbPrimaryKey, DbColumn("Id")]
        public int Id { get; set; }

        [DbColumn("Id_Door")]
        public int IdDoor { get; set; }

        [DbColumn("Id_Wood")]
        public int IdWood { get; set; }

        //=== Helpers
        public Door Door { get; set; }

        public Wood Wood { get; set; }
    }
}
