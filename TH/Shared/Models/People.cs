using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TH.Shared.Models
{
    [Table("people")]
    public class People
    {
        [Column("Id")]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
    }
}
