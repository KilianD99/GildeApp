using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.Matches;

namespace GildeApp.Api.Extensions
{
    public static class MatchExtensions
    {
        public static MatchDto ToMatchDto(this Match match)
        {
            return new MatchDto
            {
                MatchId = match.Id,
                TourneyId = match.TourneyId,
                Order = match.Order,
                Status = match.Status.ToString(),

                FirstEntryId = match.FirstEntryId,
                FirstPosition = match.FirstEntry?.Position ?? 0,
                FirstName = match.FirstEntry?.Player?.FullName ?? string.Empty,
                FirstScore = match.FirstScore,

                SecondEntryId = match.SecondEntryId,
                SecondPosition = match.SecondEntry?.Position ?? 0,
                SecondName = match.SecondEntry?.Player?.FullName ?? string.Empty,
                SecondScore = match.SecondScore,

                UpdatedAt = match.UpdatedAt
            };
        }

        public static IEnumerable<MatchDto> ToMatchListDto(this IEnumerable<Match> matches)
        {
            return matches.Select(m => m.ToMatchDto());
        }

        public static MatchDetailDto ToDetailMatchDto(this Match match)
        {
            return new MatchDetailDto
            {
                MatchId = match.Id,
                TourneyId = match.TourneyId,
                Order = match.Order,
                Status = match.Status.ToString(),

                FirstEntryId = match.FirstEntryId,
                FirstPosition = match.FirstEntry?.Position ?? 0,
                FirstName = match.FirstEntry?.Player?.FullName ?? string.Empty,
                FirstScore = match.FirstScore,

                SecondEntryId = match.SecondEntryId,
                SecondPosition = match.SecondEntry?.Position ?? 0,
                SecondName = match.SecondEntry?.Player?.FullName ?? string.Empty,
                SecondScore = match.SecondScore,

                UpdatedAt = match.UpdatedAt,

                TourneyName = match.Tourney?.Name ?? string.Empty,
                MaxScore = match.Tourney?.RuleSet?.MaxScore ?? 0
            };
        }
    }
}
