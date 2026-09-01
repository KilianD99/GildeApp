namespace GildeApp.Api.Dtos.Players
{
    public class PlayerCreateOrUpdateDto
    {
        public Guid? PlayerId { get; set; }
        public Guid TourneyId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
