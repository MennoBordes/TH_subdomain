using System.Collections.Generic;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Modules.Frame.Entities
{
    using Glass.Entities;

    [DbTable("frame_glass")]
    public class FrameGlass : Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Frame")]
        public int IdFrame { get; set; }

        [DbColumn("Id_Glass")]
        public int? IdGlass { get; set; }

        //=== Helpers

        public Frame Frame { get; set; }

        public Glass Glass { get; set; }
    }
}
