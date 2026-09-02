using System;
using GildeApp.Api.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GildeApp.Api.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<RuleSet> RuleSets { get; set; } = null!;
        public DbSet<Tourney> Tourneys { get; set; } = null!;
        public DbSet<TourneyEntry> TourneyEntries { get; set; } = null!;
        public DbSet<Weapon> Weapons { get; set; } = null!;
        public DbSet<Match> Matches { get; set; } = null!;

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
                // Restrict, not cascade: matches point at entries, so the rows have to
                // come out in order. TourneyService.DeleteAsync does that explicitly.
                entry.HasOne(e => e.Tourney)
                     .WithMany(t => t.Entries)
                     .HasForeignKey(e => e.TourneyId)
                     .OnDelete(DeleteBehavior.Restrict);

                entry.HasOne(e => e.Player)
                     .WithMany(p => p.Entries)
                     .HasForeignKey(e => e.PlayerId)
                     .OnDelete(DeleteBehavior.Restrict);

                // A position is used once per tourney, and a player enters a tourney once.
                entry.HasIndex(e => new { e.TourneyId, e.Position }).IsUnique();
                entry.HasIndex(e => new { e.TourneyId, e.PlayerId }).IsUnique();
            });

            modelBuilder.Entity<Match>(match =>
            {
                match.Property(m => m.Status).HasConversion<int>();

                match.HasOne(m => m.Tourney)
                     .WithMany(t => t.Matches)
                     .HasForeignKey(m => m.TourneyId)
                     .OnDelete(DeleteBehavior.Restrict);

                // Restrict on both sides: SQL Server refuses multiple cascade paths
                // from TourneyEntry back to the same Match row.
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
