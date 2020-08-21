using THTools.ORM;
using THTools.ORM.Common;

namespace TH.Core.Modules.Connection.Entities
{
    [DbTable("connection_lock")]
    public class ConnectionLock : Entity
    {
        [DbPrimaryKey, DbColumn("Id")]
        public int Id { get; set; }

        [DbColumn("Id_Connection")]
        public int IdConnection { get; set; }

        [DbColumn("Type")]
        public int Type { get; set; }

        [DbColumn("Name")]
        public string Name { get; set; }

        [DbColumn("Price")]
        public decimal Price { get; set; }

        // === Helper functions
        public decimal GetPrice()
        {
            return Price;
        }

        public enum Ref
        {
            Door = 0,
            Window = 1
        }
    }
}
