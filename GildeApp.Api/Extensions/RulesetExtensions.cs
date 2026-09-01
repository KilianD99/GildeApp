using GildeApp.Api.Core.Entities;
using GildeApp.Api.Dtos.Players;
using GildeApp.Api.Dtos.RuleSets;
using GildeApp.Api.Dtos.Tourneys;
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
                Weapon = new WeaponDto
                {
                    WeaponId = ruleSet.Weapon.Id,
                    Name = ruleSet.Weapon.Name
                }
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

                Weapon = new WeaponDto
                {
                    WeaponId = ruleSet.Weapon.Id,
                    Name = ruleSet.Weapon.Name
                }

            };
        }
    }
}
