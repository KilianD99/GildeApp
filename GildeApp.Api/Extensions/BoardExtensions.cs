using System;
using System.Collections.Generic;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.Board;

namespace GildeApp.Api.Extensions
{
    /// <summary>
    /// Turns a fully loaded tourney into the board. The grid is never stored: every
    /// cell here is read back off the match rows, and every total is added up fresh,
    /// so there is exactly one copy of each score in the system.
    /// </summary>
    public static class BoardExtensions
    {
        public static BoardDto ToBoardDto(this Tourney tourney)
        {
            var entries = tourney.Entries
                .OrderBy(e => e.Position)
                .ToList();

            var matches = tourney.Matches
                .OrderBy(m => m.Order)
                .ToList();

            var positionByEntryId = entries.ToDictionary(e => e.Id, e => e.Position);

            var rows = new List<BoardRowDto>();

            foreach (var entry in entries)
            {
                var row = new BoardRowDto
                {
                    EntryId = entry.Id,
                    PlayerId = entry.PlayerId,
                    Position = entry.Position,
                    Name = entry.Player?.FullName ?? string.Empty
                };

                foreach (var opponent in entries)
                {
                    if (opponent.Id == entry.Id)
                    {
                        row.Cells.Add(new BoardCellDto
                        {
                            OpponentPosition = opponent.Position,
                            IsSelf = true
                        });
                        continue;
                    }

                    var match = matches.FirstOrDefault(m =>
                        (m.FirstEntryId == entry.Id && m.SecondEntryId == opponent.Id) ||
                        (m.SecondEntryId == entry.Id && m.FirstEntryId == opponent.Id));

                    var cell = new BoardCellDto
                    {
                        OpponentPosition = opponent.Position,
                        IsSelf = false,
                        MatchId = match?.Id,
                        Status = match?.Status.ToString() ?? string.Empty
                    };

                    if (match is not null && match.Status != MatchStatus.Scheduled)
                    {
                        // Read the match from this row's side of the diagonal.
                        var isFirst = match.FirstEntryId == entry.Id;

                        cell.Score = isFirst ? match.FirstScore : match.SecondScore;
                        cell.OpponentScore = isFirst ? match.SecondScore : match.FirstScore;

                        row.TotalScore += cell.Score.Value;
                        row.TotalReceived += cell.OpponentScore.Value;
                        row.MatchesPlayed++;
                    }

                    row.Cells.Add(cell);
                }

                rows.Add(row);
            }

            // Placement: most touches scored wins. Fewest received breaks a tie, and
            // position number breaks it after that so the order is always stable.
            var ranked = rows
                .OrderByDescending(r => r.TotalScore)
                .ThenBy(r => r.TotalReceived)
                .ThenBy(r => r.Position)
                .ToList();

            for (var i = 0; i < ranked.Count; i++)
                ranked[i].Placement = i + 1;

            return new BoardDto
            {
                TourneyId = tourney.Id,
                Name = tourney.Name,
                Status = tourney.Status.ToString(),
                WeaponName = tourney.RuleSet?.Weapon?.Name ?? string.Empty,
                MaxScore = tourney.RuleSet?.MaxScore ?? 0,
                PlayerCount = entries.Count,
                MatchesTotal = matches.Count,
                MatchesFinished = matches.Count(m => m.Status == MatchStatus.Finished),
                Rows = rows,
                Matches = matches.Select(m => new BoardMatchDto
                {
                    MatchId = m.Id,
                    Order = m.Order,
                    Status = m.Status.ToString(),
                    FirstPosition = positionByEntryId.TryGetValue(m.FirstEntryId, out var fp) ? fp : 0,
                    FirstName = m.FirstEntry?.Player?.FullName ?? string.Empty,
                    FirstScore = m.FirstScore,
                    SecondPosition = positionByEntryId.TryGetValue(m.SecondEntryId, out var sp) ? sp : 0,
                    SecondName = m.SecondEntry?.Player?.FullName ?? string.Empty,
                    SecondScore = m.SecondScore,
                    UpdatedAt = m.UpdatedAt
                }).ToList()
            };
        }
    }
}
