using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.MatchSummary.Dto
{
    public class MatchBowler
    {

        public long PlayerId { get; set; }
        public string PlayerName { get; set; }
        public float? Overs { get; set; }
        public int? Runs { get; set; }
        public int? Wickets { get; set; }
        public int? Maiden { get; set; }

    }
}
