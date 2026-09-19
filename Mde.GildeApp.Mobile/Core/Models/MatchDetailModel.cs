using System;
using System.Collections.Generic;
using System.Text;

namespace GildeApp.Mobile.Core.Models
{
    public class MatchDetailModel : MatchModel
    {
        public string TourneyName { get; set; } = string.Empty;
        public int MaxScore { get; set; }
    }
}
