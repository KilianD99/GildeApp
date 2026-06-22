using System;
using System.Collections.Generic;
using System.Text;

namespace GildeApp.Api.Core.Entities
{
    public class RuleSet : BaseEntity
    {
        public bool HasDoubles { get; set; }
        public int MaxScore { get; set; }
        public int Doubles { get; set; }
        public Guid WeaponId { get; set; }
        public Weapon Weapon { get; set; }
    }
}
