using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Base.Database.Entities.Localization
{
    public class Language : Entity
    {
        public const int DEFAULT = 2;

        [DbPrimaryKey]
        [DbColumn]
        public int Id { get; set; }

        [DbColumn("machine_name")]
        public string MachineName { get; set; }

        [DbColumn]
        public string Code { get; set; }

        /// <summary> This is the Iso 639 code (example: English=en,Danish=da,Dutch=nl,Finnish=fi,French=fr_FR,German=de,Italian=it,Polish=pl,Portuguese=pt_BR,Spanish=es,Swedish=sv). </summary>
        [DbColumn("iso_code")]
        public string IsoCode { get; set; }

        public List<LanguageData> Data { get; set; }

        /// <summary> Database Reference. </summary>
        public enum Ref
        {
            English_UK = 1,
            Dutch = 2,
            English_US = 3
        }
    }
}
