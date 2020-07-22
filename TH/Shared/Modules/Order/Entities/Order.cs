using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TH.Shared.Modules.Order.Entities
{
    [Table("order")]
    public class Order
    {
        /// <summary> The unique identifier of the order. </summary>
        [Key, Column("Id")]
        public int Id { get; set; }

        /// <summary> The name of the project. </summary>
        [Column("Project_Name")]
        public string ProjectName { get; set; }

        /// <summary> The creation date of the order. </summary>
        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; }
    }
}
