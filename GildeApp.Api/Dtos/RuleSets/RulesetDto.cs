using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.Weapons;

namespace GildeApp.Api.Dtos.RuleSets
{
    public class RulesetDto
    {
        public Guid RuleSetId { get; set; }
        public Guid WeaponId { get; set; }
        public WeaponDto Weapon { get; set; }
    }
}
