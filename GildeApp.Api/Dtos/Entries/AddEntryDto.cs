using System;
using System.ComponentModel.DataAnnotations;

namespace GildeApp.Api.Dtos.Entries
{
    public class AddEntryDto
    {
        [Required]
        public Guid PlayerId { get; set; }
    }
}
