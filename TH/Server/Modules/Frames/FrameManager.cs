//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using TH.Server.Base.Database;
//using TH.Server.Base.Extensions;
//using TH.Shared.Models;

//namespace TH.Server.Modules.Frames
//{
//    public class FrameManager
//    {
//        private readonly ThDbEntities _context;

//        public FrameManager(ThDbEntities context)
//        {
//            _context = context;
//        }

//        /// <summary> Gets a kozijn. </summary>
//        /// <param name="id">The id of the kozijn te retrieve.</param>
//        /// <param name="data">Whether additional data should be retrieved.</param>
//        /// <returns>A kozijn.</returns>
//        public async Task<Frame> GetFrame(int id, bool data = false)
//        {
//            // Check
//            if (id < 1) return null;

//            // Get
//            Frame kozijn = await _context.Frame.Where(x => x.Id == id).FirstOrDefaultAsync();

//            if (kozijn == null) return null;

//            if (data)
//            {
//                this.MergeDataIntoFrames(frame: new List<Frame>() { kozijn });
//            }

//            return kozijn;
//        }

//        public async Task<List<Frame>> GetFrames(int[] ids = null)
//        {
//            var query = from x in _context.Set<Frame>() select x;

//            if (ids != null)
//            {
//                query = query.Where(x => ids.Contains(x.Id));
//            }

//            List<Frame> kozijnen = await query.ToListAsync();

//            return kozijnen;
//        }

//        public async void MergeKleurIntoFrame(List<Frame> frame)
//        {
//            if (frame.IsNullOrEmpty()) return;

//            // Create list of ids
//            List<int> ids = frame.Select(x => x.Id).ToList();

//            //List<KozijnKleur> kozijnKleuren = await _context.KozijnKleur.Where(x => ids.Contains(x.Id))
//        }

//        public void MergeDataIntoFrames(List<Frame> frame)
//        {
//            MergeKleurIntoFrame(frame);
//        }

//        public async Task<int> InsertFrame(Frame frame)
//        {
//            if (frame == null) throw new Exception("Frame is not specified.");
//            if (frame.Id > 0) throw new Exception("Invalid id specified.");

//            _context.Frame.Add(frame);
//            await _context.SaveChangesAsync();

//            return frame.Id;
//        }
//    }
//}
