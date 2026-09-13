using System.ComponentModel.DataAnnotations;
using GildeApp.Api.Core.Entities;

namespace GildeApp.Api.Dtos.Matches 
{
    public class MatchCreateOrUpdateDto
    {
        [Range(0, 999)]
        public int FirstScore { get; set; }

        [Range(0, 999)]
        public int SecondScore { get; set; }
        public bool Finish { get; set; }
    }
}
