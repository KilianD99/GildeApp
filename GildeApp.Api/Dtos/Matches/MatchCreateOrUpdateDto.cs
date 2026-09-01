using GildeApp.Api.Core.Entities;

namespace GildeApp.Api.Dtos.Matches 
{
    public class MatchCreateOrUpdateDto
    {
        public Guid? MatchId { get; set; } 
        public int FirstPlayerScore { get; set; }
        public int SecondPlayerScore { get; set; }
        public Guid FirstPlayerId { get; set; }
        public Guid SecondPlayerId { get; set; }
        public Guid TourneyId { get; set; }
    }
}
