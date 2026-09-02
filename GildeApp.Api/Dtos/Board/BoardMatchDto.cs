using System;

namespace GildeApp.Api.Dtos.Board
{
    public class BoardMatchDto
    {
        public Guid MatchId { get; set; }
        public int Order { get; set; }
        public string Status { get; set; } = string.Empty;

        public int FirstPosition { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public int FirstScore { get; set; }

        public int SecondPosition { get; set; }
        public string SecondName { get; set; } = string.Empty;
        public int SecondScore { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
