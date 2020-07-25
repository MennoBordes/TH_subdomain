using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TH2.Server.Controllers
{
    using Shared.Modules.People;
    using Shared.Modules.People.Entities;

    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        public PeopleController()
        {
            pMan = new PeopleManager();
        }
        PeopleManager pMan;

        [HttpGet("GetPeople")]
        public ActionResult<List<People>> GetPeoples()
        {
            List<People> res = pMan.GetPeoples();
            return res;
        }

        [HttpGet("GetPerson/{id}")]
        public ActionResult<People> GetPeople(int id)
        {
            People people = pMan.GetPerson(id);

            if (people == null)
            {
                return NotFound();
            }

            return people;
        }
    }
}
