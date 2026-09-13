namespace Mvc.GildeApp.mvc.Models
{
    public class RuleSetModel
    {
        public Guid RuleSetId { get; set; }
        public Guid WeaponId { get; set; }
        public int MaxScore { get; set; }
        public int Doubles { get; set; }
        public bool HasDoubles { get; set; }
        public WeaponModel? Weapon { get; set; }
    }
}
