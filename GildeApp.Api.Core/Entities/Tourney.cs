using System;
using System.Collections.Generic;

namespace GildeApp.Api.Core.Entities
{
    public class Tourney : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public TourneyStatus Status { get; set; } = TourneyStatus.Setup;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid RuleSetId { get; set; }
        public RuleSet RuleSet { get; set; } = null!;

        public ICollection<TourneyEntry> Entries { get; set; } = new List<TourneyEntry>();
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
