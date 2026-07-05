using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;
using GildeApp.Api.Dtos.Matches;
using GildeApp.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GildeApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        protected readonly IMatchService _matchService;

        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _matchService.ListAllAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var dtos = result.Data.ToMatchDtoList();
            return Ok(new ResultModel<List<MatchDto>> { Data = dtos.ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _matchService.GetByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            var dto = result.Data.ToDetailMatchDto();
            return Ok(new ResultModel<MatchDetailDto> { Data = dto });
        }

        [HttpPost]
        public async Task<IActionResult> Add(MatchCreateOrUpdateDto matchDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var match = new Match
            {
                FirstPlayerId = matchDto.FirstPlayerId,
                SecondPlayerId = matchDto.SecondPlayerId,
                TourneyId = matchDto.TourneyId,
                FirstPlayerScore = matchDto.FirstPlayerScore,
                SecondPlayerScore = matchDto.SecondPlayerScore,
            };

            var result = await _matchService.AddAsync(match);

            if (result.IsSuccess)
            {
                var createdMatch = await _matchService.GetByIdAsync(match.Id);

                if (createdMatch.IsSuccess)
                {
                    var dto = createdMatch.Data.ToDetailMatchDto();
                    return CreatedAtAction(nameof(GetById), new { id =  match.Id }, new ResultModel<MatchDetailDto> { Data = dto });
                }
            }

            return BadRequest(result.Errors);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, MatchCreateOrUpdateDto playlistDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _matchService.DoesMatchIdExistsAsync(id) == false)
                return NotFound(new { message = $"No playlist with id '{id}' found" });

            var existingMatchResult = await _matchService.GetByIdAsync(id);

            if (!existingMatchResult.IsSuccess)
                return BadRequest(existingMatchResult.Errors);

            var existingMatch = existingMatchResult.Data;
            existingMatch.Id = id;
            existingMatch.FirstPlayerId = playlistDto.FirstPlayerId;
            existingMatch.FirstPlayerScore = playlistDto.FirstPlayerScore;
            existingMatch.SecondPlayerId = playlistDto.SecondPlayerId;
            existingMatch.SecondPlayerScore = playlistDto.SecondPlayerScore;
            existingMatch.TourneyId = playlistDto.TourneyId;

            var result = await _matchService.UpdateAsync(existingMatch);

            if (result.IsSuccess)
            {
                var updatedPlaylist = await _matchService.GetByIdAsync(id);

                if (updatedPlaylist.IsSuccess)
                {
                    var dto = updatedPlaylist.Data.ToDetailMatchDto();
                    return Ok(new ResultModel<MatchDetailDto> { Data = dto });
                }
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _matchService.DoesMatchIdExistsAsync(id) == false)
                return NotFound(new { message = $"No match with an id of {id}" });

            var existingMatch = await _matchService.GetByIdAsync(id);

            if (!existingMatch.IsSuccess)
                return BadRequest(existingMatch.Errors);

        

            var result = await _matchService.DeleteAsync(existingMatch.Data);

            if (result.IsSuccess)
                return Ok(new { message = $"Playlist {existingMatch.Data.Id} deleted successfully" });

            return BadRequest(result.Errors);
        }

    }
}
