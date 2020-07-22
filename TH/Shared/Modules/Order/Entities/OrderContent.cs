using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TH.Shared.Modules.Order.Entities
{
    [Table("order_content")]
    public class OrderContent
    {
        [Key, Column("Id")]
        public int Id { get; set; }

        [ForeignKey("Order"), Column("Id_Order")]
        public int IdOrder { get; set; }
        
        [ForeignKey("Door"), Column("Id_Door")]
        public int IdDoor { get; set; }
        
        [ForeignKey("Window"), Column("Id_Window")]
        public int IdWindow { get; set; }

        [ForeignKey("Frame"), Column("Id_Frame")]
        public int IdFrame { get; set; }
    }
}
