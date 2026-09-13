using GildeApp.Api.Dtos.Tourneys;

namespace GildeApp.Api.Dtos.Players
{
    public class PlayerDetailDto : PlayerDto
    {
        public IEnumerable<PlayerHistoryDto> History { get; set; } = new List<PlayerHistoryDto>();
    }
}
