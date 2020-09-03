using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Base.Database.Entities.Localization
{
    [DbTable("language_data")]
    public class LanguageData : Entity
    {
        [DbPrimaryKey]
        [DbColumn]
        public int Id { get; set; }

        [DbColumn("language_src_id")]
        public int LanguageSrcId { get; set; }

        [DbColumn("language_id")]
        public int LanguageId { get; set; }

        [DbColumn("lable")]
        public string Label { get; set; }
    }
}
