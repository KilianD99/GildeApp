using System;

namespace GildeApp.Api.Dtos.Board
{
    /// <summary>
    /// One square of the grid, seen from the row player's side. The mirrored cell on
    /// the other side of the diagonal is the same match with the scores swapped.
    /// </summary>
    public class BoardCellDto
    {
        public int OpponentPosition { get; set; }

        /// <summary>True for the blacked-out square where a player meets themselves.</summary>
        public bool IsSelf { get; set; }

        public Guid? MatchId { get; set; }

        /// <summary>The row player's score. Null when the bout has not started.</summary>
        public int? Score { get; set; }

        public int? OpponentScore { get; set; }

        /// <summary>Scheduled, InProgress or Finished. Empty on the self cell.</summary>
        public string Status { get; set; } = string.Empty;
    }
}
