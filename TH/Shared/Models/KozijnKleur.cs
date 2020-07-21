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
        /// <summary> The unique identifier of the kozijn kleur. </summary>
        [Key, Column("Id")]
        public int Id { get; set; }

        /// <summary> The name of the kozijn kleur. </summary>
        [Column("Name")]
        public string Name { get; set; }
    }
}
