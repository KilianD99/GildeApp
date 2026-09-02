using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.RuleSets;
using GildeApp.Api.Dtos.Weapons;

namespace GildeApp.Api.Extensions
{
    public static class RulesetExtensions
    {
        public static RulesetDto ToRuleSetDto(this RuleSet ruleSet)
        {
            return new RulesetDto
            {
                RuleSetId = ruleSet.Id,
                WeaponId = ruleSet.WeaponId,
                Weapon = ruleSet.Weapon.ToWeaponSummary()
            };
        }

        public static IEnumerable<RulesetDto> ToRuleSetListDto(this IEnumerable<RuleSet> ruleSets)
        {
            return ruleSets.Select(a => a.ToRuleSetDto());
        }

        public static RuleSetDetailDto ToDetailRuleSetDto(this RuleSet ruleSet)
        {
            return new RuleSetDetailDto
            {
                RuleSetId = ruleSet.Id,
                WeaponId = ruleSet.WeaponId,
                MaxScore = ruleSet.MaxScore,
                Doubles = ruleSet.Doubles,
                HasDoubles = ruleSet.HasDoubles,
                Weapon = ruleSet.Weapon.ToWeaponSummary()
            };
        }

        public static IEnumerable<RuleSetDetailDto> ToDetailRuleSetListDto(this IEnumerable<RuleSet> ruleSets)
        {
            return ruleSets.Select(a => a.ToDetailRuleSetDto());
        }

        /// <summary>
        /// Null-safe: a rule set loaded without .Include(r => r.Weapon) would otherwise
        /// throw here rather than at the query that forgot the include.
        /// </summary>
        private static WeaponDto ToWeaponSummary(this Weapon? weapon)
        {
            if (weapon is null)
                return new WeaponDto();

            return new WeaponDto
            {
                WeaponId = weapon.Id,
                Name = weapon.Name
            };
        }
    }
}
