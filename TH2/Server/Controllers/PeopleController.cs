using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TH2.Shared.Base.Database;
using TH2.Shared.Modules.People;
using TH2.Shared.Modules.People.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TH2.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ThDbEntities _context;

        public PeopleController(ThDbEntities context)
        {
            _context = context;
            pMan = new PeopleManager(context);
        }
        PeopleManager pMan;

        [HttpGet("GetPeople")]
        public ActionResult<List<People>> GetPeoples()
        {
            List<People> res = pMan.GetPeoples();
            return res;
            //return _context.People.ToList();
        }

        [HttpGet("GetPerson/{id}")]
        public ActionResult<People> GetPeople(int id)
        {
            People people = pMan.GetPerson(id);
            //People people = await _context.People.FindAsync(id);

            if (people == null)
            {
                return NotFound();
            }

            return people;
        }
    }
}
