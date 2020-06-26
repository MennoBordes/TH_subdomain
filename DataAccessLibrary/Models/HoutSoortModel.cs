using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class HoutSoortModel
    {
        /// <summary> The unique identifier. </summary>
        public int Id { get; set; }

        /// <summary> The name of the houtsoort. </summary>
        public string Name { get; set; }

        /// <summary> The price per (100mm) (10cm) (0.1m) </summary>
        public decimal Price { get; set; }

        
        //=== Helper functions

        /// <summary> Returns the price of the houtsoort. </summary>
        public decimal GetPrice()
        {
            return Price;
        }
    }
}
