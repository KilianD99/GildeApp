using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.Players;

namespace GildeApp.Api.Extensions
{
    public static class PlayerExtensions
    {
        public static PlayerDto ToPlayerDto(this Player player)
        {
            return new PlayerDto
            {
                PlayerId = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                FullName = player.FullName
            };
        }

        public static IEnumerable<PlayerDto> ToPlayerListDto(this IEnumerable<Player> players)
        {
            return players.Select(p => p.ToPlayerDto());
        }

        public static PlayerDetailDto ToDetailPlayerDto(this Player player)
        {
            return new PlayerDetailDto
            {
                PlayerId = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                FullName = player.FullName,
                History = (player.Entries ?? new List<TourneyEntry>())
                    .Select(e => new PlayerHistoryDto
                    {
                        TourneyId = e.TourneyId,
                        TourneyName = e.Tourney?.Name ?? string.Empty,
                        Status = e.Tourney?.Status.ToString() ?? string.Empty,
                        Position = e.Position
                    })
                    .OrderBy(h => h.TourneyName)
                    .ToList()
            };
        }
    }
}
