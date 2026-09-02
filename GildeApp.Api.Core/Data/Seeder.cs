using System;
using GildeApp.Api.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GildeApp.Api.Core.Data
{
    public class Seeder
    {
        // Fixed dates: HasData is baked into a migration, so DateTime.UtcNow here would
        // make EF see a model change on every single build.
        private static readonly DateTime SeedDate = new DateTime(2026, 3, 14, 9, 0, 0, DateTimeKind.Utc);

        public static void Seed(ModelBuilder modelBuilder)
        {
            var foilId = new Guid("44444444-4444-4444-4444-444444444441");
            var epeeId = new Guid("44444444-4444-4444-4444-444444444442");
            var sabreId = new Guid("44444444-4444-4444-4444-444444444443");

            var standardRulesId = new Guid("11111111-1111-1111-1111-111111111111");
            var springTourneyId = new Guid("22222222-2222-2222-2222-222222222222");

            // Anonymous objects throughout: HasData refuses an entity instance whose
            // navigation properties are set, and several of these initialise their
            // collections inline.
            modelBuilder.Entity<Weapon>().HasData(
                new { Id = foilId, Name = "Foil" },
                new { Id = epeeId, Name = "Épée" },
                new { Id = sabreId, Name = "Sabre" }
            );

            modelBuilder.Entity<RuleSet>().HasData(
                new
                {
                    Id = standardRulesId,
                    HasDoubles = false,
                    MaxScore = 5,
                    Doubles = 0,
                    WeaponId = foilId
                }
            );

            modelBuilder.Entity<Tourney>().HasData(
                new
                {
                    Id = springTourneyId,
                    Name = "Spring Tourney 2026",
                    RuleSetId = standardRulesId,
                    Status = TourneyStatus.Running,
                    CreatedAt = SeedDate
                }
            );

            // Players exist on their own, so the same four can be entered again next season.
            var janId = new Guid("33333333-3333-3333-3333-333333333331");
            var marieId = new Guid("33333333-3333-3333-3333-333333333332");
            var larsId = new Guid("33333333-3333-3333-3333-333333333333");
            var sofieId = new Guid("33333333-3333-3333-3333-333333333334");

            modelBuilder.Entity<Player>().HasData(
                new { Id = janId, FirstName = "Jan", LastName = "Peeters" },
                new { Id = marieId, FirstName = "Marie", LastName = "Janssens" },
                new { Id = larsId, FirstName = "Lars", LastName = "De Vos" },
                new { Id = sofieId, FirstName = "Sofie", LastName = "Maes" }
            );

            var entry1 = new Guid("66666666-6666-6666-6666-666666666661");
            var entry2 = new Guid("66666666-6666-6666-6666-666666666662");
            var entry3 = new Guid("66666666-6666-6666-6666-666666666663");
            var entry4 = new Guid("66666666-6666-6666-6666-666666666664");

            modelBuilder.Entity<TourneyEntry>().HasData(
                new { Id = entry1, TourneyId = springTourneyId, PlayerId = janId, Position = 1 },
                new { Id = entry2, TourneyId = springTourneyId, PlayerId = marieId, Position = 2 },
                new { Id = entry3, TourneyId = springTourneyId, PlayerId = larsId, Position = 3 },
                new { Id = entry4, TourneyId = springTourneyId, PlayerId = sofieId, Position = 4 }
            );

            // Six bouts for four players. Two already fenced, one running, three to come:
            // enough to see every board state without touching the database by hand.
            modelBuilder.Entity<Match>().HasData(
                new
                {
                    Id = new Guid("55555555-5555-5555-5555-555555555551"),
                    TourneyId = springTourneyId,
                    FirstEntryId = entry1,
                    SecondEntryId = entry4,
                    FirstScore = 5,
                    SecondScore = 3,
                    Status = MatchStatus.Finished,
                    Order = 1,
                    UpdatedAt = (DateTime?)SeedDate
                },
                new
                {
                    Id = new Guid("55555555-5555-5555-5555-555555555552"),
                    TourneyId = springTourneyId,
                    FirstEntryId = entry2,
                    SecondEntryId = entry3,
                    FirstScore = 2,
                    SecondScore = 5,
                    Status = MatchStatus.Finished,
                    Order = 2,
                    UpdatedAt = (DateTime?)SeedDate
                },
                new
                {
                    Id = new Guid("55555555-5555-5555-5555-555555555553"),
                    TourneyId = springTourneyId,
                    FirstEntryId = entry4,
                    SecondEntryId = entry3,
                    FirstScore = 3,
                    SecondScore = 4,
                    Status = MatchStatus.InProgress,
                    Order = 3,
                    UpdatedAt = (DateTime?)SeedDate
                },
                new
                {
                    Id = new Guid("55555555-5555-5555-5555-555555555554"),
                    TourneyId = springTourneyId,
                    FirstEntryId = entry1,
                    SecondEntryId = entry2,
                    FirstScore = 0,
                    SecondScore = 0,
                    Status = MatchStatus.Scheduled,
                    Order = 4,
                    UpdatedAt = (DateTime?)null
                },
                new
                {
                    Id = new Guid("55555555-5555-5555-5555-555555555555"),
                    TourneyId = springTourneyId,
                    FirstEntryId = entry2,
                    SecondEntryId = entry4,
                    FirstScore = 0,
                    SecondScore = 0,
                    Status = MatchStatus.Scheduled,
                    Order = 5,
                    UpdatedAt = (DateTime?)null
                },
                new
                {
                    Id = new Guid("55555555-5555-5555-5555-555555555556"),
                    TourneyId = springTourneyId,
                    FirstEntryId = entry3,
                    SecondEntryId = entry1,
                    FirstScore = 0,
                    SecondScore = 0,
                    Status = MatchStatus.Scheduled,
                    Order = 6,
                    UpdatedAt = (DateTime?)null
                }
            );
        }
    }
}
