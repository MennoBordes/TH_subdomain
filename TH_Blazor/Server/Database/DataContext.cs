using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH_Blazor.Shared.Models;

namespace TH_Blazor.Server.Database
{
    public class DataContext : DbContext
    {
        public DbSet<PeopleModel> Peoples{ get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
