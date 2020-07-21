using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TH.Server.Base.Database;
using TH.Shared.Models;

namespace TH.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KozijnController : ControllerBase
    {
        /// <summary> DB context </summary>
        private readonly ThDbEntities _context;

        public KozijnController(ThDbEntities context)
        {
            _context = context;
        }

        #region kozijn
        // GET: api/<KozijnController>
        [HttpGet("GetKozijnen")]
        public async Task<ActionResult<List<Kozijn>>> GetKozijnen()
        {
            return await _context.Kozijn.ToListAsync();
        }

        // GET api/<KozijnController>/5
        [HttpGet("GetKozijn/{id}")]
        public async Task<ActionResult<Kozijn>> GetKozijn(int id)
        {
            Kozijn kozijn = await _context.Kozijn.FindAsync(id);

            if (kozijn == null)
            {
                return NotFound();
            }

            return kozijn;
        }
        #endregion

        #region kozijn kleur
        // GET: api/<KozijnController>
        [HttpGet("GetKozijnKleuren")]
        public async Task<ActionResult<List<KozijnKleur>>> GetKozijnKleuren()
        {
            return await _context.KozijnKleur.ToListAsync();
        }

        // GET api/<KozijnController>/5
        [HttpGet("GetKozijnKleur/{id}")]
        public async Task<ActionResult<KozijnKleur>> GetKozijnKleur(int id)
        {
            KozijnKleur kozijnKleur = await _context.KozijnKleur.FindAsync(id);

            if (kozijnKleur == null)
            {
                return NotFound();
            }

            return kozijnKleur;
        }
        #endregion

        #region kozijn stappen
        // GET: api/<KozijnController>
        [HttpGet("GetKozijnStappen")]
        public async Task<ActionResult<List<KozijnStappen>>> GetKozijnenStappen()
        {
            return await _context.KozijnStappen.ToListAsync();
        }

        // GET api/<KozijnController>/5
        [HttpGet("GetKozijnStappen/{id}")]
        public async Task<ActionResult<KozijnStappen>> GetKozijnStappen(int id)
        {
            KozijnStappen kozijnStappen = await _context.KozijnStappen.FindAsync(id);

            if (kozijnStappen == null)
            {
                return NotFound();
            }

            return kozijnStappen;
        }
        #endregion
    }
}
