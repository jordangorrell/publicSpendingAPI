using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using spendingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spendingAPI.Data
{
    public class SpendingContext : IdentityDbContext
    {
        public DbSet<Entry> Entries { get; set; }

        public SpendingContext(DbContextOptions<SpendingContext> options) : base(options) {}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql("Host=localhost;Database=spendingTracker;Username=postgres;Password=friffle891;Port=5432");
    }
}
