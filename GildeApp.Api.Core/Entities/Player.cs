using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GildeApp.Api.Core.Entities
{
    /// <summary>
    /// A person. A player exists independently of any tourney, so the same person
    /// can be entered into many tourneys over time without being retyped.
    /// The link between a player and a tourney is <see cref="TourneyEntry"/>.
    /// </summary>
    public class Player : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public ICollection<TourneyEntry> Entries { get; set; } = new List<TourneyEntry>();

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
