using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.Players;
using GildeApp.Api.Dtos.Tourneys;

namespace GildeApp.Api.Dtos.Matches
{
    public class MatchDetailDto : MatchDto
    {
        public string TourneyName { get; set; } = string.Empty;
        public int MaxScore { get; set; }
    }
}
