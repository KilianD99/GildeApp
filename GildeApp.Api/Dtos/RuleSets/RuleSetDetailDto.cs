namespace GildeApp.Api.Dtos.RuleSets
{
    public class RuleSetDetailDto : RulesetDto
    {
        public bool HasDoubles { get; set; }
        public int MaxScore { get; set; }
        public int Doubles { get; set; }
    }
}
