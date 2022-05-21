using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.MatchSummary.Dto
{
    public class MatchDetails
    {
        public MatchDetails()
        {
            FirstInningBatsman = new List<MatchBatsman>();
            FirstInningBowler = new List<MatchBowler>();
            SecondInningBatsman = new List<MatchBatsman>();
            SecondInningBowler = new List<MatchBowler>();
        }

        public TeamScoreDto Team1Score { get; set; }
        public TeamScoreDto Team2Score { get; set; }
        public List<MatchBatsman> FirstInningBatsman { get; set; }
        public List<MatchBowler> FirstInningBowler { get; set; }
        public List<MatchBatsman> SecondInningBatsman { get; set; }
        public List<MatchBowler> SecondInningBowler { get; set; }

    }
}
