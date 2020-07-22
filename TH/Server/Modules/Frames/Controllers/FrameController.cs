//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TH.Server.Base.Database;
//using TH.Shared.Models;

//namespace TH.Server.Modules.Frames.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class FrameController : ControllerBase
//    {
//        /// <summary> DB context </summary>
//        private readonly ThDbEntities _context;

//        public FrameController(ThDbEntities context)
//        {
//            _context = context;
//            fMan = new FrameManager(_context);
//        }

//        FrameManager fMan;

//        #region frame
//        // GET: api/<frameController>
//        [HttpGet("GetFrame")]
//        public async Task<ActionResult<List<Frame>>> GetFrame()
//        {
//            return await _context.Frame.ToListAsync();
//        }

//        // GET api/<FrameController>/5
//        [HttpGet("GetFrame/{id}")]
//        public async Task<ActionResult<Frame>> GetFrame(int id)
//        {
//            Frame frame = await _context.Frame.FindAsync(id);

//            if (frame == null)
//            {
//                return NotFound();
//            }

//            return frame;
//        }

//        [HttpPost("InsertFrame")]
//        public async Task<ActionResult<int>> InsertFrame([FromBody] Frame frame)
//        {
//            return await fMan.InsertFrame(frame);
//        }
//        #endregion

//        #region frame color
//        // GET: api/<FrameController>
//        [HttpGet("GetFrameKleuren")]
//        public async Task<ActionResult<List<FrameColor>>> GetFrameKleuren()
//        {
//            return await _context.FrameColor.ToListAsync();
//        }

//        // GET api/<FrameController>/5
//        [HttpGet("GetFrameKleur/{id}")]
//        public async Task<ActionResult<FrameColor>> GetFrameKleur(int id)
//        {
//            FrameColor frameColor = await _context.FrameColor.FindAsync(id);

//            if (frameColor == null)
//            {
//                return NotFound();
//            }

//            return frameColor;
//        }
//        #endregion

//        #region Frame Steps
//        // GET: api/<FrameController>
//        [HttpGet("GetFrameStappen")]
//        public async Task<ActionResult<List<FrameSteps>>> GetFrameSteps()
//        {
//            return await _context.FrameSteps.ToListAsync();
//        }

//        // GET api/<FrameController>/5
//        [HttpGet("GetFrameStappen/{id}")]
//        public async Task<ActionResult<FrameSteps>> GetFrameSteps(int id)
//        {
//            FrameSteps frameSteps = await _context.FrameSteps.FindAsync(id);

//            if (frameSteps == null)
//            {
//                return NotFound();
//            }

//            return frameSteps;
//        }
//        #endregion
//    }
//}
