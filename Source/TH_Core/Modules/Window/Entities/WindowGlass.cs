using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Modules.Window.Entities
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
        public int? IdGlass { get; set; }

        //=== Helpers
        public Window Window { get; set; }

        public Glass Glass { get; set; }
    }
}
