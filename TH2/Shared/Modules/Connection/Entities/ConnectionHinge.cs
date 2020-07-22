using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TH2.Shared.Modules.Connection.Entities
{
    [Table("connection_hinge")]
    public class ConnectionHinge
    {
        [Key, Column("Id")]
        public int Id { get; set; }

        [ForeignKey("connection"), Column("Id_Connection")]
        public int IdConnection { get; set; }

        [Column("Amount")]
        public int Amount { get; set; }

        [Column("Price_Piece")]
        public decimal PricePiece { get; set; }

        // === Helper functions
        public decimal GetPrice()
        {
            return PricePiece * Amount;
        }
    }
}
