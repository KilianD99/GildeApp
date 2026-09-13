using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GildeApp.Api.Core.Entities
{
    public class Player : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<TourneyEntry> Entries { get; set; } = new List<TourneyEntry>();

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
