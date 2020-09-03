using System;
using System.Collections.Generic;
using System.Linq;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Connection.Entities
{
    [DbTable("connection")]
    public class Connection : Entity
    {
        [DbPrimaryKey, DbColumn("Id")]
        public int Id { get; set; }

        [DbColumn("Type")]
        public int Type { get; set; }

        [DbColumn("Created_At")]
        public DateTime CreationDate { get; set; }


        // === Helpers

        public List<ConnectionCorner> Corners { get; set; }

        public List<ConnectionHinge> Hinges { get; set; }

        public List<ConnectionLock> Locks { get; set; }

        // === Helper functions
        public decimal GetPrice()
        {
            decimal cornerPrice = Corners.Sum(x => x.GetPrice());
            decimal hingePrice = Hinges.Sum(x => x.GetPrice());
            decimal lockPrice = Locks.Sum(x => x.GetPrice());

            return cornerPrice + hingePrice + lockPrice;
        }

        public enum ConnectionType
        {
            Door = 0,
            Window = 1,
            Frame = 2
        }
    }
}
