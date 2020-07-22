using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TH2.Shared.Modules.Connection.Entities;
using TH2.Shared.Modules.People.Entities;

namespace TH2.Shared.Base.Database
{
    public class ThDbEntities : DbContext
    {
        public ThDbEntities(DbContextOptions<ThDbEntities> options) : base(options) { }

        /// <summary> The People table. </summary>
        public DbSet<People> People { get; set; }

        /// <summary> The Connections table. </summary>
        public DbSet<Connection> Connection { get; set; }

        /// <summary> The Corner Connections table. </summary>
        public DbSet<ConnectionCorner> ConnectionCorner { get; set; }

        /// <summary> The Hinge Connections table. </summary>
        public DbSet<ConnectionHinge> ConnectionHinge { get; set; }

        /// <summary> The Lock Connections table. </summary>
        public DbSet<ConnectionLock> ConnectionLock { get; set; }
    }
}
