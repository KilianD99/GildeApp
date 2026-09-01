using System.ComponentModel.DataAnnotations;

namespace GildeApp.Api.Dtos.Weapons
{
    public class WeaponCreateOrUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
