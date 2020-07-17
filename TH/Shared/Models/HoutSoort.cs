using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TH.Shared.Models
{
    [Table("hout_soort")]
    public class HoutSoort
    {
        [Key, Column("Id")]
        /// <summary> The unique identifier. </summary>
        public int Id { get; set; }

        [Column("Name")]
        /// <summary> The name of the houtsoort. </summary>
        public string Name { get; set; }

        [Column("Price")]
        /// <summary> The price per (100mm) (10cm) (0.1m) </summary>
        public decimal Price { get; set; }


        //=== Helper functions

        /// <summary> Returns the price of the houtsoort. </summary>
        public decimal GetPrice() => Price;

        public string GetFormattedPrice() => GetPrice().ToString("0.00");
    }
}
