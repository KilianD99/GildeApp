using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.Players;
using GildeApp.Api.Dtos.Tourneys;

namespace GildeApp.Api.Dtos.Matches
{
    public class MatchDetailDto : MatchDto
    {
        public int FirstPlayerScore { get; set; }
        public int SecondPlayerScore { get; set; }

        public Guid FirstPlayerId { get; set; }
        public PlayerDto FirstPlayer { get; set; } = null!;

        public Guid SecondPlayerId { get; set; }
        public PlayerDto SecondPlayer { get; set; } = null!;

        public Guid TourneyId { get; set; }
        public TourneyDto Tourney { get; set; } = null!;
    }
}
