using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TH2.Shared.Modules.Connection.Entities
{
    [Table("connection_lock")]
    public class ConnectionLock
    {
        [Key, Column("Id")]
        public int Id { get; set; }

        [ForeignKey("connection"), Column("Id_Connection")]
        public int IdConnection { get; set; }

        [Column("Type")]
        public int Type { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Price")]
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
