using System.ComponentModel.DataAnnotations.Schema;

namespace TH_Blazor.Shared.Models
{
    [Table("erker")]
    public class Erker
    {
        [Column("Id")]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
