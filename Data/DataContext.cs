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

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Devices> Devices { get; set; }

        public DbSet<Inventory> Inventory { get; set; }

        public DbSet<Assigned> Assigned { get; set; }
//          protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         modelBuilder.Entity<Devices>()
//             .HasKey(d => d.D_Id); // Assuming the primary key for Devices table is 'Id'

//         modelBuilder.Entity<Inventory>()
//             .HasKey(i => i.I_Id); // Primary key for Inventory table

//         modelBuilder.Entity<Inventory>()
//             .HasOne<Devices>() // Define the relationship with Devices table
//             .WithMany() // One Devices can have multiple Inventories
//             .HasForeignKey(i => i.D_Id); // The foreign key column in Inventory table
//     }
// }
    }
}
