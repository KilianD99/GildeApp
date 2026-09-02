using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;
using GildeApp.Api.Dtos.Board;
using GildeApp.Api.Dtos.Entries;
using GildeApp.Api.Dtos.Tourneys;
using GildeApp.Api.Extensions;
using GildeApp.Api.Hubs;
using Microsoft.AspNetCore.Mvc;

namespace GildeApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourneyController : ControllerBase
    {
        protected readonly ITourneyService _tourneyService;
        private readonly BoardBroadcaster _broadcaster;

        public TourneyController(ITourneyService tourneyService, BoardBroadcaster broadcaster)
        {
            _tourneyService = tourneyService;
            _broadcaster = broadcaster;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _tourneyService.ListAllAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var dtos = result.Data.ToTourneyListDto();
            return Ok(new ResultModel<List<TourneyDto>> { Data = dtos.ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _tourneyService.GetFullAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            var dto = result.Data.ToDetailTourneyDto();
            return Ok(new ResultModel<TourneyDetailDto> { Data = dto });
        }

        /// <summary>
        /// The whole board in one call: rows, the grid, the totals and the placements.
        /// Both the web app and the mobile app read this.
        /// </summary>
        [HttpGet("{id}/board")]
        public async Task<IActionResult> GetBoard(Guid id)
        {
            var result = await _tourneyService.GetFullAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(new ResultModel<BoardDto> { Data = result.Data.ToBoardDto() });
        }

        [HttpPost]
        public async Task<IActionResult> Add(TourneyCreateOrUpdateDto tourneyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tourney = new Tourney
            {
                Id = Guid.NewGuid(),
                Name = tourneyDto.Name,
                RuleSetId = tourneyDto.RuleSetId
            };

            var result = await _tourneyService.AddAsync(tourney);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var created = await _tourneyService.GetFullAsync(tourney.Id);

            if (!created.IsSuccess)
                return BadRequest(created.Errors);

            var dto = created.Data.ToDetailTourneyDto();
            return CreatedAtAction(nameof(GetById), new { id = tourney.Id },
                new ResultModel<TourneyDetailDto> { Data = dto });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TourneyCreateOrUpdateDto tourneyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingResult = await _tourneyService.GetByIdAsync(id);

            if (!existingResult.IsSuccess)
                return NotFound(new { message = $"No tourney with id '{id}' found" });

            var existing = existingResult.Data;
            existing.Name = tourneyDto.Name;
            existing.RuleSetId = tourneyDto.RuleSetId;

            var result = await _tourneyService.UpdateAsync(existing);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var updated = await _tourneyService.GetFullAsync(id);
            return Ok(new ResultModel<TourneyDetailDto> { Data = updated.Data.ToDetailTourneyDto() });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _tourneyService.GetByIdAsync(id);

            if (!existing.IsSuccess)
                return NotFound(new { message = $"No tourney with an id of {id}" });

            var result = await _tourneyService.DeleteAsync(existing.Data);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(new { message = $"Tourney {existing.Data.Id} deleted successfully" });
        }

        // ---- entries -------------------------------------------------------

        [HttpGet("{id}/entries")]
        public async Task<IActionResult> GetEntries(Guid id)
        {
            var result = await _tourneyService.GetFullAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            var dtos = result.Data.Entries
                .OrderBy(e => e.Position)
                .Select(e => e.ToEntryDto())
                .ToList();

            return Ok(new ResultModel<List<TourneyEntryDto>> { Data = dtos });
        }

        [HttpPost("{id}/entries")]
        public async Task<IActionResult> AddEntry(Guid id, AddEntryDto entryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tourneyService.AddEntryAsync(id, entryDto.PlayerId);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            // Reload so the entry comes back with its player attached.
            var refreshed = await _tourneyService.GetFullAsync(id);
            var entry = refreshed.IsSuccess
                ? refreshed.Data.Entries.FirstOrDefault(e => e.Id == result.Data.Id)
                : null;

            return Ok(new ResultModel<TourneyEntryDto>
            {
                Data = (entry ?? result.Data).ToEntryDto()
            });
        }

        [HttpDelete("{id}/entries/{entryId}")]
        public async Task<IActionResult> RemoveEntry(Guid id, Guid entryId)
        {
            var result = await _tourneyService.RemoveEntryAsync(id, entryId);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(new { message = "Player removed from the tourney" });
        }

        // ---- lifecycle -----------------------------------------------------

        /// <summary>
        /// Builds every match for the round robin and starts the tourney. This lives in
        /// the API rather than in either client, so the web app and the mobile app can
        /// never disagree about what the schedule is.
        /// </summary>
        [HttpPost("{id}/generate")]
        public async Task<IActionResult> Generate(Guid id)
        {
            var result = await _tourneyService.GenerateMatchesAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            await _broadcaster.BroadcastAsync(id);

            var refreshed = await _tourneyService.GetFullAsync(id);
            return Ok(new ResultModel<BoardDto> { Data = refreshed.Data.ToBoardDto() });
        }

        [HttpPost("{id}/finish")]
        public async Task<IActionResult> Finish(Guid id)
        {
            var result = await _tourneyService.FinishAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            await _broadcaster.BroadcastAsync(id);

            var refreshed = await _tourneyService.GetFullAsync(id);
            return Ok(new ResultModel<BoardDto> { Data = refreshed.Data.ToBoardDto() });
        }
    }
}
