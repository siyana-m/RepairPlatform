using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RepairPlatform.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Data.Contexts
{
    public class AuthDbContext : IdentityDbContext<AspNetUsers>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<Repairguy> Repairguys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuring the relationship
            modelBuilder.Entity<Repairguy>()
                .HasOne(r => r.User)
                .WithOne(u => u.Repairguys)
                .HasForeignKey<Repairguy>(r => r.UserId);
        }
    }
}
