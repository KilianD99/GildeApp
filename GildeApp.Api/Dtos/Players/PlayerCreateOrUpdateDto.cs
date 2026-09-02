using System.ComponentModel.DataAnnotations;

namespace GildeApp.Api.Dtos.Players
{
    public class PlayerCreateOrUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
    }
}
