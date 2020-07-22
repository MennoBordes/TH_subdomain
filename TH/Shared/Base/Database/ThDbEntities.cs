using Microsoft.EntityFrameworkCore;
using TH.Shared.Modules.People.Entities;

namespace TH.Shared.Base.Database
{
    public class ThDbEntities : DbContext
    {
        public ThDbEntities(DbContextOptions<ThDbEntities> options) : base(options) { }

        /// <summary> The People DB table. </summary>
        public DbSet<People> People { get; set; }
    }
}
