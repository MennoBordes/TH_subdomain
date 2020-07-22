using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH.Server.Base.Database;
//using TH.Shared.Base.Database;
using TH.Shared.Modules.People.Entities;

namespace TH.Server.Modules.Peoples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ThDbEntities _context;

        public PeopleController(ThDbEntities context)
        {
            _context = context;
        }

        [HttpGet("GetPeople")]
        public List<People> GetPeoples()
        {
            return _context.People.ToList();
        }

        [HttpGet("GetPerson/{id}")]
        public async Task<ActionResult<People>> GetPeople(int id)
        {
            People people = await _context.People.FindAsync(id);

            if (people == null)
            {
                return NotFound();
            }

            return people;
        }
    }
}
