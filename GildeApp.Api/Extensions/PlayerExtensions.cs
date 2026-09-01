using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.Matches;
using GildeApp.Api.Dtos.Players;
using GildeApp.Api.Dtos.Tourneys;

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
                LastName = player.LastName
            };
        }

        public static IEnumerable<PlayerDto> ToPlayerListDto(this IEnumerable<Player> players)
        {
            return players.Select(a => a.ToPlayerDto());
        }

        public static PlayerDetailDto ToDetailPlayerDto(this Player player)
        {
            return new PlayerDetailDto
            {
                PlayerId = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                TourneyId = player.TourneyId,

                Tourney = new TourneyDto
                {
                    TourneyId = player.Tourney.Id,
                    Name = player.Tourney.Name
                }

            };
        }
    }
}
