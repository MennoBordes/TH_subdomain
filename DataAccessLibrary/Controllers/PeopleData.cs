using DataAccessLibrary.DataBase;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Controllers
{
    public class PeopleData : IPeopleData
    {
        private readonly IMySqlDataAccess _db;

        public PeopleData(IMySqlDataAccess db)
        {
            _db = db;
        }

        public Task<List<PersonModel>> GetPeople()
        {
            string sql = "select * from people";

            return _db.LoadDataList<PersonModel, dynamic>(sql, new { });
        }

        public Task InsertPerson(PersonModel person)
        {
            string sql = @"insert into People (FirstName, LastName, EmailAddress)
                           values (@FirstName, @LastName, @EmailAddress);";

            return _db.SaveData(sql, person);
        }
    }
}
