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

        #region Wood

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

        #endregion

        #region WoodKind
        [HttpGet("GetKind")]
        public WoodKind GetKind([FromBody] JToken json)
        {
            /*
            Request
                {
                    id: 1,
                    woodpaint: true
                }             
            */
            if (json["id"] == null)
                throw new Exception("Invalid Id Specified!");

            int id = json.Value<int>("id");
            bool paint = json.Value<bool>("woodpaint");

            return wMan.GetKind(id, paint);
        }

        [HttpPost("SaveKind")]
        public int SaveKind([FromBody] WoodKind kind)
        {
            return wMan.SaveKind(kind);
        }

        [HttpPost("DeleteKind")]
        public ActionResult DeleteKind([FromBody] WoodKind kind)
        {
            if (kind == null || kind.Id < 1)
                return BadRequest();

            wMan.DeleteKind(kind.Id);

            return Ok();
        }
        #endregion

        #region WoodGlassSlat
        [HttpGet("GetGlassSlat")]
        public WoodGlassSlat GetGlassSlat([FromBody] JToken json)
        {
            /*
            Request
                {
                    id: 1,
                    woodkind: true
                }             
            */
            if (json["id"] == null)
                throw new Exception("Invalid Id Specified!");

            int id = json.Value<int>("id");
            bool kind = json.Value<bool>("woodkind");

            return wMan.GetGlassSlat(id, kind);
        }

        [HttpPost("SaveGlassSlat")]
        public int SaveGlassSlat([FromBody] WoodGlassSlat glassSlat)
        {
            return wMan.SaveGlassSlat(glassSlat);
        }

        [HttpPost("DeleteGlassSlat")]
        public ActionResult DeleteGlassSlat([FromBody] WoodGlassSlat glassSlat)
        {
            if (glassSlat == null || glassSlat.Id < 1)
                return BadRequest();

            wMan.DeleteGlassSlat(glassSlat.Id);

            return Ok();
        }
        #endregion

        #region WoodPaintColor
        [HttpGet("GetPaintColor")]
        public WoodPaintColor GetPaintColor([FromBody] int id)
        {
            if (id < 1)
                throw new Exception("Invalid Id Specified!");

            return wMan.GetPaintColor(id);
        }

        [HttpPost("SavePaintColor")]
        public int SavePaintColor([FromBody] WoodPaintColor paintColor)
        {
            return wMan.SavePaintColor(paintColor);
        }

        [HttpPost("DeletePaintColor")]
        public ActionResult DeletePaintColor([FromBody] WoodPaintColor paintColor)
        {
            if (paintColor == null || paintColor.Id < 1)
                return BadRequest();

            wMan.DeletePaintColor(paintColor.Id);

            return Ok();
        }
        #endregion
    }
}
