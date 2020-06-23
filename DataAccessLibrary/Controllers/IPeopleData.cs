using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Controllers
{
    public interface IPeopleData
    {
        Task<List<PersonModel>> GetPeople();
        Task InsertPerson(PersonModel person);
    }
}