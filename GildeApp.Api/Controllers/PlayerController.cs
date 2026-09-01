using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;
using GildeApp.Api.Dtos.Matches;
using GildeApp.Api.Dtos.Players;
using GildeApp.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GildeApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        protected readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _playerService.ListAllAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var dtos = result.Data.ToPlayerListDto();
            return Ok(new ResultModel<List<PlayerDto>> { Data = dtos.ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _playerService.GetByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            var dto = result.Data.ToDetailPlayerDto();
            return Ok(new ResultModel<PlayerDetailDto> { Data = dto });
        }

        [HttpPost]
        public async Task<IActionResult> Add(PlayerCreateOrUpdateDto playerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var player = new Player
            {
                FirstName = playerDto.FirstName,
                LastName = playerDto.LastName,
                TourneyId = playerDto.TourneyId
            };

            var result = await _playerService.AddAsync(player);

            if (result.IsSuccess)
            {
                var createdPlayer = await _playerService.GetByIdAsync(player.Id);

                if (createdPlayer.IsSuccess)
                {
                    var dto = createdPlayer.Data.ToDetailPlayerDto();
                    return CreatedAtAction(nameof(GetById), new { id = player.Id }, new ResultModel<PlayerDetailDto> { Data = dto });
                }
            }

            return BadRequest(result.Errors);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PlayerCreateOrUpdateDto playerListDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _playerService.DoesPlayerIdExistsAsync(id) == false)
                return NotFound(new { message = $"No playlist with id '{id}' found" });

            var existingPlayerResult = await _playerService.GetByIdAsync(id);

            if (!existingPlayerResult.IsSuccess)
                return BadRequest(existingPlayerResult.Errors);

            var existingPlayer = existingPlayerResult.Data;
            existingPlayer.Id = id;
            existingPlayer.FirstName = playerListDto.FirstName;
            existingPlayer.LastName = playerListDto.LastName;
            existingPlayer.TourneyId = playerListDto.TourneyId;

            var result = await _playerService.UpdateAsync(existingPlayer);

            if (result.IsSuccess)
            {
                var updatedPlaylist = await _playerService.GetByIdAsync(id);

                if (updatedPlaylist.IsSuccess)
                {
                    var dto = updatedPlaylist.Data.ToDetailPlayerDto();
                    return Ok(new ResultModel<PlayerDetailDto> { Data = dto });
                }
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _playerService.DoesPlayerIdExistsAsync(id) == false)
                return NotFound(new { message = $"No player with an id of {id}" });

            var existingMatch = await _playerService.GetByIdAsync(id);

            if (!existingMatch.IsSuccess)
                return BadRequest(existingMatch.Errors);



            var result = await _playerService.DeleteAsync(existingMatch.Data);

            if (result.IsSuccess)
                return Ok(new { message = $"Player {existingMatch.Data.Id} deleted successfully" });

            return BadRequest(result.Errors);
        }
    }
}
