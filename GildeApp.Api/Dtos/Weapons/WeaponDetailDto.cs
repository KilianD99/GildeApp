namespace GildeApp.Api.Dtos.Weapons
{
    public class WeaponDetailDto : WeaponDto
    {
        public IEnumerable<WeaponRuleSetDto> RuleSets { get; set; } = new List<WeaponRuleSetDto>();
    }
}
