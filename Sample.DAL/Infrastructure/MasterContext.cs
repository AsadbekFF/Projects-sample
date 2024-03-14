using Microsoft.EntityFrameworkCore;
using Sample.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.DAL.Infrastructure
{
    public class MasterContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public MasterContext(DbContextOptions<MasterContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public void Initialize()
        {
            // Users
            if (!Users.Any())
            {
                Users.Add(new User
                {
                    Username = "admin",
                    Password = "1",
                    UpdatedDateTime = DateTime.UtcNow,
                });
                SaveChanges();
            }
        }
    }
}
