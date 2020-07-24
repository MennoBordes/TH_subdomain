using System;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TH2.Server.Modules.Connection
{
    using Newtonsoft.Json.Linq;
    using Modules.Base;
    using Shared.Modules.Connection;
    using Shared.Modules.Connection.Entities;

    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionController : BaseController
    {
        public ConnectionController()
        {
            cMan = new ConnectionManager();
        }

        ConnectionManager cMan;

        [HttpGet("GetConnection")]
        public Connection GetConnection([FromBody] JToken json)
        {
            /* Request
                {
                    id: 1,
                    corners: false,
                    hinges: false,
                    locks: false
                }             
             */
            
            if (json["id"]==null) throw new Exception("No Id specified.");

            int id = json.Value<int>("id");
            bool corners = json.Value<bool>("corners");
            bool hinges = json.Value<bool>("hinges");
            bool locks = json.Value<bool>("locks");

            return cMan.GetConnection(id, corners: corners, hinges: hinges, locks: locks);
        }

        [HttpPost("SaveConnection")]
        public int InsertConnection([FromBody] Connection connection)
        {
            return cMan.SaveConnection(connection);
        }

        [HttpPost("DeleteConnection")]
        public ActionResult DeleteConnection([FromBody] Connection connection)
        {
            if (connection == null || connection.Id < 1)
                return BadRequest();

            cMan.DeleteConnection(new int[] { connection.Id });

            return Ok();
        }
    }
}
