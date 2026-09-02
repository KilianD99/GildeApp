using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;
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

        /// <summary>
        /// Every player on file. Pass ?search= to filter, which is what the
        /// "add a player" box on the entry screen uses.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var result = string.IsNullOrWhiteSpace(search)
                ? await _playerService.ListAllAsync()
                : await _playerService.SearchAsync(search);

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

            return Ok(new ResultModel<PlayerDetailDto> { Data = result.Data.ToDetailPlayerDto() });
        }

        [HttpPost]
        public async Task<IActionResult> Add(PlayerCreateOrUpdateDto playerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var player = new Player
            {
                Id = Guid.NewGuid(),
                FirstName = playerDto.FirstName,
                LastName = playerDto.LastName
            };

            var result = await _playerService.AddAsync(player);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetById), new { id = player.Id },
                new ResultModel<PlayerDto> { Data = result.Data.ToPlayerDto() });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PlayerCreateOrUpdateDto playerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingResult = await _playerService.GetByIdAsync(id);

            if (!existingResult.IsSuccess)
                return NotFound(new { message = $"No player with id '{id}' found" });

            var existing = existingResult.Data;
            existing.FirstName = playerDto.FirstName;
            existing.LastName = playerDto.LastName;

            var result = await _playerService.UpdateAsync(existing);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(new ResultModel<PlayerDto> { Data = result.Data.ToPlayerDto() });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _playerService.GetByIdAsync(id);

            if (!existing.IsSuccess)
                return NotFound(new { message = $"No player with an id of {id}" });

            var result = await _playerService.DeleteAsync(existing.Data);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(new { message = $"Player {existing.Data.Id} deleted successfully" });
        }
    }
}
