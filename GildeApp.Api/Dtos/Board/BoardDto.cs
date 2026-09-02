using System;
using System.Collections.Generic;

namespace GildeApp.Api.Dtos.Board
{
    /// <summary>
    /// Everything the board screen needs, in one response. Both the web app and the
    /// mobile app read this, so neither one calculates a total or a placement itself
    /// and they cannot disagree about the standings.
    /// </summary>
    public class BoardDto
    {
        public Guid TourneyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string WeaponName { get; set; } = string.Empty;
        public int MaxScore { get; set; }

        public int PlayerCount { get; set; }
        public int MatchesTotal { get; set; }
        public int MatchesFinished { get; set; }

        /// <summary>Ordered by position, which is the order the rows are drawn in.</summary>
        public List<BoardRowDto> Rows { get; set; } = new();

        /// <summary>Every match in running order, for the schedule panel.</summary>
        public List<BoardMatchDto> Matches { get; set; } = new();
    }
}
