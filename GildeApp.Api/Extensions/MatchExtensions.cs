using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.Matches;
using GildeApp.Api.Dtos.Players;
using GildeApp.Api.Dtos.Tourneys;

namespace GildeApp.Api.Extensions
{
    public static class MatchExtensions
    {
        public static MatchDto ToDto(this Match match)
        {
            return new MatchDto
            {
                MatchId = match.Id,
                
            };
        }

        public static IEnumerable<MatchDto> ToDto(this IEnumerable<Match> matches)
        {
            return matches.Select(a => a.ToDto());
        }

        public static MatchDetailDto ToDetailDto(this Match match)
        {
            return new MatchDetailDto
            {
                MatchId = match.Id,
                FirstPlayerId = match.FirstPlayerId,
                SecondPlayerId = match.SecondPlayerId,
                FirstPlayerScore = match.FirstPlayerScore,
                SecondPlayerScore = match.SecondPlayerScore,
                TourneyId = match.TourneyId,

                FirstPlayer = new PlayerDto
                {
                    PlayerId = match.FirstPlayer.Id,
                    FirstName = match.FirstPlayer.FirstName,
                    LastName = match.FirstPlayer.LastName
                },
                SecondPlayer = new PlayerDto
                {
                    PlayerId = match.SecondPlayer.Id,
                    FirstName = match.SecondPlayer.FirstName,
                    LastName = match.SecondPlayer.LastName
                },
                Tourney = new TourneyDto
                {
                    TourneyId = match.Tourney.Id,
                    Name = match.Tourney.Name
                }

            };
        }
    }
}
