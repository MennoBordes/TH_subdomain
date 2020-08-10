using System;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TH2.Server.Modules.Glass
{
    using Newtonsoft.Json.Linq;
    using Shared.Modules.Glass;
    using Shared.Modules.Glass.Entities;

    [Route("api/[controller]")]
    [ApiController]
    public class GlassController : ControllerBase
    {
        GlassManager gMan;

        public GlassController()
        {
            gMan = new GlassManager();
        }

        #region Glass

        // GET: api/<GlassController>
        [HttpGet("GetGlass")]
        public Glass GetGlass([FromBody] JToken json)
        {
            /*
            Request
                {
                    id: 1,
                    ventilation: true
                }             
            */

            if (json["id"] == null)
                throw new Exception("No Id specified!");

            int id = json.Value<int>("id");
            bool ventilation = json.Value<bool>("ventilation");

            return gMan.GetGlass(id, ventilation);
        }

        [HttpPost("SaveGlass")]
        public int SaveGlass([FromBody] Glass glass)
        {
            return gMan.SaveGlass(glass);
        }

        [HttpPost("DeleteGlass")]
        public ActionResult DeleteGlass([FromBody] Glass glass)
        {
            if (glass == null || glass.Id < 1)
                return BadRequest();

            gMan.DeleteGlass(glass.Id);

            return Ok();
        }

        #endregion

        #region Ventilation

        [HttpGet("GetVentilation")]
        public GlassVentilation GetVentilation([FromBody] int id)
        {
            if (id < 1)
                throw new Exception("Invalid Id Specified!");

            return gMan.GetVentilation(id);
        }

        [HttpPost("SaveVentilation")]
        public int SaveVentilation([FromBody] GlassVentilation ventilation)
        {
            return gMan.SaveVentilation(ventilation);
        }

        [HttpPost("DeleteVentilation")]
        public ActionResult DeleteVentilation([FromBody] GlassVentilation ventilation)
        {
            if (ventilation == null || ventilation.Id < 1)
                return BadRequest();

            gMan.DeleteVentilation(ventilation.Id);

            return Ok();
        }

        #endregion
    }
}
