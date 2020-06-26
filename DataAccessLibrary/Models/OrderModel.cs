using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class OrderModel
    {
        /// <summary> The unique identifier of the order. </summary>
        public int Id { get; set; }

        /// <summary> The name of the project. </summary>
        public string ProjectName { get; set; }

        /// <summary> The creation date of the order. </summary>
        public DateTime CreatedDate { get; set; }

        // === Helpers
        /// <summary> A list of the kozijnen to order. </summary>
        public List<KozijnModel> Kozijnen { get; set; }

        /// <summary> A list of the erkers to order. </summary>
        public List<ErkerModel> Erkers { get; set; }

        /// <summary> A list of the windows to order. </summary>
        public List<WindowModel> Windows { get; set; }

        // === Helper functions

        /// <summary> Returns the total price of the order. </summary>
        public decimal GetTotalPrice() => 0.1m;

        /// <summary> Returns the total formatted price of the order. </summary>
        public string GetFormattedTotalPrice() => GetTotalPrice().ToString("0.00");
    }
}
