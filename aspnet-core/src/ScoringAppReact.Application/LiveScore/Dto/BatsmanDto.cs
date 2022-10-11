using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.LiveScore.Dto
{
    public class BatsmanDto
    {
        public long Id { get; set; }
        public int? Runs { get; set; }
        public string Name { get; set; }
        public int? Sixes { get; set; }
        public int? Fours { get; set; }
        public int? Balls { get; set; }
        public int? Dots { get; set; }
        public int[] Timeline { get; set; }
    }
}
