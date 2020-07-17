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
        [Key, Column("Id")]
        /// <summary> The unique identifier. </summary>
        public int Id { get; set; }

        [Column("Name")]
        /// <summary> The name of the option. </summary>
        public string Name { get; set; }

        [Column("Description")]
        /// <summary> The description of the option </summary>
        public string Description { get; set; }

        [Column("Base_Price")]
        /// <summary> The base price of the option. </summary>
        public decimal BasePrice { get; set; }


        //=== Helpers

        /// <summary> Returns the base price of the window option. </summary>
        public decimal GetBasePrice() => BasePrice;

        /// <summary> Returns the formatted base price of the window option. </summary>
        public string GetFormattedBasePrice() => GetBasePrice().ToString("0.00");
    }
}
