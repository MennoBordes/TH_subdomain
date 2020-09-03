using System.Collections.Generic;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Modules.Frame.Entities
{
    using Wood.Entities;

    [DbTable("frame_wood")]
    public class FrameWood: Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Frame")]
        public int IdFrame { get; set; }

        [DbColumn("Id_Wood")]
        public int IdWood { get; set; }

        //=== Helpers
        public Frame Frame { get; set; }

        public Wood Wood { get; set; }
    }
}
