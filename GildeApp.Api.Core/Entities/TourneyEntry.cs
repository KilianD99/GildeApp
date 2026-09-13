using System;
using System.Collections.Generic;
using System.Text;

namespace GildeApp.Api.Core.Entities
{
    public class TourneyEntry : BaseEntity
    {
        public Guid TourneyId { get; set; }
        public Tourney Tourney { get; set; } = null!;

        public Guid PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        public int Position { get; set; }

        public ICollection<Match> MatchesAsFirst { get; set; } = new List<Match>();
        public ICollection<Match> MatchesAsSecond { get; set; } = new List<Match>();
    }   
}
