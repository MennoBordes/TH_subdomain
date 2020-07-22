using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TH2.Shared.Modules.Connection.Entities
{
    [Table("connection")]
    public class Connection
    {
        [Key, Column("Id")]
        public int Id { get; set; }

        [Column("Type")]
        public EType Type { get; set; }

        [Column("Created_At")]
        public DateTime CreationDate { get; set; }


        // === Helpers

        [NotMapped]
        public List<ConnectionCorner> Corners { get; set; }

        [NotMapped]
        public List<ConnectionHinge> Hinges { get; set; }

        [NotMapped]
        public List<ConnectionLock> Locks { get; set; }

        // === Helper functions
        public decimal GetPrice()
        {
            decimal cornerPrice = Corners.Sum(x => x.GetPrice());
            decimal hingePrice = Hinges.Sum(x => x.GetPrice());
            decimal lockPrice = Locks.Sum(x => x.GetPrice());

            return cornerPrice + hingePrice + lockPrice;
        }

        public enum EType
        {
            Door = 0,
            Window = 1,
            Frame = 2
        }
    }
}
