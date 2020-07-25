using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Window.Entities
{
    [DbTable("window_wood")]
    class WindowWood : Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Frame")]
        public int IdFrame { get; set; }

        [DbColumn("Id_Wood")]
        public int IdWood { get; set; }
    }
}
