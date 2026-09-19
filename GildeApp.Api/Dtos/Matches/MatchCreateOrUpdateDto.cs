using System.ComponentModel.DataAnnotations;

namespace GildeApp.Api.Dtos.Matches
{
    public class MatchCreateOrUpdateDto
    {
        [Range(0, 999)]
        public int FirstScore { get; set; }

        [Range(0, 999)]
        public int SecondScore { get; set; }

        /// <summary>
        /// False while the bout is running, so the web board shows a live score.
        /// True locks the result in and hands the match back.
        /// </summary>
        public bool Finish { get; set; }
    }
}
