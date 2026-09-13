using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
