namespace GildeApp.Api.Dtos.Weapons
{
    // The rule sets belonging to a weapon, without the Weapon back-reference
    // (that would serialize in a loop: weapon -> ruleset -> weapon -> ...).
    public class WeaponRuleSetDto
    {
        public Guid RuleSetId { get; set; }
        public bool HasDoubles { get; set; }
        public int MaxScore { get; set; }
        public int Doubles { get; set; }
    }
}
