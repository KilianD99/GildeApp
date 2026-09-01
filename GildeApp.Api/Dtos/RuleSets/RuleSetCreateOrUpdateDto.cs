namespace GildeApp.Api.Dtos.RuleSets
{
    public class RuleSetCreateOrUpdateDto
    {
        public bool HasDoubles { get; set; }
        public int MaxScore { get; set; }
        public int Doubles { get; set; }
        public Guid RuleSetId { get; set; }
        public Guid WeaponId { get; set; }
    }
}
