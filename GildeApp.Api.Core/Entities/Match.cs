using System;
using System.Collections.Generic;
using System.Text;

namespace GildeApp.Api.Core.Entities
{
    public class Match : BaseEntity
    {
        public int FirstPlayerScore { get; set; }
        public int SecondPlayerScore { get; set; }

        public Guid FirstPlayerId { get; set; }
        public Player FirstPlayer { get; set; }

        public Guid SecondPlayerId { get; set; }
        public Player SecondPlayer { get; set; }

        public Guid TourneyId { get; set; }
        public Tourney Tourney { get; set; }
    }
}
