using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.Statistics.Dto
{
    public class BattingRecord
    {
        public int playerId { get; set; }
        public string PlayerName { get; set; }
        public int TeamId { get; set; }
        public int TotalMatch { get; set; }
        public int TotalInnings { get; set; }
        public int TotalBatRuns { get; set; }
        public int TotalBatBalls { get; set; }
        public int TotalFours { get; set; }
        public int TotalSixes { get; set; }
        public int NumberOf50s { get; set; }
        public int NumberOf100s { get; set; }
        public string StrikeRate { get; set; }
        public string Image { get; set; }
        public string BattingAverage { get; set; }
    }
}
