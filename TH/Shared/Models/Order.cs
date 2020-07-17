using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TH.Shared.Models
{
    [Table("order")]
    public class Order
    {
        [Key, Column("Id")]
        /// <summary> The unique identifier of the order. </summary>
        public int Id { get; set; }

        [Column("Project_Name")]
        /// <summary> The name of the project. </summary>
        public string ProjectName { get; set; }

        [Column("Created_Date")]
        /// <summary> The creation date of the order. </summary>
        public DateTime CreatedDate { get; set; }

        // === Helpers
        [NotMapped]
        /// <summary> A list of the kozijnen to order. </summary>
        public List<Kozijn> Kozijnen { get; set; }

        [NotMapped]
        /// <summary> A list of the erkers to order. </summary>
        public List<Erker> Erkers { get; set; }

        [NotMapped]
        /// <summary> A list of the windows to order. </summary>
        public List<Window> Windows { get; set; }

        // === Helper functions

        /// <summary> Returns the total price of the order. </summary>
        public decimal GetTotalPrice() => 0.1m;

        /// <summary> Returns the total formatted price of the order. </summary>
        public string GetFormattedTotalPrice() => GetTotalPrice().ToString("0.00");
    }
}
