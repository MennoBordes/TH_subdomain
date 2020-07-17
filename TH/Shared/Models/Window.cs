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
        [Key, Column("Id")]
        /// <summary> The unique identifier of the window. </summary>
        public int Id { get; set; }

        [Column("Name")]
        /// <summary> The name of the window. </summary>
        public string Name { get; set; }

        [Column("Description")]
        /// <summary> The description of the window. </summary>
        public string Description { get; set; }

        [Column("Status")]
        /// <summary> The status of the window. </summary>
        public int Status { get; set; }

        // Helpers

        [NotMapped]
        /// <summary> The options present for this window. </summary>
        public List<WindowOption> WindowOptions { get; set; }
    }
}
