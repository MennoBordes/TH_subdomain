using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TH.Shared.Models
{
    [Table("kozijn")]
    public class Kozijn
    {
        /// <summary> Default width of the kozijn. </summary>
        public const int DefaultWidth = 1000;
        /// <summary> Default height of the kozijn. </summary>
        public const int DefaultHeight = 1000;

        [Key, Column("Id")]
        /// <summary> The unique identifier. </summary>
        public int Id { get; set; }

        [Column("Name")]
        /// <summary> The name of the model. </summary>
        public string Name { get; set; }

        [Column("Description")]
        /// <summary> The description of the model. </summary>
        public string Description { get; set; }

        [Column("Width")]
        /// <summary> The width of the kozijn. </summary>
        public int Width { get; set; }

        [Column("Height")]
        /// <summary> The height of the kozijn. </summary>
        public int Height { get; set; }

        [Column("BasePrice")]
        /// <summary> The base price of the model. </summary>
        public decimal BasePrice { get; set; }


        // === Helpers

        [NotMapped]
        /// <summary> The houtsoort. </summary>
        public HoutSoort HoutSoort { get; set; }


        // === Helper functions

        /// <summary> Returns the base price of the model. </summary>
        public decimal GetBasePrice()
        {
            return BasePrice;
        }

        ///// <summary> Returns the total price of the model.  </summary>
        //public decimal GetTotalPrice()
        //{
        //    return GetBasePrice() + HoutSoort.GetPrice();
        //}

        ///// <summary> Returns the formatted total price of the model. </summary>
        //public string GetFormattedTotalPrice()
        //{
        //    return GetTotalPrice().ToString("0.00");
        //}
    }
}
