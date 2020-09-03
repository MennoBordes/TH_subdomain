using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.Connection.Entities
{
    [DbTable("Connection_corner")]
    public class ConnectionCorner : Entity
    {
        [DbPrimaryKey, DbColumn("Id")]
        public int Id { get; set; }

        [DbColumn("Id_Connection")]
        public int IdConnection { get; set; }

        [DbColumn("Amount")]
        public int Amount { get; set; }

        [DbColumn("Price_Piece")]
        public decimal PricePiece { get; set; }

        // === Helper functions
        public decimal GetPrice()
        {
            return PricePiece * Amount;
        }
    }
}
