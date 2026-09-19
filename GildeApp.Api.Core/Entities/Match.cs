using System;

namespace GildeApp.Api.Core.Entities
{
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

        public int Order { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // ---- the claim -----------------------------------------------------
        //
        // Only one judge scores a match at a time. The claim is a name plus an
        // expiry: the app renews it while the scoring screen is open, so a judge
        // whose phone dies frees the match on its own after a couple of minutes
        // instead of blocking it forever.

        /// <summary>Name of the judge currently on this match, if any.</summary>
        public string? ClaimedBy { get; set; }

        /// <summary>When the claim lapses. Past this point anyone may take it.</summary>
        public DateTime? ClaimExpiresAt { get; set; }

        /// <summary>True while a judge still holds this match.</summary>
        public bool IsClaimedAt(DateTime utcNow) =>
            ClaimedBy is not null && ClaimExpiresAt is not null && ClaimExpiresAt > utcNow;
    }
}
