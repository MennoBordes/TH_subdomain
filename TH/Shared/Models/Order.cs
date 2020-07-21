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
        /// <summary> The unique identifier of the order. </summary>
        [Key, Column("Id")]
        public int Id { get; set; }

        /// <summary> The name of the project. </summary>
        [Column("Project_Name")]
        public string ProjectName { get; set; }

        /// <summary> The creation date of the order. </summary>
        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; }

        // === Helpers

        /// <summary> A list of the kozijnen in the order. </summary>
        [NotMapped]
        public List<OrderKozijn> OrderKozijn { get; set; }

        /// <summary> A list of the kozijnen to order. </summary>
        [NotMapped]
        public List<Kozijn> Kozijnen { get; set; }

        /// <summary> A list of the erkers to order. </summary>
        [NotMapped]
        public List<Erker> Erkers { get; set; }

        /// <summary> A list of the windows to order. </summary>
        [NotMapped]
        public List<Window> Windows { get; set; }

        // === Helper functions

        /// <summary> Returns the total price of the order. </summary>
        public decimal GetTotalPrice() => 0.1m;

        /// <summary> Returns the total formatted price of the order. </summary>
        public string GetFormattedTotalPrice() => GetTotalPrice().ToString("0.00");
    }
}
