using GildeApp.Api.Dtos.Matches;
using GildeApp.Api.Dtos.Players;
using GildeApp.Api.Dtos.RuleSets;

namespace GildeApp.Api.Dtos.Tourneys
{
    public class TourneyDetailDto : TourneyDto
    {
        public Guid RuleSetId { get; set; }
        public RulesetDto RuleSet { get; set; } = null!;
        public IEnumerable<PlayerDto> Players { get; set; } = new List<PlayerDto>();
        public IEnumerable<MatchDto> Matches { get; set; } = new List<MatchDto>();
    }
}
