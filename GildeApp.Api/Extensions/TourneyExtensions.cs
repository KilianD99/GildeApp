using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.Entries;
using GildeApp.Api.Dtos.RuleSets;
using GildeApp.Api.Dtos.Tourneys;
using GildeApp.Api.Dtos.Weapons;

namespace GildeApp.Api.Extensions
{
    public static class TourneyExtensions
    {
        public static TourneyDto ToTourneyDto(this Tourney tourney)
        {
            return new TourneyDto
            {
                TourneyId = tourney.Id,
                Name = tourney.Name,
                Status = tourney.Status.ToString(),
                CreatedAt = tourney.CreatedAt,
                WeaponName = tourney.RuleSet?.Weapon?.Name ?? string.Empty,
                PlayerCount = tourney.Entries?.Count ?? 0,
                MatchesTotal = tourney.Matches?.Count ?? 0,
                MatchesFinished = tourney.Matches?.Count(m => m.Status == MatchStatus.Finished) ?? 0
            };
        }

        public static IEnumerable<TourneyDto> ToTourneyListDto(this IEnumerable<Tourney> tourneys)
        {
            return tourneys.Select(t => t.ToTourneyDto());
        }

        public static TourneyDetailDto ToDetailTourneyDto(this Tourney tourney)
        {
            return new TourneyDetailDto
            {
                TourneyId = tourney.Id,
                Name = tourney.Name,
                Status = tourney.Status.ToString(),
                CreatedAt = tourney.CreatedAt,
                WeaponName = tourney.RuleSet?.Weapon?.Name ?? string.Empty,
                PlayerCount = tourney.Entries?.Count ?? 0,
                MatchesTotal = tourney.Matches?.Count ?? 0,
                MatchesFinished = tourney.Matches?.Count(m => m.Status == MatchStatus.Finished) ?? 0,
                RuleSetId = tourney.RuleSetId,
                RuleSet = tourney.RuleSet is null ? null : new RuleSetDetailDto
                {
                    RuleSetId = tourney.RuleSet.Id,
                    WeaponId = tourney.RuleSet.WeaponId,
                    MaxScore = tourney.RuleSet.MaxScore,
                    Doubles = tourney.RuleSet.Doubles,
                    HasDoubles = tourney.RuleSet.HasDoubles,
                    Weapon = tourney.RuleSet.Weapon is null ? new WeaponDto() : new WeaponDto
                    {
                        WeaponId = tourney.RuleSet.Weapon.Id,
                        Name = tourney.RuleSet.Weapon.Name
                    }
                },
                Entries = (tourney.Entries ?? new List<TourneyEntry>())
                    .OrderBy(e => e.Position)
                    .Select(e => e.ToEntryDto())
                    .ToList()
            };
        }

        public static TourneyEntryDto ToEntryDto(this TourneyEntry entry)
        {
            return new TourneyEntryDto
            {
                EntryId = entry.Id,
                TourneyId = entry.TourneyId,
                PlayerId = entry.PlayerId,
                Name = entry.Player?.FullName ?? string.Empty,
                Position = entry.Position
            };
        }
    }
}
