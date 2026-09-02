using System;

namespace GildeApp.Api.Core.Entities
{
    /// <summary>
    /// One bout between two entries in the same tourney. A match is stored exactly
    /// once; the board renders it twice (once from each fencer's side), so there is
    /// never a second copy of the same score to keep in sync.
    /// </summary>
    public class Match : BaseEntity
    {
        public Guid TourneyId { get; set; }
        public Tourney Tourney { get; set; } = null!;

        public Guid FirstEntryId { get; set; }
        public TourneyEntry FirstEntry { get; set; } = null!;

        public Guid SecondEntryId { get; set; }
        public TourneyEntry SecondEntry { get; set; } = null!;

        public int FirstScore { get; set; }
        public int SecondScore { get; set; }

        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

        /// <summary>Position in the running order of the tourney, 1-based.</summary>
        public int Order { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
