﻿using System;
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

            FirstInningTop3Batsman = new List<MatchBatsman>();
            FirstInningTop3Bowler = new List<MatchBowler>();
            SecondInningTop3Batsman = new List<MatchBatsman>();
            SecondInningTop3Bowler = new List<MatchBowler>();
        }

        public string MatchResult { get; set; }
        public string ProfileUrl { get; set; }
        public string Ground { get; set; }
        public string Toss { get; set; }
        public long? Date { get; set; }
        public string MatchType { get; set; }
        public TeamsScoreDto Team1Score { get; set; }
        public TeamsScoreDto Team2Score { get; set; }
        public List<MatchBatsman> FirstInningBatsman { get; set; }
        public List<MatchBowler> FirstInningBowler { get; set; }
        public List<MatchBatsman> SecondInningBatsman { get; set; }
        public List<MatchBowler> SecondInningBowler { get; set; }

        public List<MatchBatsman> FirstInningTop3Batsman { get; set; }
        public List<MatchBowler> FirstInningTop3Bowler { get; set; }
        public List<MatchBatsman> SecondInningTop3Batsman { get; set; }
        public List<MatchBowler> SecondInningTop3Bowler { get; set; }

    }
}
