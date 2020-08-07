using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TH2.Server.Modules.Frame
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrameController : ControllerBase
    {
        // GET: api/<FrameController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<FrameController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FrameController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<FrameController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FrameController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
