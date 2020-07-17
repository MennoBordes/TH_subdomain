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
        [Key, Column("Id")]
        /// <summary> The unique identifier. </summary>
        public int Id { get; set; }
        [Column("Name")]
        /// <summary> The name. </summary>
        public string Name { get; set; }
    }
}
