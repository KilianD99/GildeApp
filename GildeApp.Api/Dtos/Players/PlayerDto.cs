namespace GildeApp.Api.Dtos.Players
{
    public class PlayerDto
    {
        public Guid PlayerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
