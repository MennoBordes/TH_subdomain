using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TH.Shared.Models
{
    [Table("erker")]
    public class Erker
    {
        /// <summary> The unique identifier. </summary>
        [Key, Column("Id")]
        public int Id { get; set; }

        /// <summary> The name. </summary>
        [Column("Name")]
        public string Name { get; set; }
    }
}
