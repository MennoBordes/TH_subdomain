using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class KozijnModel
    {
        /// <summary> Default width of the kozijn. </summary>
        public const int DefaultWidth = 1000;
        /// <summary> Default height of the kozijn. </summary>
        public const int DefaultHeight = 1000;

        /// <summary> The unique identifier. </summary>
        public int Id { get; set; }
        /// <summary> The name of the model. </summary>
        public string Name { get; set; }
        /// <summary> The description of the model. </summary>
        public string Description { get; set; }

        /// <summary> The width of the kozijn. </summary>
        public int Width { get; set; }
        /// <summary> The height of the kozijn. </summary>
        public int Height { get; set; }

        /// <summary> The base price of the model. </summary>
        public decimal BasePrice { get; set; }


        // === Helpers

        /// <summary> The parts int the kozijn. </summary>
        public KozijnParts KozijnParts { get; set; }

        /// <summary> The houtsoort. </summary>
        public HoutSoortModel HoutSoort { get; set; }


        // === Helper functions

        /// <summary> Returns the base price of the model. </summary>
        public decimal GetBasePrice()
        {
            return BasePrice;
        }

        /// <summary> Returns the total price of the model.  </summary>
        public decimal GetTotalPrice()
        {
            return GetBasePrice() + HoutSoort.GetPrice();
        }

        /// <summary> Returns the formatted total price of the model. </summary>
        public string GetFormattedTotalPrice()
        {
            return GetTotalPrice().ToString("0.00");
        }
    }
}
