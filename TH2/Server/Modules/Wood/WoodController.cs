using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TH2.Server.Modules.Wood
{
    using Modules.Base;
    using Shared.Modules.Wood;
    using Shared.Modules.Wood.Entities;

    [Route("api/[controller]")]
    [ApiController]
    public class WoodController : BaseController
    {
        WoodManager wMan;

        public WoodController()
        {
            wMan = new WoodManager();
        }

        [HttpGet("GetWood")]
        public Wood GetWood([FromBody] JToken json)
        {
            /*
            Request 
                {
                    id: 1,
                    kind: true,
                    glass: true,
                    paint: true
                }
             */

            if (json["id"] == null)
                throw new Exception("No Id specified!");

            int id = json.Value<int>("id");
            bool kind = json.Value<bool>("kind");
            bool glass = json.Value<bool>("glass");
            bool paint = json.Value<bool>("paint");

            return wMan.GetWood(id, woodKind: kind, woodGlass: glass, woodPaint: paint);
        }

        [HttpPost("SaveWood")]
        public int SaveWood([FromBody] Wood wood)
        {
            return wMan.SaveWood(wood);
        }

        [HttpPost("DeleteWood")]
        public ActionResult DeleteWood([FromBody] Wood wood)
        {
            if (wood == null || wood.Id < 1)
                return BadRequest();

            wMan.DeleteWood(wood.Id);

            return Ok();
        }
    }
}
