using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH.Server.Database;
using TH.Shared.Models;

namespace TH.Server.Controllers
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
