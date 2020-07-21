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

        /// <summary> The unique identifier. </summary>
        [Key, Column("Id")]
        public int Id { get; set; }

        /// <summary> The name of the model. </summary>
        [Column("Name")]
        public string Name { get; set; }

        /// <summary> The description of the model. </summary>
        [Column("Description")]
        public string Description { get; set; }

        /// <summary> The width of the kozijn. </summary>
        [Column("Width")]
        public int Width { get; set; }

        /// <summary> The height of the kozijn. </summary>
        [Column("Height")]
        public int Height { get; set; }

        /// <summary> The base price of the model. </summary>
        [Column("BasePrice")]
        public decimal BasePrice { get; set; }


        // === Helpers

        /// <summary> The houtsoort. </summary>
        [NotMapped]
        public HoutSoort HoutSoort { get; set; }

        /// <summary> The kozijn kleur. </summary>
        [NotMapped]
        public KozijnKleur KozijnKleur { get; set; }

        /// <summary> The possible steps. </summary>
        [NotMapped]
        public KozijnStappen KozijnStappen { get; set; }


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
