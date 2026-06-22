using System;
using Microsoft.EntityFrameworkCore;
using GildeApp.Api.Core.Entities;

namespace GildeApp.Api.Core.Data
{
    public class Seeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var standardRulesId = new Guid("11111111-1111-1111-1111-111111111111");
            var springTourneyId = new Guid("22222222-2222-2222-2222-222222222222");

            var janId = new Guid("33333333-3333-3333-3333-333333333331");
            var marieId = new Guid("33333333-3333-3333-3333-333333333332");
            var larsId = new Guid("33333333-3333-3333-3333-333333333333");
            var sofieId = new Guid("33333333-3333-3333-3333-333333333334");

            modelBuilder.Entity<RuleSet>().HasData(
                new RuleSet
                {
                    Id = standardRulesId,
                    HasDoubles = false,
                    MaxScore = 3,   
                    Doubles = 0,
                    WeaponId = new Guid("44444444-4444-4444-4444-444444444441")
                }
            );

            modelBuilder.Entity<Tourney>().HasData(
                new Tourney
                {
                    Id = springTourneyId,
                    Name = "Spring Tourney 2026",
                    RuleSetId = standardRulesId
                }
            );

            modelBuilder.Entity<Player>().HasData(
                new Player { Id = janId, FirstName = "Jan", LastName = "Peeters", TourneyId = springTourneyId },
                new Player { Id = marieId, FirstName = "Marie", LastName = "Janssens", TourneyId = springTourneyId },
                new Player { Id = larsId, FirstName = "Lars", LastName = "De Vos", TourneyId = springTourneyId },
                new Player { Id = sofieId, FirstName = "Sofie", LastName = "Maes", TourneyId = springTourneyId }
            );

            modelBuilder.Entity<Match>().HasData(
                new Match
                {
                    Id = new Guid("55555555-5555-5555-5555-555555555551"),
                    TourneyId = springTourneyId,
                    FirstPlayerId = janId,
                    SecondPlayerId = marieId,
                    FirstPlayerScore = 2,
                    SecondPlayerScore = 1
                },
                new Match
                {
                    Id = new Guid("55555555-5555-5555-5555-555555555552"),
                    TourneyId = springTourneyId,
                    FirstPlayerId = larsId,
                    SecondPlayerId = sofieId,
                    FirstPlayerScore = 2,
                    SecondPlayerScore = 3
                }
            );

            modelBuilder.Entity<Weapon>().HasData(
                new Weapon { Id = new Guid("44444444-4444-4444-4444-444444444441"), Name = "Foil" },
                new Weapon { Id = new Guid("44444444-4444-4444-4444-444444444442"), Name = "Épée" },
                new Weapon { Id = new Guid("44444444-4444-4444-4444-444444444443"), Name = "Sabre" }
            );
        }
    }
}