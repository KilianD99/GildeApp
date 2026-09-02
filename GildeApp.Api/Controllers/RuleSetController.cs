using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;
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

        /// <summary>
        /// Returns the full rule set, not the summary: the tourney create screen needs
        /// MaxScore to label the options.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _ruleSetService.ListAllAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var dtos = result.Data.ToDetailRuleSetListDto();
            return Ok(new ResultModel<List<RuleSetDetailDto>> { Data = dtos.ToList() });
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
                // The server owns the key. Taking it from the request body meant an
                // empty Guid whenever the caller left it out.
                Id = Guid.NewGuid(),
                MaxScore = ruleSetDto.MaxScore,
                Doubles = ruleSetDto.Doubles,
                HasDoubles = ruleSetDto.HasDoubles,
                WeaponId = ruleSetDto.WeaponId
            };

            var result = await _ruleSetService.AddAsync(ruleSet);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var created = await _ruleSetService.GetByIdAsync(ruleSet.Id);

            if (!created.IsSuccess)
                return BadRequest(created.Errors);

            var dto = created.Data.ToDetailRuleSetDto();
            return CreatedAtAction(nameof(GetById), new { id = ruleSet.Id },
                new ResultModel<RuleSetDetailDto> { Data = dto });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, RuleSetCreateOrUpdateDto ruleSetDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _ruleSetService.DoesRuleSetIdExistsAsync(id) == false)
                return NotFound(new { message = $"No ruleset with id '{id}' found" });

            var existingResult = await _ruleSetService.GetByIdAsync(id);

            if (!existingResult.IsSuccess)
                return BadRequest(existingResult.Errors);

            var existing = existingResult.Data;
            existing.WeaponId = ruleSetDto.WeaponId;
            existing.Doubles = ruleSetDto.Doubles;
            existing.HasDoubles = ruleSetDto.HasDoubles;
            existing.MaxScore = ruleSetDto.MaxScore;

            var result = await _ruleSetService.UpdateAsync(existing);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var updated = await _ruleSetService.GetByIdAsync(id);
            return Ok(new ResultModel<RuleSetDetailDto> { Data = updated.Data.ToDetailRuleSetDto() });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _ruleSetService.GetByIdAsync(id);

            if (!existing.IsSuccess)
                return NotFound(new { message = $"No ruleSet with an id of {id}" });

            var result = await _ruleSetService.DeleteAsync(existing.Data);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(new { message = $"RuleSet {existing.Data.Id} deleted successfully" });
        }
    }
}
