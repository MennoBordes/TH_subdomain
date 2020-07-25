using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Frame.Entities
{
    [DbTable("frame_wood")]
    public class FrameWood: Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }

        [DbColumn("Id_Frame")]
        public int IdFrame { get; set; }

        [DbColumn("Id_Wood")]
        public int IdWood { get; set; }
    }
}
