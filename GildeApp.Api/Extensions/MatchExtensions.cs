using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.Matches;

namespace GildeApp.Api.Extensions
{
    public static class MatchExtensions
    {
        public static MatchDto ToMatchDto(this Match match)
        {
            var now = DateTime.UtcNow;
            var isClaimed = match.IsClaimedAt(now);

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

                UpdatedAt = match.UpdatedAt,

                // A lapsed claim reads as free, so a stale row never looks taken.
                ClaimedBy = isClaimed ? match.ClaimedBy : null,
                IsClaimed = isClaimed
            };
        }

        public static IEnumerable<MatchDto> ToMatchListDto(this IEnumerable<Match> matches)
        {
            return matches.Select(m => m.ToMatchDto());
        }

        public static MatchDetailDto ToDetailMatchDto(this Match match)
        {
            var now = DateTime.UtcNow;
            var isClaimed = match.IsClaimedAt(now);

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

                ClaimedBy = isClaimed ? match.ClaimedBy : null,
                IsClaimed = isClaimed,

                TourneyName = match.Tourney?.Name ?? string.Empty,
                MaxScore = match.Tourney?.RuleSet?.MaxScore ?? 0
            };
        }
    }
}
