using System;
using System.Collections.Generic;

namespace GildeApp.Api.Core.Entities
{
    /// <summary>
    /// One player taking part in one tourney. Carries the position number (the "#"
    /// column on the board), which fixes both the row order and the order the
    /// matches are generated in.
    /// </summary>
    public class TourneyEntry : BaseEntity
    {
        public Guid TourneyId { get; set; }
        public Tourney Tourney { get; set; } = null!;

        public Guid PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        /// <summary>1-based, unique within the tourney.</summary>
        public int Position { get; set; }

        public ICollection<Match> MatchesAsFirst { get; set; } = new List<Match>();
        public ICollection<Match> MatchesAsSecond { get; set; } = new List<Match>();
    }
}
