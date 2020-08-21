using System.Collections.Generic;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Modules.Window.Entities
{
    using Wood.Entities;

    [DbTable("window_wood")]
    public class WindowWood : Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Window")]
        public int IdWindow { get; set; }

        [DbColumn("Id_Wood")]
        public int IdWood { get; set; }

        //=== Helpers
        public List<Window> Windows { get; set; }

        public List<Wood> Woods { get; set; }
    }
}
