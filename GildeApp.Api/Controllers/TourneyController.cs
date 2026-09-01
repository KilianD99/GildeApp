using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;
using GildeApp.Api.Dtos.RuleSets;
using GildeApp.Api.Dtos.Tourneys;
using GildeApp.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GildeApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourneyController : ControllerBase
    {
        protected readonly ITourneyService _tourneyService;

        public TourneyController(ITourneyService tourneyService)
        {
            _tourneyService = tourneyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _tourneyService.ListAllAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var dtos = result.Data.ToTourneyDetailDto();
            return Ok(new ResultModel<List<TourneyDto>> { Data = dtos.ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _tourneyService.GetByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            var dto = result.Data.ToDetaiTourneylDto();
            return Ok(new ResultModel<TourneyDetailDto> { Data = dto });
        }

        [HttpPost]
        public async Task<IActionResult> Add(TourneyCreateOrUpdateDto tourneyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tourney = new Tourney
            {
                Name = tourneyDto.Name,
                RuleSetId = tourneyDto.RuleSetId
            };

            var result = await _tourneyService.AddAsync(tourney);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var createdTourney = await _tourneyService.GetByIdAsync(tourney.Id);

            if (!createdTourney.IsSuccess)
                return BadRequest(createdTourney.Errors);

            var dto = createdTourney.Data.ToDetaiTourneylDto();
            return CreatedAtAction(nameof(GetById), new { id = tourney.Id }, new ResultModel<TourneyDetailDto> { Data = dto });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TourneyCreateOrUpdateDto tourneyListDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _tourneyService.DoesTourneyIdExistsAsync(id) == false)
                return NotFound(new { message = $"No ruleset with id '{id}' found" });

            var existingTourneyResult = await _tourneyService.GetByIdAsync(id);

            if (!existingTourneyResult.IsSuccess)
                return BadRequest(existingTourneyResult.Errors);

            var existingTourney = existingTourneyResult.Data;
            existingTourney.Id = id;
            existingTourney.Name = tourneyListDto.Name;
            existingTourney.RuleSetId = tourneyListDto.RuleSetId;                    


            var result = await _tourneyService.UpdateAsync(existingTourney);

            if (result.IsSuccess)
            {
                var updatedTourney = await _tourneyService.GetByIdAsync(id);

                if (updatedTourney.IsSuccess)
                {
                    var dto = updatedTourney.Data.ToDetaiTourneylDto();
                    return Ok(new ResultModel<TourneyDetailDto> { Data = dto });
                }
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _tourneyService.DoesTourneyIdExistsAsync(id) == false)
                return NotFound(new { message = $"No tourney with an id of {id}" });

            var existingTourney = await _tourneyService.GetByIdAsync(id);

            if (!existingTourney.IsSuccess)
                return BadRequest(existingTourney.Errors);



            var result = await _tourneyService.DeleteAsync(existingTourney.Data);

            if (result.IsSuccess)
                return Ok(new { message = $"Tourney {existingTourney.Data.Id} deleted successfully" });

            return BadRequest(result.Errors);
        }
    }
}
