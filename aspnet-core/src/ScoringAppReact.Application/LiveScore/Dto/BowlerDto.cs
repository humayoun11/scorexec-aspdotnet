using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.LiveScore.Dto
{
    public class BowlerDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int? Runs { get; set; }
        public float? Overs { get; set; }
        public int? Balls { get; set; }
        public int? TotalBalls { get; set; }
        public int? Wickets { get; set; }
        public int? Maidens { get; set; }
        public int? Dots { get; set; }
        public bool NewOver { get; set; }
        public int[] Timeline { get; set; }
    }
}
