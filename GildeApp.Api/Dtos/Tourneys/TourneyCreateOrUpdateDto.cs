namespace GildeApp.Api.Dtos.Tourneys
{
    public class TourneyCreateOrUpdateDto
    {
        public Guid? TourneyId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public Guid RuleSetId { get; set; }
    }
}
