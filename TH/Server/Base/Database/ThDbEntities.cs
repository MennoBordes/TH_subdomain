using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH.Shared.Modules.Order.Entities;
using TH.Shared.Modules.People.Entities;

namespace TH.Server.Base.Database
{
    public class ThDbEntities : DbContext
    {
        public ThDbEntities(DbContextOptions<ThDbEntities> options): base(options) { }

        /// <summary>
        /// The People DB table.
        /// </summary>
        public DbSet<People> People { get; set; }

        /// <summary>
        /// The Order DB table.
        /// </summary>
        public DbSet<Order> Order { get; set; }
        
    }
}
