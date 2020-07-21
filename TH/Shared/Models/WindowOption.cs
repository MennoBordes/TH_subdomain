using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TH.Shared.Models
{
    [Table("window_options")]
    public class WindowOption
    {
        /// <summary> The unique identifier. </summary>
        [Key, Column("Id")]
        public int Id { get; set; }

        /// <summary> The name of the option. </summary>
        [Column("Name")]
        public string Name { get; set; }

        /// <summary> The description of the option </summary>
        [Column("Description")]
        public string Description { get; set; }

        /// <summary> The base price of the option. </summary>
        [Column("Base_Price")]
        public decimal BasePrice { get; set; }


        //=== Helpers

        /// <summary> Returns the base price of the window option. </summary>
        public decimal GetBasePrice() => BasePrice;

        /// <summary> Returns the formatted base price of the window option. </summary>
        public string GetFormattedBasePrice() => GetBasePrice().ToString("0.00");
    }
}
