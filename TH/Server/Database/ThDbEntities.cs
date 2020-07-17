using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH.Shared.Models;

namespace TH.Server.Database
{
    public class ThDbEntities : DbContext
    {
        public ThDbEntities(DbContextOptions<ThDbEntities> options): base(options) { }

        /// <summary>
        /// The People DB table.
        /// </summary>
        public DbSet<People> People { get; set; }

        /// <summary>
        /// The Kozijn DB table.
        /// </summary>
        public DbSet<Kozijn> Kozijn { get; set; }

        /// <summary>
        /// The KozijnKleur DB table.
        /// </summary>
        public DbSet<KozijnKleur> KozijnKleur { get; set; }

        /// <summary>
        /// The KozijnStappen DB table.
        /// </summary>
        public DbSet<KozijnStappen> KozijnStappen { get; set; }

        /// <summary>
        /// The Order DB table.
        /// </summary>
        public DbSet<Order> Order { get; set; }

        /// <summary>
        /// The Erker DB table.
        /// </summary>
        public DbSet<Erker> Erker { get; set; }

        /// <summary>
        /// The HoutSoort DB table.
        /// </summary>
        public DbSet<HoutSoort> HoutSoort { get; set; }

        /// <summary>
        /// The Window DB table.
        /// </summary>
        public DbSet<Window> Window { get; set; }

        /// <summary>
        /// The WindowOption DB table.
        /// </summary>
        public DbSet<WindowOption> WindowOption { get; set; }
    }
}
