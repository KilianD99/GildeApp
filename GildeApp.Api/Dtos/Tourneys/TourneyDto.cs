using System;

namespace GildeApp.Api.Dtos.Tourneys
{
    public class TourneyDto
    {
        public Guid TourneyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string WeaponName { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
        public int MatchesTotal { get; set; }
        public int MatchesFinished { get; set; }
    }
}
