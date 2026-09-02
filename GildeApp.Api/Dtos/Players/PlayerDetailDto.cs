using System;
using System.Collections.Generic;

namespace GildeApp.Api.Dtos.Players
{
    public class PlayerDetailDto : PlayerDto
    {
        /// <summary>Every tourney this player has been entered in.</summary>
        public IEnumerable<PlayerHistoryDto> History { get; set; } = new List<PlayerHistoryDto>();
    }

    public class PlayerHistoryDto
    {
        public Guid TourneyId { get; set; }
        public string TourneyName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Position { get; set; }
    }
}
