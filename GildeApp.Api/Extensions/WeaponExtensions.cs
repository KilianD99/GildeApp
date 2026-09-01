using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.Weapons;

namespace GildeApp.Api.Extensions
{
    public static class WeaponExtensions
    {
        public static WeaponDto ToWeaponDto(this Weapon weapon)
        {
            return new WeaponDto
            {
                WeaponId = weapon.Id,
                Name = weapon.Name
            };
        }

        public static IEnumerable<WeaponDto> ToWeaponListDto(this IEnumerable<Weapon> weapons)
        {
            return weapons.Select(w => w.ToWeaponDto());
        }

        public static WeaponDetailDto ToDetailWeaponDto(this Weapon weapon)
        {
            return new WeaponDetailDto
            {
                WeaponId = weapon.Id,
                Name = weapon.Name,
                RuleSets = (weapon.RuleSets ?? new List<RuleSet>())
                    .Select(r => new WeaponRuleSetDto
                    {
                        RuleSetId = r.Id,
                        HasDoubles = r.HasDoubles,
                        MaxScore = r.MaxScore,
                        Doubles = r.Doubles
                    })
                    .ToList()
            };
        }
    }
}
