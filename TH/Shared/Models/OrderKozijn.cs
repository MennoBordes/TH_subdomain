using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TH.Shared.Models
{
    [Table("order_kozijn")]
    public class OrderKozijn
    {
        [Key, Column("Id")]
        /// <summary> The primary key. </summary>
        public int Id { get; set; }

        /// <summary> The parent Id </summary>
        [ForeignKey("Order"), Column("Order_Id")]
        public int OrderId { get; set; }
        
    }
}
