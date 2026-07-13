using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.Tourneys;

namespace GildeApp.Api.Extensions
{
    public static class TourneyExtensions
    {
        public static TourneyDto ToTourneyDto(this Tourney tourney)
        {
            return new TourneyDto
            {
                TourneyId = tourney.Id,
                Name = tourney.Name
            };
        }

        public static IEnumerable<TourneyDto> ToTourneyDetailDto(this IEnumerable<Tourney> tourneys)
        {
            return tourneys.Select(t => t.ToTourneyDto());
        }

        public static TourneyDetailDto ToDetaiTourneylDto(this Tourney tourney)
        {
            return new TourneyDetailDto
            {
                TourneyId = tourney.Id,
                Name = tourney.Name,
                RuleSetId = tourney.RuleSetId,
                RuleSet = tourney.RuleSet.ToRuleSetDto(),
                Players = tourney.Players.Select(p => p.ToDetailPlayerDto()),
                Matches = tourney.Matches.Select(m => m.ToDetailMatchDto())
            };
        }
    }
}
