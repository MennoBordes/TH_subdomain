using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TH.Shared.Models
{
    [Table("kozijn_kleur")]
    public class KozijnKleur
    {
        [Key, Column("Id")]
        /// <summary> The unique identifier of the kozijn kleur. </summary>
        public int Id { get; set; }

        [Column("Name")]
        /// <summary> The name of the kozijn kleur. </summary>
        public string Name { get; set; }
    }
}
