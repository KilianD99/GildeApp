using System;
using System.Collections.Generic;
using GildeApp.Api.Dtos.Entries;
using GildeApp.Api.Dtos.RuleSets;

namespace GildeApp.Api.Dtos.Tourneys
{
    public class TourneyDetailDto : TourneyDto
    {
        public Guid RuleSetId { get; set; }
        public RuleSetDetailDto? RuleSet { get; set; }
        public IEnumerable<TourneyEntryDto> Entries { get; set; } = new List<TourneyEntryDto>();
    }
}
