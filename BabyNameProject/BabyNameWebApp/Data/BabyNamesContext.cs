using System;
using Microsoft.EntityFrameworkCore;
using BabyNameWebApp.Models;

namespace BabyNameWebApp.Data
{
    public class BabyNamesContext : DbContext
    {
        public BabyNamesContext(DbContextOptions<BabyNamesContext> options) : base(options)
        {
        }

        public DbSet<BabyName> BabyNames { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BabyName>().ToTable("AllBabyNames");
        }
    }
}