using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;
using GildeApp.Api.Dtos.Weapons;
using GildeApp.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GildeApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeaponController : ControllerBase
    {
        protected readonly IWeaponService _weaponService;

        public WeaponController(IWeaponService weaponService)
        {
            _weaponService = weaponService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _weaponService.ListAllAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var dtos = result.Data.ToWeaponListDto();
            return Ok(new ResultModel<List<WeaponDto>> { Data = dtos.ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _weaponService.GetByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            var dto = result.Data.ToDetailWeaponDto();
            return Ok(new ResultModel<WeaponDetailDto> { Data = dto });
        }

        [HttpPost]
        public async Task<IActionResult> Add(WeaponCreateOrUpdateDto weaponDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var weapon = new Weapon
            {
                Id = Guid.NewGuid(),
                Name = weaponDto.Name
            };

            var result = await _weaponService.AddAsync(weapon);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var createdWeapon = await _weaponService.GetByIdAsync(weapon.Id);

            if (!createdWeapon.IsSuccess)
                return BadRequest(createdWeapon.Errors);

            var dto = createdWeapon.Data.ToDetailWeaponDto();
            return CreatedAtAction(nameof(GetById), new { id = weapon.Id }, new ResultModel<WeaponDetailDto> { Data = dto });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, WeaponCreateOrUpdateDto weaponDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _weaponService.DoesWeaponIdExistsAsync(id) == false)
                return NotFound(new { message = $"No weapon with id '{id}' found" });

            var existingWeaponResult = await _weaponService.GetByIdAsync(id);

            if (!existingWeaponResult.IsSuccess)
                return BadRequest(existingWeaponResult.Errors);

            var existingWeapon = existingWeaponResult.Data;
            existingWeapon.Name = weaponDto.Name;

            var result = await _weaponService.UpdateAsync(existingWeapon);

            if (result.IsSuccess)
            {
                var updatedWeapon = await _weaponService.GetByIdAsync(id);

                if (updatedWeapon.IsSuccess)
                {
                    var dto = updatedWeapon.Data.ToDetailWeaponDto();
                    return Ok(new ResultModel<WeaponDetailDto> { Data = dto });
                }
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _weaponService.DoesWeaponIdExistsAsync(id) == false)
                return NotFound(new { message = $"No weapon with an id of {id}" });

            var existingWeapon = await _weaponService.GetByIdAsync(id);

            if (!existingWeapon.IsSuccess)
                return BadRequest(existingWeapon.Errors);

            var result = await _weaponService.DeleteAsync(existingWeapon.Data);

            if (result.IsSuccess)
                return Ok(new { message = $"Weapon {existingWeapon.Data.Id} deleted successfully" });

            return BadRequest(result.Errors);
        }
    }
}
