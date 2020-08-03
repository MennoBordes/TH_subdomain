﻿using System.Collections.Generic;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Window.Entities
{
    using Glass.Entities;

    [DbTable("window_glass")]
    public class WindowGlass : Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Window")]
        public int IdWindow { get; set; }

        [DbColumn("Id_Glass")]
        public int IdGlass { get; set; }

        //=== Helpers
        public List<Window> Windows { get; set; }

        public List<Glass> Glasses { get; set; }
    }
}
