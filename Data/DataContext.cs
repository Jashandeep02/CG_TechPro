using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CG_TechPro.Models;
using Microsoft.EntityFrameworkCore;

namespace CG_TechPro.Data
{
    public class DataContext : DbContext
    {
        private readonly DbContextOptions _options;
        public DataContext(DbContextOptions options) : base(options)
        {
            _options = options;
        }

        public DbSet<Users> Users { get; set; }

        public DbSet<Employee> Employee { get; set; }

        public DbSet<Devices> Devices { get; set; }

        public DbSet<Inventory> Inventory { get; set; }

        public DbSet<Assigned> Assigned { get; set; }
    }
}