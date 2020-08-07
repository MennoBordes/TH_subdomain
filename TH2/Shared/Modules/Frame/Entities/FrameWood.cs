﻿using System.Collections.Generic;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Frame.Entities
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
        public List<Frame> Frames { get; set; }

        public List<Wood> Woods { get; set; }
    }
}