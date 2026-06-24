using System;
using System.Collections.Generic;
using System.Text;
using GildeApp.Api.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GildeApp.Api.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<RuleSet> RuleSets { get; set; }
        public DbSet<Tourney> Tourneys { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Match> Matches { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);   

            modelBuilder.Entity<Match>()
                .HasOne(m => m.FirstPlayer)
                .WithMany()
                .HasForeignKey(m => m.FirstPlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.SecondPlayer)
                .WithMany()
                .HasForeignKey(m => m.SecondPlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            Seeder.Seed(modelBuilder);
        }
    }
}
