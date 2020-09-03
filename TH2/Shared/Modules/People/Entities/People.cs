using System;
using THTools.ORM;
using THTools.ORM.Common;

namespace TH2.Shared.Modules.People.Entities
{
    [DbTable("people")]
    public class People : Entity
    {
        [DbPrimaryKey, DbColumn]
        public int Id { get; set; }
        [DbColumn]
        public string FirstName { get; set; }
        [DbColumn]
        public string LastName { get; set; }
        [DbColumn]
        public DateTime DateOfBirth { get; set; }
        [DbColumn]
        public string EmailAddress { get; set; }
    }
}
