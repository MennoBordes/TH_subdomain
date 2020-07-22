using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TH2.Server.Modules.Connection
{
    using Newtonsoft.Json.Linq;
    using Modules.Base;
    using Shared.Base.Database;
    using Shared.Modules.Connection;
    using Shared.Modules.Connection.Entities;
    using System.Net.Http;

    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionController : BaseController
    {
        private readonly ThDbEntities _context;

        public ConnectionController(ThDbEntities context)
        {
            _context = context;
            cMan = new ConnectionManager(context);
        }

        ConnectionManager cMan;

        [HttpGet("GetConnection")]
        public Connection GetConnection([FromBody] JToken json)
        {
            /* Request
                {
                    id: 1,
                    data: false
                }             
             */
            
            if (json["id"]==null) throw new Exception("No Id specified.");

            int id = json.Value<int>("id");
            bool retrieveData = json.Value<bool>("data");

            return cMan.GetConnection(id, data:retrieveData);
        }

        [HttpPost("InsertConnection")]
        public int InsertConnection([FromBody] Connection connection)
        {
            return cMan.InsertConnection(connection);
        }

        [HttpPost("UpdateConnection")]
        public ActionResult UpdateConnection([FromBody] Connection connection)
        {
            if (connection == null || connection.Id < 1)
                return BadRequest();

            connection.CreationDate = DateTime.Now;
            cMan.UpdateConnection(connection);

            return Ok();
        }

        [HttpPost("DeleteConnection")]
        public ActionResult DeleteConnection([FromBody] Connection connection)
        {
            if (connection == null || connection.Id < 1)
                return BadRequest();
            cMan.DeleteConnection(connection);

            return Ok();
        }
    }
}
