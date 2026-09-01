using GildeApp.Api.Dtos.Tourneys;

namespace GildeApp.Api.Dtos.Players
{
    public class PlayerDetailDto : PlayerDto
    {
        public Guid TourneyId { get; set; }
        public TourneyDto Tourney { get; set; } = null!;
    }
}
