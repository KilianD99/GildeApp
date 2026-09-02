using System;
using System.Collections.Generic;

namespace GildeApp.Api.Dtos.Board
{
    public class BoardRowDto
    {
        public Guid EntryId { get; set; }
        public Guid PlayerId { get; set; }
        public int Position { get; set; }
        public string Name { get; set; } = string.Empty;

        /// <summary>All the touches this player scored, added up.</summary>
        public int TotalScore { get; set; }

        /// <summary>All the touches scored against them. Breaks ties on total score.</summary>
        public int TotalReceived { get; set; }

        public int MatchesPlayed { get; set; }

        /// <summary>1 = highest total score.</summary>
        public int Placement { get; set; }

        /// <summary>One cell per position, in position order, including this player's own.</summary>
        public List<BoardCellDto> Cells { get; set; } = new();
    }
}
