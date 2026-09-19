using GildeApp.Api.Core.Entities;
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
        /// <summary>
        /// The judge's name, sent by the mobile app on every scoring call. This is
        /// not authentication -- anyone can put any name in this header. It exists so
        /// the app can say "Marie is on this one" instead of "taken". Swap it for a
        /// real token before this runs anywhere public.
        /// </summary>
        public const string JudgeHeader = "X-Judge-Name";

        protected readonly IMatchService _matchService;
        private readonly BoardBroadcaster _broadcaster;

        public MatchController(IMatchService matchService, BoardBroadcaster broadcaster)
        {
            _matchService = matchService;
            _broadcaster = broadcaster;
        }

        private string? JudgeName =>
            Request.Headers.TryGetValue(JudgeHeader, out var values)
                ? values.FirstOrDefault()?.Trim()
                : null;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _matchService.ListAllAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var dtos = result.Data.ToMatchListDto();
            return Ok(new ResultModel<List<MatchDto>> { Data = dtos.ToList() });
        }

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
        /// Takes the match so nobody else can score it, or renews a claim this judge
        /// already holds. 409 means another judge got there first.
        /// </summary>
        [HttpPost("{id}/claim")]
        public async Task<IActionResult> Claim(Guid id)
        {
            var judge = JudgeName;

            if (string.IsNullOrWhiteSpace(judge))
                return BadRequest(new { message = $"Missing {JudgeHeader} header" });

            var result = await _matchService.ClaimAsync(id, judge);

            if (!result.IsSuccess)
                return Conflict(result.Errors);

            await _broadcaster.BroadcastAsync(result.Data.TourneyId);

            return Ok(new ResultModel<MatchDetailDto> { Data = result.Data.ToDetailMatchDto() });
        }

        /// <summary>Hands the match back without finishing it.</summary>
        [HttpPost("{id}/release")]
        public async Task<IActionResult> Release(Guid id)
        {
            var judge = JudgeName;

            if (string.IsNullOrWhiteSpace(judge))
                return BadRequest(new { message = $"Missing {JudgeHeader} header" });

            var result = await _matchService.ReleaseAsync(id, judge);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            await _broadcaster.BroadcastAsync(result.Data.TourneyId);

            return Ok(new ResultModel<MatchDetailDto> { Data = result.Data.ToDetailMatchDto() });
        }

        /// <summary>
        /// What the app posts after each touch. Send Finish = false for a live score,
        /// true to lock the result in. Every successful call renews the claim.
        /// </summary>
        [HttpPut("{id}/score")]
        public async Task<IActionResult> SubmitScore(Guid id, MatchCreateOrUpdateDto scoreDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var judge = JudgeName;

            if (string.IsNullOrWhiteSpace(judge))
                return BadRequest(new { message = $"Missing {JudgeHeader} header" });

            var result = await _matchService.SubmitScoreAsync(
                id, scoreDto.FirstScore, scoreDto.SecondScore, scoreDto.Finish, judge);

            if (!result.IsSuccess)
                return Conflict(result.Errors);

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
