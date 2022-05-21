using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.MatchSummary.Dto
{
    public class MatchBatsman
    {
        public long PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int? Runs { get; set; }
        public int? Balls { get; set; }
        public int? Six { get; set; }
        public int? Four { get; set; }
        public int? HowOut { get; set; }
        public string HowOutNormalized { get; set; }
        public string Fielder { get; set; }
        public string Bowler { get; set; }
        public string HomeTeam { get; set; }
        public string OppTeam { get; set; }

    }
}
