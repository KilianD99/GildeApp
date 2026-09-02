using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;
using GildeApp.Api.Dtos.Matches;
using GildeApp.Api.Extensions;
using GildeApp.Api.Hubs;
using Microsoft.AspNetCore.Mvc;

namespace GildeApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        protected readonly IMatchService _matchService;
        private readonly BoardBroadcaster _broadcaster;

        public MatchController(IMatchService matchService, BoardBroadcaster broadcaster)
        {
            _matchService = matchService;
            _broadcaster = broadcaster;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _matchService.ListAllAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var dtos = result.Data.ToMatchListDto();
            return Ok(new ResultModel<List<MatchDto>> { Data = dtos.ToList() });
        }

        /// <summary>The judge's match list for one tourney, in running order.</summary>
        [HttpGet("tourney/{tourneyId}")]
        public async Task<IActionResult> GetForTourney(Guid tourneyId)
        {
            var result = await _matchService.ListForTourneyAsync(tourneyId);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            var dtos = result.Data.ToMatchListDto();
            return Ok(new ResultModel<List<MatchDto>> { Data = dtos.ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _matchService.GetByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(new ResultModel<MatchDetailDto> { Data = result.Data.ToDetailMatchDto() });
        }

        /// <summary>
        /// What the mobile app posts after each touch, or once at the end of the bout.
        /// Send Finish = false for a live score, true to lock the result in.
        /// </summary>
        [HttpPut("{id}/score")]
        public async Task<IActionResult> SubmitScore(Guid id, MatchScoreDto scoreDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _matchService.SubmitScoreAsync(
                id, scoreDto.FirstScore, scoreDto.SecondScore, scoreDto.Finish);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            // Everyone watching this tourney's board gets the new standings immediately.
            await _broadcaster.BroadcastAsync(result.Data.TourneyId);

            return Ok(new ResultModel<MatchDetailDto> { Data = result.Data.ToDetailMatchDto() });
        }

        [HttpPost("{id}/reopen")]
        public async Task<IActionResult> Reopen(Guid id)
        {
            var result = await _matchService.ReopenAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            await _broadcaster.BroadcastAsync(result.Data.TourneyId);

            return Ok(new ResultModel<MatchDetailDto> { Data = result.Data.ToDetailMatchDto() });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _matchService.GetByIdAsync(id);

            if (!existing.IsSuccess)
                return NotFound(new { message = $"No match with an id of {id}" });

            var tourneyId = existing.Data.TourneyId;
            var result = await _matchService.DeleteAsync(existing.Data);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            await _broadcaster.BroadcastAsync(tourneyId);

            return Ok(new { message = $"Match {existing.Data.Id} deleted successfully" });
        }
    }
}
