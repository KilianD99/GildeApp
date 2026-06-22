using System;
using System.Collections.Generic;
using System.Text;

namespace GildeApp.Api.Core.Entities
{
    public class Tourney
    {
        public string Name { get; set; }
        public ICollection<Player> Players { get; set; }
        public RuleSet RuleSet { get; set; }
    }
}
