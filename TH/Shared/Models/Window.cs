using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TH.Shared.Models
{
    [Table("windows")]
    public class Window
    {
        /// <summary> The unique identifier of the window. </summary>
        [Key, Column("Id")]
        public int Id { get; set; }

        /// <summary> The name of the window. </summary>
        [Column("Name")]
        public string Name { get; set; }

        /// <summary> The description of the window. </summary>
        [Column("Description")]
        public string Description { get; set; }

        /// <summary> The status of the window. </summary>
        [Column("Status")]
        public int Status { get; set; }

        // Helpers

        /// <summary> The options present for this window. </summary>
        [NotMapped]
        public List<WindowOption> WindowOptions { get; set; }
    }
}
