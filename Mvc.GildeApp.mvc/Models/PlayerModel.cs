namespace Mvc.GildeApp.mvc.Models
{
    public class PlayerModel
    {
        public Guid PlayerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
