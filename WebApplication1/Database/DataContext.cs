using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplication1.Database
{
    public class DataContext : DbContext
    {
        public DbSet<People> Peoples { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
