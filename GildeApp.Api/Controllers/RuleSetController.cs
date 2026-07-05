using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;
using GildeApp.Api.Dtos.Players;
using GildeApp.Api.Dtos.RuleSets;
using GildeApp.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GildeApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuleSetController : ControllerBase
    {
        protected readonly IRuleSetService _ruleSetService;

        public RuleSetController(IRuleSetService ruleSetService)
        {
            _ruleSetService = ruleSetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _ruleSetService.ListAllAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var dtos = result.Data.ToRuleSetListDto();
            return Ok(new ResultModel<List<RulesetDto>> { Data = dtos.ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _ruleSetService.GetByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            var dto = result.Data.ToDetailRuleSetDto();
            return Ok(new ResultModel<RuleSetDetailDto> { Data = dto });
        }

        [HttpPost]
        public async Task<IActionResult> Add(RuleSetCreateOrUpdateDto ruleSetDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ruleSet = new RuleSet
            {
                Id = ruleSetDto.RuleSetId,
                MaxScore = ruleSetDto.MaxScore,
                Doubles = ruleSetDto.Doubles,
                HasDoubles = ruleSetDto.HasDoubles,
                WeaponId = ruleSetDto.WeaponId,
            };

            var result = await _ruleSetService.AddAsync(ruleSet);

            if (result.IsSuccess)
            {
                var createdPlayer = await _ruleSetService.GetByIdAsync(ruleSet.Id);

                if (createdPlayer.IsSuccess)
                {
                    var dto = createdPlayer.Data.ToDetailRuleSetDto();
                    return CreatedAtAction(nameof(GetById), new { id = ruleSet.Id }, new ResultModel<RuleSetDetailDto> { Data = dto });
                }
            }

            return BadRequest(result.Errors);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, RuleSetCreateOrUpdateDto ruleSetListDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _ruleSetService.DoesRuleSetIdExistsAsync(id) == false)
                return NotFound(new { message = $"No ruleset with id '{id}' found" });

            var existingRuleSetResult = await _ruleSetService.GetByIdAsync(id);

            if (!existingRuleSetResult.IsSuccess)
                return BadRequest(existingRuleSetResult.Errors);

            var existingRuleSet = existingRuleSetResult.Data;
            existingRuleSet.Id = id;
            existingRuleSet.WeaponId = ruleSetListDto.WeaponId;
            existingRuleSet.Doubles = ruleSetListDto.Doubles;
            existingRuleSet.HasDoubles = ruleSetListDto.HasDoubles;
            existingRuleSet.MaxScore = ruleSetListDto.MaxScore;
            

            var result = await _ruleSetService.UpdateAsync(existingRuleSet);

            if (result.IsSuccess)
            {
                var updatedRuleSet = await _ruleSetService.GetByIdAsync(id);

                if (updatedRuleSet.IsSuccess)
                {
                    var dto = updatedRuleSet.Data.ToDetailRuleSetDto();
                    return Ok(new ResultModel<RuleSetDetailDto> { Data = dto });
                }
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _ruleSetService.DoesRuleSetIdExistsAsync(id) == false)
                return NotFound(new { message = $"No ruleSet with an id of {id}" });

            var existingRuleSet = await _ruleSetService.GetByIdAsync(id);

            if (!existingRuleSet.IsSuccess)
                return BadRequest(existingRuleSet.Errors);



            var result = await _ruleSetService.DeleteAsync(existingRuleSet.Data);

            if (result.IsSuccess)
                return Ok(new { message = $"RuleSet {existingRuleSet.Data.Id} deleted successfully" });

            return BadRequest(result.Errors);
        }
    }
}
