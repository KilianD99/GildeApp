using System;
using System.ComponentModel.DataAnnotations;

namespace GildeApp.Api.Dtos.Tourneys
{
    public class TourneyCreateOrUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid RuleSetId { get; set; }
    }
}
