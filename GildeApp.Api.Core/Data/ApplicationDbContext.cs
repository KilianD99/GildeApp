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
        public DbSet<TourneyEntry> TourneyEntries { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Match> Matches { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Player>(player =>
            {
                player.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
                player.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Tourney>(tourney =>
            {
                tourney.Property(t => t.Name).IsRequired().HasMaxLength(200);
                tourney.Property(t => t.Status).HasConversion<int>();

                tourney.HasOne(t => t.RuleSet)
                       .WithMany()
                       .HasForeignKey(t => t.RuleSetId)
                       .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TourneyEntry>(entry =>
            {
                entry.HasOne(e => e.Tourney)
                     .WithMany(t => t.Entries)
                     .HasForeignKey(e => e.TourneyId)
                     .OnDelete(DeleteBehavior.Restrict);

                entry.HasOne(e => e.Player)
                     .WithMany(p => p.Entries)
                     .HasForeignKey(e => e.PlayerId)
                     .OnDelete(DeleteBehavior.Restrict);

                entry.HasIndex(e => new { e.TourneyId, e.Position }).IsUnique();
                entry.HasIndex(e => new { e.TourneyId, e.PlayerId }).IsUnique();
            });

            modelBuilder.Entity<Match>(match =>
            {
                match.Property(m => m.Status).HasConversion<int>();
                match.Property(m => m.ClaimedBy).HasMaxLength(100);

                match.HasOne(m => m.Tourney)
                     .WithMany(t => t.Matches)
                     .HasForeignKey(m => m.TourneyId)
                     .OnDelete(DeleteBehavior.Restrict);

                match.HasOne(m => m.FirstEntry)
                     .WithMany(e => e.MatchesAsFirst)
                     .HasForeignKey(m => m.FirstEntryId)
                     .OnDelete(DeleteBehavior.Restrict);

                match.HasOne(m => m.SecondEntry)
                     .WithMany(e => e.MatchesAsSecond)
                     .HasForeignKey(m => m.SecondEntryId)
                     .OnDelete(DeleteBehavior.Restrict);

                match.HasIndex(m => new { m.TourneyId, m.Order });
            });

            Seeder.Seed(modelBuilder);
        }
    }
}