using System;
using System.Collections.Generic;
using System.Text;

namespace GildeApp.Api.Core.Entities
{
    public class RuleSet
    {
        public bool HasDoubles { get; set; }
        public int MaxScore { get; set; }
        public int FirstPlayerScore { get; set; }
        public int SecondPlayerScore { get; set; }
        public int Doubles { get; set; }
    }
}
